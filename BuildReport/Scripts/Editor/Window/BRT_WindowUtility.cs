using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BuildReportTool.Window
{
    public static class Utility
    {
        public static void DrawCentralMessage(Rect position, string msg)
        {
            float w = 300;
            float h = 100;
            var   x = (position.width - w) * 0.5f;
            var   y = (position.height - h) * 0.25f;

            GUI.Label(new(x, y, w, h), msg);
        }

        public static void PingSelectedAssets(AssetList list)
        {
            var newSelection = new List<Object>(list.GetSelectedCount());

            var iterator = list.GetSelectedEnumerator();
            while (iterator.MoveNext())
            {
                var loadedObject =
                    AssetDatabase.LoadAssetAtPath(iterator.Current.Key, typeof(Object));
                if (loadedObject != null) newSelection.Add(loadedObject);
            }

            Selection.objects = newSelection.ToArray();
        }

        public static void PingAssetInProject(string file)
        {
            if (string.IsNullOrEmpty(file)) return;

            if (!file.StartsWith("Assets/") && !file.StartsWith("Packages/")) return;

            // thanks to http://answers.unity3d.com/questions/37180/how-to-highlight-or-select-an-asset-in-project-win.html
            var asset = AssetDatabase.LoadMainAssetAtPath(file);
            if (asset != null)
            {
                var temp = GUI.skin;
                GUI.skin = null;

                //EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(file, typeof(Object)));
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
                EditorUtility.FocusProjectWindow();

                GUI.skin = temp;
            }
        }

        public static string GetProperBuildSizeDesc(BuildInfo buildReportToDisplay)
        {
            var buildPlatform =
                ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
                    buildReportToDisplay.BuildTargetUsed);

            switch (buildPlatform)
            {
                case BuildPlatform.MacOSX32:        return Labels.BUILD_SIZE_MACOSX_DESC;
                case BuildPlatform.MacOSX64:        return Labels.BUILD_SIZE_MACOSX_DESC;
                case BuildPlatform.MacOSXUniversal: return Labels.BUILD_SIZE_MACOSX_DESC;

                case BuildPlatform.Windows32: return Labels.BUILD_SIZE_WINDOWS_DESC;
                case BuildPlatform.Windows64: return Labels.BUILD_SIZE_WINDOWS_DESC;

                case BuildPlatform.Linux32:        return Labels.BUILD_SIZE_STANDALONE_DESC;
                case BuildPlatform.Linux64:        return Labels.BUILD_SIZE_STANDALONE_DESC;
                case BuildPlatform.LinuxUniversal: return Labels.BUILD_SIZE_LINUX_UNIVERSAL_DESC;

                case BuildPlatform.Android:
                    if (buildReportToDisplay.AndroidCreateProject) return Labels.BUILD_SIZE_ANDROID_WITH_PROJECT_DESC;

                    if (buildReportToDisplay.AndroidUseAPKExpansionFiles) return Labels.BUILD_SIZE_ANDROID_WITH_OBB_DESC;

                    return Labels.BUILD_SIZE_ANDROID_DESC;

                case BuildPlatform.iOS: return Labels.BUILD_SIZE_IOS_DESC;

                case BuildPlatform.Web: return Labels.BUILD_SIZE_WEB_DESC;
            }

            return "";
        }

        public static void DrawLargeSizeDisplay(string label, string desc, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            var labelStyle                     = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (labelStyle == null) labelStyle = GUI.skin.label;

            var descStyle                    = GUI.skin.FindStyle(Settings.TINY_HELP_STYLE_NAME);
            if (descStyle == null) descStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.BIG_NUMBER_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            GUILayout.BeginVertical();
            GUILayout.Label(label, labelStyle);
            GUILayout.Label(desc, descStyle);
            GUILayout.Label(value, valueStyle);
            GUILayout.EndVertical();
        }
    }
}