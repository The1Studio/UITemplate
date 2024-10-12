//#define BRT_ASSET_LIST_SCREEN_DEBUG

using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace BuildReportTool.Window.Screen
{
    public partial class AssetList : BaseScreen
    {
        #if BRT_ASSET_LIST_SCREEN_DEBUG
	bool _showDebugText;
	readonly GUIContent _debugLabel = new GUIContent();
	StringBuilder _debugText;
        #endif

        private const int SCROLLBAR_BOTTOM_PADDING = 5;
        private const int BOTTOM_STATUS_BAR_HEIGHT = 20;
        private const int DOUBLE_CLICK_THRESHOLD   = 2;

        private FileFilterGroup _configuredFileFilterGroup;

        /// <summary>
        /// Which field of the asset we are sorting the list in.
        /// </summary>
        private BuildReportTool.AssetList.SortType _currentSortType = BuildReportTool.AssetList.SortType.RawSize;

        private TextureData.DataId _currentTextureDataSortType = TextureData.DataId.None;

        private TextureData.DataId _hoveredTextureDataId = TextureData.DataId.None;
        private TextureData.DataId _clickedTextureDataId = TextureData.DataId.None;

        private string _overridenTextureDataTooltipText;

        private MeshData.DataId _currentMeshDataSortType = MeshData.DataId.None;

        private MeshData.DataId _hoveredMeshDataId = MeshData.DataId.None;
        private MeshData.DataId _clickedMeshDataId = MeshData.DataId.None;

        /// <summary>
        /// Whether we are sorting ascending or descending.
        /// </summary>
        private BuildReportTool.AssetList.SortOrder _currentSortOrder = BuildReportTool.AssetList.SortOrder.Descending;

        /// <summary>
        /// Last clicked entry in the main asset list.
        /// This is used to check if user is clicking on the same asset that's already selected, or a new one.
        /// </summary>
        private int _assetListEntryLastClickedIdx = -1;

        /// <summary>
        /// -1 is already used to signify the "All" list (i.e. no filter)
        /// so we need something else to signify no value.
        /// </summary>
        private const int NO_FILTER_VALUE = -2;

        private int _filterIdxOfLastClickedAssetListEntry = NO_FILTER_VALUE;

        /// <summary>
        /// Hovered entry in the main asset list
        /// </summary>
        private int _assetListEntryHoveredIdx = -1;

        /// <summary>
        /// Hovered entry in the asset usage ancestry list
        /// </summary>
        private int _assetUsageAncestryHoveredIdx = -1;

        /// <summary>
        /// Hovered entry in the flattened asset users list
        /// </summary>
        private int _assetUserEntryHoveredIdx = -1;

        private bool _shouldShowThumbnailOnHoveredAsset;

        /// <summary>
        /// We record the time an asset has been clicked
        /// to check for a double-click.
        /// We use this instead of Event.current.clickCount
        /// because this way is easier.
        /// </summary>
        private double _assetListEntryLastClickedTime;

        private          Vector2    _assetListScrollPos;
        private          Rect       _assetPathColumnHeaderRect;
        private readonly GUIContent _assetPathCheckboxLabel = new("", "Display Asset's Path or just the filename.");

        private readonly GUIContent _textureDataTooltipLabel = new();

        /// <summary>
        /// Re-used entry in the main asset list.
        /// </summary>
        private readonly GUIContent _assetListEntry = new();

        public enum ListToDisplay
        {
            Invalid,
            UsedAssets,
            UnusedAssets,
        }

        private ListToDisplay _currentListDisplayed = ListToDisplay.Invalid;

        private bool _mouseIsPreviouslyInWindow;

        private bool _mouseIsOnOverlayControl;

        private          Rect       _showColumnOptionButtonRect;
        private          bool       _showColumnOptions;
        private readonly GUIContent _showColumnLabel = new("Columns");
        private readonly GUIContent _columnLabel     = new();

        private bool _showPageNumbers = true;

        // =================================================================================

        public override string Name => "";

        public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData, UnityBuildReport unityBuildReport)
        {
            this.RefreshConfiguredFileFilters();

            if (BuildReportTool.Options.ShouldUseConfiguredFileFilters())
            {
                var listToDisplay = this.GetAssetListToDisplay(buildReport);
                if (listToDisplay != null) listToDisplay.SortIfNeeded(this._configuredFileFilterGroup);
            }

            this._currentSortType  = BuildReportTool.AssetList.SortType.RawSize;
            this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Descending;
        }

        public override void Update(
            double            timeNow,
            double            deltaTime,
            BuildInfo         buildReportToDisplay,
            AssetDependencies assetDependencies
        )
        {
            this.UpdateSearch(timeNow, buildReportToDisplay);
        }

        public void SetListToDisplay(ListToDisplay t)
        {
            this._currentListDisplayed = t;
        }

        private bool IsShowingUnusedAssets => this._currentListDisplayed == ListToDisplay.UnusedAssets;

        private bool IsShowingUsedAssets => this._currentListDisplayed == ListToDisplay.UsedAssets;

        private BuildReportTool.AssetList GetAssetListToDisplay(BuildInfo buildReportToDisplay)
        {
            if (buildReportToDisplay == null) return null;

            if (this._currentListDisplayed == ListToDisplay.UsedAssets)
                return buildReportToDisplay.UsedAssets;
            else if (this._currentListDisplayed == ListToDisplay.UnusedAssets) return buildReportToDisplay.UnusedAssets;

            Debug.LogError("Invalid list to display type");
            return null;
        }

        private void DrawUnderlay(BuildInfo buildReportToDisplay)
        {
            this.DrawOverlay(buildReportToDisplay, false);
        }

        private void DrawOverlay(BuildInfo buildReportToDisplay, bool isOverlay = true)
        {
            const int TOGGLE_EXTRA_WIDTH = 9;
            const int TOGGLE_SPACING     = 2;

            var prevEnabled = GUI.enabled;

            if (this._showColumnOptions)
            {
                var columnOptionsBg = new Rect();
                columnOptionsBg.x      = this._showColumnOptionButtonRect.x;
                columnOptionsBg.y      = this._showColumnOptionButtonRect.yMax;
                columnOptionsBg.width  = UnityEngine.Screen.width - this._showColumnOptionButtonRect.x - 19;
                columnOptionsBg.height = 412;

                columnOptionsBg.x = UnityEngine.Screen.width - columnOptionsBg.width - 19;

                var bgStyle                  = GUI.skin.FindStyle(isOverlay ? "PopupPanel" : "HiddenScrollbar");
                if (bgStyle == null) bgStyle = GUI.skin.box;
                GUI.Box(columnOptionsBg, GUIContent.none, bgStyle);

                this._mouseIsOnOverlayControl = columnOptionsBg.Contains(Event.current.mousePosition);
                if (this._mouseIsOnOverlayControl)
                {
                    this._hoveredTextureDataId            = TextureData.DataId.None;
                    this._hoveredMeshDataId               = MeshData.DataId.None;
                    this._overridenTextureDataTooltipText = null;
                }

                var rect = new Rect();
                rect.x = columnOptionsBg.x + 10;
                rect.y = columnOptionsBg.y + 5;

                var startX = rect.x;

                //rect.width = 300;
                //rect.height = 28;
                //GUI.Label(rect, string.Format("Screen.width: {0} Screen.height: {1}", UnityEngine.Screen.width.ToString(), UnityEngine.Screen.height.ToString()));

                const int COLUMN_SPACING = 15;
                const int GROUP_SPACING  = 5;

                float normalColumnsHeight = 0;

                float textureDataLowestY      = 0;
                float textureDataColumn1Width = 0;
                float textureDataColumn2Width = 0;

                var toggleHeaderStyle                            = GUI.skin.FindStyle("Header3");
                if (toggleHeaderStyle == null) toggleHeaderStyle = GUI.skin.label;

                this._columnLabel.text = "Columns:";
                rect.size              = toggleHeaderStyle.CalcSize(this._columnLabel);
                GUI.Label(rect, this._columnLabel, toggleHeaderStyle);
                rect.y += rect.height - 2;

                this._columnLabel.text                      =  "Asset Path";
                rect.size                                   =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                  += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowColumnAssetPath =  GUI.Toggle(rect, BuildReportTool.Options.ShowColumnAssetPath, this._columnLabel);
                rect.x                                      += rect.width + COLUMN_SPACING;
                normalColumnsHeight                         =  Mathf.Max(normalColumnsHeight, rect.height);

                this._columnLabel.text                            =  "Size Before Build";
                rect.size                                         =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                        += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowColumnSizeBeforeBuild =  GUI.Toggle(rect, BuildReportTool.Options.ShowColumnSizeBeforeBuild, this._columnLabel);
                rect.x                                            += rect.width + COLUMN_SPACING;
                normalColumnsHeight                               =  Mathf.Max(normalColumnsHeight, rect.height);

                this._columnLabel.text                        =  "Size In Build";
                rect.size                                     =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                    += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowColumnSizeInBuild =  GUI.Toggle(rect, BuildReportTool.Options.ShowColumnSizeInBuild, this._columnLabel);
                rect.x                                        += rect.width + COLUMN_SPACING;
                normalColumnsHeight                           =  Mathf.Max(normalColumnsHeight, rect.height);

                rect.x =  startX;
                rect.y += normalColumnsHeight + TOGGLE_SPACING;

                normalColumnsHeight                             =  0;
                this._columnLabel.text                          =  "Raw Size (for Unused Assets)";
                rect.size                                       =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                      += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowColumnUnusedRawSize =  GUI.Toggle(rect, BuildReportTool.Options.ShowColumnUnusedRawSize, this._columnLabel);
                rect.x                                          += rect.width + COLUMN_SPACING;
                normalColumnsHeight                             =  Mathf.Max(normalColumnsHeight, rect.height);

                this._columnLabel.text                               =  "Imported Size (for Unused Assets)";
                rect.size                                            =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                           += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowColumnUnusedImportedSize =  GUI.Toggle(rect, BuildReportTool.Options.ShowColumnUnusedImportedSize, this._columnLabel);
                rect.x                                               += rect.width + COLUMN_SPACING;
                normalColumnsHeight                                  =  Mathf.Max(normalColumnsHeight, rect.height);

                rect.y += normalColumnsHeight + 8;

                // -----------------------------------------------------------------------

                // column 1
                rect.x = startX;

                this._columnLabel.text = "Texture Data Columns:";
                rect.size              = toggleHeaderStyle.CalcSize(this._columnLabel);
                GUI.Label(rect, this._columnLabel, toggleHeaderStyle);
                rect.y += rect.height - 2;
                var toggleYStart = rect.y;

                #region Texture Column 1

                this._columnLabel.text                               =  "Texture Type";
                rect.size                                            =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                           += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnTextureType =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnTextureType, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.TextureType;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                          =  "Is sRGB (Color Texture)";
                rect.size                                       =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                      += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnIsSRGB =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnIsSRGB, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.IsSRGB;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                               =  "Alpha Source";
                rect.size                                            =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                           += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnAlphaSource =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnAlphaSource, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.AlphaSource;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                       =  "Alpha Is Transparency";
                rect.size                                                    =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                   += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnAlphaIsTransparency =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnAlphaIsTransparency, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.AlphaIsTransparency;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                  =  "Ignore PNG Gamma";
                rect.size                                               =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                              += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnIgnorePngGamma =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnIgnorePngGamma, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.IgnorePngGamma;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                              =  "Read/Write Enabled";
                rect.size                                           =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                          += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnIsReadable =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnIsReadable, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.IsReadable;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                // -------------------------------------------
                rect.y += GROUP_SPACING;

                this._columnLabel.text                                   =  "Mip Map Generated";
                rect.size                                                =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                               += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnMipMapGenerated =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnMipMapGenerated, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.MipMapGenerated;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                =  "Mip Map Filter";
                rect.size                                             =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                            += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnMipMapFilter =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnMipMapFilter, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.MipMapFilter;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                    =  "Streaming Mip Maps";
                rect.size                                                 =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnStreamingMipMaps =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnStreamingMipMaps, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.StreamingMipMaps;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                 =  "Border Mip Maps";
                rect.size                                              =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                             += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnBorderMipMaps =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnBorderMipMaps, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.BorderMipMaps;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                           =  "Preserve Coverage Mip Maps";
                rect.size                                                        =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                       += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnPreserveCoverageMipMaps =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnPreserveCoverageMipMaps, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.PreserveCoverageMipMaps;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);

                this._columnLabel.text                                  =  "Fade Mip Maps";
                rect.size                                               =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                              += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnFadeOutMipMaps =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnFadeOutMipMaps, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.FadeOutMipMaps;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn1Width =  Mathf.Max(textureDataColumn1Width, rect.width);
                textureDataLowestY      =  Mathf.Max(textureDataLowestY, rect.y);

                #endregion

                // -----------------------------------------------------------------------
                // column 2
                rect.x += textureDataColumn1Width + COLUMN_SPACING;
                rect.y =  toggleYStart;

                #region Texture Column 2

                this._columnLabel.text                                          =  "Imported Width & Height";
                rect.size                                                       =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                      += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnImportedWidthAndHeight =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnImportedWidthAndHeight, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.ImportedWidthAndHeight;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                                      =  "Source Width & Height";
                rect.size                                                   =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                  += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnRealWidthAndHeight =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnRealWidthAndHeight, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.RealWidthAndHeight;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                             =  "Non-Power of 2 Scale";
                rect.size                                          =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                         += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnNPotScale =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnNPotScale, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.NPotScale;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                                  =  "Max Texture Size";
                rect.size                                               =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                              += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnMaxTextureSize =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnMaxTextureSize, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.MaxTextureSize;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                                   =  "Resize Algorithm";
                rect.size                                                =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                               += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnResizeAlgorithm =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnResizeAlgorithm, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.TextureResizeAlgorithm;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                // -----------------------------------------------------------------------
                rect.y += GROUP_SPACING;

                this._columnLabel.text                                    =  "Sprite Mode";
                rect.size                                                 =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnSpriteImportMode =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnSpriteImportMode, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.SpriteImportMode;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                                    =  "Sprite Packing Tag";
                rect.size                                                 =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnSpritePackingTag =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnSpritePackingTag, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.SpritePackingTag;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                                       =  "Sprite Pixels Per Unit";
                rect.size                                                    =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                   += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnSpritePixelsPerUnit =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnSpritePixelsPerUnit, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.SpritePixelsPerUnit;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);

                this._columnLabel.text                                             =  "Qualifies for Sprite Packing";
                rect.size                                                          =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                         += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnQualifiesForSpritePacking =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnQualifiesForSpritePacking, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.QualifiesForSpritePacking;
                rect.y                  += rect.height + TOGGLE_SPACING;
                textureDataColumn2Width =  Mathf.Max(textureDataColumn2Width, rect.width);
                textureDataLowestY      =  Mathf.Max(textureDataLowestY, rect.y);

                #endregion

                // -----------------------------------------------------------------------
                // column 3
                rect.x += textureDataColumn2Width + COLUMN_SPACING;
                rect.y =  toggleYStart;

                #region Texture Column 3

                this._columnLabel.text                                 =  "Texture Format";
                rect.size                                              =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                             += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnTextureFormat =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnTextureFormat, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.TextureFormat;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                                   =  "Compression Type";
                rect.size                                                =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                               += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnCompressionType =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnCompressionType, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.CompressionType;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                                         =  "Compression Crunched";
                rect.size                                                      =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                     += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnCompressionIsCrunched =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnCompressionIsCrunched, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.CompressionIsCrunched;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                                      =  "Compression Quality";
                rect.size                                                   =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                  += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnCompressionQuality =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnCompressionQuality, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.CompressionQuality;
                rect.y += rect.height + TOGGLE_SPACING;

                // -----------------------------------------------------------------------
                rect.y += GROUP_SPACING;

                this._columnLabel.text                            =  "Wrap Mode";
                rect.size                                         =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                        += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnWrapMode =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnWrapMode, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.WrapMode;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                             =  "Wrap Mode U";
                rect.size                                          =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                         += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnWrapModeU =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnWrapModeU, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.WrapModeU;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                             =  "Wrap Mode V";
                rect.size                                          =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                         += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnWrapModeV =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnWrapModeV, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.WrapModeV;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                             =  "Wrap Mode W";
                rect.size                                          =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                         += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnWrapModeW =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnWrapModeW, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.WrapModeW;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                              =  "Filter Mode";
                rect.size                                           =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                          += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnFilterMode =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnFilterMode, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.FilterMode;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                              =  "Anisotropic Filtering Level";
                rect.size                                           =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                          += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowTextureColumnAnisoLevel =  GUI.Toggle(rect, BuildReportTool.Options.ShowTextureColumnAnisoLevel, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = TextureData.DataId.AnisoLevel;
                rect.y += rect.height + TOGGLE_SPACING;

                textureDataLowestY = Mathf.Max(textureDataLowestY, rect.y);

                #endregion

                // -----------------------------------------------------------------------

                float meshColumn1Width = 0;
                float meshColumn2Width = 0;

                // column 1
                rect.x = startX;
                rect.y = textureDataLowestY + 8;

                this._columnLabel.text = "Mesh Data Columns:";
                rect.size              = toggleHeaderStyle.CalcSize(this._columnLabel);
                GUI.Label(rect, this._columnLabel, toggleHeaderStyle);
                rect.y       += rect.height - 2;
                toggleYStart =  rect.y;

                #region Mesh Column 1

                this._columnLabel.text                                =  "Non-Skinned Mesh Count";
                rect.size                                             =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                            += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnMeshFilterCount =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnMeshFilterCount, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.MeshFilterCount;
                rect.y           += rect.height + TOGGLE_SPACING;
                meshColumn1Width =  Mathf.Max(meshColumn1Width, rect.width);

                this._columnLabel.text                                         =  "Skinned Mesh Count";
                rect.size                                                      =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                                     += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnSkinnedMeshRendererCount =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnSkinnedMeshRendererCount, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.SkinnedMeshRendererCount;
                rect.y           += rect.height + TOGGLE_SPACING;
                meshColumn1Width =  Mathf.Max(meshColumn1Width, rect.width);

                #endregion

                // -----------------------------------------------------------------------
                // column 2

                rect.x += meshColumn1Width + COLUMN_SPACING;
                rect.y =  toggleYStart;

                #region Mesh Column 2

                this._columnLabel.text                             =  "Sub-Mesh Count";
                rect.size                                          =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                         += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnSubMeshCount =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnSubMeshCount, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.SubMeshCount;
                rect.y           += rect.height + TOGGLE_SPACING;
                meshColumn2Width =  Mathf.Max(meshColumn2Width, rect.width);

                this._columnLabel.text                            =  "Vertex Count";
                rect.size                                         =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                        += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnVertexCount =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnVertexCount, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.VertexCount;
                rect.y           += rect.height + TOGGLE_SPACING;
                meshColumn2Width =  Mathf.Max(meshColumn2Width, rect.width);

                this._columnLabel.text                              =  "Face Count";
                rect.size                                           =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                          += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnTriangleCount =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnTriangleCount, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.TriangleCount;
                rect.y           += rect.height + TOGGLE_SPACING;
                meshColumn2Width =  Mathf.Max(meshColumn2Width, rect.width);

                #endregion

                // -----------------------------------------------------------------------
                // column 3

                rect.x += meshColumn2Width + COLUMN_SPACING;
                rect.y =  toggleYStart;

                #region Mesh Column 3

                this._columnLabel.text                              =  "Animation Type";
                rect.size                                           =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                          += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnAnimationType =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnAnimationType, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.AnimationType;
                rect.y += rect.height + TOGGLE_SPACING;

                this._columnLabel.text                                   =  "Animation Clip Count";
                rect.size                                                =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width                                               += TOGGLE_EXTRA_WIDTH;
                BuildReportTool.Options.ShowMeshColumnAnimationClipCount =  GUI.Toggle(rect, BuildReportTool.Options.ShowMeshColumnAnimationClipCount, this._columnLabel);
                if (rect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = MeshData.DataId.AnimationClipCount;
                rect.y += rect.height + TOGGLE_SPACING;

                #endregion

                // -----------------------------------------------------------------------

                // input catcher
                GUI.Button(columnOptionsBg, GUIContent.none, "HiddenScrollbar");
            }

            if (this._showSearchOptions)
            {
                var searchOptionsBg = new Rect();
                searchOptionsBg.x      = this._searchTextfieldRect.x + 8;
                searchOptionsBg.y      = this._searchTextfieldRect.yMax;
                searchOptionsBg.width  = this._searchTextfieldRect.width - 11;
                searchOptionsBg.height = 130;

                this._mouseIsOnOverlayControl = searchOptionsBg.Contains(Event.current.mousePosition);
                if (this._mouseIsOnOverlayControl)
                {
                    this._hoveredTextureDataId            = TextureData.DataId.None;
                    this._hoveredMeshDataId               = MeshData.DataId.None;
                    this._overridenTextureDataTooltipText = null;
                }

                var bgStyle                  = GUI.skin.FindStyle(isOverlay ? "PopupPanel" : "HiddenScrollbar");
                if (bgStyle == null) bgStyle = GUI.skin.box;
                GUI.Box(searchOptionsBg, GUIContent.none, bgStyle);

                var rect = new Rect();
                rect.x = searchOptionsBg.x + 10;
                rect.y = searchOptionsBg.y + 5;

                var startX = rect.x;

                var toggleHeaderStyle = GUI.skin.FindStyle("Header3");
                var radioLeftStyle    = GUI.skin.FindStyle("RadioLeft");
                var radioMidStyle     = GUI.skin.FindStyle("RadioMiddle");
                var radioRightStyle   = GUI.skin.FindStyle("RadioRight");

                if (toggleHeaderStyle == null) toggleHeaderStyle = GUI.skin.label;
                if (radioLeftStyle == null) radioLeftStyle       = GUI.skin.toggle;
                if (radioMidStyle == null) radioMidStyle         = GUI.skin.toggle;
                if (radioRightStyle == null) radioRightStyle     = GUI.skin.toggle;

                float searchTypeHeight = 0;

                this._columnLabel.text = "Search Method:";
                rect.size              = toggleHeaderStyle.CalcSize(this._columnLabel);
                GUI.Label(rect, this._columnLabel, toggleHeaderStyle);
                rect.y += rect.height + 5;

                this._columnLabel.text = "Basic";
                rect.size              = radioLeftStyle.CalcSize(this._columnLabel);
                var pressedBasic                                                                      = GUI.Toggle(rect, BuildReportTool.Options.SearchTypeIsBasic, this._columnLabel, radioLeftStyle);
                if (rect.Contains(Event.current.mousePosition)) this._overridenTextureDataTooltipText = "<b><color=white>Basic</color></b>\n\nUse * for wildcard.";
                rect.x           += rect.width;
                searchTypeHeight =  Mathf.Max(searchTypeHeight, rect.height);

                if (pressedBasic && !BuildReportTool.Options.SearchTypeIsBasic)
                {
                    BuildReportTool.Options.SearchType = SearchType.Basic;
                    this.UpdateSearchNow(buildReportToDisplay);
                }

                this._columnLabel.text = "Regex";
                rect.size              = radioMidStyle.CalcSize(this._columnLabel);
                var pressedRegex                                                                      = GUI.Toggle(rect, BuildReportTool.Options.SearchTypeIsRegex, this._columnLabel, radioMidStyle);
                if (rect.Contains(Event.current.mousePosition)) this._overridenTextureDataTooltipText = "<b><color=white>Regular Expression</color></b>";
                rect.x           += rect.width;
                searchTypeHeight =  Mathf.Max(searchTypeHeight, rect.height);

                if (pressedRegex && !BuildReportTool.Options.SearchTypeIsRegex)
                {
                    BuildReportTool.Options.SearchType = SearchType.Regex;
                    this.UpdateSearchNow(buildReportToDisplay);
                }

                this._columnLabel.text = "Fuzzy";
                rect.size              = radioRightStyle.CalcSize(this._columnLabel);
                var pressedFuzzy                                                                      = GUI.Toggle(rect, BuildReportTool.Options.SearchTypeIsFuzzy, this._columnLabel, radioRightStyle);
                if (rect.Contains(Event.current.mousePosition)) this._overridenTextureDataTooltipText = "<b><color=white>Fuzzy Search</color></b>\n\nApproximate string matching.";
                searchTypeHeight = Mathf.Max(searchTypeHeight, rect.height);

                if (pressedFuzzy && !BuildReportTool.Options.SearchTypeIsFuzzy)
                {
                    BuildReportTool.Options.SearchType = SearchType.Fuzzy;
                    this.UpdateSearchNow(buildReportToDisplay);
                }

                rect.x =  startX;
                rect.y += searchTypeHeight + 20;

                this._columnLabel.text =  "Search through filenames only\n(ignore path when searching)";
                rect.size              =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width             += TOGGLE_EXTRA_WIDTH;
                rect.height            += 3;
                var newSearchFilenameOnly = GUI.Toggle(rect, BuildReportTool.Options.SearchFilenameOnly, this._columnLabel);
                if (newSearchFilenameOnly != BuildReportTool.Options.SearchFilenameOnly)
                {
                    BuildReportTool.Options.SearchFilenameOnly = newSearchFilenameOnly;
                    this.UpdateSearchNow(buildReportToDisplay);
                }
                rect.y += rect.height + TOGGLE_SPACING;

                var usingFuzzy = BuildReportTool.Options.SearchTypeIsFuzzy;
                GUI.enabled            =  prevEnabled && !usingFuzzy;
                this._columnLabel.text =  "Case Sensitive Search";
                rect.size              =  GUI.skin.toggle.CalcSize(this._columnLabel);
                rect.width             += TOGGLE_EXTRA_WIDTH;
                var newSearchCaseSensitive = GUI.Toggle(rect, BuildReportTool.Options.SearchCaseSensitive, this._columnLabel);
                if (newSearchCaseSensitive != BuildReportTool.Options.SearchCaseSensitive)
                {
                    BuildReportTool.Options.SearchCaseSensitive = newSearchCaseSensitive;
                    this.UpdateSearchNow(buildReportToDisplay);
                }
                if (usingFuzzy && rect.Contains(Event.current.mousePosition)) this._overridenTextureDataTooltipText = "Not applicable to Fuzzy Search. Fuzzy Search is always Case Insensitive.";
                rect.y      += rect.height + TOGGLE_SPACING;
                GUI.enabled =  prevEnabled;

                // input catcher
                GUI.Button(searchOptionsBg, GUIContent.none, "HiddenScrollbar");
            }
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
            if (buildReportToDisplay == null || !buildReportToDisplay.HasUsedAssets)
            {
                requestRepaint = false;
                return;
            }

            #if BRT_ASSET_LIST_SCREEN_DEBUG
			if (_debugText == null)
			{
				_debugText = new StringBuilder();
			}
			else
			{
				_debugText.Length = 0;
			}
            #endif

            // init variables to use
            // --------------------------------------------------------------------------

            var fileFilterGroupToUse = buildReportToDisplay.FileFilters;

            if (BuildReportTool.Options.ShouldUseConfiguredFileFilters()) fileFilterGroupToUse = this._configuredFileFilterGroup;

            var listToDisplay = this.GetAssetListToDisplay(buildReportToDisplay);

            if (listToDisplay == null)
            {
                if (this.IsShowingUsedAssets)
                    Utility.DrawCentralMessage(position, "No \"Used Assets\" included in this build report.");
                else if (this.IsShowingUnusedAssets) Utility.DrawCentralMessage(position, "No \"Unused Assets\" included in this build report.");

                requestRepaint = false;
                return;
            }

            // continually request repaint, since we need to check the rects
            // generated by the GUILayout (using GUILayoutUtility.GetLastRect())
            // to make the hover checks work. GetLastRect() only works during
            // repaint event.
            //
            // later checks below can set requestRepaint to false if there's no
            // need to repaint, to help lessen cpu usage
            requestRepaint = true;

            // GUI
            // --------------------------------------------------------------------------

            this.DrawUnderlay(buildReportToDisplay);

            GUILayout.Space(1);

            // Toolbar at top
            // ------------------------------------------------

            this.DrawTopBar(position, buildReportToDisplay, fileFilterGroupToUse);

            // Actual Asset List
            // ------------------------------------------------

            if (buildReportToDisplay.HasUsedAssets)
            {
                this.DrawAssetList(position,
                    buildReportToDisplay,
                    assetDependencies,
                    textureData,
                    meshData,
                    listToDisplay,
                    fileFilterGroupToUse,
                    BuildReportTool.Options.AssetListPaginationLength);
                GUILayout.FlexibleSpace();
            }

            // Asset Usage Panel for selected
            // ------------------------------------------------

            this.DrawAssetUsage(position, listToDisplay, buildReportToDisplay, assetDependencies);

            var selectedName = listToDisplay.GetSelectedCount() == 1 ? listToDisplay.GetLastSelected().Name : null;

            // .unity files (scenes) are users themselves, but no asset uses them, because they are the ones directly
            // included in builds
            var selectedAssetHasUsers = listToDisplay.GetSelectedCount() == 1 && !string.IsNullOrEmpty(selectedName) && assetDependencies != null && assetDependencies.GetAssetDependencies().ContainsKey(selectedName);

            var isAssetUsagePanelShown = selectedAssetHasUsers || this._selectedIsAResourcesAsset;

            // Status bar at bottom
            // ------------------------------------------------

            // reserve space for the bottom bar
            // later we draw the bottom bar using GUILayout.BeginArea/EndArea
            GUILayout.Space(BOTTOM_STATUS_BAR_HEIGHT);

            var bottomBarRect = new Rect(0, 0, position.width, BOTTOM_STATUS_BAR_HEIGHT);

            if (isAssetUsagePanelShown)
                // bottom bar is anchored to the bottom of the window
                // but move it up a bit, since the bottom-most portion is now occupied by the Asset Usage Panel
                bottomBarRect.y = position.height - BOTTOM_STATUS_BAR_HEIGHT - this._assetUsageRect.height;
            else
                // bottom bar is anchored to the bottom of the window
                bottomBarRect.y = position.height - BOTTOM_STATUS_BAR_HEIGHT;

            // --------------------------

            if (Event.current.mousePosition.y >= position.height || Event.current.mousePosition.y <= this._assetPathColumnHeaderRect.yMin - 5 || Event.current.mousePosition.x <= 0 || Event.current.mousePosition.x >= position.width)
            {
                if (!this._mouseIsPreviouslyInWindow)
                    // mouse is outside the area that shows tooltips
                    // set requestRepaint to false to help lessen cpu usage
                    requestRepaint = false;
                this._mouseIsPreviouslyInWindow = false;
            }
            else
            {
                this._mouseIsPreviouslyInWindow = true;
            }

            // if mouse is too far below, above, or to the right for showing tooltip
            // in main asset list, then prevent tooltip from showing in that situation
            // note: this doesn't prevent tooltips in the asset usage panel
            if (Event.current.type == EventType.Repaint && (Event.current.mousePosition.y >= bottomBarRect.y || Event.current.mousePosition.y <= this._assetPathColumnHeaderRect.yMax || Event.current.mousePosition.x >= position.width)) this._assetListEntryHoveredIdx = -1;

            var shouldShowThumbnailTooltipNow = BuildReportTool.Options.ShowTooltipThumbnail && this._shouldShowThumbnailOnHoveredAsset && (this._assetListEntryHoveredIdx != -1 || this._assetUsageAncestryHoveredIdx != -1 || this._assetUserEntryHoveredIdx != -1);

            var zoomInChanged = false;
            if (shouldShowThumbnailTooltipNow)
            {
                var prevZoomedIn = BRT_BuildReportWindow.ZoomedInThumbnails;

                // if thumbnail is currently showing, we process the inputs
                // (ctrl zooms in on thumbnail, alt toggles alpha blend)
                BRT_BuildReportWindow.ProcessThumbnailControls();

                if (prevZoomedIn != BRT_BuildReportWindow.ZoomedInThumbnails) zoomInChanged = true;
            }
            else
                // no thumbnail currently shown. ensure the controls that
                // need to be reset to initial state are reset
            {
                BRT_BuildReportWindow.ResetThumbnailControls();
            }

            if (!zoomInChanged && !Event.current.alt && !BRT_BuildReportWindow.MouseMovedNow && !BRT_BuildReportWindow.LastMouseMoved)
                // mouse hasn't moved, and no request to zoom-in thumbnail or toggle thumbnail alpha
                // no need to repaint because nothing has changed
                // set requestRepaint to false to help lessen cpu usage
                requestRepaint = false;

            // --------------------------
            // actual contents of Bottom Bar

            var statusBarBgStyle                           = GUI.skin.FindStyle(Settings.STATUS_BAR_BG_STYLE_NAME);
            if (statusBarBgStyle == null) statusBarBgStyle = GUI.skin.box;

            var statusBarLabelStyle                              = GUI.skin.FindStyle(Settings.STATUS_BAR_LABEL_STYLE_NAME);
            if (statusBarLabelStyle == null) statusBarLabelStyle = GUI.skin.label;

            var selectedInfoLabel = string.Format(
                "{0}{1}. {2}{3} ({4}%)",
                Labels.SELECTED_QTY_LABEL,
                listToDisplay.GetSelectedCount().ToString("N0"),
                Labels.SELECTED_SIZE_LABEL,
                listToDisplay.GetReadableSizeOfSumSelection(),
                listToDisplay.GetPercentageOfSumSelection().ToString("N"));

            GUILayout.BeginArea(bottomBarRect);

            GUILayout.BeginHorizontal(statusBarBgStyle,
                BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(selectedInfoLabel,
                statusBarLabelStyle,
                BRT_BuildReportWindow.LayoutNone);
            GUILayout.FlexibleSpace();

            if (shouldShowThumbnailTooltipNow)
                GUILayout.Label(
                    "Hold Ctrl to zoom-in on the thumbnail. Press Alt to show/hide alpha transparency.",
                    statusBarLabelStyle,
                    BRT_BuildReportWindow.LayoutNone);
            else
                GUILayout.Label(
                    "Click on an asset's name to include it in size calculations or batch deletions. Shift-click to select many. Ctrl-click to toggle selection.",
                    statusBarLabelStyle,
                    BRT_BuildReportWindow.LayoutNone);

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            // --------------------------

            #if BRT_ASSET_LIST_SCREEN_DEBUG
		_debugText.AppendFormat("Event.current.mousePosition.x: {0}\nposition.width: {1}\nshouldShowThumbnailTooltipNow: {2}\n",
			Event.current.mousePosition.x.ToString(CultureInfo.InvariantCulture),
			position.width.ToString(CultureInfo.InvariantCulture),
			shouldShowThumbnailTooltipNow);
            #endif

            // ------------------------------------------------

            this.DrawOverlay(buildReportToDisplay);

            // ------------------------------------------------
            // Tooltip

            var shouldShowAssetEndUsersTooltipNow = BuildReportTool.Options.ShowAssetPrimaryUsersInTooltipIfAvailable && BRT_BuildReportWindow.ShouldHoveredAssetShowEndUserTooltip(assetDependencies) && (this._assetListEntryHoveredIdx != -1 || this._assetUsageAncestryHoveredIdx != -1 || this._assetUserEntryHoveredIdx != -1);

            if (Event.current.type == EventType.Repaint)
            {
                if (!string.IsNullOrEmpty(this._overridenTextureDataTooltipText))
                {
                    this._textureDataTooltipLabel.text = this._overridenTextureDataTooltipText;
                    var tooltipTextStyle                           = GUI.skin.FindStyle("TooltipText");
                    if (tooltipTextStyle == null) tooltipTextStyle = GUI.skin.box;

                    const int MAX_TOOLTIP_WIDTH = 240;
                    var       tooltipSize       = tooltipTextStyle.CalcSize(this._textureDataTooltipLabel);
                    if (tooltipSize.x > MAX_TOOLTIP_WIDTH)
                    {
                        tooltipSize.x = MAX_TOOLTIP_WIDTH;
                        tooltipSize.y = tooltipTextStyle.CalcHeight(this._textureDataTooltipLabel, tooltipSize.x);
                    }

                    var tooltipRect = BRT_BuildReportWindow.DrawTooltip(position, tooltipSize.x, tooltipSize.y, 5);
                    GUI.Label(tooltipRect, this._textureDataTooltipLabel, tooltipTextStyle);
                }
                else if (shouldShowThumbnailTooltipNow && shouldShowAssetEndUsersTooltipNow && !this._mouseIsOnOverlayControl)
                    // draw thumbnail and end users below it
                {
                    BRT_BuildReportWindow.DrawThumbnailEndUsersTooltip(position, assetDependencies, textureData);
                }
                else if (shouldShowAssetEndUsersTooltipNow && !this._mouseIsOnOverlayControl)
                    // draw only end users in tooltip
                {
                    BRT_BuildReportWindow.DrawEndUsersTooltip(position, assetDependencies);
                }
                else if (shouldShowThumbnailTooltipNow && !this._mouseIsOnOverlayControl)
                    // draw only thumbnail in tooltip
                {
                    BRT_BuildReportWindow.DrawThumbnailTooltip(position, textureData);
                }
                else if (this._hoveredTextureDataId != TextureData.DataId.None || this._hoveredMeshDataId != MeshData.DataId.None)
                {
                    if (this._hoveredTextureDataId != TextureData.DataId.None)
                        this._textureDataTooltipLabel.text =
                            TextureData.GetTooltipTextFromId(this._hoveredTextureDataId);
                    else if (this._hoveredMeshDataId != MeshData.DataId.None) this._textureDataTooltipLabel.text = MeshData.GetTooltipTextFromId(this._hoveredMeshDataId);

                    if (!string.IsNullOrEmpty(this._textureDataTooltipLabel.text))
                    {
                        var tooltipTextStyle                           = GUI.skin.FindStyle("TooltipText");
                        if (tooltipTextStyle == null) tooltipTextStyle = GUI.skin.box;

                        const int MAX_TOOLTIP_WIDTH = 400;
                        var       tooltipSize       = tooltipTextStyle.CalcSize(this._textureDataTooltipLabel);
                        if (tooltipSize.x > MAX_TOOLTIP_WIDTH)
                        {
                            tooltipSize.x = MAX_TOOLTIP_WIDTH;
                            tooltipSize.y = tooltipTextStyle.CalcHeight(this._textureDataTooltipLabel, tooltipSize.x);
                        }

                        var tooltipRect = BRT_BuildReportWindow.DrawTooltip(position, tooltipSize.x, tooltipSize.y, 5);
                        GUI.Label(tooltipRect, this._textureDataTooltipLabel, tooltipTextStyle);
                    }
                }
            }

            #if BRT_ASSET_LIST_SCREEN_DEBUG
		// Debug text
		// ------------------------------------------------

		_showDebugText = GUI.Toggle(new Rect(position.width - 90, 20, 90, 20),
			_showDebugText, "Show Debug", "Button");

		if (_showDebugText)
		{
			_debugLabel.text = _debugText.ToString();
			var debugStyle = GUI.skin.FindStyle("DebugOverlay");
			if (debugStyle == null)
			{
				debugStyle = GUI.skin.box;
			}
			var debugLabelSize = debugStyle.CalcSize(_debugLabel);

			GUI.Label(new Rect(position.width - debugLabelSize.x, 0, debugLabelSize.x, debugLabelSize.y), _debugLabel, debugStyle);
		}
            #endif
        }

        public void ToggleSort(BuildReportTool.AssetList assetList, BuildReportTool.AssetList.SortType newSortType, FileFilterGroup fileFilters)
        {
            this._currentTextureDataSortType = TextureData.DataId.None;

            if (this._currentSortType != newSortType)
            {
                this._currentSortType  = newSortType;
                this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Descending; // descending by default
            }
            else
            {
                // already in this sort type
                // now toggle the sort order
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                {
                    this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Ascending;
                }
                else if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Ascending)
                {
                    if (this._searchResults != null)
                    {
                        // clicked again while sort order is in ascending
                        // now disable it (which means sorting goes back to sort by search rank)
                        this._currentSortType  = BuildReportTool.AssetList.SortType.None;
                        this._currentSortOrder = BuildReportTool.AssetList.SortOrder.None;
                    }
                    else
                    {
                        this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Descending;
                    }
                }
            }

            if (this._searchResults != null)
            {
                if (this._currentSortType == BuildReportTool.AssetList.SortType.None)
                    // no column used as sort
                    // revert to sorting by search rank
                    this.SortBySearchRank(this._searchResults, this._lastSearchText);
                else
                    AssetListUtility.SortAssetList(this._searchResults, this._currentSortType, this._currentSortOrder);
            }
            else
            {
                assetList.Sort(this._currentSortType, this._currentSortOrder, fileFilters);
            }
        }

        private void ToggleSort(
            BuildReportTool.AssetList assetList,
            TextureData               textureData,
            TextureData.DataId        newSortType,
            FileFilterGroup           fileFilters
        )
        {
            if (this._currentSortType != BuildReportTool.AssetList.SortType.TextureData || this._currentTextureDataSortType != newSortType)
            {
                this._currentSortType            = BuildReportTool.AssetList.SortType.TextureData;
                this._currentTextureDataSortType = newSortType;
                this._currentSortOrder           = BuildReportTool.AssetList.SortOrder.Descending; // descending by default
            }
            else
            {
                // already in this sort type
                // now toggle the sort order
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                {
                    this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Ascending;
                }
                else if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Ascending)
                {
                    if (this._searchResults != null)
                    {
                        // clicked again while sort order is in ascending
                        // now disable it (which means sorting goes back to sort by search rank)
                        this._currentSortType            = BuildReportTool.AssetList.SortType.None;
                        this._currentSortOrder           = BuildReportTool.AssetList.SortOrder.None;
                        this._currentTextureDataSortType = TextureData.DataId.None;
                    }
                    else
                    {
                        this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Descending;
                    }
                }
            }

            if (this._searchResults != null)
            {
                if (this._currentSortType == BuildReportTool.AssetList.SortType.None && this._currentTextureDataSortType == TextureData.DataId.None)
                    // no column used as sort
                    // revert to sorting by search rank
                    this.SortBySearchRank(this._searchResults, this._lastSearchText);
                else
                    AssetListUtility.SortAssetList(this._searchResults, textureData, this._currentTextureDataSortType, this._currentSortOrder);
            }
            else
            {
                assetList.Sort(textureData, this._currentTextureDataSortType, this._currentSortOrder, fileFilters);
            }
        }

        private void ToggleSort(
            BuildReportTool.AssetList assetList,
            MeshData                  meshData,
            MeshData.DataId           newSortType,
            FileFilterGroup           fileFilters
        )
        {
            if (this._currentSortType != BuildReportTool.AssetList.SortType.MeshData || this._currentMeshDataSortType != newSortType)
            {
                this._currentSortType         = BuildReportTool.AssetList.SortType.MeshData;
                this._currentMeshDataSortType = newSortType;
                this._currentSortOrder        = BuildReportTool.AssetList.SortOrder.Descending; // descending by default
            }
            else
            {
                // already in this sort type
                // now toggle the sort order
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                {
                    this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Ascending;
                }
                else if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Ascending)
                {
                    if (this._searchResults != null)
                    {
                        // clicked again while sort order is in ascending
                        // now disable it (which means sorting goes back to sort by search rank)
                        this._currentSortType         = BuildReportTool.AssetList.SortType.None;
                        this._currentSortOrder        = BuildReportTool.AssetList.SortOrder.None;
                        this._currentMeshDataSortType = MeshData.DataId.None;
                    }
                    else
                    {
                        this._currentSortOrder = BuildReportTool.AssetList.SortOrder.Descending;
                    }
                }
            }

            if (this._searchResults != null)
            {
                if (this._currentSortType == BuildReportTool.AssetList.SortType.None && this._currentMeshDataSortType == MeshData.DataId.None)
                    // no column used as sort
                    // revert to sorting by search rank
                    this.SortBySearchRank(this._searchResults, this._lastSearchText);
                else
                    AssetListUtility.SortAssetList(this._searchResults, meshData, this._currentMeshDataSortType, this._currentSortOrder);
            }
            else
            {
                assetList.Sort(meshData, this._currentMeshDataSortType, this._currentSortOrder, fileFilters);
            }
        }

        private void RefreshConfiguredFileFilters()
        {
            // reload used FileFilterGroup but save current used filter idx
            // to be re-set afterwards

            var tempIdx = 0;

            if (this._configuredFileFilterGroup != null) tempIdx = this._configuredFileFilterGroup.GetSelectedFilterIdx();

            this._configuredFileFilterGroup = FiltersUsed.GetProperFileFilterGroupToUse();

            this._configuredFileFilterGroup.ForceSetSelectedFilterIdx(tempIdx);
        }

        private void DrawTopBar(
            Rect            position,
            BuildInfo       buildReportToDisplay,
            FileFilterGroup fileFilterGroupToUse
        )
        {
            var assetListUsed = this.GetAssetListToDisplay(buildReportToDisplay);

            if (assetListUsed == null) return;

            Texture2D prevArrow;
            var       prevArrowStyle = GUI.skin.FindStyle(Settings.BIG_LEFT_ARROW_ICON_STYLE_NAME);
            if (prevArrowStyle != null)
                prevArrow = prevArrowStyle.normal.background;
            else
                prevArrow = null;

            Texture2D nextArrow;
            var       nextArrowStyle = GUI.skin.FindStyle(Settings.BIG_RIGHT_ARROW_ICON_STYLE_NAME);
            if (nextArrowStyle != null)
                nextArrow = nextArrowStyle.normal.background;
            else
                nextArrow = null;

            var columnStyle                                      = GUI.skin.FindStyle(Settings.COLUMN_ICON_STYLE_NAME);
            if (columnStyle != null) this._showColumnLabel.image = columnStyle.normal.background;

            var topBarBgStyle                        = GUI.skin.FindStyle(Settings.TOP_BAR_BG_STYLE_NAME);
            if (topBarBgStyle == null) topBarBgStyle = GUI.skin.label;

            var topBarButtonStyle                            = GUI.skin.FindStyle(Settings.TOP_BAR_BTN_STYLE_NAME);
            if (topBarButtonStyle == null) topBarButtonStyle = GUI.skin.button;

            var topBarLabelStyle                           = GUI.skin.FindStyle(Settings.TOP_BAR_LABEL_STYLE_NAME);
            if (topBarLabelStyle == null) topBarLabelStyle = GUI.skin.label;

            var searchDropdownStyle                              = GUI.skin.FindStyle("TextField-Search-DropDown");
            if (searchDropdownStyle == null) searchDropdownStyle = GUI.skin.label;

            var searchTextStyle                          = GUI.skin.FindStyle("TextField-Search-Text");
            if (searchTextStyle == null) searchTextStyle = GUI.skin.textField;

            var searchClearStyle                           = GUI.skin.FindStyle("TextField-Search-ClearButton");
            if (searchClearStyle == null) searchClearStyle = GUI.skin.button;

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutHeight11);

            GUILayout.Label(" ", topBarBgStyle, BRT_BuildReportWindow.LayoutNone);

            // ------------------------------------------------------------------------------------------------------
            // File Filters

            var selectedFilterChanged = fileFilterGroupToUse.Draw(assetListUsed, position.width - 100);

            if (selectedFilterChanged)
            {
                this._assetListEntryLastClickedIdx         = -1;
                this._filterIdxOfLastClickedAssetListEntry = NO_FILTER_VALUE;
            }

            // ------------------------------------------------------------------------------------------------------

            GUILayout.Space(20);

            // ------------------------------------------------------------------------------------------------------
            // Unused Assets Batch

            if (this.IsShowingUnusedAssets && buildReportToDisplay.ProcessUnusedAssetsInBatches)
            {
                var batchNumber = buildReportToDisplay.UnusedAssetsBatchIdx + 1;

                var prevButton = prevArrow != null
                    ? GUILayout.Button(prevArrow, topBarButtonStyle)
                    : GUILayout.Button("Previous", topBarButtonStyle);
                if (prevButton && batchNumber - 1 >= 1)
                    // move to previous batch
                    ReportGenerator.MoveUnusedAssetsBatchToPrev(buildReportToDisplay, fileFilterGroupToUse);

                var batchLabel = string.Format("Batch {0}", batchNumber.ToString());
                GUILayout.Label(batchLabel, topBarLabelStyle);

                var nextButton = nextArrow != null
                    ? GUILayout.Button(nextArrow, topBarButtonStyle)
                    : GUILayout.Button("Next", topBarButtonStyle);
                if (nextButton)
                    // move to next batch
                    // (possible to have no new batch anymore. if so, it will just fail silently)
                    ReportGenerator.MoveUnusedAssetsBatchToNext(buildReportToDisplay, fileFilterGroupToUse);
                GUILayout.Space(8);
            }

            // ------------------------------------------------------------------------------------------------------

            // ------------------------------------------------------------------------------------------------------
            // Paginate Buttons

            var assetListToUse = assetListUsed.GetListToDisplay(fileFilterGroupToUse);

            // how many assets overall in this entire list
            var assetListLength = 0;
            if (this._searchResults != null && this._searchResults.Length > 0)
                assetListLength                              = this._searchResults.Length;
            else if (assetListToUse != null) assetListLength = assetListToUse.Length;

            // index of first asset to show, for current page.
            // This is an offset from the entire asset list.
            var viewOffset = this._searchResults != null
                ? this._searchViewOffset
                : assetListUsed.GetViewOffsetForDisplayedList(fileFilterGroupToUse);

            if (GUILayout.Button(prevArrow,
                    topBarButtonStyle,
                    BRT_BuildReportWindow.LayoutNone)
                && viewOffset - BuildReportTool.Options.AssetListPaginationLength >= 0)
            {
                if (this._searchResults != null)
                    this._searchViewOffset -= BuildReportTool.Options.AssetListPaginationLength;
                else
                    assetListUsed.SetViewOffsetForDisplayedList(fileFilterGroupToUse,
                        viewOffset - BuildReportTool.Options.AssetListPaginationLength);

                this._assetListScrollPos.y = 0;
            }

            string paginateLabel;

            if (this._showPageNumbers)
            {
                var totalPageNumbers = assetListLength / BuildReportTool.Options.AssetListPaginationLength;
                if (assetListLength % BuildReportTool.Options.AssetListPaginationLength > 0) ++totalPageNumbers;

                // the max number of digits for the displayed offset counters
                var assetCountDigitNumFormat = string.Format("D{0}", totalPageNumbers.ToString().Length.ToString());

                paginateLabel = string.Format("Page {0} of {1}",
                    (viewOffset / BuildReportTool.Options.AssetListPaginationLength + 1).ToString(assetCountDigitNumFormat),
                    totalPageNumbers.ToString());
            }
            else
            {
                // number of assets in current page
                var pageLength = Mathf.Min(viewOffset + BuildReportTool.Options.AssetListPaginationLength, assetListLength);

                // the max number of digits for the displayed offset counters
                var assetCountDigitNumFormat = string.Format("D{0}", assetListLength.ToString().Length.ToString());

                var offsetNonZeroBased = viewOffset + (pageLength > 0 ? 1 : 0);

                paginateLabel = string.Format("Page {0} - {1} of {2}",
                    offsetNonZeroBased.ToString(assetCountDigitNumFormat),
                    pageLength.ToString(assetCountDigitNumFormat),
                    assetListLength.ToString(assetCountDigitNumFormat));
            }

            if (GUILayout.Button(paginateLabel,
                topBarLabelStyle,
                BRT_BuildReportWindow.LayoutNone))
                this._showPageNumbers = !this._showPageNumbers;

            if (GUILayout.Button(nextArrow,
                    topBarButtonStyle,
                    BRT_BuildReportWindow.LayoutNone)
                && viewOffset + BuildReportTool.Options.AssetListPaginationLength < assetListLength)
            {
                if (this._searchResults != null)
                    this._searchViewOffset += BuildReportTool.Options.AssetListPaginationLength;
                else
                    assetListUsed.SetViewOffsetForDisplayedList(fileFilterGroupToUse,
                        viewOffset + BuildReportTool.Options.AssetListPaginationLength);

                this._assetListScrollPos.y = 0;
            }

            // ------------------------------------------------------------------------------------------------------

            GUILayout.FlexibleSpace();

            var newShowColumnOptions = GUILayout.Toggle(this._showColumnOptions,
                this._showColumnLabel,
                topBarButtonStyle,
                BRT_BuildReportWindow.LayoutNone);
            if (newShowColumnOptions != this._showColumnOptions)
            {
                this._showColumnOptions = newShowColumnOptions;
                if (this._showColumnOptions) this._showSearchOptions = false;
            }
            if (Event.current.type == EventType.Repaint) this._showColumnOptionButtonRect = GUILayoutUtility.GetLastRect();

            var newSearchOptions = GUILayout.Toggle(this._showSearchOptions,
                GUIContent.none,
                searchDropdownStyle,
                BRT_BuildReportWindow.LayoutNone);
            if (newSearchOptions != this._showSearchOptions)
            {
                this._showSearchOptions = newSearchOptions;
                if (this._showSearchOptions) this._showColumnOptions = false;
            }
            if (Event.current.type == EventType.Repaint) this._searchTextfieldRect = GUILayoutUtility.GetLastRect();
            this._searchTextInput = GUILayout.TextField(this._searchTextInput,
                searchTextStyle,
                BRT_BuildReportWindow.LayoutMinWidth200);
            if (GUILayout.Button(GUIContent.none, searchClearStyle, BRT_BuildReportWindow.LayoutNone)) this.ClearSearch();
            if (Event.current.type == EventType.Repaint) this._searchTextfieldRect.xMax = GUILayoutUtility.GetLastRect().xMax;

            // ------------------------------------------------------------------------------------------------------
            // Recalculate Imported sizes
            // (makes sense only for unused assets)

            if (this._currentListDisplayed != ListToDisplay.UsedAssets
                && GUILayout.Button(Labels.RECALC_IMPORTED_SIZES,
                    topBarButtonStyle,
                    BRT_BuildReportWindow.LayoutNone))
                assetListUsed.PopulateImportedSizes();

            if (!BuildReportTool.Options.AutoResortAssetsWhenUnityEditorRegainsFocus
                && BuildReportTool.Options.GetSizeBeforeBuildForUsedAssets
                && this._currentListDisplayed == ListToDisplay.UsedAssets
                && GUILayout.Button(Labels.RECALC_SIZE_BEFORE_BUILD,
                    topBarButtonStyle,
                    BRT_BuildReportWindow.LayoutNone))
                assetListUsed.PopulateSizeInAssetsFolder();

            // ------------------------------------------------------------------------------------------------------

            // ------------------------------------------------------------------------------------------------------
            // Selection buttons

            if (GUILayout.Button(Labels.SELECT_ALL_LABEL,
                topBarButtonStyle,
                BRT_BuildReportWindow.LayoutNone))
                assetListUsed.AddAllDisplayedToSumSelection(fileFilterGroupToUse);

            if (GUILayout.Button(Labels.SELECT_NONE_LABEL,
                topBarButtonStyle,
                BRT_BuildReportWindow.LayoutNone))
            {
                assetListUsed.ClearSelection();
                this._assetListEntryLastClickedIdx         = -1;
                this._filterIdxOfLastClickedAssetListEntry = NO_FILTER_VALUE;
            }

            // ------------------------------------------------------------------------------------------------------

            // ------------------------------------------------------------------------------------------------------
            // Delete buttons

            if (this.ShouldShowDeleteButtons(buildReportToDisplay))
            {
                GUI.backgroundColor = Color.red;
                const string DEL_SELECTED_LABEL = "Delete selected";
                if (GUILayout.Button(DEL_SELECTED_LABEL,
                    topBarButtonStyle,
                    BRT_BuildReportWindow.LayoutNone))
                    this.InitiateDeleteSelectedUsed(buildReportToDisplay);

                const string DELETE_ALL_LABEL = "Delete all";
                if (GUILayout.Button(DELETE_ALL_LABEL,
                    topBarButtonStyle,
                    BRT_BuildReportWindow.LayoutNone))
                    this.InitiateDeleteAllUnused(buildReportToDisplay);

                GUI.backgroundColor = Color.white;
            }

            // ------------------------------------------------------------------------------------------------------

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        private GUIStyle GetColumnHeaderStyle(BuildReportTool.AssetList.SortType sortTypeNeeded)
        {
            var styleResult = Settings.LIST_COLUMN_HEADER_STYLE_NAME;

            if (this._currentSortType == sortTypeNeeded)
            {
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                    styleResult = Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
                else
                    styleResult = Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
            }

            var style                = GUI.skin.FindStyle(styleResult);
            if (style == null) style = GUI.skin.label;

            return style;
        }

        private void DrawAssetList(
            Rect                      position,
            BuildInfo                 buildReportToDisplay,
            AssetDependencies         assetDependencies,
            TextureData               textureData,
            MeshData                  meshData,
            BuildReportTool.AssetList list,
            FileFilterGroup           filter,
            int                       length
        )
        {
            if (list == null) return;

            if (this._searchResults != null && this._searchResults.Length == 0)
            {
                this.DrawCentralMessage(position, "No search results");
                return;
            }

            var listEntryStyle                         = GUI.skin.FindStyle(Settings.LIST_SMALL_STYLE_NAME);
            if (listEntryStyle == null) listEntryStyle = GUI.skin.label;

            var listAltEntryStyle                            = GUI.skin.FindStyle(Settings.LIST_SMALL_ALT_STYLE_NAME);
            if (listAltEntryStyle == null) listAltEntryStyle = GUI.skin.label;

            var listSelectedEntryStyle                                 = GUI.skin.FindStyle(Settings.LIST_SMALL_SELECTED_NAME);
            if (listSelectedEntryStyle == null) listSelectedEntryStyle = GUI.skin.label;

            SizePart[] assetListToUse;

            var hasSearchResults = this._searchResults != null;

            if (hasSearchResults && this._searchResults.Length > 0)
                assetListToUse = this._searchResults;
            else
                assetListToUse = list.GetListToDisplay(filter);

            if (assetListToUse == null) return;

            var messageStyle                       = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (messageStyle == null) messageStyle = GUI.skin.label;

            if (assetListToUse.Length == 0)
            {
                GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
                GUILayout.Space(10);
                GUILayout.Label(Labels.NO_FILES_FOR_THIS_CATEGORY_LABEL,
                    messageStyle,
                    BRT_BuildReportWindow.LayoutNone);
                GUILayout.EndHorizontal();

                return;
            }

            var hiddenHorizontalScrollbarStyle                                         = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenHorizontalScrollbarStyle == null) hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;

            var hiddenVerticalScrollbarStyle                                       = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenVerticalScrollbarStyle == null) hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var textureStyle                       = GUI.skin.FindStyle("DrawTexture");
            if (textureStyle == null) textureStyle = GUI.skin.label;

            var listButtonStyle                          = GUI.skin.FindStyle("ListButton");
            if (listButtonStyle == null) listButtonStyle = GUI.skin.button;

            EditorGUIUtility.SetIconSize(BRT_BuildReportWindow.IconSize);

            var viewOffset = hasSearchResults ? this._searchViewOffset : list.GetViewOffsetForDisplayedList(filter);

            // if somehow view offset was out of bounds of the SizePart[] array
            // reset it to zero
            if (viewOffset >= assetListToUse.Length)
            {
                list.SetViewOffsetForDisplayedList(filter, 0);
                viewOffset = 0;
            }

            var len = Mathf.Min(viewOffset + length, assetListToUse.Length);

            // --------------------------------------------------------------------------------------------------------

            var showTextureColumns = filter.SelectedFilterIdx >= 0 && filter.GetSelectedFilterLabel() == BuildReportTool.Options.FileFilterNameForTextureData && textureData != null && textureData.HasContents;

            var showMeshColumns = filter.SelectedFilterIdx >= 0 && filter.GetSelectedFilterLabel() == BuildReportTool.Options.FileFilterNameForMeshData && meshData != null && meshData.HasContents;

            // --------------------------------------------------------------------------------------------------------

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);

            // --------------------------------------------------------------------------------------------------------

            #region Column: Asset Path and Name

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
            var useAlt = false;

            #region Header

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);

            var assetPathCheckboxRect = this._assetPathColumnHeaderRect;
            assetPathCheckboxRect.x      += 5;
            assetPathCheckboxRect.y      += 1;
            assetPathCheckboxRect.width  =  20;
            assetPathCheckboxRect.height -= 1;

            BuildReportTool.Options.ShowColumnAssetPath = GUI.Toggle(assetPathCheckboxRect, BuildReportTool.Options.ShowColumnAssetPath, this._assetPathCheckboxLabel);

            var sortTypeAssetFullPathStyleName = this.GetColumnHeaderStyle(BuildReportTool.AssetList.SortType.AssetFullPath);
            if (GUILayout.Button("     Asset Path",
                sortTypeAssetFullPathStyleName,
                BRT_BuildReportWindow.LayoutListHeight))
                this.ToggleSort(list, BuildReportTool.AssetList.SortType.AssetFullPath, filter);

            if (Event.current.type == EventType.Repaint)
            {
                this._assetPathColumnHeaderRect = GUILayoutUtility.GetLastRect();
                #if BRT_ASSET_LIST_SCREEN_DEBUG
			_debugText.AppendFormat("_assetPathColumnHeaderRect: {0}\nyMax: {1}\n", _assetPathColumnHeaderRect.ToString(),
				_assetPathColumnHeaderRect.yMax.ToString(CultureInfo.InvariantCulture));
                #endif
            }

            GUI.Toggle(assetPathCheckboxRect, BuildReportTool.Options.ShowColumnAssetPath, this._assetPathCheckboxLabel);

            // -----------------------------------------------------------------

            var sortTypeAssetFilenameStyleName = this.GetColumnHeaderStyle(BuildReportTool.AssetList.SortType.AssetFilename);
            if (GUILayout.Button("Asset Filename", sortTypeAssetFilenameStyleName, BRT_BuildReportWindow.LayoutListHeight)) this.ToggleSort(list, BuildReportTool.AssetList.SortType.AssetFilename, filter);

            GUILayout.EndHorizontal();

            #endregion

            // --------------------------------------------------------------------------------------------------------

            var newEntryHoveredIdx = -1;

            this._assetListScrollPos = GUILayout.BeginScrollView(this._assetListScrollPos,
                hiddenHorizontalScrollbarStyle,
                hiddenVerticalScrollbarStyle,
                BRT_BuildReportWindow.LayoutNone);

            var mousePos = Event.current.mousePosition;

            for (var n = viewOffset; n < len; ++n)
            {
                var b = assetListToUse[n];

                var styleToUse = useAlt
                    ? listAltEntryStyle
                    : listEntryStyle;
                var inSumSelect             = list.InSumSelection(b);
                if (inSumSelect) styleToUse = listSelectedEntryStyle;

                #region Entry Bg

                GUILayout.BeginHorizontal(styleToUse, BRT_BuildReportWindow.LayoutNone);

                if (!BuildReportTool.Options.DoubleClickOnAssetWillPing)
                {
                    // only asset entries inside the top-level Assets or Packages folder can be pinged
                    if (b.Name.IsInAssetsFolder() || b.Name.IsInPackagesFolder())
                    {
                        if (GUILayout.Button("Ping", listButtonStyle, BRT_BuildReportWindow.LayoutPingButton))
                        {
                            if (list.GetSelectedCount() > 1 && list.InSumSelection(b) && Event.current.alt)
                                // ping multiple
                                Utility.PingSelectedAssets(list);
                            else
                                Utility.PingAssetInProject(b.Name);
                        }
                    }
                    else
                        // add spacing where the ping button would be,
                        // so that this entry aligns with the rest
                    {
                        GUILayout.Space(38);
                    }
                }

                this._assetListEntry.image = AssetDatabase.GetCachedIcon(b.Name);
                var hasIcon = this._assetListEntry.image != null;

                var mouseIsOnEmptySpaceForIcon = false;

                if (!hasIcon)
                {
                    // entry has no icon, just add space so it aligns with the other entries
                    GUILayout.Label(string.Empty, textureStyle, BRT_BuildReportWindow.LayoutIconWidth);

                    if (Event.current.type == EventType.Repaint)
                    {
                        var emptySpaceForIconRect                                                = GUILayoutUtility.GetLastRect();
                        if (emptySpaceForIconRect.Contains(mousePos)) mouseIsOnEmptySpaceForIcon = true;
                    }
                }

                this._assetListEntry.text = this.GetPrettyAssetPath(b.Name, n, BuildReportTool.Options.ShowColumnAssetPath, inSumSelect);

                var temp      = styleToUse.normal.textColor;
                var origLeft  = styleToUse.padding.left;
                var origRight = styleToUse.padding.right;

                styleToUse.normal.textColor = styleToUse.onNormal.textColor;
                styleToUse.padding.right    = 0;

                styleToUse.normal.textColor = temp;
                styleToUse.padding.left     = 2;

                // the asset icon and name

                if (GUILayout.Button(this._assetListEntry, styleToUse, BRT_BuildReportWindow.LayoutListHeight))
                {
                    if (Event.current.control)
                    {
                        if (!inSumSelect)
                        {
                            list.AddToSumSelection(b);
                            this._assetListEntryLastClickedIdx = n;
                        }
                        else
                        {
                            list.ToggleSumSelection(b);
                            this._assetListEntryLastClickedIdx = -1;
                        }
                    }
                    else if (Event.current.shift)
                    {
                        if (this._assetListEntryLastClickedIdx != -1)
                        {
                            // select all from last clicked to here
                            if (this._assetListEntryLastClickedIdx < n)
                                for (var addToSelectIdx = this._assetListEntryLastClickedIdx; addToSelectIdx <= n; ++addToSelectIdx)
                                    list.AddToSumSelection(assetListToUse[addToSelectIdx]);
                            else if (this._assetListEntryLastClickedIdx > n)
                                for (var addToSelectIdx = n; addToSelectIdx <= this._assetListEntryLastClickedIdx; ++addToSelectIdx)
                                    list.AddToSumSelection(assetListToUse[addToSelectIdx]);
                        }
                        else
                        {
                            list.AddToSumSelection(b);
                        }

                        this._assetListEntryLastClickedIdx = n;
                    }
                    else
                    {
                        // single select
                        // -----------------------------

                        // double-click detection for pinging
                        if (BuildReportTool.Options.DoubleClickOnAssetWillPing && EditorApplication.timeSinceStartup - this._assetListEntryLastClickedTime < DOUBLE_CLICK_THRESHOLD && (b.Name.IsInAssetsFolder() || b.Name.IsInPackagesFolder()))
                        {
                            if (list.GetSelectedCount() > 1 && list.InSumSelection(b) && Event.current.alt)
                                // double-clicking on one of the selected assets while holding alt
                                // ping multiple
                                Utility.PingSelectedAssets(list);
                            else if (this._assetListEntryLastClickedIdx == n)
                                // 2nd click on the same asset (i.e. double-click)
                                Utility.PingAssetInProject(b.Name);
                        }

                        // --------------------
                        // selecting a different asset
                        // click with no ctrl, alt, or shift
                        if ((this._assetListEntryLastClickedIdx != n || this._filterIdxOfLastClickedAssetListEntry != filter.SelectedFilterIdx) && !Event.current.alt)
                        {
                            list.ClearSelection();
                            list.AddToSumSelection(b);

                            this._assetListEntryLastClickedIdx         = n;
                            this._filterIdxOfLastClickedAssetListEntry = filter.SelectedFilterIdx;

                            // --------------------
                            // update what's shown in the Asset Usage Panel

                            if (b.Name.IsInResourcesFolder())
                            {
                                // this is a Resources asset
                                this._selectedIsAResourcesAsset    = true;
                                this._selectedResourcesAssetPath   = b.Name;
                                this._selectedResourcesAsset.text  = this._selectedResourcesAssetPath.GetFileNameOnly();
                                this._selectedResourcesAsset.image = AssetDatabase.GetCachedIcon(this._selectedResourcesAssetPath);
                            }
                            else
                            {
                                this._selectedIsAResourcesAsset  = false;
                                this._selectedResourcesAssetPath = null;
                            }

                            if (this._selectedAssetUsageDisplayIdx == ASSET_USAGE_DISPLAY_ALL)
                            {
                                this._selectedAssetUserIdx = -1;
                                this._assetUsageAncestry.Clear();
                                this.SetAssetUsageHistoryToFirstEndUser(b.Name, assetDependencies);
                            }
                            else if (this._selectedAssetUsageDisplayIdx == ASSET_USAGE_DISPLAY_DIRECT)
                            {
                                this.ChangeAssetUserCrumbRootIfNeeded(b.Name);
                            }
                        }

                        this._assetListEntryLastClickedTime = EditorApplication.timeSinceStartup;
                    }
                }

                styleToUse.padding.right = origRight;
                styleToUse.padding.left  = origLeft;

                #if BRT_ASSET_LIST_SCREEN_DEBUG
			//_debugText.AppendFormat("mousePos: {0}\n", mousePos.ToString());
                #endif

                if (Event.current.type == EventType.Repaint)
                {
                    // Have to do this during Repaint event because
                    // GUILayoutUtility.GetLastRect() only works during that time.
                    //
                    // The problem is that our hover check should really only be done during
                    // MouseMove event instead. The way it is right now, doing the hover check
                    // every Repaint event, means it's doing this check over and over
                    // even if the mouse is sitting still at the same position.
                    //
                    // However, we do mitigate this by not calling EditorWindow.Repaint() if
                    // it's not needed.
                    //
                    // Also, getting GetLastRect() during Repaint event and then using
                    // that value during MouseMove event means having to store the rect value
                    // in a variable. Since we're checking for each entry in the asset list,
                    // we'd have to store all the rects in a List
                    // (we have one rect for each entry in the asset list).
                    //
                    // I don't know if that is too much processing,
                    // so I'm leaving the code the way it is right now.

                    var assetListEntryRect = GUILayoutUtility.GetLastRect();

                    // note: Rects of the asset list entries do not overlap.
                    // So actually, throughout all the iterations of this for-loop,
                    // this if can only be successful once.
                    if (assetListEntryRect.Contains(mousePos) || mouseIsOnEmptySpaceForIcon)
                    {
                        newEntryHoveredIdx = n;

                        // ----------------
                        // update what is considered the hovered asset, for use later on
                        // when the tooltip will be drawn
                        BRT_BuildReportWindow.UpdateHoveredAsset(b.Name,
                            assetListEntryRect,
                            this.IsShowingUsedAssets,
                            buildReportToDisplay,
                            assetDependencies);

                        // ----------------
                        // put a border on the icon to signify that it's the one being hovered
                        // note: _assetListEntry.image currently has the icon of the asset we hovered
                        var iconHoveredRect = assetListEntryRect;
                        if (this._assetListEntry.image != null)
                            iconHoveredRect.x += 1;
                        else
                            iconHoveredRect.x -= 15;

                        iconHoveredRect.y      += 2;
                        iconHoveredRect.width  =  17;
                        iconHoveredRect.height =  16;

                        var iconHoveredStyle                           = GUI.skin.FindStyle("IconHovered");
                        if (iconHoveredStyle == null) iconHoveredStyle = GUI.skin.label;
                        GUI.Box(iconHoveredRect, this._assetListEntry.image, iconHoveredStyle);

                        // ----------------
                        // if mouse is hovering over the correct area, we signify that
                        // the tooltip thumbnail should be drawn
                        if (BuildReportTool.Options.ShowTooltipThumbnail && (BuildReportTool.Options.ShowThumbnailOnHoverLabelToo || Mathf.Abs(mousePos.x - assetListEntryRect.x) < BRT_BuildReportWindow.ICON_WIDTH_WITH_PADDING) && BRT_BuildReportWindow.GetAssetPreview(b.Name) != null)
                            this._shouldShowThumbnailOnHoveredAsset = true;
                        else
                            this._shouldShowThumbnailOnHoveredAsset = false;
                    }
                }

                GUILayout.EndHorizontal();

                #endregion

                useAlt = !useAlt;
            } // end of for-loop for drawing all asset names

            if (Event.current.type == EventType.Repaint) this._assetListEntryHoveredIdx = newEntryHoveredIdx;

            #if BRT_ASSET_LIST_SCREEN_DEBUG
		_debugText.AppendFormat("_assetListEntryLastClickedTime: {0}\n",
			_assetListEntryLastClickedTime.ToString(CultureInfo.InvariantCulture));
		_debugText.AppendFormat("_assetListEntryHovered: {0}\n", _assetListEntryHoveredIdx.ToString());
            #endif

            GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

            GUILayout.EndScrollView();

            GUILayout.EndVertical(); // end of column: asset path and name

            #endregion

            // --------------------------------------------------------------------------------------------------------

            #region Columns: Texture Data

            if (Event.current.type == EventType.Repaint)
            {
                this._hoveredTextureDataId            = TextureData.DataId.None;
                this._overridenTextureDataTooltipText = null;
            }

            this._clickedTextureDataId = TextureData.DataId.None;

            if (showTextureColumns)
            {
                if (BuildReportTool.Options.ShowTextureColumnTextureType)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.TextureType,
                        "Type",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnIsSRGB)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.IsSRGB,
                        "Is sRGB?",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnAlphaSource)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.AlphaSource,
                        "Alpha Source",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnAlphaIsTransparency)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.AlphaIsTransparency,
                        "Alpha is Transparency",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnIgnorePngGamma)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.IgnorePngGamma,
                        "Ignore PNG Gamma",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnIsReadable)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.IsReadable,
                        "Read/Write Enabled",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                // -------------------------------
                if (BuildReportTool.Options.ShowTextureColumnMipMapGenerated)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.MipMapGenerated,
                        "MipMap Generated",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnMipMapFilter)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.MipMapFilter,
                        "MipMap Filter",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnStreamingMipMaps)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.StreamingMipMaps,
                        "Streaming MipMaps",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnBorderMipMaps)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.BorderMipMaps,
                        "Border MipMaps",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnPreserveCoverageMipMaps)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.PreserveCoverageMipMaps,
                        "Preserve Coverage MipMaps",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnFadeOutMipMaps)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.FadeOutMipMaps,
                        "Fade Out MipMaps",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                // -------------------------------
                if (BuildReportTool.Options.ShowTextureColumnSpriteImportMode)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.SpriteImportMode,
                        "Sprite Mode",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnSpritePackingTag)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.SpritePackingTag,
                        "Sprite Packing Tag",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnSpritePixelsPerUnit)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.SpritePixelsPerUnit,
                        "Sprite Pixels-Per-Unit",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnQualifiesForSpritePacking)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.QualifiesForSpritePacking,
                        "Qualifies for Sprite Packing",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnWrapMode)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.WrapMode,
                        "Wrap Mode",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnWrapModeU)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.WrapModeU,
                        "Wrap Mode U",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnWrapModeV)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.WrapModeV,
                        "Wrap Mode V",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnWrapModeW)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.WrapModeW,
                        "Wrap Mode W",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                // -------------------------------
                if (BuildReportTool.Options.ShowTextureColumnFilterMode)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.FilterMode,
                        "Filter Mode",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnAnisoLevel)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.AnisoLevel,
                        "Anisotropic Level",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                // -------------------------------
                if (BuildReportTool.Options.ShowTextureColumnTextureFormat)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.TextureFormat,
                        "Format",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnCompressionType)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.CompressionType,
                        "Compression Type",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnCompressionIsCrunched)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.CompressionIsCrunched,
                        "Compression Crunched",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnCompressionQuality)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.CompressionQuality,
                        "Compression Quality",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                // -------------------------------
                if (BuildReportTool.Options.ShowTextureColumnResizeAlgorithm)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.TextureResizeAlgorithm,
                        "Resize Algorithm",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnMaxTextureSize)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.MaxTextureSize,
                        "Max Size",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnNPotScale)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.NPotScale,
                        "Non-Power Of 2 Scale",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnImportedWidthAndHeight)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.ImportedWidthAndHeight,
                        "Imported Width & Height",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowTextureColumnRealWidthAndHeight)
                    this.DrawTextureDataColumn(viewOffset,
                        len,
                        TextureData.DataId.RealWidthAndHeight,
                        "Source Width & Height",
                        true,
                        false,
                        list,
                        textureData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                // -------------------------------

                if (this._clickedTextureDataId != TextureData.DataId.None)
                {
                    this.ToggleSort(list, textureData, this._clickedTextureDataId, filter);
                    this._clickedTextureDataId = TextureData.DataId.None;
                }
            }

            #endregion

            // --------------------------------------------------------------------------------------------------------

            #region Columns: Mesh Data

            if (Event.current.type == EventType.Repaint) this._hoveredMeshDataId = MeshData.DataId.None;

            this._clickedMeshDataId = MeshData.DataId.None;

            if (showMeshColumns)
            {
                if (BuildReportTool.Options.ShowMeshColumnMeshFilterCount)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.MeshFilterCount,
                        "Non-skinned",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowMeshColumnSkinnedMeshRendererCount)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.SkinnedMeshRendererCount,
                        "Skinned",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowMeshColumnSubMeshCount)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.SubMeshCount,
                        "SubMesh Count",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowMeshColumnVertexCount)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.VertexCount,
                        "Vertices",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowMeshColumnTriangleCount)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.TriangleCount,
                        "Faces",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowMeshColumnAnimationType)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.AnimationType,
                        "Animation Type",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);
                if (BuildReportTool.Options.ShowMeshColumnAnimationClipCount)
                    this.DrawMeshDataColumn(viewOffset,
                        len,
                        MeshData.DataId.AnimationClipCount,
                        "Animations",
                        true,
                        false,
                        list,
                        meshData,
                        assetListToUse,
                        ref this._assetListScrollPos);

                // -------------------------------

                if (this._clickedMeshDataId != MeshData.DataId.None)
                {
                    this.ToggleSort(list, meshData, this._clickedMeshDataId, filter);
                    this._clickedMeshDataId = MeshData.DataId.None;
                }
            }

            #endregion

            // --------------------------------------------------------------------------------------------------------

            #region Column: Raw File Size (Size Before Build)

            var pressedRawSizeSortBtn = false;

            var pressedSizeBeforeBuildSortBtn = false;

            if (this.IsShowingUsedAssets && BuildReportTool.Options.ShowColumnSizeBeforeBuild && assetListToUse[0].SizeInAssetsFolderBytes != -1)
                pressedSizeBeforeBuildSortBtn = this.DrawColumn(viewOffset,
                    len,
                    BuildReportTool.AssetList.SortType.SizeBeforeBuild,
                    "Size Before Build   ",
                    true,
                    false,
                    list,
                    assetListToUse,
                    (b) => b.SizeInAssetsFolder,
                    ref this._assetListScrollPos);

            if (this.IsShowingUsedAssets && BuildReportTool.Options.ShowColumnSizeInBuild && BuildReportTool.Options.ShowImportedSizeForUsedAssets)
                pressedRawSizeSortBtn = this.DrawColumn(viewOffset,
                    len,
                    BuildReportTool.AssetList.SortType.ImportedSizeOrRawSize,
                    "Size In Build",
                    true,
                    false,
                    list,
                    assetListToUse,
                    (b) =>
                    {
                        // assets in the "StreamingAssets" folder do not have an imported size
                        // in those cases, the raw size is the same as the imported size
                        // so just use the raw size
                        if (b.ImportedSize == "N/A") return b.RawSize;

                        return b.ImportedSize;
                    },
                    ref this._assetListScrollPos);

            if ((this.IsShowingUnusedAssets && BuildReportTool.Options.ShowColumnUnusedRawSize) || (this.IsShowingUsedAssets && BuildReportTool.Options.ShowColumnSizeInBuild && !BuildReportTool.Options.ShowImportedSizeForUsedAssets))
                pressedRawSizeSortBtn = this.DrawColumn(viewOffset,
                    len,
                    BuildReportTool.AssetList.SortType.RawSize,
                    this.IsShowingUnusedAssets ? "Raw Size" : "Size In Build",
                    true,
                    false,
                    list,
                    assetListToUse,
                    (b) => b.RawSize,
                    ref this._assetListScrollPos);

            #endregion

            var showScrollbarForImportedSize = this.IsShowingUnusedAssets;

            // --------------------------------------------------------------------------------------------------------

            #region Column: Imported File Size

            var pressedImpSizeSortBtn = false;

            if (this.IsShowingUnusedAssets && BuildReportTool.Options.ShowColumnUnusedImportedSize)
                pressedImpSizeSortBtn = this.DrawColumn(viewOffset,
                    len,
                    BuildReportTool.AssetList.SortType.ImportedSize,
                    "Imported Size   ",
                    true,
                    showScrollbarForImportedSize,
                    list,
                    assetListToUse,
                    (b) => b.ImportedSize,
                    ref this._assetListScrollPos);

            #endregion

            // --------------------------------------------------------------------------------------------------------

            #region Column: Percentage to Total Size

            var pressedPercentSortBtn = false;

            if (this.IsShowingUsedAssets)
                pressedPercentSortBtn = this.DrawColumn(viewOffset,
                    len,
                    BuildReportTool.AssetList.SortType.PercentSize,
                    "Percent   ",
                    true,
                    true,
                    list,
                    assetListToUse,
                    (b) =>
                    {
                        var text                   = string.Format("{0}%", b.Percentage.ToString(CultureInfo.InvariantCulture));
                        if (b.Percentage < 0) text = Labels.NON_APPLICABLE_PERCENTAGE_LABEL;

                        return text;
                    },
                    ref this._assetListScrollPos);

            #endregion

            // --------------------------------------------------------------------------------------------------------

            #region Handle Sort Change

            if (pressedRawSizeSortBtn)
            {
                var sortType                                                                                    = BuildReportTool.AssetList.SortType.RawSize;
                if (this.IsShowingUsedAssets && BuildReportTool.Options.ShowImportedSizeForUsedAssets) sortType = BuildReportTool.AssetList.SortType.ImportedSizeOrRawSize;

                this.ToggleSort(list, sortType, filter);
            }
            else if (pressedSizeBeforeBuildSortBtn)
            {
                this.ToggleSort(list, BuildReportTool.AssetList.SortType.SizeBeforeBuild, filter);
            }
            else if (pressedImpSizeSortBtn)
            {
                this.ToggleSort(list, BuildReportTool.AssetList.SortType.ImportedSize, filter);
            }
            else if (pressedPercentSortBtn)
            {
                this.ToggleSort(list, BuildReportTool.AssetList.SortType.PercentSize, filter);
            }

            #endregion

            GUILayout.EndHorizontal();
        }

        public static string GetPathColor(bool inSumSelect)
        {
            const string PATH_COLOR_UNSELECTED_WHITE_SKIN = "4f4f4fff";
            const string PATH_COLOR_SELECTED_WHITE_SKIN   = "cececeff";

            const string PATH_COLOR_UNSELECTED_DARK_SKIN = "767676ff";
            const string PATH_COLOR_SELECTED_DARK_SKIN   = "c2c2c2ff";

            var colorUsedForPath                                                                          = PATH_COLOR_UNSELECTED_WHITE_SKIN;
            if (EditorGUIUtility.isProSkin || BRT_BuildReportWindow.FORCE_USE_DARK_SKIN) colorUsedForPath = PATH_COLOR_UNSELECTED_DARK_SKIN;

            if (inSumSelect)
            {
                if (EditorGUIUtility.isProSkin || BRT_BuildReportWindow.FORCE_USE_DARK_SKIN)
                    colorUsedForPath = PATH_COLOR_SELECTED_DARK_SKIN;
                else
                    colorUsedForPath = PATH_COLOR_SELECTED_WHITE_SKIN;
            }

            return colorUsedForPath;
        }

        private delegate string ColumnDisplayDelegate(SizePart b);

        private bool DrawColumn(
            int                                sta,
            int                                end,
            BuildReportTool.AssetList.SortType columnType,
            string                             columnName,
            bool                               allowSort,
            bool                               showScrollbar,
            BuildReportTool.AssetList          assetListCollection,
            SizePart[]                         assetList,
            ColumnDisplayDelegate              dataToDisplay,
            ref Vector2                        scrollbarPos
        )
        {
            var buttonPressed = false;

            var hiddenHorizontalScrollbarStyle                                         = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenHorizontalScrollbarStyle == null) hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;

            var hiddenVerticalScrollbarStyle                                       = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenVerticalScrollbarStyle == null) hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var listEntryStyle                         = GUI.skin.FindStyle(Settings.LIST_SMALL_STYLE_NAME);
            if (listEntryStyle == null) listEntryStyle = GUI.skin.label;

            var listAltEntryStyle                            = GUI.skin.FindStyle(Settings.LIST_SMALL_ALT_STYLE_NAME);
            if (listAltEntryStyle == null) listAltEntryStyle = GUI.skin.label;

            var listSelectedEntryStyle                                 = GUI.skin.FindStyle(Settings.LIST_SMALL_SELECTED_NAME);
            if (listSelectedEntryStyle == null) listSelectedEntryStyle = GUI.skin.label;

            // ----------------------------------------------------------

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            // ----------------------------------------------------------
            // column header
            var sortTypeStyleName = Settings.LIST_COLUMN_HEADER_STYLE_NAME;
            if (allowSort && this._currentSortType == columnType)
            {
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                    sortTypeStyleName = Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
                else
                    sortTypeStyleName = Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
            }
            var sortTypeStyle                        = GUI.skin.FindStyle(sortTypeStyleName);
            if (sortTypeStyle == null) sortTypeStyle = GUI.skin.label;

            if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort) buttonPressed = true;

            // ----------------------------------------------------------
            // scrollbar
            if (showScrollbar)
                scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
                    hiddenHorizontalScrollbarStyle,
                    verticalScrollbarStyle,
                    BRT_BuildReportWindow.LayoutNone);
            else
                scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
                    hiddenHorizontalScrollbarStyle,
                    hiddenVerticalScrollbarStyle,
                    BRT_BuildReportWindow.LayoutNone);

            // ----------------------------------------------------------
            // actual contents
            var useAlt = false;

            for (var n = sta; n < end; ++n)
            {
                var b = assetList[n];

                var styleToUse                                        = useAlt ? listAltEntryStyle : listEntryStyle;
                if (assetListCollection.InSumSelection(b)) styleToUse = listSelectedEntryStyle;

                GUILayout.Label(dataToDisplay(b), styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);

                useAlt = !useAlt;
            }

            GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            return buttonPressed;
        }

        private void DrawTextureDataColumn(
            int                       sta,
            int                       end,
            TextureData.DataId        textureDataId,
            string                    columnName,
            bool                      allowSort,
            bool                      showScrollbar,
            BuildReportTool.AssetList assetListCollection,
            TextureData               textureData,
            SizePart[]                assetList,
            ref Vector2               scrollbarPos
        )
        {
            var hiddenHorizontalScrollbarStyle                                         = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenHorizontalScrollbarStyle == null) hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;

            var hiddenVerticalScrollbarStyle                                       = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenVerticalScrollbarStyle == null) hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var listEntryStyle                         = GUI.skin.FindStyle(Settings.LIST_SMALL_STYLE_NAME);
            if (listEntryStyle == null) listEntryStyle = GUI.skin.label;

            var listAltEntryStyle                            = GUI.skin.FindStyle(Settings.LIST_SMALL_ALT_STYLE_NAME);
            if (listAltEntryStyle == null) listAltEntryStyle = GUI.skin.label;

            var listSelectedEntryStyle                                 = GUI.skin.FindStyle(Settings.LIST_SMALL_SELECTED_NAME);
            if (listSelectedEntryStyle == null) listSelectedEntryStyle = GUI.skin.label;

            // ----------------------------------------------------------

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            // ----------------------------------------------------------
            // column header
            var sortTypeStyleName = Settings.LIST_COLUMN_HEADER_STYLE_NAME;
            if (allowSort && this._currentSortType == BuildReportTool.AssetList.SortType.TextureData && this._currentTextureDataSortType == textureDataId)
            {
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                    sortTypeStyleName = Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
                else
                    sortTypeStyleName = Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
            }
            var sortTypeStyle                        = GUI.skin.FindStyle(sortTypeStyleName);
            if (sortTypeStyle == null) sortTypeStyle = GUI.skin.label;

            if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort) this._clickedTextureDataId = textureDataId;

            if (Event.current.type == EventType.Repaint)
            {
                var lastRect                                                                   = GUILayoutUtility.GetLastRect();
                if (lastRect.Contains(Event.current.mousePosition)) this._hoveredTextureDataId = textureDataId;
            }

            // ----------------------------------------------------------
            // scrollbar
            if (showScrollbar)
                scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
                    hiddenHorizontalScrollbarStyle,
                    verticalScrollbarStyle,
                    BRT_BuildReportWindow.LayoutNone);
            else
                scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
                    hiddenHorizontalScrollbarStyle,
                    hiddenVerticalScrollbarStyle,
                    BRT_BuildReportWindow.LayoutNone);

            // ----------------------------------------------------------
            // actual contents
            var useAlt = false;

            var textureDataEntries = textureData.GetTextureData();
            for (var n = sta; n < end; ++n)
            {
                var b = assetList[n];

                var styleToUse                                        = useAlt ? listAltEntryStyle : listEntryStyle;
                if (assetListCollection.InSumSelection(b)) styleToUse = listSelectedEntryStyle;

                var dataToDisplay                      = string.Empty;
                var assetHasTextureData                = textureDataEntries.ContainsKey(b.Name);
                if (assetHasTextureData) dataToDisplay = textureDataEntries[b.Name].ToDisplayedValue(textureDataId);

                GUILayout.Label(dataToDisplay, styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);
                if (assetHasTextureData && Event.current.type == EventType.Repaint && textureDataEntries[b.Name].IsOverriden(textureDataId))
                {
                    var lastRect                                                                              = GUILayoutUtility.GetLastRect();
                    if (lastRect.Contains(Event.current.mousePosition)) this._overridenTextureDataTooltipText = textureDataEntries[b.Name].GetOverridingMessage(textureDataId);
                }

                useAlt = !useAlt;
            }

            GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private void DrawMeshDataColumn(
            int                       sta,
            int                       end,
            MeshData.DataId           meshDataId,
            string                    columnName,
            bool                      allowSort,
            bool                      showScrollbar,
            BuildReportTool.AssetList assetListCollection,
            MeshData                  meshData,
            SizePart[]                assetList,
            ref Vector2               scrollbarPos
        )
        {
            var hiddenHorizontalScrollbarStyle                                         = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenHorizontalScrollbarStyle == null) hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;

            var hiddenVerticalScrollbarStyle                                       = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenVerticalScrollbarStyle == null) hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var listEntryStyle                         = GUI.skin.FindStyle(Settings.LIST_SMALL_STYLE_NAME);
            if (listEntryStyle == null) listEntryStyle = GUI.skin.label;

            var listAltEntryStyle                            = GUI.skin.FindStyle(Settings.LIST_SMALL_ALT_STYLE_NAME);
            if (listAltEntryStyle == null) listAltEntryStyle = GUI.skin.label;

            var listSelectedEntryStyle                                 = GUI.skin.FindStyle(Settings.LIST_SMALL_SELECTED_NAME);
            if (listSelectedEntryStyle == null) listSelectedEntryStyle = GUI.skin.label;

            // ----------------------------------------------------------

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            // ----------------------------------------------------------
            // column header
            var sortTypeStyleName = Settings.LIST_COLUMN_HEADER_STYLE_NAME;
            if (allowSort && this._currentSortType == BuildReportTool.AssetList.SortType.MeshData && this._currentMeshDataSortType == meshDataId)
            {
                if (this._currentSortOrder == BuildReportTool.AssetList.SortOrder.Descending)
                    sortTypeStyleName = Settings.LIST_COLUMN_HEADER_DESC_STYLE_NAME;
                else
                    sortTypeStyleName = Settings.LIST_COLUMN_HEADER_ASC_STYLE_NAME;
            }
            var sortTypeStyle                        = GUI.skin.FindStyle(sortTypeStyleName);
            if (sortTypeStyle == null) sortTypeStyle = GUI.skin.label;

            if (GUILayout.Button(columnName, sortTypeStyle, BRT_BuildReportWindow.LayoutListHeight) && allowSort) this._clickedMeshDataId = meshDataId;

            if (Event.current.type == EventType.Repaint)
            {
                var lastRect                                                                = GUILayoutUtility.GetLastRect();
                if (lastRect.Contains(Event.current.mousePosition)) this._hoveredMeshDataId = meshDataId;
            }

            // ----------------------------------------------------------
            // scrollbar
            if (showScrollbar)
                scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
                    hiddenHorizontalScrollbarStyle,
                    verticalScrollbarStyle,
                    BRT_BuildReportWindow.LayoutNone);
            else
                scrollbarPos = GUILayout.BeginScrollView(scrollbarPos,
                    hiddenHorizontalScrollbarStyle,
                    hiddenVerticalScrollbarStyle,
                    BRT_BuildReportWindow.LayoutNone);

            // ----------------------------------------------------------
            // actual contents
            var useAlt = false;

            var meshDataEntries = meshData.GetMeshData();
            for (var n = sta; n < end; ++n)
            {
                var b = assetList[n];

                var styleToUse = useAlt
                    ? listAltEntryStyle
                    : listEntryStyle;
                if (assetListCollection.InSumSelection(b)) styleToUse = listSelectedEntryStyle;

                var dataToDisplay                   = string.Empty;
                var assetHasMeshData                = meshDataEntries.ContainsKey(b.Name);
                if (assetHasMeshData) dataToDisplay = meshDataEntries[b.Name].ToDisplayedValue(meshDataId);

                GUILayout.Label(dataToDisplay, styleToUse, BRT_BuildReportWindow.LayoutListHeightMinWidth90);

                useAlt = !useAlt;
            }

            GUILayout.Space(SCROLLBAR_BOTTOM_PADDING);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
    }
}