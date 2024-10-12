using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildReportTool
{
    /// <summary>
    /// Any attempt to serialize <see cref="UnityEditor.Build.Reporting.BuildReport"/> results in errors:<br/><br/>
    ///
    /// When using <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>:<br/>
    /// <c>SerializationException: Type 'UnityEditor.Build.Reporting.BuildReport' in Assembly 'UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' is not marked as serializable.</c><br/><br/>
    ///
    /// When using <see cref="UnityEngine.JsonUtility"/>:<br/>
    /// <c>ArgumentException: JsonUtility.ToJson does not support engine types.</c><br/><br/>
    ///
    /// When using <see cref="System.Xml.Serialization.XmlSerializer"/>:<br/>
    /// It works, but only <see cref="UnityEditor.Build.Reporting.BuildReport.name"/> and <see cref="UnityEditor.Build.Reporting.BuildReport.hideFlags"/> get serialized. The actual important data doesn't get saved.<br/>
    /// Note: There is <see cref="System.Xml.Serialization.XmlAttributeOverrides"/> but that still can't serialize read-only properties.
    /// <see cref="UnityEditor.Build.Reporting.BuildReport"/> unfortunately has some important read-only properties such as
    /// <see cref="UnityEditor.Build.Reporting.BuildReport.files"/> and <see cref="UnityEditor.Build.Reporting.BuildReport.steps"/>.<br/><br/>
    ///
    /// So we have to make this dummy class that essentially mimics <see cref="UnityEditor.Build.Reporting.BuildReport"/>, but defined as properly serializable this time.
    /// We also favor saving enums into strings since they are merely displayed for the user, and will not be further processed.
    /// Converting them to string also helps with backwards compatibility in case a future version of Unity deletes an enum value or renames it.
    /// </summary>
    [Serializable]
    public class UnityBuildReport : IDataFile
    {
        /// <summary>
        /// Name of project folder.
        /// </summary>
        public string ProjectName;

        /// <summary>
        /// Type of build that the project was configured to, at the time that UnityBuildReport was collected.
        /// </summary>
        public string BuildType;

        /// <summary>
        /// When UnityBuildReport was collected.
        /// </summary>
        public DateTime TimeGot;

        public UnityEditor.BuildOptions BuildOptions;

        public bool HasBuildOption(UnityEditor.BuildOptions optionToCheck)
        {
            return (this.BuildOptions & optionToCheck) == optionToCheck;
        }

        // -----------------------------------------

        public OutputFile[]       OutputFiles;
        public BuildProcessStep[] BuildProcessSteps;

        #if UNITY_2018_1_OR_NEWER
        public void SetFrom(UnityEditor.Build.Reporting.BuildReport buildReport)
        {
            var outputFolder = buildReport.summary.outputPath;
            int outputPathLength;

            if (System.IO.Directory.Exists(outputFolder))
            {
                if (outputFolder.EndsWith("/") || outputFolder.EndsWith("\\"))
                    outputPathLength = outputFolder.Length;
                else
                    // +1 for the trailing slash, we want to remove
                    // the slash at the start of our file entries
                    outputPathLength = outputFolder.Length + 1;
            }
            else if (System.IO.File.Exists(outputFolder))
            {
                // output path is a file, likely the executable file
                // so get the parent folder of that file
                outputFolder = System.IO.Path.GetDirectoryName(outputFolder);

                if (!string.IsNullOrEmpty(outputFolder))
                    // +1 for the trailing slash, we want to remove
                    // the slash at the start of our file entries
                    outputPathLength = outputFolder.Length + 1;
                else
                    // output file has no parent folder?
                    return;
            }
            else
                // output path doesn't exist
            {
                outputPathLength = 0;
            }

            this.BuildOptions = buildReport.summary.options;

            outputFolder = outputFolder.Replace("\\", "/");

            #if !UNITY_2022_1_OR_NEWER
			var files = buildReport.files;
            #else
            var files = buildReport.GetFiles();
            #endif
            var outputFiles = new List<OutputFile>(files.Length);
            this.OutputFiles = new OutputFile[files.Length];
            for (var i = 0; i < files.Length; ++i)
            {
                if (!files[i].path.StartsWith(outputFolder))
                    // file is not inside the build folder, likely a temporary or debug file (like a pdb file)
                    //Debug.Log($"Found file not in build {i}: {buildReport.files[i].path}");
                    continue;

                OutputFile newEntry;
                newEntry.FilePath = files[i].path.Substring(outputPathLength);
                newEntry.Role     = files[i].role;
                newEntry.Size     = files[i].size;
                outputFiles.Add(newEntry);
            }
            this.OutputFiles = outputFiles.ToArray();

            this._totalBuildTime   = new(0);
            this.BuildProcessSteps = new BuildProcessStep[buildReport.steps.Length];
            for (var i = 0; i < this.BuildProcessSteps.Length; ++i)
            {
                this.BuildProcessSteps[i].Depth    = buildReport.steps[i].depth;
                this.BuildProcessSteps[i].Name     = buildReport.steps[i].name;
                this.BuildProcessSteps[i].Duration = buildReport.steps[i].duration;

                if (this.BuildProcessSteps[i].Depth == 1) this._totalBuildTime += this.BuildProcessSteps[i].Duration;

                this.BuildProcessSteps[i].SetInfoLogCount(0);
                this.BuildProcessSteps[i].SetWarnLogCount(0);
                this.BuildProcessSteps[i].SetErrorLogCount(0);

                this.BuildProcessSteps[i].SetCollapsedInfoLogCount(0);
                this.BuildProcessSteps[i].SetCollapsedWarnLogCount(0);
                this.BuildProcessSteps[i].SetCollapsedErrorLogCount(0);

                var messages = buildReport.steps[i].messages;
                if (messages == null || messages.Length == 0)
                {
                    this.BuildProcessSteps[i].BuildLogMessages = null;
                    this.BuildProcessSteps[i].SetCollapsedBuildLogMessages(null);
                }
                else
                {
                    this.BuildProcessSteps[i].BuildLogMessages = new BuildLogMessage[messages.Length];
                    var collapsedMessages = new List<BuildLogMessage>();
                    for (var m = 0; m < messages.Length; ++m)
                    {
                        this.BuildProcessSteps[i].BuildLogMessages[m].Message = messages[m].content;

                        var logType = messages[m].type;
                        this.BuildProcessSteps[i].BuildLogMessages[m].LogType = logType.ToString();
                        if (logType == LogType.Log)
                            this.BuildProcessSteps[i].IncrementInfoLogCount();
                        else if (logType == LogType.Warning)
                            this.BuildProcessSteps[i].IncrementWarnLogCount();
                        else
                            this.BuildProcessSteps[i].IncrementErrorLogCount();

                        var alreadyIn = false;
                        for (var c = 0; c < collapsedMessages.Count; ++c)
                        {
                            if (collapsedMessages[c].Message == messages[m].content)
                            {
                                var entryToModify = collapsedMessages[c];
                                entryToModify.SetCount(collapsedMessages[c].Count + 1);
                                collapsedMessages[c] = entryToModify;
                                alreadyIn            = true;
                                break;
                            }
                        }

                        if (alreadyIn) continue;

                        var entryToAdd = this.BuildProcessSteps[i].BuildLogMessages[m];
                        entryToAdd.SetCount(1);
                        collapsedMessages.Add(entryToAdd);

                        if (logType == LogType.Log)
                            this.BuildProcessSteps[i].IncrementCollapsedInfoLogCount();
                        else if (logType == LogType.Warning)
                            this.BuildProcessSteps[i].IncrementCollapsedWarnLogCount();
                        else
                            this.BuildProcessSteps[i].IncrementCollapsedErrorLogCount();
                    }

                    this.BuildProcessSteps[i].SetCollapsedBuildLogMessages(collapsedMessages.ToArray());
                }
            }
        }
        #endif

        // -----------------------------------------

        private TimeSpan _totalBuildTime;

        public TimeSpan TotalBuildTime => this._totalBuildTime;

        public void OnBeforeSave()
        {
        }

        public void OnAfterLoad()
        {
            this._totalBuildTime = new(0);
            for (var i = 0; i < this.BuildProcessSteps.Length; ++i)
            {
                if (this.BuildProcessSteps[i].Depth == 1) this._totalBuildTime += this.BuildProcessSteps[i].Duration;

                var messages = this.BuildProcessSteps[i].BuildLogMessages;

                if (messages != null)
                {
                    var collapsedMessages = new List<BuildLogMessage>();

                    this.BuildProcessSteps[i].SetInfoLogCount(0);
                    this.BuildProcessSteps[i].SetWarnLogCount(0);
                    this.BuildProcessSteps[i].SetErrorLogCount(0);

                    this.BuildProcessSteps[i].SetCollapsedInfoLogCount(0);
                    this.BuildProcessSteps[i].SetCollapsedWarnLogCount(0);
                    this.BuildProcessSteps[i].SetCollapsedErrorLogCount(0);

                    for (var m = 0; m < messages.Length; ++m)
                    {
                        var logType = GetLogType(messages[m].LogType);

                        switch (logType)
                        {
                            case CheckLogType.Info:
                                this.BuildProcessSteps[i].IncrementInfoLogCount();
                                break;
                            case CheckLogType.Warn:
                                this.BuildProcessSteps[i].IncrementWarnLogCount();
                                break;
                            case CheckLogType.Error:
                                this.BuildProcessSteps[i].IncrementErrorLogCount();
                                break;
                        }

                        var alreadyIn = false;
                        for (var c = 0; c < collapsedMessages.Count; ++c)
                        {
                            if (collapsedMessages[c].Message == messages[m].Message)
                            {
                                var entryToModify = collapsedMessages[c];
                                entryToModify.SetCount(collapsedMessages[c].Count + 1);
                                collapsedMessages[c] = entryToModify;
                                alreadyIn            = true;
                                break;
                            }
                        }

                        if (alreadyIn) continue;

                        var entryToAdd = messages[m];
                        entryToAdd.SetCount(1);
                        collapsedMessages.Add(entryToAdd);

                        switch (logType)
                        {
                            case CheckLogType.Info:
                                this.BuildProcessSteps[i].IncrementCollapsedInfoLogCount();
                                break;
                            case CheckLogType.Warn:
                                this.BuildProcessSteps[i].IncrementCollapsedWarnLogCount();
                                break;
                            case CheckLogType.Error:
                                this.BuildProcessSteps[i].IncrementCollapsedErrorLogCount();
                                break;
                        }
                    }

                    this.BuildProcessSteps[i].SetCollapsedBuildLogMessages(collapsedMessages.ToArray());
                }
            }
        }

        private enum CheckLogType
        {
            Info,
            Warn,
            Error,
        }

        private static CheckLogType GetLogType(string logType)
        {
            if (logType.Contains("Warn"))
                return CheckLogType.Warn;
            else if (logType.Contains("Log"))
                return CheckLogType.Info;
            else
                return CheckLogType.Error;
        }

        private string _savedPath;

        public void SetSavedPath(string savedPath)
        {
            this._savedPath = savedPath.Replace("\\", "/");
        }

        public string SavedPath => this._savedPath;

        public string GetDefaultFilename()
        {
            return Util.GetUnityBuildReportDefaultFilename(this.ProjectName, this.BuildType, this.TimeGot);
        }
    }

    [Serializable]
    public struct OutputFile
    {
        public string FilePath;
        public string Role;
        public ulong  Size;
    }

    [Serializable]
    public struct BuildProcessStep
    {
        public int               Depth;
        public string            Name;
        public BuildLogMessage[] BuildLogMessages;

        private int _infoLogCount;
        private int _warnLogCount;
        private int _errorLogCount;

        public int InfoLogCount  => this._infoLogCount;
        public int WarnLogCount  => this._warnLogCount;
        public int ErrorLogCount => this._errorLogCount;

        public void IncrementInfoLogCount()
        {
            ++this._infoLogCount;
        }

        public void IncrementWarnLogCount()
        {
            ++this._warnLogCount;
        }

        public void IncrementErrorLogCount()
        {
            ++this._errorLogCount;
        }

        public void SetInfoLogCount(int newInfoLogCount)
        {
            this._infoLogCount = newInfoLogCount;
        }

        public void SetWarnLogCount(int newWarnLogCount)
        {
            this._warnLogCount = newWarnLogCount;
        }

        public void SetErrorLogCount(int newErrorLogCount)
        {
            this._errorLogCount = newErrorLogCount;
        }

        private BuildLogMessage[] _collapsedBuildLogMessages;
        public  BuildLogMessage[] CollapsedBuildLogMessages => this._collapsedBuildLogMessages;

        public void SetCollapsedBuildLogMessages(BuildLogMessage[] newCollapsedBuildLogMessages)
        {
            this._collapsedBuildLogMessages = newCollapsedBuildLogMessages;
        }

        private int _collapsedInfoLogCount;
        private int _collapsedWarnLogCount;
        private int _collapsedErrorLogCount;

        public int CollapsedInfoLogCount  => this._collapsedInfoLogCount;
        public int CollapsedWarnLogCount  => this._collapsedWarnLogCount;
        public int CollapsedErrorLogCount => this._collapsedErrorLogCount;

        public void IncrementCollapsedInfoLogCount()
        {
            ++this._collapsedInfoLogCount;
        }

        public void IncrementCollapsedWarnLogCount()
        {
            ++this._collapsedWarnLogCount;
        }

        public void IncrementCollapsedErrorLogCount()
        {
            ++this._collapsedErrorLogCount;
        }

        public void SetCollapsedInfoLogCount(int newInfoLogCount)
        {
            this._collapsedInfoLogCount = newInfoLogCount;
        }

        public void SetCollapsedWarnLogCount(int newWarnLogCount)
        {
            this._collapsedWarnLogCount = newWarnLogCount;
        }

        public void SetCollapsedErrorLogCount(int newErrorLogCount)
        {
            this._collapsedErrorLogCount = newErrorLogCount;
        }

        private TimeSpan _duration;

        [System.Xml.Serialization.XmlIgnore] public TimeSpan Duration { get => this._duration; set => this._duration = value; }

        [System.Xml.Serialization.XmlElement("Duration")] public long DurationTicks { get => this._duration.Ticks; set => this._duration = new(value); }
    }

    [Serializable]
    public struct BuildLogMessage
    {
        public string LogType;
        public string Message;

        private int _count;
        public  int Count => this._count;

        public void SetCount(int newCount)
        {
            this._count = newCount;
        }
    }
}