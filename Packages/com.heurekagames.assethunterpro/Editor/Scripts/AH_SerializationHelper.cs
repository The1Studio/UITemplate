using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    internal class AH_SerializationHelper
    {
        public delegate void NewBuildInfoCreatedDelegate(string path);

        public static NewBuildInfoCreatedDelegate NewBuildInfoCreated;

        public const string BuildInfoExtension = "ahbuildinfo";
        public const string SettingsExtension  = "ahsetting";
        public const string FileDumpExtension  = "ahfiledump";

        public const string DateTimeFormat = "yyyy_MM_dd_HH_mm_ss";

        internal static void SerializeAndSave(AH_SerializedBuildInfo ahBuildInfo)
        {
            var buildinfoFileName = ahBuildInfo.buildTargetInfo + "_" + ahBuildInfo.dateTime + "." + BuildInfoExtension;
            var filePath          = GetBuildInfoFolder() + System.IO.Path.DirectorySeparatorChar + buildinfoFileName;
            System.IO.Directory.CreateDirectory(GetBuildInfoFolder());

            System.IO.File.WriteAllText(filePath, JsonUtility.ToJson(ahBuildInfo));
            if (AH_SettingsManager.Instance.AutoOpenLog) EditorUtility.RevealInFinder(filePath);

            if (NewBuildInfoCreated != null) NewBuildInfoCreated(filePath);
        }

        internal static string GetDateString()
        {
            return DateTime.Now.ToString(DateTimeFormat);
        }

        internal static void SerializeAndSaveJSON(object instance, string path)
        {
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(instance));
        }

        internal static void SerializeAndSaveCSV(AH_ElementList elementList, string path)
        {
            var rowData = new List<string[]>();

            // Creating First row of titles manually..
            var rowDataTemp = new string[5];
            rowDataTemp[0] = "GUID";
            rowDataTemp[1] = "Path";
            rowDataTemp[2] = "Bytes";
            rowDataTemp[3] = "UsedInBuild";
            rowDataTemp[4] = "RefScenes";
            rowData.Add(rowDataTemp);

            foreach (var item in elementList.elements)
            {
                rowDataTemp    = new string[5];
                rowDataTemp[0] = item.GUID;
                rowDataTemp[1] = item.relativePath;
                rowDataTemp[2] = item.fileSize.ToString();
                rowDataTemp[3] = item.usedInBuild.ToString();
                rowDataTemp[4] = item.scenesReferencingAsset != null ? string.Join(",", item.scenesReferencingAsset) : "";
                rowData.Add(rowDataTemp);
            }

            var output = new string[rowData.Count][];

            for (var i = 0; i < output.Length; i++) output[i] = rowData[i];

            var length    = output.GetLength(0);
            var delimiter = ",";

            var sb = new StringBuilder();

            for (var index = 0; index < length; index++) sb.AppendLine(string.Join(delimiter, output[index]));

            System.IO.File.WriteAllText(path, sb.ToString());
        }

        internal static AH_SerializedBuildInfo LoadBuildReport(string path)
        {
            var fileContent = "";
            try
            {
                fileContent = System.IO.File.ReadAllText(path);
            }
            catch (System.IO.FileNotFoundException e)
            {
                EditorUtility.DisplayDialog(
                    "File Not Found",
                    "Unable to find:" + Environment.NewLine + path,
                    "Ok");

                Debug.LogError("AH: Unable to find: " + path + Environment.NewLine + e);

                return null;
            }

            try
            {
                var buildInfo = JsonUtility.FromJson<AH_SerializedBuildInfo>(fileContent);
                buildInfo.Sort();
                return buildInfo;
            }
            catch (Exception e)
            {
                Debug.LogError("AH: JSON Parse error of " + path + Environment.NewLine + "- " + e.ToString());
                return null;
            }
        }

        internal static string GetBuildInfoFolder()
        {
            return AH_SettingsManager.Instance.BuildInfoPath;
        }

        internal static string GetSettingFolder()
        {
            var userpreferencesPath = AH_SettingsManager.Instance.UserPreferencePath;
            var dirInfo             = System.IO.Directory.CreateDirectory(userpreferencesPath);
            return dirInfo.FullName;
        }

        internal static string GetBackupFolder()
        {
            return System.IO.Directory.GetParent(Application.dataPath).FullName;
        }

        internal static void LoadSettings(AH_SettingsManager instance, string path)
        {
            var text = System.IO.File.ReadAllText(path);
            try
            {
                EditorJsonUtility.FromJsonOverwrite(text, instance);
            }
            catch (Exception e)
            {
                Debug.LogError("AH: JSON Parse error of " + path + Environment.NewLine + "- " + e.ToString());
            }
        }
    }
}