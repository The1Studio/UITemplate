#if THEONE_LOCALIZATION
using UnityEditor;
using UnityEngine;

namespace TheOne.Tool.Localization
{
    public static class LocalizationMenuItems
    {
        [MenuItem("Tools/TheOne/Localization/Open Dashboard", priority = 1)]
        private static void OpenDashboard()
        {
            EditorWindow.GetWindow<SimpleLocalizationDashboard>("Simple Localization Dashboard").Show();
        }

        [MenuItem("Tools/TheOne/Localization/Create Config", priority = 11)]
        private static void CreateConfig()
        {
            var config = ScriptableObject.CreateInstance<AutoLocalizationConfig>();

            var path = EditorUtility.SaveFilePanelInProject(
                "Create Auto Localization Config",
                "AutoLocalizationConfig",
                "asset",
                "Choose location for the config file");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorGUIUtility.PingObject(config);
                Selection.activeObject = config;

                Debug.Log($"✅ Created AutoLocalizationConfig at: {path}");
            }
        }

        [MenuItem("Tools/TheOne/Localization/Quick Add Entry", priority = 21)]
        private static void QuickAddEntry()
        {
            var window = ScriptableObject.CreateInstance<QuickAddEntryWindow>();
            window.titleContent = new GUIContent("Quick Add Entry");
            window.ShowModal();
        }
    }

    public class QuickAddEntryWindow : EditorWindow
    {
        private string key = "";
        private string englishText = "";

        private void OnGUI()
        {
            this.minSize = new Vector2(400, 200);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Quick Add Localization Entry", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            this.key = EditorGUILayout.TextField("Key:", this.key);
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("English Text:");
            this.englishText = EditorGUILayout.TextArea(this.englishText, GUILayout.Height(60));

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = !string.IsNullOrEmpty(this.key) && !string.IsNullOrEmpty(this.englishText);
            if (GUILayout.Button("Add Entry", GUILayout.Height(30)))
            {
                AutoLocalizationManager.AddLocalizationEntry(this.key, this.englishText);
                this.key = "";
                this.englishText = "";
            }
            GUI.enabled = true;

            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                this.Close();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Tip: You can also right-click on Text components and select 'Add to Localization' for faster workflow!",
                MessageType.Info);
        }
    }
}
#endif