using UnityEngine;

namespace BuildReportTool.Window.Screen
{
    public class ExtraData : BaseScreen
    {
        private Vector2 _scrollPos = Vector2.zero;

        public override string Name => Labels.EXTRA_DATA_CATEGORY_LABEL;

        public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData, UnityBuildReport unityBuildReport)
        {
        }

        public override void DrawGUI(
            Rect                      position,
            BuildInfo                 buildReportToDisplay,
            AssetDependencies         assetDependencies,
            TextureData               textureData,
            MeshData                  meshData,
            UnityBuildReport          unityBuildReport,
            BuildReportTool.ExtraData extraData,
            out bool                  requestRepaint
        )
        {
            if (buildReportToDisplay == null || string.IsNullOrEmpty(extraData.Contents))
            {
                requestRepaint = false;
                return;
            }

            requestRepaint = false;

            var textStyle                    = GUI.skin.FindStyle(Settings.SETTING_VALUE_STYLE_NAME);
            if (textStyle == null) textStyle = GUI.skin.label;

            GUILayout.Space(2); // top padding for scrollbar

            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10); // extra left padding

            GUILayout.BeginVertical();

            GUILayout.Space(10); // top padding

            GUILayout.Label(extraData.Contents, textStyle);

            GUILayout.Space(10); // bottom padding

            GUILayout.EndVertical();

            GUILayout.Space(20); // extra right padding
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }
    }
}