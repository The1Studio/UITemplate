using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using HeurekaGames.Utils;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SceneReferenceWindow : EditorWindow
    {
        private static AH_SceneReferenceWindow m_window;
        private        Vector2                 scrollPos;

        [SerializeField] private float btnMinWidthSmall = 50;

        private List<string> m_allScenesInProject;
        private List<string> m_allScenesInBuildSettings;
        private List<string> m_allEnabledScenesInBuildSettings;
        private List<string> m_allUnreferencedScenes;
        private List<string> m_allDisabledScenesInBuildSettings;

        private static readonly string WINDOWNAME = "AH Scenes";

        [MenuItem("Tools/Asset Hunter PRO/Scene overview")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Scene overview")]
        public static void Init()
        {
            m_window                    = GetWindow<AH_SceneReferenceWindow>(WINDOWNAME, true, typeof(AH_Window));
            m_window.titleContent.image = AH_EditorData.Icons.Scene;
            m_window.GetSceneInfo();
        }

        private void GetSceneInfo()
        {
            this.m_allScenesInProject               = AH_Utils.GetAllSceneNames().ToList<string>();
            this.m_allScenesInBuildSettings         = AH_Utils.GetAllSceneNamesInBuild().ToList<string>();
            this.m_allEnabledScenesInBuildSettings  = AH_Utils.GetEnabledSceneNamesInBuild().ToList<string>();
            this.m_allDisabledScenesInBuildSettings = this.SubtractSceneArrays(this.m_allScenesInBuildSettings, this.m_allEnabledScenesInBuildSettings);
            this.m_allUnreferencedScenes            = this.SubtractSceneArrays(this.m_allScenesInProject, this.m_allScenesInBuildSettings);
        }

        //Get the subset of scenes where we subtract "secondary" from "main"
        private List<string> SubtractSceneArrays(List<string> main, List<string> secondary)
        {
            return main.Except<string>(secondary).ToList<string>();
        }

        private void OnFocus()
        {
            this.GetSceneInfo();
        }

        private void OnGUI()
        {
            if (!m_window) Init();

            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Dark, "SCENE REFERENCES");

            //Show all used types
            EditorGUILayout.BeginVertical();

            //Make sure this window has focus to update contents
            this.Repaint();

            if (this.m_allEnabledScenesInBuildSettings.Count == 0) Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Icons.IconLargeWhite, 310f, 110f, "There are no enabled scenes in build settings");

            this.drawScenes("These scenes are added and enabled in build settings", this.m_allEnabledScenesInBuildSettings);
            this.drawScenes("These scenes are added to build settings but disabled", this.m_allDisabledScenesInBuildSettings);
            this.drawScenes("These scenes are not referenced anywhere in build settings", this.m_allUnreferencedScenes);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void drawScenes(string headerMsg, List<string> scenes)
        {
            if (scenes.Count > 0)
            {
                EditorGUILayout.HelpBox(headerMsg, MessageType.Info);
                foreach (var scenePath in scenes)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Ping", GUILayout.Width(this.btnMinWidthSmall)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(UnityEngine.Object));
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    }
                    EditorGUILayout.LabelField(scenePath);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Separator();
            }
        }
    }
}