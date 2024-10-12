using System.Globalization;
using UnityEngine;

namespace BuildReportTool.Window.Screen
{
    public class SizeStats : BaseScreen
    {
        public override string Name => Labels.SIZE_STATS_CATEGORY_LABEL;

        public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData, UnityBuildReport unityBuildReport)
        {
        }

        private Vector2 _assetListScrollPos;

        private bool _hasTotalBuildSize;
        private bool _hasUsedAssetsTotalSize;
        private bool _hasBuildSizes;
        private bool _hasCompressedBuildSize;
        private bool _hasMonoDLLsToDisplay;
        private bool _hasUnityEngineDLLsToDisplay;
        private bool _hasScriptDLLsToDisplay;

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
            if (buildReportToDisplay == null)
            {
                requestRepaint = false;
                return;
            }

            requestRepaint = false;

            if (Event.current.type == EventType.Layout)
            {
                this._hasTotalBuildSize = !string.IsNullOrEmpty(buildReportToDisplay.TotalBuildSize) && !string.IsNullOrEmpty(buildReportToDisplay.BuildFilePath);

                this._hasUsedAssetsTotalSize = !string.IsNullOrEmpty(buildReportToDisplay.UsedTotalSize);
                this._hasCompressedBuildSize = !string.IsNullOrEmpty(buildReportToDisplay.CompressedBuildSize);
                this._hasBuildSizes          = buildReportToDisplay.BuildSizes != null;
                this._hasMonoDLLsToDisplay   = buildReportToDisplay.MonoDLLs != null && buildReportToDisplay.MonoDLLs.Length > 0;

                this._hasUnityEngineDLLsToDisplay = buildReportToDisplay.UnityEngineDLLs != null && buildReportToDisplay.UnityEngineDLLs.Length > 0;

                this._hasScriptDLLsToDisplay =
                    buildReportToDisplay.ScriptDLLs != null && buildReportToDisplay.ScriptDLLs.Length > 0;
            }

            GUILayout.Space(2); // top padding for scrollbar

            this._assetListScrollPos = GUILayout.BeginScrollView(this._assetListScrollPos);

            GUILayout.Space(10); // top padding for content

            GUILayout.BeginHorizontal();
            GUILayout.Space(10); // extra left padding

            this.DrawTotalSize(buildReportToDisplay);

            GUILayout.Space(Settings.CATEGORY_HORIZONTAL_SPACING);
            GUILayout.BeginVertical();

            this.DrawBuildSizes(buildReportToDisplay);

            GUILayout.Space(Settings.CATEGORY_VERTICAL_SPACING);

            this.DrawDLLList(buildReportToDisplay);

            GUILayout.EndVertical();
            GUILayout.Space(20); // extra right padding
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        private void DrawTotalSize(BuildInfo buildReportToDisplay)
        {
            GUILayout.BeginVertical();

            var bigLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (bigLabelStyle == null) bigLabelStyle = GUI.skin.label;

            var descStyle                    = GUI.skin.FindStyle(Settings.TINY_HELP_STYLE_NAME);
            if (descStyle == null) descStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.BIG_NUMBER_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            if (buildReportToDisplay.HasOldSizeValues)
            {
                // in old sizes:
                // TotalBuildSize is really the used assets size
                // CompressedBuildSize if present is the total build size

                Utility.DrawLargeSizeDisplay(Labels.USED_TOTAL_SIZE_LABEL,
                    Labels.USED_TOTAL_SIZE_DESC,
                    buildReportToDisplay.TotalBuildSize);
                GUILayout.Space(40);
                Utility.DrawLargeSizeDisplay(Labels.BUILD_TOTAL_SIZE_LABEL,
                    Utility.GetProperBuildSizeDesc(buildReportToDisplay),
                    buildReportToDisplay.CompressedBuildSize);
            }
            else
            {
                // Total Build Size
                if (this._hasTotalBuildSize)
                {
                    GUILayout.BeginVertical();

                    var buildPlatform =
                        ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
                            buildReportToDisplay.BuildTargetUsed);

                    GUILayout.Label(
                        buildPlatform == BuildPlatform.iOS ? Labels.BUILD_XCODE_SIZE_LABEL : Labels.BUILD_TOTAL_SIZE_LABEL,
                        bigLabelStyle);

                    GUILayout.Label(Util.GetBuildSizePathDescription(buildReportToDisplay),
                        descStyle);

                    GUILayout.Label(buildReportToDisplay.TotalBuildSize, valueStyle);
                    GUILayout.EndVertical();

                    this.DrawAuxiliaryBuildSizes(buildReportToDisplay);
                    GUILayout.Space(40);
                }

                // Used Assets
                if (this._hasUsedAssetsTotalSize)
                {
                    Utility.DrawLargeSizeDisplay(Labels.USED_TOTAL_SIZE_LABEL,
                        Labels.USED_TOTAL_SIZE_DESC,
                        buildReportToDisplay.UsedTotalSize);
                    GUILayout.Space(40);
                }

                // Unused Assets
                if (buildReportToDisplay.UnusedAssetsIncludedInCreation)
                {
                    Utility.DrawLargeSizeDisplay(Labels.UNUSED_TOTAL_SIZE_LABEL,
                        Labels.UNUSED_TOTAL_SIZE_DESC,
                        buildReportToDisplay.UnusedTotalSize);

                    if (buildReportToDisplay.ProcessUnusedAssetsInBatches)
                    {
                        GUILayout.Space(10);

                        GUILayout.BeginHorizontal();
                        var warning = GUI.skin.FindStyle("Icon-Warning");
                        if (warning != null)
                        {
                            var warningIcon = warning.normal.background;

                            var iconWidth  = warning.fixedWidth;
                            var iconHeight = warning.fixedHeight;

                            GUI.DrawTexture(GUILayoutUtility.GetRect(iconWidth, iconHeight), warningIcon);
                        }
                        GUILayout.Label(string.Format(Labels.UNUSED_TOTAL_IS_FROM_BATCH, buildReportToDisplay.UnusedAssetsEntriesPerBatch), descStyle);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawAuxiliaryBuildSizes(BuildInfo buildReportToDisplay)
        {
            var buildPlatform =
                ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
                    buildReportToDisplay.BuildTargetUsed);

            var bigLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (bigLabelStyle == null) bigLabelStyle = GUI.skin.label;

            var medLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_SUBTITLE_BOLD_STYLE_NAME);
            if (medLabelStyle == null) medLabelStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.BIG_NUMBER_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            if (buildPlatform == BuildPlatform.Web)
            {
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                GUILayout.Label(Labels.WEB_UNITY3D_FILE_SIZE_LABEL, medLabelStyle);
                GUILayout.Label(buildReportToDisplay.WebFileBuildSize, valueStyle);
                GUILayout.EndVertical();
            }
            else if (buildPlatform == BuildPlatform.Android)
            {
                if (!buildReportToDisplay.AndroidCreateProject && buildReportToDisplay.AndroidUseAPKExpansionFiles)
                {
                    GUILayout.Space(20);
                    GUILayout.BeginVertical();
                    GUILayout.Label(Labels.ANDROID_APK_FILE_SIZE_LABEL, medLabelStyle);
                    GUILayout.Label(buildReportToDisplay.AndroidApkFileBuildSize, bigLabelStyle);
                    GUILayout.EndVertical();

                    GUILayout.Space(20);
                    GUILayout.BeginVertical();
                    GUILayout.Label(Labels.ANDROID_OBB_FILE_SIZE_LABEL, medLabelStyle);
                    GUILayout.Label(buildReportToDisplay.AndroidObbFileBuildSize, bigLabelStyle);
                    GUILayout.EndVertical();
                }
                else if (buildReportToDisplay.AndroidCreateProject && buildReportToDisplay.AndroidUseAPKExpansionFiles)
                {
                    GUILayout.Space(20);
                    GUILayout.BeginVertical();
                    GUILayout.Label(Labels.ANDROID_OBB_FILE_SIZE_LABEL, medLabelStyle);
                    GUILayout.Label(buildReportToDisplay.AndroidObbFileBuildSize, bigLabelStyle);
                    GUILayout.EndVertical();
                }
            }

            // Streaming Assets
            if (buildReportToDisplay.HasStreamingAssets)
            {
                GUILayout.Space(20);
                Utility.DrawLargeSizeDisplay(Labels.STREAMING_ASSETS_TOTAL_SIZE_LABEL,
                    Labels.STREAMING_ASSETS_SIZE_DESC,
                    buildReportToDisplay.StreamingAssetsSize);
            }
        }

        private void DrawBuildSizes(BuildInfo buildReportToDisplay)
        {
            if (this._hasCompressedBuildSize) GUILayout.BeginVertical();

            var bigLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (bigLabelStyle == null) bigLabelStyle = GUI.skin.label;

            var medLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_SUBTITLE_BOLD_STYLE_NAME);
            if (medLabelStyle == null) medLabelStyle = GUI.skin.label;

            var labelStyle                     = GUI.skin.FindStyle(Settings.INFO_SUBTITLE_STYLE_NAME);
            if (labelStyle == null) labelStyle = GUI.skin.label;

            GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_LABEL, bigLabelStyle);

            if (this._hasCompressedBuildSize)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_MSG_PRE_BOLD, labelStyle);
                GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_MSG_BOLD, medLabelStyle);
                GUILayout.Label(Labels.TOTAL_SIZE_BREAKDOWN_MSG_POST_BOLD, labelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            if (this._hasBuildSizes)
            {
                GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);

                this.DrawNames(buildReportToDisplay.BuildSizes);
                this.DrawReadableSizes(buildReportToDisplay.BuildSizes);
                this.DrawPercentages(buildReportToDisplay.BuildSizes);

                GUILayout.EndHorizontal();
            }
        }

        private void DrawDLLList(BuildInfo buildReportToDisplay)
        {
            var buildPlatform =
                ReportGenerator.GetBuildPlatformFromString(buildReportToDisplay.BuildType,
                    buildReportToDisplay.BuildTargetUsed);

            var bigLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (bigLabelStyle == null) bigLabelStyle = GUI.skin.label;

            GUILayout.BeginHorizontal();

            // column 1
            GUILayout.BeginVertical();
            if (this._hasMonoDLLsToDisplay)
            {
                GUILayout.Label(Labels.MONO_DLLS_LABEL, bigLabelStyle);
                {
                    GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);
                    this.DrawNames(buildReportToDisplay.MonoDLLs);
                    this.DrawReadableSizes(buildReportToDisplay.MonoDLLs);
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(20);
            }

            if (this._hasUnityEngineDLLsToDisplay) this.DrawScriptDLLsList(buildReportToDisplay, buildPlatform);

            GUILayout.EndVertical();

            GUILayout.Space(15);

            // column 2
            GUILayout.BeginVertical();
            if (this._hasUnityEngineDLLsToDisplay)
            {
                GUILayout.Label(Labels.UNITY_ENGINE_DLLS_LABEL, bigLabelStyle);
                {
                    GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);
                    this.DrawNames(buildReportToDisplay.UnityEngineDLLs);
                    this.DrawReadableSizes(buildReportToDisplay.UnityEngineDLLs);
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                this.DrawScriptDLLsList(buildReportToDisplay, buildPlatform);
            }

            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawScriptDLLsList(
            BuildInfo     buildReportToDisplay,
            BuildPlatform buildPlatform
        )
        {
            if (!this._hasScriptDLLsToDisplay) return;

            var bigLabelStyle                        = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (bigLabelStyle == null) bigLabelStyle = GUI.skin.label;

            GUILayout.Label(Labels.SCRIPT_DLLS_LABEL, bigLabelStyle);
            {
                GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutMaxWidth500);
                this.DrawNames(buildReportToDisplay.ScriptDLLs);
                this.DrawReadableSizes(buildReportToDisplay.ScriptDLLs);
                GUILayout.EndHorizontal();
            }
        }

        private void DrawNames(SizePart[] list)
        {
            if (list == null) return;

            var listNormalStyle                          = GUI.skin.FindStyle(Settings.LIST_NORMAL_STYLE_NAME);
            if (listNormalStyle == null) listNormalStyle = GUI.skin.label;

            var listAltStyle                       = GUI.skin.FindStyle(Settings.LIST_NORMAL_ALT_STYLE_NAME);
            if (listAltStyle == null) listAltStyle = GUI.skin.label;

            GUILayout.BeginVertical();
            var useAlt = false;
            foreach (var b in list)
            {
                if (b.IsTotal) continue;
                var styleToUse = useAlt ? listAltStyle : listNormalStyle;
                GUILayout.Label(b.Name, styleToUse);
                useAlt = !useAlt;
            }

            GUILayout.EndVertical();
        }

        private void DrawReadableSizes(SizePart[] list)
        {
            if (list == null) return;

            var listNormalStyle                          = GUI.skin.FindStyle(Settings.LIST_NORMAL_STYLE_NAME);
            if (listNormalStyle == null) listNormalStyle = GUI.skin.label;

            var listAltStyle                       = GUI.skin.FindStyle(Settings.LIST_NORMAL_ALT_STYLE_NAME);
            if (listAltStyle == null) listAltStyle = GUI.skin.label;

            GUILayout.BeginVertical();
            var useAlt = false;
            foreach (var b in list)
            {
                if (b.IsTotal) continue;
                var styleToUse = useAlt ? listAltStyle : listNormalStyle;
                GUILayout.Label(b.Size, styleToUse);
                useAlt = !useAlt;
            }

            GUILayout.EndVertical();
        }

        private void DrawPercentages(SizePart[] list)
        {
            if (list == null) return;

            var listNormalStyle                          = GUI.skin.FindStyle(Settings.LIST_NORMAL_STYLE_NAME);
            if (listNormalStyle == null) listNormalStyle = GUI.skin.label;

            var listAltStyle                       = GUI.skin.FindStyle(Settings.LIST_NORMAL_ALT_STYLE_NAME);
            if (listAltStyle == null) listAltStyle = GUI.skin.label;

            GUILayout.BeginVertical();
            var useAlt = false;
            foreach (var b in list)
            {
                if (b.IsTotal) continue;
                var styleToUse = useAlt ? listAltStyle : listNormalStyle;
                GUILayout.Label(string.Format("{0}%", b.Percentage.ToString(CultureInfo.InvariantCulture)), styleToUse);
                useAlt = !useAlt;
            }

            GUILayout.EndVertical();
        }
    }
}