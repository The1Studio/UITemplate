using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.IMGUI.Controls;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl;
using System.Collections.Generic;
using HeurekaGames.Utils;

//Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_Window : EditorWindow
    {
        public const   int       WINDOWMENUITEMPRIO = 11;
        public static  string    VERSION            = "1.0.0";
        private static AH_Window m_window;

        [NonSerialized]  private bool                   m_Initialized;
        [SerializeField] private TreeViewState          m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] private MultiColumnHeaderState m_MultiColumnHeaderState;

        private SearchField              m_SearchField;
        private AH_TreeViewWithTreeModel m_TreeView;

        [SerializeField] public AH_BuildInfoManager buildInfoManager;
        public                  bool                m_BuildLogLoaded { get; set; }

        //Button guiContent
        [SerializeField] private GUIContent guiContentLoadBuildInfo;
        [SerializeField] private GUIContent guiContentSettings;
        [SerializeField] private GUIContent guiContentGenerateReferenceGraph;
        [SerializeField] private GUIContent guiContentDuplicates;

        //Only avaliable in 2018
        #if UNITY_2018_1_OR_NEWER
        [SerializeField] private GUIContent guiContentBuildReport;
        #endif
        [SerializeField] private GUIContent guiContentReadme;
        [SerializeField] private GUIContent guiContentDeleteAll;
        [SerializeField] private GUIContent guiContentRefresh;

        //UI Rect
        private                Vector2                               uiStartPos      = new(10, 50);
        public static          float                                 ButtonMaxHeight = 18;
        public static readonly Heureka_ResourceLoader.HeurekaPackage myPackage       = Heureka_ResourceLoader.HeurekaPackage.AHP;

        //Add menu named "Asset Hunter" to the window menu  
        [MenuItem("Tools/Asset Hunter PRO/Asset Hunter PRO _%h", priority = WINDOWMENUITEMPRIO)]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Asset Hunter PRO", priority = WINDOWMENUITEMPRIO)]
        public static void OpenAssetHunter()
        {
            if (!m_window) initializeWindow();
        }

        private static AH_Window initializeWindow()
        {
            //Open ReadMe
            Heureka_PackageDataManagerEditor.SelectReadme();

            m_window = GetWindow<AH_Window>();

            AH_TreeViewSelectionInfo.OnAssetDeleted += m_window.OnAssetDeleted;
            #if UNITY_2018_1_OR_NEWER
            EditorApplication.projectChanged += m_window.OnProjectChanged;
            #elif UNITY_5_6_OR_NEWER
            EditorApplication.projectWindowChanged += m_window.OnProjectChanged;
            #endif

            if (m_window.buildInfoManager == null) m_window.buildInfoManager = CreateInstance<AH_BuildInfoManager>();

            m_window.initializeGUIContent();

            //Subscribe to changes to list of ignored items
            AH_SettingsManager.Instance.IgnoreListUpdatedEvent += m_window.OnIgnoreListUpdatedEvent;

            return m_window;
        }

        internal static AH_BuildInfoManager GetBuildInfoManager()
        {
            if (!m_window) initializeWindow();

            return m_window.buildInfoManager;
        }

        private void OnEnable()
        {
            AH_SerializationHelper.NewBuildInfoCreated += this.onBuildInfoCreated;
            VERSION                                    =  Heureka_Utils.GetVersionNumber<AH_Window>();
        }

        private void OnDisable()
        {
            AH_SerializationHelper.NewBuildInfoCreated -= this.onBuildInfoCreated;
        }

        private void OnInspectorUpdate()
        {
            if (!m_window) initializeWindow();
        }

        private void OnGUI()
        {
            /*if (Application.isPlaying)
                return;*/

            this.InitIfNeeded();
            this.doHeader();

            if (this.buildInfoManager != null && this.buildInfoManager.IsProjectClean())
            {
                Heureka_WindowStyler.DrawCenteredImage(m_window, AH_EditorData.Icons.Achievement);
                return;
            }

            if (this.buildInfoManager == null || !this.buildInfoManager.HasSelection)
            {
                this.doNoBuildInfoLoaded();
                return;
            }

            this.doSearchBar(this.toolbarRect);
            this.doTreeView(this.multiColumnTreeViewRect);

            this.doBottomToolBar(this.bottomToolbarRect);
        }

        private void OnProjectChanged()
        {
            this.buildInfoManager.ProjectDirty = true;
        }

        //Callback
        private void OnAssetDeleted()
        {
            //TODO need to improve the deletion of empty folder. Currently leaves meta file behind, causing warnings
            if (EditorUtility.DisplayDialog("Delete empty folders", "Do you want to delete any empty folders?", "Yes", "No")) this.deleteEmptyFolders();

            //This might be called excessively
            if (AH_SettingsManager.Instance.AutoRefreshLog)
            {
                this.RefreshBuildLog();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Refresh Asset Hunter Log", "Do you want to refresh the loaded log", "Yes", "No")) this.RefreshBuildLog();
            }
        }

        //callback
        private void onBuildInfoCreated(string path)
        {
            if (EditorUtility.DisplayDialog(
                "New buildinfo log created",
                "Do you want to load it into Asset Hunter",
                "Ok",
                "Cancel"))
            {
                this.m_Initialized = false;
                this.buildInfoManager.SelectBuildInfo(path);
            }
        }

        private void InitIfNeeded()
        {
            //We dont need to do stuff when in play mode
            if (this.buildInfoManager && this.buildInfoManager.HasSelection && !this.m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (this.m_TreeViewState == null) this.m_TreeViewState = new();

                var firstInit   = this.m_MultiColumnHeaderState == null;
                var headerState = AH_TreeViewWithTreeModel.CreateDefaultMultiColumnHeaderState(this.multiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(this.m_MultiColumnHeaderState, headerState)) MultiColumnHeaderState.OverwriteSerializedFields(this.m_MultiColumnHeaderState, headerState);
                this.m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new AH_MultiColumnHeader(headerState);
                if (firstInit) multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<AH_TreeviewElement>(this.buildInfoManager.GetTreeViewData());

                this.m_TreeView = new(this.m_TreeViewState, multiColumnHeader, treeModel);

                this.m_SearchField                         =  new();
                this.m_SearchField.downOrUpArrowKeyPressed += this.m_TreeView.SetFocusAndEnsureSelectedItem;

                this.m_Initialized                 = true;
                this.buildInfoManager.ProjectDirty = false;
            }

            //This is an (ugly) fix to make sure we dotn loose our icons due to some singleton issues after play/stop
            if (this.guiContentRefresh.image == null) this.initializeGUIContent();
        }

        private void deleteEmptyFolders()
        {
            var emptyfolders = new List<string>();
            this.checkEmptyFolder(Application.dataPath, emptyfolders);

            if (emptyfolders.Count > 0)
            {
                #if UNITY_2020_1_OR_NEWER
                var failedPaths = new List<string>();
                AssetDatabase.DeleteAssets(emptyfolders.ToArray(), failedPaths);
                #else
            foreach (var folder in emptyfolders)
            {
                    FileUtil.DeleteFileOrDirectory(folder);
                //AssetDatabase.DeleteAsset(folder);
            }
                #endif
                Debug.Log($"AH: Deleted {emptyfolders.Count} empty folders ");
                AssetDatabase.Refresh();
            }
        }

        private bool checkEmptyFolder(string dataPath, List<string> emptyfolders)
        {
            if (dataPath.EndsWith(".git", StringComparison.InvariantCultureIgnoreCase)) return false;

            var files         = System.IO.Directory.GetFiles(dataPath);
            var hasValidAsset = false;

            for (var i = 0; i < files.Length; i++)
            {
                string relativePath;
                string assetID;
                AH_Utils.GetRelativePathAndAssetID(files[i], out relativePath, out assetID);

                //This folder has a valid asset inside
                if (!string.IsNullOrEmpty(assetID))
                {
                    hasValidAsset = true;
                    break;
                }
            }

            var folders               = System.IO.Directory.GetDirectories(dataPath);
            var hasFolderWithContents = false;

            for (var i = 0; i < folders.Length; i++)
            {
                var folderIsEmpty = this.checkEmptyFolder(folders[i], emptyfolders);
                if (!folderIsEmpty)
                    hasFolderWithContents = true;
                else
                    emptyfolders.Add(FileUtil.GetProjectRelativePath(folders[i]));
            }

            return !hasValidAsset && !hasFolderWithContents;
        }

        private void initializeGUIContent()
        {
            this.titleContent = Heureka_ResourceLoader.GetContentWithTitle(myPackage, Heureka_ResourceLoader.IconNames.TabIconAHP, "Asset Hunter");

            this.guiContentLoadBuildInfo          = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.LoadLog, "Load", "Load info from a previous build");
            this.guiContentSettings               = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.Settings, "Settings", "Open settings");
            this.guiContentGenerateReferenceGraph = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.ReferenceGraph, "Dependencies", "See asset dependency graph");
            this.guiContentDuplicates             = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.Duplicate, "Duplicates", "Find duplicate assets");

            //Only avaliable in 2018
            #if UNITY_2018_1_OR_NEWER
            this.guiContentBuildReport = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.Report, "Report", "Build report overview (Build size information)");
            #endif
            this.guiContentReadme    = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.Help, "Info", "Open the readme file for all installed Heureka Games products");
            this.guiContentDeleteAll = Heureka_ResourceLoader.GetContent(myPackage, AH_EditorData.IconNames.Delete, "Clean ALL", "Delete ALL unused assets in project. Remember to manually exclude relevant assets in the settings window"); //new GUIContent("Clean ALL", AH_EditorData.Instance.DeleteIcon.Icon, "Delete ALL unused assets in project ({0}) Remember to manually exclude relevant assets in the settings window");
            this.guiContentRefresh   = Heureka_ResourceLoader.GetContentWithTooltip(myPackage, AH_EditorData.IconNames.Refresh, "Refresh data");                                                                                              //new GUIContent(AH_EditorData.Instance.RefreshIcon.Icon, "Refresh data");
        }

        private void doNoBuildInfoLoaded()
        {
            Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Icons.IconLargeWhite, 380f, 110f, "Buildinfo not yet loaded" + Environment.NewLine + "Load existing / create new build");
        }

        private void doHeader()
        {
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Pink, "ASSET HUNTER PRO", VERSION);
            EditorGUILayout.BeginHorizontal();

            var infoLoaded = this.buildInfoManager != null && this.buildInfoManager.HasSelection;
            if (infoLoaded)
            {
                var RefreshGUIContent = new GUIContent(this.guiContentRefresh);
                var origColor         = GUI.color;
                if (this.buildInfoManager.ProjectDirty)
                {
                    GUI.color                 = Heureka_WindowStyler.clr_Red;
                    RefreshGUIContent.tooltip = string.Format("{0}{1}", RefreshGUIContent.tooltip, " (Project has changed which means that treeview is out of date)");
                }

                if (this.doSelectionButton(RefreshGUIContent)) this.RefreshBuildLog();

                GUI.color = origColor;
            }

            if (this.doSelectionButton(this.guiContentLoadBuildInfo)) this.openBuildInfoSelector();

            if (this.doSelectionButton(this.guiContentDuplicates)) AH_DuplicatesWindow.Init(Docker.DockPosition.Left);

            if (this.doSelectionButton(this.guiContentGenerateReferenceGraph)) AH_DependencyGraphWindow.Init(Docker.DockPosition.Right);

            //Only avaliable in 2018
            #if UNITY_2018_1_OR_NEWER
            if (infoLoaded && this.doSelectionButton(this.guiContentBuildReport)) AH_BuildReportWindow.Init();
            #endif
            if (this.doSelectionButton(this.guiContentSettings)) AH_SettingsWindow.Init(true);

            if (infoLoaded && this.m_TreeView.GetCombinedUnusedSize() > 0)
            {
                var sizeAsString = AH_Utils.GetSizeAsString(this.m_TreeView.GetCombinedUnusedSize());

                var instancedGUIContent = new GUIContent(this.guiContentDeleteAll);
                instancedGUIContent.tooltip = string.Format(instancedGUIContent.tooltip, sizeAsString);
                if (AH_SettingsManager.Instance.HideButtonText) instancedGUIContent.text = null;

                GUIStyle btnStyle = "button";
                var      newStyle = new GUIStyle(btnStyle);
                newStyle.normal.textColor = Heureka_WindowStyler.clr_Pink;

                this.m_TreeView.DrawDeleteAllButton(instancedGUIContent, newStyle, GUILayout.MaxHeight(AH_SettingsManager.Instance.HideButtonText ? ButtonMaxHeight * 2f : ButtonMaxHeight));
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(20);

            if (this.m_TreeView != null) this.m_TreeView.AssetSelectionToolBarGUI();

            if (this.doSelectionButton(this.guiContentReadme)) Heureka_PackageDataManagerEditor.SelectReadme();

            if (this.doPromotionButton()) Application.OpenURL(Heureka_EditorData.Links.FromAHPToSmartBuilder);

            EditorGUILayout.EndHorizontal();
        }

        private void doSearchBar(Rect rect)
        {
            if (this.m_TreeView != null) this.m_TreeView.searchString = this.m_SearchField.OnGUI(rect, this.m_TreeView.searchString);
        }

        private void doTreeView(Rect rect)
        {
            if (this.m_TreeView != null) this.m_TreeView.OnGUI(rect);
        }

        private void doBottomToolBar(Rect rect)
        {
            if (this.m_TreeView == null) return;

            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = "miniButton";

                if (GUILayout.Button("Expand All", style)) this.m_TreeView.ExpandAll();

                if (GUILayout.Button("Collapse All", style)) this.m_TreeView.CollapseAll();

                GUILayout.Label("Build: " + this.buildInfoManager.GetSelectedBuildDate() + " (" + this.buildInfoManager.GetSelectedBuildTarget() + ")");
                GUILayout.FlexibleSpace();
                GUILayout.Label(this.buildInfoManager.TreeView != null ? AssetDatabase.GetAssetPath(this.buildInfoManager.TreeView) : string.Empty);
                GUILayout.FlexibleSpace();

                if (((AH_MultiColumnHeader)this.m_TreeView.multiColumnHeader).mode == AH_MultiColumnHeader.Mode.SortedList || !string.IsNullOrEmpty(this.m_TreeView.searchString))
                    if (GUILayout.Button("Return to Treeview", style))
                        this.m_TreeView.ShowTreeMode();
                var exportContent = new GUIContent("Export list", "Export all the assets in the list above to a json file");
                if (GUILayout.Button(exportContent, style))
                {
                    var buildInfo = this.buildInfoManager.GetSelectedBuildDate() + "_" + this.buildInfoManager.GetSelectedBuildTarget() + "_" + ((AH_MultiColumnHeader)this.m_TreeView.multiColumnHeader).ShowMode;

                    var menu = new GenericMenu();
                    menu.AddItem(new("JSON"), false, () => AH_ElementList.DumpCurrentListToJSONFile(this.m_TreeView, buildInfo));
                    menu.AddItem(new("CSV"), false, () => AH_ElementList.DumpCurrentListToCSVFile(this.m_TreeView, buildInfo));
                    menu.ShowAsContext();
                }
            }
            GUILayout.EndArea();
        }

        private bool doPromotionButton()
        {
            if (AH_SettingsManager.Instance.HideNewsButton) return false;

            GUIStyle   buttonStyle = null;
            GUIContent btnContent;
            if (AH_SettingsManager.Instance.HideButtonText)
            {
                btnContent      = new(AH_EditorData.Contents.News);
                btnContent.text = null;

                buttonStyle = new(GUI.skin.button)
                {
                    padding = new(0, 0, 0, 0),
                };
            }
            else
            {
                btnContent = AH_EditorData.Contents.News;

                buttonStyle = GUI.skin.button;
            }

            var btnSize = AH_SettingsManager.Instance.HideButtonText ? ButtonMaxHeight * 2f : ButtonMaxHeight;

            return GUILayout.Button(btnContent, buttonStyle, GUILayout.MaxHeight(btnSize));
        }

        private bool doSelectionButton(GUIContent content)
        {
            var btnContent                                                  = new GUIContent(content);
            if (AH_SettingsManager.Instance.HideButtonText) btnContent.text = null;

            return GUILayout.Button(btnContent, GUILayout.MaxHeight(AH_SettingsManager.Instance.HideButtonText ? ButtonMaxHeight * 2f : ButtonMaxHeight));
        }

        private void OnIgnoreListUpdatedEvent()
        {
            this.buildInfoManager.ProjectDirty = true;

            if (AH_SettingsManager.Instance.AutoOpenLog) this.RefreshBuildLog();
        }

        private void RefreshBuildLog()
        {
            if (this.buildInfoManager != null && this.buildInfoManager.HasSelection)
            {
                this.m_Initialized = false;
                this.buildInfoManager.RefreshBuildInfo();
            }
        }

        private void openBuildInfoSelector()
        {
            var fileSelected = EditorUtility.OpenFilePanel("", AH_SerializationHelper.GetBuildInfoFolder(), AH_SerializationHelper.BuildInfoExtension);
            if (!string.IsNullOrEmpty(fileSelected))
            {
                this.m_Initialized = false;
                this.buildInfoManager.SelectBuildInfo(fileSelected);
            }
        }

        private Rect toolbarRect => new(this.UiStartPos.x, this.UiStartPos.y + (AH_SettingsManager.Instance.HideButtonText ? 20 : 0), this.position.width - this.UiStartPos.x * 2, 20f);

        private Rect multiColumnTreeViewRect => new(this.UiStartPos.x, this.UiStartPos.y + 20 + (AH_SettingsManager.Instance.HideButtonText ? 20 : 0), this.position.width - this.UiStartPos.x * 2, this.position.height - 90 - (AH_SettingsManager.Instance.HideButtonText ? 20 : 0));

        private Rect assetInfoRect => new(this.UiStartPos.x, this.position.height - 66f, this.position.width - this.UiStartPos.x * 2, 16f);

        private Rect bottomToolbarRect => new(this.UiStartPos.x, this.position.height - 18, this.position.width - this.UiStartPos.x * 2, 16f);

        public Vector2 UiStartPos { get => this.uiStartPos; set => this.uiStartPos = value; }

        private void OnDestroy()
        {
            AH_TreeViewSelectionInfo.OnAssetDeleted -= m_window.OnAssetDeleted;
            #if UNITY_2018_1_OR_NEWER
            EditorApplication.projectChanged -= m_window.OnProjectChanged;
            #elif UNITY_5_6_OR_NEWER
            EditorApplication.projectWindowChanged -= m_window.OnProjectChanged;
            #endif
        }
    }
}