using HeurekaGames.Utils;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SettingsWindow : EditorWindow
    {
        private const  string            WINDOWNAME = "AH Settings";
        private        Vector2           scrollPos;
        private static AH_SettingsWindow m_window;

        [MenuItem("Tools/Asset Hunter PRO/Settings")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Settings")]
        public static void OpenAssetHunter()
        {
            Init(false);
        }

        public static void Init(bool attemptDock, Docker.DockPosition dockPosition = Docker.DockPosition.Right)
        {
            var firstInit = m_window == null;

            m_window                    = GetWindow<AH_SettingsWindow>(WINDOWNAME, true);
            m_window.titleContent.image = AH_EditorData.Icons.Settings;

            var mainWindows = Resources.FindObjectsOfTypeAll<AH_Window>();
            if (attemptDock && mainWindows.Length != 0 && firstInit) Docker.Dock(mainWindows[0], m_window, dockPosition);
        }

        private void OnInspectorUpdate()
        {
            this.Repaint();
        }

        private void OnGUI()
        {
            if (!m_window) Init(true);

            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_dBlue, "SETTINGS");

            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset Settings"))
                if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset Settings completely", "OK", "CANCEL"))
                    AH_SettingsManager.Instance.ResetAll();
            if (GUILayout.Button("Save prefs to file")) AH_SettingsManager.Instance.SaveToFile();
            if (GUILayout.Button("Load prefs from file")) AH_SettingsManager.Instance.LoadFromFile();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            AH_SettingsManager.Instance.DrawSettings();

            EditorGUILayout.Space();

            AH_SettingsManager.Instance.DrawIgnored();

            EditorGUILayout.EndScrollView();
        }
    }
}