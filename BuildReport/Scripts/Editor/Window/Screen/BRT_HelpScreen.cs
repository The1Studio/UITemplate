using UnityEngine;
using UnityEditor;

namespace BuildReportTool.Window.Screen
{
    public class Help : BaseScreen
    {
        public override string Name => Labels.HELP_CATEGORY_LABEL;

        private const int LABEL_LENGTH = 16000;

        public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData, UnityBuildReport unityBuildReport)
        {
            const string README_FILENAME = "README.txt";
            var          readmeContents  = Util.GetPackageFileContents(README_FILENAME);

            const string CHANGELOG_FILENAME = "VERSION.txt";
            var          changelogContents  = Util.GetPackageFileContents(CHANGELOG_FILENAME);

            if (!string.IsNullOrEmpty(readmeContents) && readmeContents.Length > LABEL_LENGTH) readmeContents = readmeContents.Substring(0, LABEL_LENGTH);

            if (!string.IsNullOrEmpty(changelogContents) && changelogContents.Length > LABEL_LENGTH) changelogContents = changelogContents.Substring(0, LABEL_LENGTH);

            if (this._readmeGuiContent == null) this._readmeGuiContent = new();
            if (!string.IsNullOrEmpty(readmeContents))
                this._readmeGuiContent.text = readmeContents;
            else
                this._readmeGuiContent.text = "README.txt not found";
            this._needToUpdateReadmeHeight = true;

            if (this._changelogGuiContent == null) this._changelogGuiContent = new();
            if (!string.IsNullOrEmpty(changelogContents))
                this._changelogGuiContent.text = changelogContents;
            else
                this._changelogGuiContent.text = "VERSION.txt not found";
            this._needToUpdateChangelogHeight = true;
        }

        private static readonly GUILayoutOption[] ButtonsLayout = { GUILayout.Width(230), GUILayout.Height(60) };

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
            requestRepaint = false;

            var helpTextStyle                        = GUI.skin.FindStyle(HELP_CONTENT_GUI_STYLE);
            if (helpTextStyle == null) helpTextStyle = GUI.skin.label;

            if (this._needToUpdateReadmeHeight)
            {
                this._readmeHeight             = helpTextStyle.CalcHeight(this._readmeGuiContent, HELP_CONTENT_WIDTH);
                this._needToUpdateReadmeHeight = false;
            }

            if (this._needToUpdateChangelogHeight)
            {
                this._changelogHeight             = helpTextStyle.CalcHeight(this._changelogGuiContent, HELP_CONTENT_WIDTH);
                this._needToUpdateChangelogHeight = false;
            }

            GUI.SetNextControlName("BRT_HelpUnfocuser");
            GUI.TextField(new(-100, -100, 10, 10), "");

            GUILayout.Space(10); // extra top padding

            GUILayout.BeginHorizontal();
            var newSelectedHelpIdx = GUILayout.SelectionGrid(this._selectedHelpContentsIdx, this._helpTypeLabels, 1, ButtonsLayout);

            if (newSelectedHelpIdx != this._selectedHelpContentsIdx) GUI.FocusControl("BRT_HelpUnfocuser");

            this._selectedHelpContentsIdx = newSelectedHelpIdx;

            //GUILayout.Space((position.width - HELP_CONTENT_WIDTH) * 0.5f);

            if (this._selectedHelpContentsIdx == HELP_TYPE_README_IDX)
            {
                this._readmeScrollPos = GUILayout.BeginScrollView(this._readmeScrollPos);

                EditorGUILayout.SelectableLabel(this._readmeGuiContent.text,
                    helpTextStyle,
                    GUILayout.Width(HELP_CONTENT_WIDTH),
                    GUILayout.Height(this._readmeHeight));

                GUILayout.EndScrollView();
            }
            else if (this._selectedHelpContentsIdx == HELP_TYPE_CHANGELOG_IDX)
            {
                this._changelogScrollPos = GUILayout.BeginScrollView(this._changelogScrollPos);

                EditorGUILayout.SelectableLabel(this._changelogGuiContent.text,
                    helpTextStyle,
                    GUILayout.Width(HELP_CONTENT_WIDTH),
                    GUILayout.Height(this._changelogHeight));

                GUILayout.EndScrollView();
            }

            GUILayout.EndHorizontal();
        }

        private       int _selectedHelpContentsIdx;
        private const int HELP_TYPE_README_IDX    = 0;
        private const int HELP_TYPE_CHANGELOG_IDX = 1;

        private const string HELP_CONTENT_GUI_STYLE = "label";
        private const int    HELP_CONTENT_WIDTH     = 500;

        private readonly string[] _helpTypeLabels = { "Help (README)", "Version Changelog" };

        private Vector2 _readmeScrollPos;
        private float   _readmeHeight;
        private bool    _needToUpdateReadmeHeight;

        private Vector2 _changelogScrollPos;
        private float   _changelogHeight;
        private bool    _needToUpdateChangelogHeight;

        private GUIContent _readmeGuiContent;
        private GUIContent _changelogGuiContent;
    }
}