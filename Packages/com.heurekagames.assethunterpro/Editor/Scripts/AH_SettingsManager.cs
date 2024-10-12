using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SettingsManager
    {
        private static readonly AH_SettingsManager instance = new();

        #region singleton

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AH_SettingsManager()
        {
            instance.Init();
        }

        private AH_SettingsManager()
        {
        }

        public static AH_SettingsManager Instance => instance;

        #endregion

        public delegate void IgnoreListUpdatedHandler();

        public event IgnoreListUpdatedHandler IgnoreListUpdatedEvent;

        #region Fields

        [SerializeField] private int ignoredListChosenIndex;

        private static readonly string ProjectPostFix = "." + Application.dataPath;

        private static readonly string PrefsAutoCreateLog     = "AH.AutoCreateLog" + ProjectPostFix;
        private static readonly string PrefsAutoOpenLog       = "AH.AutoOpenLog" + ProjectPostFix;
        private static readonly string PrefsAutoRefreshLog    = "AH.AutoRefreshLog" + ProjectPostFix;
        private static readonly string PrefsEstimateAssetSize = "AH.PrefsEstimateAssetSize" + ProjectPostFix;

        private static readonly string PrefsHideButtonText      = "AH.HideButtonText" + ProjectPostFix;
        private static readonly string PrefsHideNewsButton      = "AH.HideNewsButton" + ProjectPostFix;
        private static readonly string PrefsIgnoreScriptFiles   = "AH.IgnoreScriptfiles" + ProjectPostFix;
        private static readonly string PrefsIgnoredTypes        = "AH.DefaultIgnoredTypes" + ProjectPostFix;
        private static readonly string PrefsIgnoredPathEndsWith = "AH.IgnoredPathEndsWith" + ProjectPostFix;
        private static readonly string PrefsIgnoredExtensions   = "AH.IgnoredExtensions" + ProjectPostFix;
        private static readonly string PrefsIgnoredFiles        = "AH.IgnoredFiles" + ProjectPostFix;
        private static readonly string PrefsIgnoredFolders      = "AH.IgnoredFolders" + ProjectPostFix;
        private static readonly string PrefsUserPrefPath        = "AH.UserPrefPath" + ProjectPostFix;
        private static readonly string PrefsBuildInfoPath       = "AH.BuildInfoPath" + ProjectPostFix;

        internal static readonly bool   InitialValueAutoCreateLog     = true;
        internal static readonly bool   InitialValueAutoOpenLog       = false;
        internal static readonly bool   InitialValueAutoRefreshLog    = false;
        internal static readonly bool   InitialValueEstimateAssetSize = false;
        internal static readonly bool   InitialValueHideButtonText    = true;
        internal static readonly bool   InitialValueHideNewsButton    = false;
        internal static readonly bool   InitialIgnoreScriptFiles      = true;
        internal static readonly string InitialUserPrefPath           = Application.dataPath + System.IO.Path.DirectorySeparatorChar + "AH_Prefs";
        internal static readonly string InitialBuildInfoPath          = System.IO.Directory.GetParent(Application.dataPath).FullName + System.IO.Path.DirectorySeparatorChar + "SerializedBuildInfo";

        //Types to Ignore by default
        #if UNITY_2017_3_OR_NEWER
        internal static readonly List<Type> InitialValueIgnoredTypes = new()
        {
            #if UNITY_2021_2_OR_NEWER
            typeof(ShaderInclude), //Have to exclude this here because Unitys AssetDatabase.GetDependencies() does not include shaderincludes for some reason :(
            #endif
            typeof(UnityEditorInternal.AssemblyDefinitionAsset)
            #if !AH_SCRIPT_ALLOW //DEFINED IN AH_PREPROCESSOR
            , typeof(MonoScript),
            #endif
        };
        #else
        internal readonly static List<Type> InitialValueIgnoredTypes = new List<Type>() {
#if !AH_SCRIPT_ALLOW //DEFINED IN AH_PREPROCESSOR
            typeof(MonoScript)
#endif
        };
        #endif

        //File extensions to Ignore by default
        internal static readonly List<string> InitialValueIgnoredExtensions = new()
        {
            ".dll",
            "." + AH_SerializationHelper.SettingsExtension,
            "." + AH_SerializationHelper.BuildInfoExtension,
        };

        //List of strings which, if contained in asset path, is ignored (Editor, Resources, etc)
        internal static readonly List<string> InitialValueIgnoredPathEndsWith = new()
        {
            string.Format("{0}heureka", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}editor", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}plugins", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}gizmos", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}editor default resources", System.IO.Path.DirectorySeparatorChar),
        };

        internal static readonly List<string> InitialValueIgnoredFiles   = new();
        internal static readonly List<string> InitialValueIgnoredFolders = new();

        [SerializeField] private AH_ExclusionTypeList ignoredListTypes;
        [SerializeField] private AH_IgnoreList        ignoredListPathEndsWith;
        [SerializeField] private AH_IgnoreList        ignoredListExtensions;
        [SerializeField] private AH_IgnoreList        ignoredListFiles;
        [SerializeField] private AH_IgnoreList        ignoredListFolders;

        #endregion

        #region Properties

        [SerializeField] public bool AutoCreateLog { get => (!EditorPrefs.HasKey(PrefsAutoCreateLog) && InitialValueAutoCreateLog) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoCreateLog)); internal set => EditorPrefs.SetInt(PrefsAutoCreateLog, AH_Utils.BoolToInt(value)); }

        [SerializeField] public bool AutoOpenLog { get => (!EditorPrefs.HasKey(PrefsAutoOpenLog) && InitialValueAutoOpenLog) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoOpenLog)); internal set => EditorPrefs.SetInt(PrefsAutoOpenLog, AH_Utils.BoolToInt(value)); }

        [SerializeField] public bool AutoRefreshLog { get => (!EditorPrefs.HasKey(PrefsAutoRefreshLog) && InitialValueAutoRefreshLog) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoRefreshLog)); internal set => EditorPrefs.SetInt(PrefsAutoRefreshLog, AH_Utils.BoolToInt(value)); }

        [SerializeField] public bool EstimateAssetSize { get => (!EditorPrefs.HasKey(PrefsEstimateAssetSize) && InitialValueEstimateAssetSize) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsEstimateAssetSize)); internal set => EditorPrefs.SetInt(PrefsEstimateAssetSize, AH_Utils.BoolToInt(value)); }

        [SerializeField] public bool HideButtonText { get => (!EditorPrefs.HasKey(PrefsHideButtonText) && InitialValueHideButtonText) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsHideButtonText)); internal set => EditorPrefs.SetInt(PrefsHideButtonText, AH_Utils.BoolToInt(value)); }

        [SerializeField] public bool HideNewsButton { get => (!EditorPrefs.HasKey(PrefsHideNewsButton) && InitialValueHideNewsButton) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsHideNewsButton)); internal set => EditorPrefs.SetInt(PrefsHideNewsButton, AH_Utils.BoolToInt(value)); }

        [SerializeField] public bool IgnoreScriptFiles { get => (!EditorPrefs.HasKey(PrefsIgnoreScriptFiles) && InitialIgnoreScriptFiles) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsIgnoreScriptFiles)); internal set => EditorPrefs.SetInt(PrefsIgnoreScriptFiles, AH_Utils.BoolToInt(value)); }

        [SerializeField]
        public string UserPreferencePath
        {
            get
            {
                if (EditorPrefs.HasKey(PrefsUserPrefPath))
                    return EditorPrefs.GetString(PrefsUserPrefPath);
                else
                    return InitialUserPrefPath;
            }
            internal set => EditorPrefs.SetString(PrefsUserPrefPath, value);
        }

        [SerializeField]
        public string BuildInfoPath
        {
            get
            {
                if (EditorPrefs.HasKey(PrefsBuildInfoPath))
                    return EditorPrefs.GetString(PrefsBuildInfoPath);
                else
                    return InitialBuildInfoPath;
            }
            internal set => EditorPrefs.SetString(PrefsBuildInfoPath, value);
        }

        public GUIContent[] GUIcontentignoredLists = new GUIContent[5]
        {
            new("Endings"),
            new("Types"),
            new("Folders"),
            new("Files"),
            new("Extentions"),
        };

        #endregion

        private void Init()
        {
            this.ignoredListPathEndsWith = new(new IgnoredEventActionPathEndsWith(0, this.onIgnoreButtonDown), InitialValueIgnoredPathEndsWith, PrefsIgnoredPathEndsWith);
            this.ignoredListTypes        = new(new IgnoredEventActionType(1, this.onIgnoreButtonDown), InitialValueIgnoredTypes, PrefsIgnoredTypes);
            this.ignoredListFolders      = new(new IgnoredEventActionFolder(2, this.onIgnoreButtonDown), InitialValueIgnoredFolders, PrefsIgnoredFolders);
            this.ignoredListFiles        = new(new IgnoredEventActionFile(3, this.onIgnoreButtonDown), InitialValueIgnoredFiles, PrefsIgnoredFiles);
            this.ignoredListExtensions   = new(new IgnoredEventActionExtension(4, this.onIgnoreButtonDown), InitialValueIgnoredExtensions, PrefsIgnoredExtensions);

            //Todo subscribing to these 5 times, means that we might refresh buildinfo 5 times when reseting...We might be able to batch that somehow
            this.ignoredListPathEndsWith.ListUpdatedEvent += this.OnListUpdatedEvent;
            this.ignoredListTypes.ListUpdatedEvent        += this.OnListUpdatedEvent;
            this.ignoredListFolders.ListUpdatedEvent      += this.OnListUpdatedEvent;
            this.ignoredListFiles.ListUpdatedEvent        += this.OnListUpdatedEvent;
            this.ignoredListExtensions.ListUpdatedEvent   += this.OnListUpdatedEvent;
        }

        private void OnListUpdatedEvent()
        {
            if (this.IgnoreListUpdatedEvent != null) this.IgnoreListUpdatedEvent();
        }

        internal void ResetAll()
        {
            this.ignoredListPathEndsWith.Reset();
            this.ignoredListTypes.Reset();
            this.ignoredListExtensions.Reset();
            this.ignoredListFiles.Reset();
            this.ignoredListFolders.Reset();

            this.AutoCreateLog      = InitialValueAutoCreateLog;
            this.AutoOpenLog        = InitialValueAutoOpenLog;
            this.AutoRefreshLog     = InitialValueAutoRefreshLog;
            this.EstimateAssetSize  = InitialValueEstimateAssetSize;
            this.HideButtonText     = InitialValueHideButtonText;
            this.IgnoreScriptFiles  = InitialIgnoreScriptFiles;
            this.UserPreferencePath = InitialUserPrefPath;
            this.BuildInfoPath      = InitialBuildInfoPath;
        }

        internal void DrawIgnored()
        {
            EditorGUILayout.HelpBox("IGNORE ASSETS" + Environment.NewLine + "-Select asset in project view to ignore", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            this.ignoredListChosenIndex = GUILayout.Toolbar(this.ignoredListChosenIndex, this.GUIcontentignoredLists);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            this.drawIgnoreButtons();

            switch (this.ignoredListChosenIndex)
            {
                case 0:
                    this.ignoredListPathEndsWith.OnGUI();
                    break;
                case 1:
                    this.ignoredListTypes.OnGUI();
                    break;
                case 2:
                    this.ignoredListFolders.OnGUI();
                    break;
                case 3:
                    this.ignoredListFiles.OnGUI();
                    break;
                case 4:
                    this.ignoredListExtensions.OnGUI();
                    break;
                default: break;
            }
        }

        private void drawIgnoreButtons()
        {
            GUILayout.Space(12);
            this.ignoredListPathEndsWith.DrawIgnoreButton();
            this.ignoredListTypes.DrawIgnoreButton();
            this.ignoredListFolders.DrawIgnoreButton();
            this.ignoredListFiles.DrawIgnoreButton();
            this.ignoredListExtensions.DrawIgnoreButton();
            GUILayout.Space(4);
        }

        //Callback from Ignore button down
        private void onIgnoreButtonDown(int exclusionIndex)
        {
            this.ignoredListChosenIndex = exclusionIndex;
        }

        //public List<Type> GetIgnoredTypes() { return ignoredListTypes.GetIgnored(); }
        public List<string> GetIgnoredPathEndsWith()
        {
            return this.ignoredListPathEndsWith.GetIgnored();
        }

        public List<string> GetIgnoredFileExtentions()
        {
            return this.ignoredListExtensions.GetIgnored();
        }

        public List<string> GetIgnoredFiles()
        {
            return this.ignoredListFiles.GetIgnored();
        }

        public List<string> GetIgnoredFolders()
        {
            return this.ignoredListFolders.GetIgnored();
        }

        private int drawSetting(string title, int value, int min, int max, string prefixAppend)
        {
            EditorGUILayout.PrefixLabel(title + prefixAppend);
            return EditorGUILayout.IntSlider(value, min, max);
        }

        internal void DrawSettings()
        {
            EditorGUILayout.HelpBox("File save locations", MessageType.None);

            this.UserPreferencePath = this.drawSettingsFolder("User prefs", this.UserPreferencePath, InitialUserPrefPath);
            this.BuildInfoPath      = this.drawSettingsFolder("Build info", this.BuildInfoPath, InitialBuildInfoPath);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Settings", MessageType.None);
            this.AutoCreateLog     = this.drawSetting("Auto create log when building", this.AutoCreateLog, InitialValueAutoCreateLog);
            this.AutoOpenLog       = this.drawSetting("Auto open log location after building", this.AutoOpenLog, InitialValueAutoOpenLog);
            this.AutoRefreshLog    = this.drawSetting("Auto refresh when project changes", this.AutoRefreshLog, InitialValueAutoRefreshLog);
            this.EstimateAssetSize = this.drawSetting("Estimate runtime filesize for each asset", this.EstimateAssetSize, InitialValueEstimateAssetSize);
            this.HideButtonText    = this.drawSetting("Hide buttontexts", this.HideButtonText, InitialValueHideButtonText);
            this.HideNewsButton    = this.drawSetting("Hide 'News' button", this.HideNewsButton, InitialValueHideNewsButton);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            this.IgnoreScriptFiles = this.drawSetting("Ignore script files", this.IgnoreScriptFiles, InitialIgnoreScriptFiles);

            if (EditorGUI.EndChangeCheck())
            {
                //ADD OR REMOVE DEFINITION FOR PREPROCESSING
                AH_PreProcessor.AddDefineSymbols(AH_PreProcessor.DefineScriptAllow, !this.IgnoreScriptFiles);
                this.ignoredListTypes.IgnoreType(typeof(MonoScript), this.IgnoreScriptFiles);

                if (!this.IgnoreScriptFiles) EditorUtility.DisplayDialog("Now detecting unused scripts", "This is an experimental feature, and it cannot promise with any certainty that script files marked as unused are indeed unused. Only works with scripts that are directly used in a scene - Use at your own risk", "Ok");
            }

            var content = new GUIContent("EXPERIMENTAL FEATURE!", EditorGUIUtility.IconContent("console.warnicon.sml").image, "Cant be 100% sure script files are usused, so you need to handle with care");
            //TODO PARTIAL CLASSES
            //INHERITANCE
            //AddComponent<Type>
            //Reflection
            //Interfaces

            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private string drawSettingsFolder(string title, string path, string defaultVal)
        {
            var validPath = path;
            var newPath   = "";

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select", GUILayout.ExpandWidth(false))) newPath = EditorUtility.OpenFolderPanel("Select folder", path, "");

            if (newPath != "") validPath = newPath;

            var content = new GUIContent(title + ": " + AH_Utils.ShrinkPathMiddle(validPath, 44), title + " is saved at " + validPath);

            GUILayout.Label(content, defaultVal != path ? EditorStyles.boldLabel : EditorStyles.label);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            return validPath;
        }

        private bool drawSetting(string title, bool value, bool defaultVal)
        {
            return EditorGUILayout.ToggleLeft(title, value, defaultVal != value ? EditorStyles.boldLabel : EditorStyles.label);
        }

        internal bool HasIgnoredFolder(string folderPath, string assetID)
        {
            var IgnoredEnding = this.ignoredListPathEndsWith.ContainsElement(folderPath, assetID);
            var folderIgnored = this.ignoredListFolders.ContainsElement(folderPath, assetID);

            return IgnoredEnding || folderIgnored;
        }

        internal void AddIgnoredFolder(string element)
        {
            this.ignoredListFolders.AddToignoredList(element);
        }

        internal void AddIgnoredAssetTypes(string element)
        {
            this.ignoredListTypes.AddToignoredList(element);
        }

        internal void AddIgnoredAssetGUIDs(string element)
        {
            this.ignoredListFiles.AddToignoredList(element);
        }

        internal bool HasIgnoredAsset(string relativePath, string assetID)
        {
            var IgnoredType      = this.ignoredListTypes.ContainsElement(relativePath, assetID);
            var IgnoredFile      = this.ignoredListFiles.ContainsElement(relativePath, assetID);
            var IgnoredExtension = this.ignoredListExtensions.ContainsElement(relativePath, assetID);

            return IgnoredType || IgnoredFile || IgnoredExtension;
        }

        internal void SaveToFile()
        {
            var path = EditorUtility.SaveFilePanel(
                "Save current settings",
                AH_SerializationHelper.GetSettingFolder(),
                "AH_UserPrefs_" + Environment.UserName,
                AH_SerializationHelper.SettingsExtension);

            if (path.Length != 0) AH_SerializationHelper.SerializeAndSaveJSON(instance, path);

            AssetDatabase.Refresh();
        }

        internal void LoadFromFile()
        {
            var path = EditorUtility.OpenFilePanel(
                "settings",
                AH_SerializationHelper.GetSettingFolder(),
                AH_SerializationHelper.SettingsExtension
            );

            if (path.Length != 0)
            {
                AH_SerializationHelper.LoadSettings(instance, path);
                this.ignoredListTypes.Save();
                this.ignoredListPathEndsWith.Save();
                this.ignoredListTypes.Save();
                this.ignoredListExtensions.Save();
                this.ignoredListFolders.Save();
            }
        }
    }
}