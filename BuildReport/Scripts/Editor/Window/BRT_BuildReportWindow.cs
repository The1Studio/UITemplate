//#define BRT_SHOW_MINOR_WARNINGS

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using BuildReportTool;
using BuildReportTool.Window;

// can't put this in a namespace since older versions of Unity doesn't allow that
public class BRT_BuildReportWindow : EditorWindow
{
    private const int ICON_WIDTH              = 16;
    public const  int ICON_WIDTH_WITH_PADDING = 20;
    public const  int LIST_HEIGHT             = 20;

    public static Vector2 IconSize = new(15, 15);

    public static readonly GUILayoutOption[] LayoutNone = { };

    public static readonly GUILayoutOption[] LayoutListHeight =
        { GUILayout.Height(LIST_HEIGHT), GUILayout.ExpandHeight(false) };

    public static readonly GUILayoutOption[] LayoutListHeightMinWidth90 =
        { GUILayout.MinWidth(90), GUILayout.Height(LIST_HEIGHT) };

    public static readonly GUILayoutOption[] LayoutNoExpandWidth =
        { GUILayout.ExpandWidth(false) };

    public static readonly GUILayoutOption[] LayoutExpandWidth =
        { GUILayout.ExpandWidth(true) };

    public static readonly GUILayoutOption[] LayoutMinHeight30 =
        { GUILayout.MinHeight(30), GUILayout.ExpandHeight(true) };

    public static readonly GUILayoutOption[] LayoutHeight11    = { GUILayout.Height(11) };
    public static readonly GUILayoutOption[] LayoutHeight18    = { GUILayout.Height(18) };
    public static readonly GUILayoutOption[] LayoutHeight21    = { GUILayout.Height(21) };
    public static readonly GUILayoutOption[] LayoutHeight25    = { GUILayout.Height(25) };
    public static readonly GUILayoutOption[] LayoutMinWidth200 = { GUILayout.MinWidth(200) };
    public static readonly GUILayoutOption[] LayoutPingButton  = { GUILayout.Width(37) };
    public static readonly GUILayoutOption[] LayoutIconWidth   = { GUILayout.Width(ICON_WIDTH) };
    public static readonly GUILayoutOption[] Layout20x16       = { GUILayout.Width(20), GUILayout.Height(16) };
    public static readonly GUILayoutOption[] Layout20x25       = { GUILayout.Width(20), GUILayout.Height(25) };
    public static readonly GUILayoutOption[] Layout20x30       = { GUILayout.Width(20), GUILayout.Height(30) };
    public static readonly GUILayoutOption[] Layout28x30       = { GUILayout.Width(28), GUILayout.Height(30) };
    public static readonly GUILayoutOption[] Layout100To400x30 = { GUILayout.MinWidth(100), GUILayout.MaxWidth(400), GUILayout.Height(30) };
    public static readonly GUILayoutOption[] LayoutTo100x30    = { GUILayout.MaxWidth(100), GUILayout.Height(30) };

    public static readonly GUILayoutOption[] Layout100x30      = { GUILayout.MinWidth(100), GUILayout.Height(30), GUILayout.ExpandWidth(true) };
    public static readonly GUILayoutOption[] LayoutMaxWidth500 = { GUILayout.MaxWidth(500) };

    public const string STYLE_BREADCRUMB_LEFT = "GUIEditor.BreadcrumbLeft";
    public const string STYLE_BREADCRUMB_MID  = "GUIEditor.BreadcrumbMid";

    private void OnDisable()
    {
        this.ForceStopFileLoadThread();
        IsOpen = false;
    }

    private void OnFocus()
    {
        if (Options.AutoResortAssetsWhenUnityEditorRegainsFocus)
        {
            this._usedAssetsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
            this._unusedAssetsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);

            // check if configured file filters changed and only then do we need to recategorize

            if (Options.ShouldUseConfiguredFileFilters()) this.RecategorizeDisplayedBuildInfo();
        }
    }

    private void OnEnable()
    {
        //Debug.Log("BuildReportWindow.OnEnable() " + System.DateTime.Now);

        #if UNITY_5_6_OR_NEWER
        this.wantsMouseEnterLeaveWindow = true;
        #endif
        this.wantsMouseMove = true;

        IsOpen = true;

        this.InitGUISkin();

        if (Util.BuildInfoHasContents(_buildInfo))
        {
            //Debug.Log("recompiled " + _buildInfo.SavedPath);
            if (!string.IsNullOrEmpty(_buildInfo.SavedPath))
            {
                var loadedBuild                                        = Util.OpenSerializedBuildInfo(_buildInfo.SavedPath);
                if (Util.BuildInfoHasContents(loadedBuild)) _buildInfo = loadedBuild;
            }
            else
            {
                if (_buildInfo.HasUsedAssets)
                    _buildInfo.UsedAssets.AssignPerCategoryList(
                        ReportGenerator.SegregateAssetSizesPerCategory(_buildInfo.UsedAssets.All,
                            _buildInfo.FileFilters));

                if (_buildInfo.HasUnusedAssets)
                    _buildInfo.UnusedAssets.AssignPerCategoryList(
                        ReportGenerator.SegregateAssetSizesPerCategory(_buildInfo.UnusedAssets.All,
                            _buildInfo.FileFilters));
            }
        }

        this._usedAssetsScreen.SetListToDisplay(BuildReportTool.Window.Screen.AssetList.ListToDisplay.UsedAssets);
        this._unusedAssetsScreen.SetListToDisplay(BuildReportTool.Window.Screen.AssetList.ListToDisplay.UnusedAssets);

        this._overviewScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._buildSettingsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._buildStepsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._sizeStatsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._usedAssetsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._unusedAssetsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._extraDataScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);

        this._optionsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._helpScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
    }

    private double _lastTime;

    private void OnInspectorUpdate()
    {
        var deltaTime = EditorApplication.timeSinceStartup - this._lastTime;
        this._lastTime = EditorApplication.timeSinceStartup;

        if (this.IsInUsedAssetsCategory)
            this._usedAssetsScreen.Update(EditorApplication.timeSinceStartup, deltaTime, _buildInfo, _assetDependencies);
        else if (this.IsInUnusedAssetsCategory) this._unusedAssetsScreen.Update(EditorApplication.timeSinceStartup, deltaTime, _buildInfo, _assetDependencies);

        if (_buildInfo != null && ReportGenerator.IsFinishedGettingValues) this.OnFinishGeneratingBuildReport();

        // if Unity Editor has finished making a build and we are scheduled to create a Build Report...
        if (Util.ShouldGetBuildReportNow && !ReportGenerator.IsStillGettingValues && !EditorApplication.isCompiling)
            //Debug.Log("BuildReportWindow getting build info right after the build... " + System.DateTime.Now);
            this.Refresh(true);

        if (this._finishedOpeningFromThread) this.OnFinishOpeningBuildReportFile();
    }

    private void Update()
    {
        if (_buildInfo != null)
            if (_buildInfo.RequestedToRefresh)
            {
                this.Repaint();
                _buildInfo.FlagFinishedRefreshing();
            }
    }

    // ==========================================================================================
    // sub-screens

    private readonly BuildReportTool.Window.Screen.Overview _overviewScreen = new();

    private readonly BuildReportTool.Window.Screen.BuildSettings _buildSettingsScreen = new();

    private readonly BuildReportTool.Window.Screen.BuildStepsScreen _buildStepsScreen = new();

    private readonly BuildReportTool.Window.Screen.SizeStats _sizeStatsScreen    = new();
    private readonly BuildReportTool.Window.Screen.AssetList _usedAssetsScreen   = new();
    private readonly BuildReportTool.Window.Screen.AssetList _unusedAssetsScreen = new();
    private readonly BuildReportTool.Window.Screen.ExtraData _extraDataScreen    = new();

    private readonly BuildReportTool.Window.Screen.Options _optionsScreen = new();
    private readonly BuildReportTool.Window.Screen.Help    _helpScreen    = new();

    // ==========================================================================================

    public static string GetValueMessage { set; get; }

    private static bool _loadingValuesFromThread;

    public static bool LoadingValuesFromThread => _loadingValuesFromThread;

    private static bool _noGuiSkinFound;

    /// <summary>
    /// The Build Report we're displaying.
    /// </summary>
    private static BuildInfo _buildInfo;

    /// <summary>
    /// The Asset Dependencies data being used
    /// for whichever Build Report is displayed.
    /// </summary>
    private static AssetDependencies _assetDependencies;

    /// <summary>
    /// The TextureData being used
    /// for whichever Build Report is displayed.
    /// </summary>
    private static TextureData _textureData;

    /// <summary>
    /// The MeshData being used
    /// for whichever Build Report is displayed.
    /// </summary>
    private static MeshData _meshData;

    private static UnityBuildReport _unityBuildReport;

    private static ExtraData _extraData;

    public const bool FORCE_USE_DARK_SKIN = false;

    private GUISkin _usedSkin = null;

    public static bool IsOpen { get; set; }

    public static bool ZoomedInThumbnails           { get; set; }
    public static bool ShowThumbnailsWithAlphaBlend { get; set; }

    private static Vector2 _lastMousePos;
    private static bool    _lastMouseMoved;

    public enum AssetInfoType
    {
        None,
        InStreamingAssetsFolder,
        InAPackage,
        InAResourcesFolder,
        ASceneInBuild,
    }

    /// <summary>
    /// Rect of whatever asset is hovered on
    /// </summary>
    public static Rect HoveredAssetEntryRect;

    /// <summary>
    /// Asset path of whatever asset is hovered
    /// </summary>
    public static string HoveredAssetEntryPath;

    public static readonly List<GUIContent> HoveredAssetEndUsers = new();

    public static void UpdateHoveredAsset(
        string            hoveredAssetPath,
        Rect              hoveredAssetRect,
        bool              showingUsedAssets,
        BuildInfo         buildReportToDisplay,
        AssetDependencies assetDependencies
    )
    {
        var alreadyUsingSameAssetPath =
            hoveredAssetPath.Equals(HoveredAssetEntryPath, StringComparison.OrdinalIgnoreCase);

        if (!alreadyUsingSameAssetPath) HoveredAssetEntryPath = hoveredAssetPath;

        // even if the new hovered asset to assign is the same as the current one,
        // its rect might have moved, so we always assign it
        HoveredAssetEntryRect = hoveredAssetRect;

        if (Options.ShowAssetPrimaryUsersInTooltipIfAvailable && !alreadyUsingSameAssetPath)
        {
            UpdateHoveredAssetType(hoveredAssetPath, showingUsedAssets);

            if (HoveredAssetIsASceneInBuild)
                UpdateSceneInBuildLabel(SceneIsInBuildLabel,
                    buildReportToDisplay.ScenesInBuild,
                    HoveredAssetEntryPath);

            AssignHoveredAssetEndUsers(assetDependencies);
        }
    }

    public static void UpdateSceneInBuildLabel(
        GUIContent               destination,
        BuildInfo.SceneInBuild[] scenesInBuild,
        string                   scenePath
    )
    {
        var foundSceneInBuild = false;
        for (int sceneN = 0, sceneLen = scenesInBuild.Length; sceneN < sceneLen; ++sceneN)
        {
            if (scenesInBuild[sceneN].Path.Equals(scenePath, StringComparison.OrdinalIgnoreCase))
            {
                destination.text  = string.Format(SCENE_IN_BUILD_LABEL_WITH_INDEX_FORMAT, sceneN.ToString());
                foundSceneInBuild = true;
                break;
            }
        }

        if (!foundSceneInBuild)
            // This doesn't make sense though. If we're showing used assets,
            // the scene *should* be in the ScenesInBuild array.
            //
            // One possibility is that the user might have had a custom build script
            // that was manipulating the values in UnityEditor.EditorBuildSettings.scenes
            // after Build Report generation recorded it into the ScenesInBuild array.
            //
            destination.text = SCENE_IN_BUILD_LABEL;
    }

    private static List<GUIContent> GetEndUserLabelsFor(AssetDependencies assetDependencies, string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath) || assetDependencies == null) return null;

        List<GUIContent> endUsersListToUse = null;

        var dependencies = assetDependencies.GetAssetDependencies();
        if (dependencies.ContainsKey(assetPath))
        {
            var entry                            = dependencies[assetPath];
            if (entry != null) endUsersListToUse = entry.GetEndUserLabels();
        }

        return endUsersListToUse;
    }

    private static void AssignHoveredAssetEndUsers(AssetDependencies assetDependencies)
    {
        AssetDependencies.PopulateAssetEndUsers(HoveredAssetEntryPath, assetDependencies);
    }

    private static AssetInfoType _hoveredAssetType = AssetInfoType.None;

    private static void UpdateHoveredAssetType(string hoveredAssetPath, bool showingUsedAssets)
    {
        if (hoveredAssetPath.IsInStreamingAssetsFolder())
            _hoveredAssetType = AssetInfoType.InStreamingAssetsFolder;
        else if (hoveredAssetPath.IsInPackagesFolder())
            _hoveredAssetType = AssetInfoType.InAPackage;
        else if (hoveredAssetPath.IsInResourcesFolder())
            _hoveredAssetType = AssetInfoType.InAResourcesFolder;
        else if (hoveredAssetPath.IsSceneFile() && showingUsedAssets)
            _hoveredAssetType = AssetInfoType.ASceneInBuild;
        else
            _hoveredAssetType = AssetInfoType.None;
    }

    public static bool HoveredAssetIsASceneInBuild => _hoveredAssetType == AssetInfoType.ASceneInBuild;

    public static bool ShouldHoveredAssetShowEndUserTooltip(AssetDependencies assetDependencies)
    {
        if (_hoveredAssetType != AssetInfoType.None) return true;

        var endUsersListToUse = GetEndUserLabelsFor(assetDependencies, HoveredAssetEntryPath);

        return endUsersListToUse != null && endUsersListToUse.Count > 0;
    }

    public static GUIContent GetAppropriateEndUserLabelForHovered()
    {
        switch (_hoveredAssetType)
        {
            case AssetInfoType.InAPackage: return InPackagesLabel;

            case AssetInfoType.InStreamingAssetsFolder: return InStreamingAssetsLabel;

            case AssetInfoType.InAResourcesFolder:
            {
                if (HoveredAssetEndUsers.Count > 0)
                    return AResourcesAssetButAlsoUsedByLabel;
                else
                    return AResourcesAssetLabel;
            }

            case AssetInfoType.ASceneInBuild: return SceneIsInBuildLabel;

            default: return UsedByLabel;
        }
    }

    /// <summary>
    /// "Used by:"
    /// </summary>
    private static readonly GUIContent UsedByLabel = new("Used by:");

    /// <summary>
    /// "Asset is in a Resources folder"
    /// </summary>
    private static readonly GUIContent AResourcesAssetLabel = new("Asset is in a Resources folder");

    /// <summary>
    /// "Asset is in the StreamingAssets folder"
    /// </summary>
    private static readonly GUIContent InStreamingAssetsLabel = new("File is in the StreamingAssets folder");

    /// <summary>
    /// "A Resources asset, but also used by:"
    /// </summary>
    private static readonly GUIContent AResourcesAssetButAlsoUsedByLabel = new("A Resources asset, but also used by:");

    /// <summary>
    /// "Scene is in build"
    /// </summary>
    public static readonly GUIContent SceneIsInBuildLabel = new("Scene is in build");

    private const string SCENE_IN_BUILD_LABEL_WITH_INDEX_FORMAT = "Scene is in build at index <b>{0}</b>";
    private const string SCENE_IN_BUILD_LABEL                   = "Scene is in build";

    private static readonly GUIContent InPackagesLabel = new("Asset is from the Packages folder");

    private Texture2D _toolbarIconLog;
    private Texture2D _toolbarIconOpen;
    private Texture2D _toolbarIconSave;
    private Texture2D _toolbarIconOptions;
    private Texture2D _toolbarIconHelp;

    private GUIContent _toolbarLabelLog;
    private GUIContent _toolbarLabelOpen;
    private GUIContent _toolbarLabelSave;
    private GUIContent _toolbarLabelOptions;
    private GUIContent _toolbarLabelHelp;

    private void RecategorizeDisplayedBuildInfo()
    {
        if (Util.BuildInfoHasContents(_buildInfo)) ReportGenerator.RecategorizeAssetList(_buildInfo);
    }

    private static void GetFromNative(GUIStyle ownStyle, GUIStyle nativeStyle, out GUIStyle styleToAssign, string desiredName = null)
    {
        if (nativeStyle == null)
        {
            styleToAssign = null;
            return;
        }

        if (ownStyle == null)
        {
            // make our own copy of the native skin
            styleToAssign = new(nativeStyle);
            if (!string.IsNullOrEmpty(desiredName)) styleToAssign.name = desiredName;
        }
        else
        {
            styleToAssign = null;

            if (!string.IsNullOrEmpty(desiredName)) ownStyle.name = desiredName;

            // ensure our skin uses Unity's builtin look
            ownStyle.normal.background   = nativeStyle.normal.background;
            ownStyle.hover.background    = nativeStyle.hover.background;
            ownStyle.active.background   = nativeStyle.active.background;
            ownStyle.onNormal.background = nativeStyle.onNormal.background;
            ownStyle.onHover.background  = nativeStyle.onHover.background;
            ownStyle.onActive.background = nativeStyle.onActive.background;

            #if UNITY_5_6_OR_NEWER
            if (nativeStyle.normal.scaledBackgrounds != null && nativeStyle.normal.scaledBackgrounds.Length > 0)
            {
                ownStyle.normal.scaledBackgrounds = new Texture2D[nativeStyle.normal.scaledBackgrounds.Length];
                for (var i = 0; i < nativeStyle.normal.scaledBackgrounds.Length; i++) ownStyle.normal.scaledBackgrounds[i] = nativeStyle.normal.scaledBackgrounds[i];
            }
            else
            {
                if (ownStyle.normal.scaledBackgrounds != null) ownStyle.normal.scaledBackgrounds = null;
            }

            if (nativeStyle.hover.scaledBackgrounds != null && nativeStyle.hover.scaledBackgrounds.Length > 0)
            {
                ownStyle.hover.scaledBackgrounds = new Texture2D[nativeStyle.hover.scaledBackgrounds.Length];
                for (var i = 0; i < nativeStyle.hover.scaledBackgrounds.Length; i++) ownStyle.hover.scaledBackgrounds[i] = nativeStyle.hover.scaledBackgrounds[i];
            }
            else
            {
                if (ownStyle.hover.scaledBackgrounds != null) ownStyle.hover.scaledBackgrounds = null;
            }

            if (nativeStyle.active.scaledBackgrounds != null && nativeStyle.active.scaledBackgrounds.Length > 0)
            {
                ownStyle.active.scaledBackgrounds = new Texture2D[nativeStyle.active.scaledBackgrounds.Length];
                for (var i = 0; i < nativeStyle.active.scaledBackgrounds.Length; i++) ownStyle.active.scaledBackgrounds[i] = nativeStyle.active.scaledBackgrounds[i];
            }
            else
            {
                if (ownStyle.active.scaledBackgrounds != null) ownStyle.active.scaledBackgrounds = null;
            }

            if (nativeStyle.onNormal.scaledBackgrounds != null && nativeStyle.onNormal.scaledBackgrounds.Length > 0)
            {
                ownStyle.onNormal.scaledBackgrounds = new Texture2D[nativeStyle.onNormal.scaledBackgrounds.Length];
                for (var i = 0; i < nativeStyle.onNormal.scaledBackgrounds.Length; i++) ownStyle.onNormal.scaledBackgrounds[i] = nativeStyle.onNormal.scaledBackgrounds[i];
            }
            else
            {
                if (ownStyle.onNormal.scaledBackgrounds != null) ownStyle.onNormal.scaledBackgrounds = null;
            }

            if (nativeStyle.onHover.scaledBackgrounds != null && nativeStyle.onHover.scaledBackgrounds.Length > 0)
            {
                ownStyle.onHover.scaledBackgrounds = new Texture2D[nativeStyle.onHover.scaledBackgrounds.Length];
                for (var i = 0; i < nativeStyle.onHover.scaledBackgrounds.Length; i++) ownStyle.onHover.scaledBackgrounds[i] = nativeStyle.onHover.scaledBackgrounds[i];
            }
            else
            {
                if (ownStyle.onHover.scaledBackgrounds != null) ownStyle.onHover.scaledBackgrounds = null;
            }

            if (nativeStyle.onActive.scaledBackgrounds != null && nativeStyle.onActive.scaledBackgrounds.Length > 0)
            {
                ownStyle.onActive.scaledBackgrounds = new Texture2D[nativeStyle.onActive.scaledBackgrounds.Length];
                for (var i = 0; i < nativeStyle.onActive.scaledBackgrounds.Length; i++) ownStyle.onActive.scaledBackgrounds[i] = nativeStyle.onActive.scaledBackgrounds[i];
            }
            else
            {
                if (ownStyle.onActive.scaledBackgrounds != null) ownStyle.onActive.scaledBackgrounds = null;
            }
            #endif

            ownStyle.normal.textColor   = nativeStyle.normal.textColor;
            ownStyle.hover.textColor    = nativeStyle.hover.textColor;
            ownStyle.active.textColor   = nativeStyle.active.textColor;
            ownStyle.onNormal.textColor = nativeStyle.onNormal.textColor;
            ownStyle.onHover.textColor  = nativeStyle.onHover.textColor;
            ownStyle.onActive.textColor = nativeStyle.onActive.textColor;

            ownStyle.border.top    = nativeStyle.border.top;
            ownStyle.border.bottom = nativeStyle.border.bottom;
            ownStyle.border.left   = nativeStyle.border.left;
            ownStyle.border.right  = nativeStyle.border.right;

            ownStyle.margin.top    = nativeStyle.margin.top;
            ownStyle.margin.bottom = nativeStyle.margin.bottom;
            ownStyle.margin.left   = nativeStyle.margin.left;
            ownStyle.margin.right  = nativeStyle.margin.right;

            ownStyle.padding.top    = nativeStyle.padding.top;
            ownStyle.padding.bottom = nativeStyle.padding.bottom;
            ownStyle.padding.left   = nativeStyle.padding.left;
            ownStyle.padding.right  = nativeStyle.padding.right;

            ownStyle.overflow.top    = nativeStyle.overflow.top;
            ownStyle.overflow.bottom = nativeStyle.overflow.bottom;
            ownStyle.overflow.left   = nativeStyle.overflow.left;
            ownStyle.overflow.right  = nativeStyle.overflow.right;

            ownStyle.font      = nativeStyle.font;
            ownStyle.fontStyle = nativeStyle.fontStyle;
            ownStyle.alignment = nativeStyle.alignment;

            ownStyle.richText = nativeStyle.richText;
            ownStyle.wordWrap = nativeStyle.wordWrap;

            ownStyle.contentOffset = nativeStyle.contentOffset;

            ownStyle.fixedWidth  = nativeStyle.fixedWidth;
            ownStyle.fixedHeight = nativeStyle.fixedHeight;

            ownStyle.stretchWidth  = nativeStyle.stretchWidth;
            ownStyle.stretchHeight = nativeStyle.stretchHeight;
        }
    }

    private void InitGUISkin()
    {
        string guiSkinToUse;
        if (EditorGUIUtility.isProSkin || FORCE_USE_DARK_SKIN)
            guiSkinToUse = Settings.DARK_GUI_SKIN_FILENAME;
        else
            guiSkinToUse = Settings.DEFAULT_GUI_SKIN_FILENAME;

        // try default path
        this._usedSkin = AssetDatabase.LoadAssetAtPath(
            string.Format("{0}/GUI/{1}", Options.BUILD_REPORT_TOOL_DEFAULT_PATH, guiSkinToUse),
            typeof(GUISkin)) as GUISkin;

        if (this._usedSkin == null)
        {
            #if BRT_SHOW_MINOR_WARNINGS
			Debug.LogWarning(BuildReportTool.Options.BUILD_REPORT_PACKAGE_MOVED_MSG);
            #endif

            var folderPath = Util.FindAssetFolder(Application.dataPath,
                Options.BUILD_REPORT_TOOL_DEFAULT_FOLDER_NAME);
            if (!string.IsNullOrEmpty(folderPath))
            {
                folderPath = folderPath.Replace('\\', '/');
                var assetsIdx                   = folderPath.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
                if (assetsIdx != -1) folderPath = folderPath.Substring(assetsIdx + 8, folderPath.Length - assetsIdx - 8);
                //Debug.Log(folderPath);

                this._usedSkin = AssetDatabase.LoadAssetAtPath(string.Format("Assets/{0}/GUI/{1}", folderPath, guiSkinToUse),
                    typeof(GUISkin)) as GUISkin;
            }
            else
            {
                Debug.LogError(Options.BUILD_REPORT_PACKAGE_MISSING_MSG);
            }

            //Debug.Log("_usedSkin " + (_usedSkin != null));
        }

        if (this._usedSkin != null)
        {
            // weirdo enum labels used to get either light or dark skin
            // (https://forum.unity.com/threads/editorguiutility-getbuiltinskin-not-working-correctly-in-unity-4-3.211504/#post-1430038)
            var nativeSkin =
                EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector);

            if (!(EditorGUIUtility.isProSkin || FORCE_USE_DARK_SKIN))
            {
                this._usedSkin.verticalScrollbar           = nativeSkin.verticalScrollbar;
                this._usedSkin.verticalScrollbarThumb      = nativeSkin.verticalScrollbarThumb;
                this._usedSkin.verticalScrollbarUpButton   = nativeSkin.verticalScrollbarUpButton;
                this._usedSkin.verticalScrollbarDownButton = nativeSkin.verticalScrollbarDownButton;

                this._usedSkin.horizontalScrollbar            = nativeSkin.horizontalScrollbar;
                this._usedSkin.horizontalScrollbarThumb       = nativeSkin.horizontalScrollbarThumb;
                this._usedSkin.horizontalScrollbarLeftButton  = nativeSkin.horizontalScrollbarLeftButton;
                this._usedSkin.horizontalScrollbarRightButton = nativeSkin.horizontalScrollbarRightButton;

                // change the toggle skin to use the Unity builtin look, but keep our settings
                var toggleSaved = new GUIStyle(this._usedSkin.toggle);

                // make our own copy of the native skin toggle so that editing it won't affect the rest of the editor GUI
                var nativeToggleCopy = new GUIStyle(nativeSkin.toggle);

                this._usedSkin.toggle               = nativeToggleCopy;
                this._usedSkin.toggle.font          = toggleSaved.font;
                this._usedSkin.toggle.fontSize      = toggleSaved.fontSize;
                this._usedSkin.toggle.border        = toggleSaved.border;
                this._usedSkin.toggle.margin        = toggleSaved.margin;
                this._usedSkin.toggle.padding       = toggleSaved.padding;
                this._usedSkin.toggle.overflow      = toggleSaved.overflow;
                this._usedSkin.toggle.contentOffset = toggleSaved.contentOffset;

                this._usedSkin.box       = nativeSkin.box;
                this._usedSkin.label     = nativeSkin.label;
                this._usedSkin.textField = nativeSkin.textField;
                this._usedSkin.button    = nativeSkin.button;

                this._usedSkin.label.wordWrap = true;
            }

            var miniButtonStyle = this._usedSkin.FindStyle("MiniButton");
            if (miniButtonStyle != null)
                if (miniButtonStyle.normal.background == null)
                {
                    miniButtonStyle.normal.background   = nativeSkin.button.normal.background;
                    miniButtonStyle.active.background   = nativeSkin.button.active.background;
                    miniButtonStyle.onNormal.background = nativeSkin.button.onNormal.background;
                    miniButtonStyle.onActive.background = nativeSkin.button.onActive.background;
                }

            // ----------------------------------------------------
            // Add styles we need

            var nativeReorderableListDragHandle   = nativeSkin.GetStyle("RL DragHandle");
            var nativeReorderableListHeader       = nativeSkin.GetStyle("RL Header");
            var nativeReorderableListFooter       = nativeSkin.GetStyle("RL Footer");
            var nativeReorderableListBg           = nativeSkin.GetStyle("RL Background");
            var nativeReorderableListFooterButton = nativeSkin.GetStyle("RL FooterButton");
            var nativeReorderableListElement      = nativeSkin.GetStyle("RL Element");
            var nativeReorderableListEmptyHeader  = nativeSkin.FindStyle("RL Empty Header");

            var reorderableListDragHandle   = this._usedSkin.FindStyle("RL DragHandle");
            var reorderableListHeader       = this._usedSkin.FindStyle("RL Header");
            var reorderableListFooter       = this._usedSkin.FindStyle("RL Footer");
            var reorderableListBg           = this._usedSkin.FindStyle("RL Background");
            var reorderableListFooterButton = this._usedSkin.FindStyle("RL FooterButton");
            var reorderableListElement      = this._usedSkin.FindStyle("RL Element");
            var reorderableListEmptyHeader  = this._usedSkin.FindStyle("RL Empty Header");

            GUIStyle reorderableListDragHandleToAssign;
            GUIStyle reorderableListHeaderToAssign;
            GUIStyle reorderableListFooterToAssign;
            GUIStyle reorderableListBgToAssign;
            GUIStyle reorderableListFooterButtonToAssign;
            GUIStyle reorderableListElementToAssign;
            GUIStyle reorderableListEmptyHeaderToAssign;
            GetFromNative(reorderableListDragHandle, nativeReorderableListDragHandle, out reorderableListDragHandleToAssign);
            GetFromNative(reorderableListHeader, nativeReorderableListHeader, out reorderableListHeaderToAssign);
            GetFromNative(reorderableListFooter, nativeReorderableListFooter, out reorderableListFooterToAssign);
            GetFromNative(reorderableListBg, nativeReorderableListBg, out reorderableListBgToAssign);
            GetFromNative(reorderableListFooterButton, nativeReorderableListFooterButton, out reorderableListFooterButtonToAssign);
            GetFromNative(reorderableListElement, nativeReorderableListElement, out reorderableListElementToAssign);
            GetFromNative(reorderableListEmptyHeader, nativeReorderableListEmptyHeader, out reorderableListEmptyHeaderToAssign);

            var nativeErrorIcon   = nativeSkin.FindStyle("CN EntryErrorIconSmall");
            var nativeWarningIcon = nativeSkin.FindStyle("CN EntryWarnIconSmall");
            var nativeLogIcon     = nativeSkin.FindStyle("CN EntryInfoIconSmall");

            var logMessageIcons    = this._usedSkin.FindStyle("LogMessageIcons");
            var addLogMessageIcons = logMessageIcons == null;
            if (addLogMessageIcons)
            {
                logMessageIcons      = new();
                logMessageIcons.name = "LogMessageIcons";
            }

            if (nativeLogIcon != null && nativeLogIcon.normal.background != null) logMessageIcons.normal.background = nativeLogIcon.normal.background;

            if (nativeWarningIcon != null && nativeWarningIcon.normal.background != null) logMessageIcons.hover.background = nativeWarningIcon.normal.background;

            if (nativeErrorIcon != null && nativeErrorIcon.normal.background != null) logMessageIcons.active.background = nativeErrorIcon.normal.background;

            #region LeftCrumb

            var      nativeLeftCrumb   = nativeSkin.GetStyle(STYLE_BREADCRUMB_LEFT);
            var      leftCrumb         = this._usedSkin.FindStyle(STYLE_BREADCRUMB_LEFT);
            GUIStyle leftCrumbToAssign = null;
            if (leftCrumb == null)
            {
                // make our own copy of the native skin left crumb so that editing it won't affect the rest of the editor GUI
                leftCrumbToAssign = new(nativeLeftCrumb);

                leftCrumbToAssign.fixedHeight = 19;

                #if UNITY_2019_1_OR_NEWER
                // in Unity 2019+, the styles have changed,
                // so ensure we modify it so it fits our GUI
                leftCrumbToAssign.overflow.top    = 1;
                leftCrumbToAssign.overflow.bottom = 1;
                #else
				leftCrumbToAssign.overflow.top = 0;
				leftCrumbToAssign.overflow.bottom = 0;
                #endif
            }
            else
            {
                // ensure our left crumb skin uses Unity's builtin look, but keep our settings
                leftCrumb.normal.background   = nativeLeftCrumb.normal.background;
                leftCrumb.hover.background    = nativeLeftCrumb.hover.background;
                leftCrumb.active.background   = nativeLeftCrumb.active.background;
                leftCrumb.onNormal.background = nativeLeftCrumb.onNormal.background;
                leftCrumb.onHover.background  = nativeLeftCrumb.onHover.background;
                leftCrumb.onActive.background = nativeLeftCrumb.onActive.background;

                leftCrumb.fixedHeight = 19;

                #if UNITY_2019_1_OR_NEWER
                // in Unity 2019+, the styles have changed,
                // so ensure we modify it so it fits our GUI
                leftCrumb.overflow.top    = 1;
                leftCrumb.overflow.bottom = 1;
                #else
				leftCrumb.overflow.top = 0;
				leftCrumb.overflow.bottom = 0;
                #endif
            }

            #endregion

            // ----------------------------------------------------

            #region MidCrumb

            var      nativeMidCrumb   = nativeSkin.GetStyle(STYLE_BREADCRUMB_MID);
            var      midCrumb         = this._usedSkin.FindStyle(STYLE_BREADCRUMB_MID);
            GUIStyle midCrumbToAssign = null;

            if (midCrumb == null)
            {
                // make our own copy of the native skin mid crumb so that editing it won't affect the rest of the editor GUI
                midCrumbToAssign = new(nativeMidCrumb);

                midCrumbToAssign.fixedHeight = 19;

                #if UNITY_2019_1_OR_NEWER
                // in Unity 2019+, the styles have changed,
                // so ensure we modify it so it fits our GUI
                midCrumbToAssign.overflow.top    = 1;
                midCrumbToAssign.overflow.bottom = 1;
                #else
				midCrumbToAssign.overflow.top = 0;
				midCrumbToAssign.overflow.bottom = 0;
                #endif
            }
            else
            {
                // ensure our mid crumb skin uses Unity's builtin look, but keep our settings
                midCrumb.normal.background   = nativeMidCrumb.normal.background;
                midCrumb.hover.background    = nativeMidCrumb.hover.background;
                midCrumb.active.background   = nativeMidCrumb.active.background;
                midCrumb.onNormal.background = nativeMidCrumb.onNormal.background;
                midCrumb.onHover.background  = nativeMidCrumb.onHover.background;
                midCrumb.onActive.background = nativeMidCrumb.onActive.background;

                midCrumb.fixedHeight = 19;

                #if UNITY_2019_1_OR_NEWER
                // in Unity 2019+, the styles have changed,
                // so ensure we modify it so it fits our GUI
                midCrumb.overflow.top    = 1;
                midCrumb.overflow.bottom = 1;
                #else
				midCrumb.overflow.top = 0;
				midCrumb.overflow.bottom = 0;
                #endif
            }

            #endregion

            // ----------------------------------------------------

            if (leftCrumbToAssign != null || midCrumbToAssign != null || reorderableListDragHandleToAssign != null || reorderableListHeaderToAssign != null || reorderableListFooterToAssign != null || reorderableListBgToAssign != null || reorderableListFooterButtonToAssign != null || reorderableListElementToAssign != null || reorderableListEmptyHeaderToAssign != null || addLogMessageIcons)
            {
                // append these styles to the GUISkin
                // but since it's an array, we have to create a new array and place it there first
                var newStyles = new List<GUIStyle>(this._usedSkin.customStyles);
                if (leftCrumbToAssign != null) newStyles.Add(leftCrumbToAssign);
                if (midCrumbToAssign != null) newStyles.Add(midCrumbToAssign);
                if (reorderableListDragHandleToAssign != null) newStyles.Add(reorderableListDragHandleToAssign);
                if (reorderableListHeaderToAssign != null) newStyles.Add(reorderableListHeaderToAssign);
                if (reorderableListFooterToAssign != null) newStyles.Add(reorderableListFooterToAssign);
                if (reorderableListBgToAssign != null) newStyles.Add(reorderableListBgToAssign);
                if (reorderableListFooterButtonToAssign != null) newStyles.Add(reorderableListFooterButtonToAssign);
                if (reorderableListElementToAssign != null) newStyles.Add(reorderableListElementToAssign);
                if (reorderableListEmptyHeaderToAssign != null) newStyles.Add(reorderableListEmptyHeaderToAssign);
                if (addLogMessageIcons) newStyles.Add(logMessageIcons);
                this._usedSkin.customStyles = newStyles.ToArray();
            }

            // ----------------------------------------------------

            this._toolbarIconLog     = this._usedSkin.GetStyle("Icon-Toolbar-Log").normal.background;
            this._toolbarIconOpen    = this._usedSkin.GetStyle("Icon-Toolbar-Open").normal.background;
            this._toolbarIconSave    = this._usedSkin.GetStyle("Icon-Toolbar-Save").normal.background;
            this._toolbarIconOptions = this._usedSkin.GetStyle("Icon-Toolbar-Options").normal.background;
            this._toolbarIconHelp    = this._usedSkin.GetStyle("Icon-Toolbar-Help").normal.background;

            this._toolbarLabelLog     = new(Labels.REFRESH_LABEL, this._toolbarIconLog);
            this._toolbarLabelOpen    = new(Labels.OPEN_LABEL, this._toolbarIconOpen);
            this._toolbarLabelSave    = new(Labels.SAVE_LABEL, this._toolbarIconSave);
            this._toolbarLabelOptions = new(Labels.OPTIONS_CATEGORY_LABEL, this._toolbarIconOptions);
            this._toolbarLabelHelp    = new(Labels.HELP_CATEGORY_LABEL, this._toolbarIconHelp);
        }
        else
        {
            this._toolbarLabelLog     = new(Labels.REFRESH_LABEL);
            this._toolbarLabelOpen    = new(Labels.OPEN_LABEL);
            this._toolbarLabelSave    = new(Labels.SAVE_LABEL);
            this._toolbarLabelOptions = new(Labels.OPTIONS_CATEGORY_LABEL);
            this._toolbarLabelHelp    = new(Labels.HELP_CATEGORY_LABEL);
        }
    }

    public void Init(BuildInfo buildInfo)
    {
        _buildInfo = buildInfo;

        this.minSize = new(903, 440);
    }

    /// <summary>
    /// Creates a Build Report and shows it on-screen.
    /// </summary>
    /// Called either when the "Get Log" button is pressed in this EditorWindow
    /// (called in <see cref="DrawTopRowButtons"/>, which is called in <see cref="OnGUI"/>),
    /// or in <see cref="Update"/>, when it has detected that a build has completed and
    /// a Build Report creation was scheduled.
    private void Refresh(bool fromBuild)
    {
        this.GoToOverviewScreen();
        ReportGenerator.RefreshData(fromBuild, ref _buildInfo, ref _assetDependencies, ref _textureData, ref _meshData);
    }

    private bool IsWaitingForBuildCompletionToGenerateBuildReport => Util.ShouldGetBuildReportNow && EditorApplication.isCompiling;

    private void OnFinishOpeningBuildReportFile()
    {
        this._finishedOpeningFromThread = false;

        if (Util.BuildInfoHasContents(_buildInfo))
        {
            this._buildSettingsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
            this._buildStepsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
            this._usedAssetsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
            this._unusedAssetsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
            this._sizeStatsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
            this._extraDataScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);

            _buildInfo.OnAfterLoad();
            _buildInfo.SetSavedPath(this._lastOpenedBuildInfoFilePath);
        }

        this.Repaint();
        this.GoToOverviewScreen();
    }

    private void OnFinishGeneratingBuildReport()
    {
        ReportGenerator.OnFinishedGetValues(_buildInfo, _assetDependencies, _textureData, _meshData);
        _buildInfo.UnescapeAssetNames();

        this.GoToOverviewScreen();

        _unityBuildReport = ReportGenerator.LastKnownUnityBuildReport;
        if (_unityBuildReport != null) Debug.Log(string.Format("UnityBuildReport displayed is now: {0}", _unityBuildReport.SavedPath));

        this._buildSettingsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        this._buildStepsScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
    }

    private void GoToOverviewScreen()
    {
        this._selectedCategoryIdx = OVERVIEW_IDX;
    }

    // ==========================================================================

    // ==========================================================================

    private int _fileFilterGroupToUseOnOpeningOptionsWindow = 0;
    private int _fileFilterGroupToUseOnClosingOptionsWindow = 0;

    private int _selectedCategoryIdx = 0;

    private bool IsInOverviewCategory => this._selectedCategoryIdx == OVERVIEW_IDX;

    private bool IsInBuildSettingsCategory => this._selectedCategoryIdx == BUILD_SETTINGS_IDX;

    private bool IsInBuildStepsCategory => this._selectedCategoryIdx == BUILD_STEPS_IDX;

    private bool IsInWarningsErrorsCategory => this._selectedCategoryIdx == WARNING_ERRORS_IDX;

    private bool IsInSizeStatsCategory => this._selectedCategoryIdx == SIZE_STATS_IDX;

    private bool IsInUsedAssetsCategory => this._selectedCategoryIdx == USED_ASSETS_IDX;

    private bool IsInUnusedAssetsCategory => this._selectedCategoryIdx == UNUSED_ASSETS_IDX;

    private bool IsInExtraDataCategory => this._selectedCategoryIdx == EXTRA_DATA_IDX;

    private bool IsInOptionsCategory => this._selectedCategoryIdx == OPTIONS_IDX;

    private bool IsInHelpCategory => this._selectedCategoryIdx == HELP_IDX;

    private const int OVERVIEW_IDX       = 0;
    private const int BUILD_SETTINGS_IDX = 1;
    private const int BUILD_STEPS_IDX    = 2;
    private const int WARNING_ERRORS_IDX = 3;
    private const int SIZE_STATS_IDX     = 4;
    private const int USED_ASSETS_IDX    = 5;
    private const int UNUSED_ASSETS_IDX  = 6;
    private const int EXTRA_DATA_IDX     = 7;

    private const int OPTIONS_IDX = 8;
    private const int HELP_IDX    = 9;

    private bool   _finishedOpeningFromThread   = false;
    private string _lastOpenedBuildInfoFilePath = "";

    private void _OpenBuildInfo(string filepath)
    {
        if (string.IsNullOrEmpty(filepath)) return;

        this._finishedOpeningFromThread = false;
        GetValueMessage                 = "Opening...";
        var loadedBuild = Util.OpenSerializedBuildInfo(filepath, false);

        if (Util.BuildInfoHasContents(loadedBuild))
        {
            _buildInfo                        = loadedBuild;
            this._lastOpenedBuildInfoFilePath = filepath;
        }
        else
        {
            Debug.LogError(string.Format("Build Report Tool: Invalid data in build info file: {0}", filepath));
        }

        var assetDependenciesFilePath = Util.GetAssetDependenciesFilenameFromBuildInfo(filepath);
        if (System.IO.File.Exists(assetDependenciesFilePath))
        {
            var loadedAssetDependencies                             = Util.OpenSerialized<AssetDependencies>(assetDependenciesFilePath);
            if (loadedAssetDependencies != null) _assetDependencies = loadedAssetDependencies;
        }
        else
        {
            _assetDependencies = null;
        }

        var textureDataFilePath = Util.GetTextureDataFilenameFromBuildInfo(filepath);
        if (System.IO.File.Exists(textureDataFilePath))
        {
            var loadedTextureData                       = Util.OpenSerialized<TextureData>(textureDataFilePath);
            if (loadedTextureData != null) _textureData = loadedTextureData;
        }
        else
        {
            _textureData = null;
        }

        var meshDataFilePath = Util.GetMeshDataFilenameFromBuildInfo(filepath);
        if (System.IO.File.Exists(meshDataFilePath))
        {
            var loadedMeshData                    = Util.OpenSerialized<MeshData>(meshDataFilePath);
            if (loadedMeshData != null) _meshData = loadedMeshData;
        }
        else
        {
            _meshData = null;
        }

        var unityBuildReportFilePath = Util.GetUnityBuildReportFilenameFromBuildInfo(filepath);
        if (System.IO.File.Exists(unityBuildReportFilePath))
            try
            {
                var loadedUnityBuildReport =
                    Util.OpenSerialized<UnityBuildReport>(unityBuildReportFilePath);
                if (loadedUnityBuildReport != null)
                    _unityBuildReport = loadedUnityBuildReport;
                //Debug.Log(string.Format("UnityBuildReport displayed is now: {0}", _unityBuildReport.SavedPath));
                else
                    _unityBuildReport = null;
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("Can't open additional build info data due to Unity version incompatibility.\n\n{0}", e));
                _unityBuildReport = null;
            }
        else
            //Debug.LogWarning(string.Format("Not found: {0}", unityBuildReportFilePath));
            _unityBuildReport = null;

        var extraDataFilePath = Util.GetExtraDataFilename(filepath).Replace('\\', '/');
        if (System.IO.File.Exists(extraDataFilePath))
        {
            _extraData.Contents  = System.IO.File.ReadAllText(extraDataFilePath);
            _extraData.SavedPath = extraDataFilePath;
        }
        else
        {
            _extraData.Contents  = null;
            _extraData.SavedPath = null;
        }

        this._finishedOpeningFromThread = true;

        GetValueMessage = "";
    }

    private Thread _currentBuildReportFileLoadThread = null;

    private bool IsCurrentlyOpeningAFile => this._currentBuildReportFileLoadThread != null && this._currentBuildReportFileLoadThread.ThreadState == ThreadState.Running;

    private void ForceStopFileLoadThread()
    {
        if (this.IsCurrentlyOpeningAFile)
            try
            {
                //Debug.LogFormat(this, "Build Report Tool: Stopping file load background thread...");
                this._currentBuildReportFileLoadThread.Abort();
                Debug.LogFormat(this, "Build Report Tool: File load background thread stopped.");
            }
            catch (ThreadStateException)
            {
            }
    }

    private void OpenBuildInfoInBackgroundIfNeeded(string filepath)
    {
        if (string.IsNullOrEmpty(filepath)) return;

        // user selected the asset dependencies file for the build report
        // derive the build report file from it
        if (filepath.DoesFileBeginWith("DEP-"))
        {
            var path     = System.IO.Path.GetDirectoryName(filepath);
            var filename = filepath.GetFileNameOnly();
            filepath = string.Format("{0}/{1}", path, filename.Substring(4)); // filename without the "DEP-" at the start
        }
        else if (filepath.DoesFileBeginWith("TextureData-"))
        {
            var path     = System.IO.Path.GetDirectoryName(filepath);
            var filename = filepath.GetFileNameOnly();
            filepath = string.Format("{0}/{1}", path, filename.Substring(12)); // filename without the "TextureData-" at the start
        }
        else if (filepath.DoesFileBeginWith("UBR-"))
        {
            var path     = System.IO.Path.GetDirectoryName(filepath);
            var filename = filepath.GetFileNameOnly();
            filepath = string.Format("{0}/{1}", path, filename.Substring(4)); // filename without the "UBR-" at the start
        }
        else if (filepath.DoesFileBeginWith("ExtraData-"))
        {
            var path     = System.IO.Path.GetDirectoryName(filepath);
            var filename = filepath.GetFileNameOnly();
            filepath = string.Format("{0}/{1}", path, filename.Substring(10)); // filename without the "ExtraData-" at the start
        }

        if (!Options.UseThreadedFileLoading)
        {
            this._OpenBuildInfo(filepath);
        }
        else
        {
            if (this._currentBuildReportFileLoadThread != null && this._currentBuildReportFileLoadThread.ThreadState == ThreadState.Running) this.ForceStopFileLoadThread();

            this._currentBuildReportFileLoadThread = new(() => this.LoadThread(filepath));
            this._currentBuildReportFileLoadThread.Start();
            Debug.LogFormat(this, "Build Report Tool: Started new load background thread...");
        }
    }

    private void LoadThread(string filepath)
    {
        this._OpenBuildInfo(filepath);
        Debug.LogFormat(this, "Build Report Tool: Load background thread finished.");
    }

    private void DrawCentralMessage(string msg)
    {
        float w = 300;
        float h = 100;
        var   x = (this.position.width - w) * 0.5f;
        var   y = (this.position.height - h) * 0.25f;

        GUI.Label(new(x, y, w, h), msg);
    }

    private void DrawWarningMessage(string msg)
    {
        float w = 400;
        float h = 100;
        var   x = (this.position.width - w) * 0.5f;
        var   y = (this.position.height - h) * 0.25f + 100 + 40;

        var msgRect = new Rect(x, y, w, h);
        GUI.Label(msgRect, msg);

        var warning = GUI.skin.FindStyle("Icon-Warning");
        if (warning != null)
        {
            var warningIcon = warning.normal.background;

            var iconWidth  = warning.fixedWidth;
            var iconHeight = warning.fixedHeight;

            GUI.DrawTexture(new(msgRect.x - iconWidth, msgRect.y, iconWidth, iconHeight), warningIcon);
        }
    }

    private void DrawTopRowButtons()
    {
        var toolbarX = 10;

        var leftToolbarStyle                           = GUI.skin.FindStyle(Settings.TOOLBAR_LEFT_STYLE_NAME);
        if (leftToolbarStyle == null) leftToolbarStyle = GUI.skin.button;

        var midToolbarStyle                          = GUI.skin.FindStyle(Settings.TOOLBAR_MIDDLE_STYLE_NAME);
        if (midToolbarStyle == null) midToolbarStyle = GUI.skin.button;

        var rightToolbarStyle                            = GUI.skin.FindStyle(Settings.TOOLBAR_RIGHT_STYLE_NAME);
        if (rightToolbarStyle == null) rightToolbarStyle = GUI.skin.button;

        if (GUI.Button(new(toolbarX, 5, 50, 40), this._toolbarLabelLog, leftToolbarStyle) && !LoadingValuesFromThread) this.Refresh(false);

        toolbarX += 50;
        if (GUI.Button(new(toolbarX, 5, 40, 40), this._toolbarLabelOpen, midToolbarStyle) && !LoadingValuesFromThread)
        {
            var filepath = EditorUtility.OpenFilePanel(
                Labels.OPEN_SERIALIZED_BUILD_INFO_TITLE,
                Options.BuildReportSavePath,
                "xml");

            this.OpenBuildInfoInBackgroundIfNeeded(filepath);
        }

        toolbarX += 40;

        if (GUI.Button(new(toolbarX, 5, 40, 40), this._toolbarLabelSave, rightToolbarStyle) && Util.BuildInfoHasContents(_buildInfo))
        {
            var filepath = EditorUtility.SaveFilePanel(
                Labels.SAVE_MSG,
                Options.BuildReportSavePath,
                _buildInfo.GetDefaultFilename(),
                "xml");

            if (!string.IsNullOrEmpty(filepath))
            {
                Util.Serialize(_buildInfo, filepath);

                if (_assetDependencies != null && _assetDependencies.HasContents)
                {
                    var assetDependenciesFilePath = Util.GetAssetDependenciesFilenameFromBuildInfo(filepath);
                    Util.Serialize(_assetDependencies, assetDependenciesFilePath);
                }

                if (_textureData != null && _textureData.HasContents)
                {
                    var textureDataFilePath = Util.GetTextureDataFilenameFromBuildInfo(filepath);
                    Util.Serialize(_textureData, textureDataFilePath);
                }

                if (_meshData != null && _meshData.HasContents)
                {
                    var meshDataFilePath = Util.GetMeshDataFilenameFromBuildInfo(filepath);
                    Util.Serialize(_meshData, meshDataFilePath);
                }

                if (_unityBuildReport != null)
                {
                    var unityBuildReportFilePath = Util.GetUnityBuildReportFilenameFromBuildInfo(filepath);
                    Util.Serialize(_unityBuildReport, unityBuildReportFilePath);
                }
            }
        }

        toolbarX += 40;

        toolbarX += 20;

        if (GUI.Button(new(toolbarX, 5, 55, 40), this._toolbarLabelOptions, leftToolbarStyle))
        {
            this._selectedCategoryIdx = OPTIONS_IDX;
            Options.UpdatePreviousSearchType();
        }

        toolbarX += 55;
        if (GUI.Button(new(toolbarX, 5, 70, 40), this._toolbarLabelHelp, rightToolbarStyle))
        {
            this._selectedCategoryIdx = HELP_IDX;
            this._helpScreen.RefreshData(_buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport);
        }
    }

    private bool _buildInfoHasNoContentsToDisplay = false;

    private void OnGUI()
    {
        if (Event.current.type == EventType.Layout)
        {
            _noGuiSkinFound                       = this._usedSkin == null;
            _loadingValuesFromThread              = !string.IsNullOrEmpty(GetValueMessage);
            this._buildInfoHasNoContentsToDisplay = !Util.BuildInfoHasContents(_buildInfo);
        }

        //GUI.Label(new Rect(5, 100, 800, 20), "BuildReportTool.Util.ShouldReload: " + BuildReportTool.Util.ShouldReload + " EditorApplication.isCompiling: " + EditorApplication.isCompiling);
        if (!_noGuiSkinFound)
            GUI.skin = this._usedSkin;
        //GUI.Label(new Rect(20, 20, 500, 100), BuildReportTool.Options.BUILD_REPORT_PACKAGE_MISSING_MSG);
        //return;
        else
            GUI.Label(new(300, -25, 500, 100), Options.BUILD_REPORT_GUI_SKIN_MISSING_MSG);

        this.DrawTopRowButtons();

        if (GUI.skin.FindStyle(Settings.VERSION_STYLE_NAME) != null)
            GUI.Label(new(0, 0, this.position.width, 20),
                Info.ReadableVersion,
                Settings.VERSION_STYLE_NAME);
        else
            GUI.Label(new(this.position.width - 160, 0, this.position.width, 20), Info.ReadableVersion);

        // loading message
        if (LoadingValuesFromThread)
        {
            this.DrawCentralMessage(GetValueMessage);
            return;
        }

        var requestRepaint = false;

        // content to show when there is no build report on display
        if (this._buildInfoHasNoContentsToDisplay)
        {
            if (this.IsInOptionsCategory)
            {
                GUILayout.Space(40);
                this._optionsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaint);
            }
            else if (this.IsInHelpCategory)
            {
                GUILayout.Space(40);
                this._helpScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaint);
            }
            else if (this.IsWaitingForBuildCompletionToGenerateBuildReport)
            {
                this.DrawCentralMessage(Labels.WAITING_FOR_BUILD_TO_COMPLETE_MSG);
            }
            else
            {
                this.DrawCentralMessage(Labels.NO_BUILD_INFO_FOUND_MSG);

                if (ReportGenerator.CheckIfUnityHasNoLogArgument()) this.DrawWarningMessage(Labels.FOUND_NO_LOG_ARGUMENT_MSG);
            }

            if (requestRepaint) this.Repaint();

            return;
        }

        GUILayout.Space(50); // top padding (top row buttons are 40 pixels)

        var mouseHasMoved = Mathf.Abs(Event.current.mousePosition.x - _lastMousePos.x) > 0 || Mathf.Abs(Event.current.mousePosition.y - _lastMousePos.y) > 0;

        // category buttons

        var oldSelectedCategoryIdx = this._selectedCategoryIdx;

        var leftTabStyle                       = GUI.skin.FindStyle(Settings.TAB_LEFT_STYLE_NAME);
        if (leftTabStyle == null) leftTabStyle = GUI.skin.button;

        var midTabStyle                      = GUI.skin.FindStyle(Settings.TAB_MIDDLE_STYLE_NAME);
        if (midTabStyle == null) midTabStyle = GUI.skin.button;

        var rightTabStyle                        = GUI.skin.FindStyle(Settings.TAB_RIGHT_STYLE_NAME);
        if (rightTabStyle == null) rightTabStyle = GUI.skin.button;

        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(this.IsInOverviewCategory, "Overview", leftTabStyle, LayoutExpandWidth)) this._selectedCategoryIdx = OVERVIEW_IDX;

        if (GUILayout.Toggle(this.IsInBuildSettingsCategory, "Project Settings", midTabStyle, LayoutExpandWidth)) this._selectedCategoryIdx = BUILD_SETTINGS_IDX;

        if (_unityBuildReport != null && GUILayout.Toggle(this.IsInBuildStepsCategory, "Build Process", midTabStyle, LayoutExpandWidth)) this._selectedCategoryIdx = BUILD_STEPS_IDX;

        if (GUILayout.Toggle(this.IsInSizeStatsCategory, "Size Stats", midTabStyle, LayoutExpandWidth)) this._selectedCategoryIdx = SIZE_STATS_IDX;

        if (!string.IsNullOrEmpty(_extraData.Contents) && GUILayout.Toggle(this.IsInExtraDataCategory, "Extra Data", midTabStyle, LayoutExpandWidth)) this._selectedCategoryIdx = EXTRA_DATA_IDX;

        if (GUILayout.Toggle(this.IsInUsedAssetsCategory, "Used Assets", midTabStyle, LayoutExpandWidth))
        {
            if (this._selectedCategoryIdx != USED_ASSETS_IDX && Options.HasSearchTypeChanged) this._usedAssetsScreen.UpdateSearchNow(_buildInfo);

            this._selectedCategoryIdx = USED_ASSETS_IDX;
        }

        if (GUILayout.Toggle(this.IsInUnusedAssetsCategory, "Unused Assets", rightTabStyle, LayoutExpandWidth))
        {
            if (this._selectedCategoryIdx != UNUSED_ASSETS_IDX && Options.HasSearchTypeChanged) this._unusedAssetsScreen.UpdateSearchNow(_buildInfo);

            this._selectedCategoryIdx = UNUSED_ASSETS_IDX;
        }

        /*GUILayout.Space(20);

        if (GUILayout.Toggle(IsInOptionsCategory, _toolbarLabelOptions, leftTabStyle, LayoutExpandWidth))
        {
            _selectedCategoryIdx = OPTIONS_IDX;
        }
        if (GUILayout.Toggle(IsInHelpCategory, _toolbarLabelHelp, rightTabStyle, LayoutExpandWidth))
        {
            _selectedCategoryIdx = HELP_IDX;
        }*/
        GUILayout.EndHorizontal();

        if (oldSelectedCategoryIdx != OPTIONS_IDX && this._selectedCategoryIdx == OPTIONS_IDX)
            // moving into the options screen
        {
            this._fileFilterGroupToUseOnOpeningOptionsWindow = Options.FilterToUseInt;
        }
        else if (oldSelectedCategoryIdx == OPTIONS_IDX && this._selectedCategoryIdx != OPTIONS_IDX)
        {
            // moving away from the options screen
            this._fileFilterGroupToUseOnClosingOptionsWindow = Options.FilterToUseInt;

            if (this._fileFilterGroupToUseOnOpeningOptionsWindow != this._fileFilterGroupToUseOnClosingOptionsWindow) this.RecategorizeDisplayedBuildInfo();
        }

        var requestRepaintOnTabs = false;

        // main content
        GUILayout.BeginHorizontal();
        //GUILayout.Space(3); // left padding
        GUILayout.BeginVertical();

        if (this.IsInOverviewCategory)
            this._overviewScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInBuildSettingsCategory)
            this._buildSettingsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInBuildStepsCategory)
            this._buildStepsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInSizeStatsCategory)
            this._sizeStatsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInUsedAssetsCategory)
            this._usedAssetsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInUnusedAssetsCategory)
            this._unusedAssetsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInExtraDataCategory)
            this._extraDataScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInOptionsCategory)
            this._optionsScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);
        else if (this.IsInHelpCategory) this._helpScreen.DrawGUI(this.position, _buildInfo, _assetDependencies, _textureData, _meshData, _unityBuildReport, _extraData, out requestRepaintOnTabs);

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        //GUILayout.Space(5); // right padding
        GUILayout.EndHorizontal();

        //GUILayout.Space(10); // bottom padding

        if (requestRepaintOnTabs) _buildInfo.FlagOkToRefresh();

        _lastMousePos   = Event.current.mousePosition;
        _lastMouseMoved = mouseHasMoved;
    }

    public static bool LastMouseMoved => _lastMouseMoved;

    public static bool MouseMovedNow =>
        Mathf.Abs(Event.current.mousePosition.x - _lastMousePos.x) > 0 || Mathf.Abs(Event.current.mousePosition.y - _lastMousePos.y) > 0;

    // =====================================================================================

    public static Texture GetAssetPreview(SizePart sizePart)
    {
        if (sizePart == null) return null;

        return GetAssetPreview(sizePart.Name);
    }

    public static Texture GetAssetPreview(string assetName)
    {
        if (string.IsNullOrEmpty(assetName)) return null;

        Texture thumbnailImage = null;
        if (assetName.IsTextureFile())
        {
            #if UNITY_5_6_OR_NEWER
            thumbnailImage = AssetDatabase.LoadAssetAtPath<Texture>(assetName);
            #else
			thumbnailImage = (Texture)AssetDatabase.LoadAssetAtPath(assetName, typeof(Texture));
            #endif
        }
        else //if (_assetListEntryHovered.Name.EndsWith(".prefab") || BuildReportTool.Util.IsFileAUnityMesh(_assetListEntryHovered.Name))
        {
            #if UNITY_5_6_OR_NEWER
            var loadedObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
            #else
			var loadedObj = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityEngine.Object));
            #endif

            if (loadedObj != null) thumbnailImage = AssetPreview.GetAssetPreview(loadedObj);
            //thumbnailImage = AssetPreview.GetMiniThumbnail(loadedObj);
        }

        return thumbnailImage;
    }

    public static Vector2 GetThumbnailSize()
    {
        Vector2 thumbnailSize;
        thumbnailSize.x = ZoomedInThumbnails
            ? Options.TooltipThumbnailZoomedInWidth
            : Options.TooltipThumbnailWidth;

        thumbnailSize.y = ZoomedInThumbnails
            ? Options.TooltipThumbnailZoomedInHeight
            : Options.TooltipThumbnailHeight;
        return thumbnailSize;
    }

    public static Rect DrawTooltip(Rect position, float desiredWidth, float desiredHeight, float additionalPadding = 0)
    {
        var tooltipStyle                       = GUI.skin.FindStyle("Tooltip");
        if (tooltipStyle == null) tooltipStyle = GUI.skin.box;
        var tooltipRect                        = new Rect();

        // --------------------------------------------------

        var tooltipPos = Event.current.mousePosition;

        // offset for mouse
        tooltipPos.x += 18;
        tooltipPos.y += 15;

        // --------------------------------------------------
        // initially tooltip is to the right and below the mouse

        tooltipRect.width  = desiredWidth;
        tooltipRect.height = desiredHeight;

        tooltipRect.width  += tooltipStyle.border.horizontal + additionalPadding * 2;
        tooltipRect.height += tooltipStyle.border.vertical + additionalPadding * 2;

        tooltipRect.x = tooltipPos.x - tooltipStyle.border.left;
        tooltipRect.y = tooltipPos.y - tooltipStyle.border.top;

        // --------------------------------------------------

        if (tooltipRect.xMax > position.width)
        {
            // move tooltip to the left
            tooltipPos.x  = Event.current.mousePosition.x - 5 - tooltipRect.width;
            tooltipRect.x = tooltipPos.x - tooltipStyle.border.left;

            if (tooltipRect.x < 0)
            {
                tooltipPos.x  = position.width - tooltipRect.width;
                tooltipRect.x = tooltipPos.x - tooltipStyle.border.left;
            }
        }

        // --------------------------------------------------

        if (tooltipRect.yMax > position.height)
        {
            // move tooltip above mouse
            tooltipPos.y  = Event.current.mousePosition.y + 3 - tooltipRect.height;
            tooltipRect.y = tooltipPos.y - tooltipStyle.border.top;

            if (tooltipRect.y < 0)
            {
                tooltipPos.y  = position.height - tooltipRect.height;
                tooltipRect.y = tooltipPos.y - tooltipStyle.border.top;
            }
        }

        // --------------------------------------------------

        GUI.Box(tooltipRect, string.Empty, tooltipStyle);

        return new(tooltipPos.x + additionalPadding, tooltipPos.y + additionalPadding, desiredWidth, desiredHeight);
    }

    public static void ProcessThumbnailControls()
    {
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.LeftAlt || Event.current.keyCode == KeyCode.RightAlt)) ShowThumbnailsWithAlphaBlend = !ShowThumbnailsWithAlphaBlend;

        if (Event.current.keyCode == KeyCode.LeftControl || Event.current.keyCode == KeyCode.RightControl)
        {
            if (Event.current.type == EventType.KeyDown)
                ZoomedInThumbnails                                             = true;
            else if (Event.current.type == EventType.KeyUp) ZoomedInThumbnails = false;
        }
    }

    public static void ResetThumbnailControls()
    {
        // ensure that thumbnails are not zoomed in
        ZoomedInThumbnails = false;
    }

    public static void DrawThumbnail(float posX, float posY, Vector2 thumbnailSize, Texture thumbnailImage)
    {
        var thumbnailRect = new Rect(posX, posY, thumbnailSize.x, thumbnailSize.y);
        GUI.DrawTexture(thumbnailRect, thumbnailImage, ScaleMode.ScaleToFit, ShowThumbnailsWithAlphaBlend);
    }

    public static Vector2 GetEndUsersListSize(GUIContent label, List<GUIContent> endUsers)
    {
        var assetStyle                     = GUI.skin.FindStyle("Asset");
        if (assetStyle == null) assetStyle = GUI.skin.label;
        var labelStyle                     = GUI.skin.FindStyle("TooltipText");
        if (labelStyle == null) labelStyle = GUI.skin.box;

        var endUsersSize = Vector2.zero;

        var labelSize = labelStyle.CalcSize(label);
        endUsersSize.x =  Mathf.Max(endUsersSize.x, labelSize.x);
        endUsersSize.y += labelSize.y;

        if (endUsers != null)
        {
            EditorGUIUtility.SetIconSize(IconSize);

            for (int n = 0, len = endUsers.Count; n < len; ++n)
            {
                var endUserSize = assetStyle.CalcSize(endUsers[n]);

                endUsersSize.x =  Mathf.Max(endUsersSize.x, endUserSize.x);
                endUsersSize.y += endUserSize.y;
            }
        }

        return endUsersSize;
    }

    public static void DrawEndUsersList(Vector2 pos, GUIContent label, List<GUIContent> endUsers)
    {
        var assetStyle                     = GUI.skin.FindStyle("Asset");
        if (assetStyle == null) assetStyle = GUI.skin.label;
        var labelStyle                     = GUI.skin.FindStyle("TooltipText");
        if (labelStyle == null) labelStyle = GUI.skin.box;

        var endUserRect = new Rect(pos.x, pos.y, 0, 0);

        endUserRect.size = labelStyle.CalcSize(label);
        GUI.Label(endUserRect, label, labelStyle);

        if (endUsers != null && endUsers.Count > 0)
        {
            endUserRect.y += endUserRect.height;

            EditorGUIUtility.SetIconSize(IconSize);

            for (int n = 0, len = endUsers.Count; n < len; ++n)
            {
                endUserRect.size = assetStyle.CalcSize(endUsers[n]);

                GUI.Label(endUserRect, endUsers[n], assetStyle);

                endUserRect.y += endUserRect.height;
            }
        }
    }

    // -----------------------------------------------

    public static void DrawThumbnailTooltip(Rect position, TextureData textureData)
    {
        DrawThumbnailTooltip(position, HoveredAssetEntryPath, HoveredAssetEntryRect, textureData);
    }

    private static readonly GUIContent TextureDataTooltipLabel = new();

    private static bool GetTextureDataForTooltip(string assetPath, TextureData textureData, out Vector2 labelSize)
    {
        if (textureData == null)
        {
            labelSize = Vector2.zero;
            return false;
        }

        var data = textureData.GetTextureData();
        if (data.ContainsKey(assetPath))
        {
            if (data[assetPath].IsImportedWidthAndHeightDifferentFromReal)
            {
                if (ZoomedInThumbnails)
                    TextureDataTooltipLabel.text = string.Format("{0} ({1}) {2} (source: {3})",
                        data[assetPath].TextureType,
                        data[assetPath].GetShownTextureFormat(),
                        data[assetPath].ToDisplayedValue(TextureData.DataId.ImportedWidthAndHeight),
                        data[assetPath].ToDisplayedValue(TextureData.DataId.RealWidthAndHeight));
                else
                    TextureDataTooltipLabel.text = string.Format("{0} ({1})\n{2} (source: {3})",
                        data[assetPath].TextureType,
                        data[assetPath].GetShownTextureFormat(),
                        data[assetPath].ToDisplayedValue(TextureData.DataId.ImportedWidthAndHeight),
                        data[assetPath].ToDisplayedValue(TextureData.DataId.RealWidthAndHeight));
            }
            else
            {
                TextureDataTooltipLabel.text = string.Format("{0} ({1}) {2}",
                    data[assetPath].TextureType,
                    data[assetPath].GetShownTextureFormat(),
                    data[assetPath].ToDisplayedValue(TextureData.DataId.ImportedWidthAndHeight));
            }

            var labelStyle                     = GUI.skin.FindStyle("TooltipText");
            if (labelStyle == null) labelStyle = GUI.skin.box;
            labelSize = labelStyle.CalcSize(TextureDataTooltipLabel);

            return true;
        }
        else
        {
            labelSize = Vector2.zero;
            return false;
        }
    }

    public static void DrawThumbnailTooltip(Rect position, string assetPath, Rect assetRect, TextureData textureData)
    {
        var thumbnailImage = GetAssetPreview(assetPath);

        if (thumbnailImage != null)
        {
            var desiredSize   = Vector2.zero;
            var thumbnailSize = GetThumbnailSize();
            desiredSize.x = thumbnailSize.x;
            desiredSize.y = thumbnailSize.y;

            Vector2 textureDataLabelSize;
            var     showTextureData = GetTextureDataForTooltip(assetPath, textureData, out textureDataLabelSize);
            if (showTextureData)
            {
                desiredSize.x =  Mathf.Max(desiredSize.x, textureDataLabelSize.x);
                desiredSize.y += textureDataLabelSize.y;
            }

            var tooltipRect = DrawTooltip(position, desiredSize.x, desiredSize.y);

            DrawThumbnail(tooltipRect.x, tooltipRect.y, thumbnailSize, thumbnailImage);

            if (showTextureData)
            {
                var labelStyle                     = GUI.skin.FindStyle("TooltipText");
                if (labelStyle == null) labelStyle = GUI.skin.box;

                GUI.Label(new(
                        tooltipRect.x,
                        tooltipRect.y + thumbnailSize.y,
                        textureDataLabelSize.x,
                        textureDataLabelSize.y),
                    TextureDataTooltipLabel,
                    labelStyle);
            }
        }
    }

    public static void DrawEndUsersTooltip(Rect position, AssetDependencies assetDependencies)
    {
        var endUsersListToUse = GetEndUserLabelsFor(assetDependencies, HoveredAssetEntryPath);
        DrawEndUsersTooltip(position,
            GetAppropriateEndUserLabelForHovered(),
            endUsersListToUse,
            HoveredAssetEntryRect);
    }

    public static void DrawEndUsersTooltip(Rect position, GUIContent label, List<GUIContent> endUsers, Rect assetRect)
    {
        var endUsersSize = GetEndUsersListSize(label, endUsers);

        var tooltipRect = DrawTooltip(position, endUsersSize.x, endUsersSize.y);

        DrawEndUsersList(tooltipRect.position, label, endUsers);
    }

    public static void DrawThumbnailEndUsersTooltip(Rect position, AssetDependencies assetDependencies, TextureData textureData)
    {
        var endUsersListToUse = GetEndUserLabelsFor(assetDependencies, HoveredAssetEntryPath);
        DrawThumbnailEndUsersTooltip(position,
            HoveredAssetEntryPath,
            GetAppropriateEndUserLabelForHovered(),
            endUsersListToUse,
            HoveredAssetEntryRect,
            textureData);
    }

    public static void DrawThumbnailEndUsersTooltip(
        Rect             position,
        string           assetPath,
        GUIContent       label,
        List<GUIContent> endUsers,
        Rect             assetRect,
        TextureData      textureData
    )
    {
        var thumbnailImage = GetAssetPreview(assetPath);

        if (thumbnailImage != null)
        {
            var usedBySpacing = 5;

            var thumbnailSize = GetThumbnailSize();

            // compute end users height and width
            // then create a tooltip size that encompasses both thumbnail and end users list

            var endUsersSize = GetEndUsersListSize(label, endUsers);
            endUsersSize.y += usedBySpacing;

            var tooltipSize = new Vector2(Mathf.Max(thumbnailSize.x, endUsersSize.x),
                thumbnailSize.y + endUsersSize.y);

            Vector2 textureDataLabelSize;
            var     showTextureData = GetTextureDataForTooltip(assetPath, textureData, out textureDataLabelSize);
            if (showTextureData)
            {
                tooltipSize.x =  Mathf.Max(tooltipSize.x, textureDataLabelSize.x);
                tooltipSize.y += textureDataLabelSize.y;
            }

            var tooltipRect = DrawTooltip(position, tooltipSize.x, tooltipSize.y);

            // --------
            // now draw the contents

            DrawThumbnail(tooltipRect.x, tooltipRect.y, thumbnailSize, thumbnailImage);

            if (showTextureData)
            {
                var labelStyle                     = GUI.skin.FindStyle("TooltipText");
                if (labelStyle == null) labelStyle = GUI.skin.box;
                GUI.Label(new(
                        tooltipRect.x,
                        tooltipRect.y + thumbnailSize.y,
                        textureDataLabelSize.x,
                        textureDataLabelSize.y),
                    TextureDataTooltipLabel,
                    labelStyle);
            }

            var endUsersPos = tooltipRect.position;
            endUsersPos.y += thumbnailSize.y + textureDataLabelSize.y + usedBySpacing;
            DrawEndUsersList(endUsersPos, label, endUsers);
        }
    }

    // =====================================================================================
}

#endif