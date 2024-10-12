using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.DependencyGraph;
using UnityEditorInternal;
using HeurekaGames.Utils;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_DependencyGraphWindow : EditorWindow
    {
        private static          AH_DependencyGraphWindow  window;
        [SerializeField] public AH_DependencyGraphManager dependencyGraphManager;

        private GUIContent lockedReference;
        private GUIContent unlockedReference;
        private GUIContent contentToggleRefsTo;
        private GUIContent contentToggleRefsFrom;

        [SerializeField] private GUIContent guiContentRefresh;

        [SerializeField] private SearchField searchField;
        private                  bool        initialized;

        // Editor gameObjectEditor;
        private UnityEngine.Object previewObject;

        //UI Rect
        private                  Vector2   uiStartPos = new(10, 50);
        [SerializeField] private bool      seeRefsToInProject;
        [SerializeField] private bool      seeRefsFromInProject;
        private                  Texture2D previewTexture;
        private static readonly  string    WINDOWNAME = "AH Dependency Graph";

        //Add menu named "Dependency Graph" to the window menu  
        [MenuItem("Tools/Asset Hunter PRO/Dependency Graph _%#h", priority = AH_Window.WINDOWMENUITEMPRIO + 1)]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Dependency Graph", priority = AH_Window.WINDOWMENUITEMPRIO + 1)]
        public static void OpenDependencyGraph()
        {
            Init();
        }

        public static void Init()
        {
            window = GetWindow<AH_DependencyGraphWindow>(WINDOWNAME, true);
            if (window.dependencyGraphManager == null) window.dependencyGraphManager = AH_DependencyGraphManager.instance;

            window.initializeGUIContent();
        }

        public static void Init(Docker.DockPosition dockPosition = Docker.DockPosition.Right)
        {
            Init();

            var mainWindows = Resources.FindObjectsOfTypeAll<AH_Window>();
            if (mainWindows.Length != 0) Docker.Dock(mainWindows[0], window, dockPosition);
        }

        private void OnEnable()
        {
            Selection.selectionChanged               += this.OnSelectionChanged;
            EditorApplication.projectChanged         += this.EditorApplication_projectChanged;
            EditorApplication.projectWindowItemOnGUI += this.EditorApplication_ProjectWindowItemCallback;

            this.contentToggleRefsFrom      = new(EditorGUIUtility.IconContent("sv_icon_dot9_sml"));
            this.contentToggleRefsFrom.text = "Is dependency";

            this.contentToggleRefsTo      = new(EditorGUIUtility.IconContent("sv_icon_dot12_sml"));
            this.contentToggleRefsTo.text = "Has dependencies";

            this.lockedReference = new()
            {
                tooltip = "Target Asset is locked, click to unlock",
                image   = EditorGUIUtility.IconContent("LockIcon-On").image,
            };
            this.unlockedReference = new()
            {
                tooltip = "Target Asset is unlocked, click to lock",
                image   = EditorGUIUtility.IconContent("LockIcon").image,
            };
            this.seeRefsToInProject   = EditorPrefs.GetBool("AHP_seeRefsToInProject", true);
            this.seeRefsFromInProject = EditorPrefs.GetBool("AHP_seeRefsFromInProject", true);
        }

        private void EditorApplication_ProjectWindowItemCallback(string guid, Rect r)
        {
            //If nothing references this asset, ignore it
            if (!this.seeRefsFromInProject && !this.seeRefsToInProject) return;

            var frame = new Rect(r);
            frame.x += frame.width;

            if (this.seeRefsFromInProject && this.dependencyGraphManager != null && this.dependencyGraphManager.GetReferencesFrom().ContainsKey(guid))
            {
                frame.x     += -12;
                frame.width += 10f;

                GUI.Label(frame, this.contentToggleRefsFrom.image, EditorStyles.miniLabel);
            }
            if (this.seeRefsToInProject && this.dependencyGraphManager != null && this.dependencyGraphManager.GetReferencesTo().ContainsKey(guid))
            {
                frame.x     += -12f;
                frame.width += 10f;

                GUI.Label(frame, this.contentToggleRefsTo.image, EditorStyles.miniLabel);
            }
        }

        private void EditorApplication_projectChanged()
        {
            if (this.dependencyGraphManager == null)
            {
                this.initIfNeeded();
                return;
            }

            this.dependencyGraphManager.IsDirty = true;
            this.dependencyGraphManager.ResetHistory();
        }

        private void OnGUI()
        {
            this.initIfNeeded();
            this.doHeader();

            if (this.dependencyGraphManager != null)
            {
                //If window has no cached data
                if (!this.dependencyGraphManager.HasCache())
                {
                    Heureka_WindowStyler.DrawCenteredMessage(window, AH_EditorData.Icons.RefFromWhite, 240f, 110f, "No Graph" + Environment.NewLine + "Build Graph");
                    EditorGUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    var origClr = GUI.backgroundColor;

                    GUI.backgroundColor = Heureka_WindowStyler.clr_Red;
                    if (GUILayout.Button("Build Graph", GUILayout.Height(40))) this.dependencyGraphManager.RefreshReferenceGraph();
                    GUI.backgroundColor = origClr;
                    EditorGUILayout.EndVertical();
                    return;
                }

                if (this.dependencyGraphManager.HasSelection)
                {
                    using (new EditorGUILayout.VerticalScope("box"))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button($"{this.dependencyGraphManager.GetSelectedName()}" + (this.dependencyGraphManager.LockedSelection ? " (Locked)" : ""), GUILayout.ExpandWidth(true))) EditorGUIUtility.PingObject(this.dependencyGraphManager.SelectedAsset);

                            if (GUILayout.Button(this.dependencyGraphManager.LockedSelection ? this.lockedReference : this.unlockedReference, EditorStyles.boldLabel, GUILayout.ExpandWidth(false))) this.dependencyGraphManager.LockedSelection = !this.dependencyGraphManager.LockedSelection;
                        }
                        this.drawPreview();
                    }

                    var viewFrom    = this.dependencyGraphManager.GetTreeViewFrom();
                    var isValidFrom = viewFrom?.treeModel?.numberOfDataElements > 1;

                    var viewTo    = this.dependencyGraphManager.GetTreeViewTo();
                    var isValidTo = viewTo?.treeModel?.numberOfDataElements > 1;

                    using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandWidth(true)))
                    {
                        using (new EditorGUILayout.HorizontalScope("box", GUILayout.ExpandWidth(true)))
                        {
                            GUILayout.Label(AH_EditorData.Icons.RefFrom, GUILayout.Width(32), GUILayout.Height(32));
                            using (new EditorGUILayout.VerticalScope(GUILayout.Height(32)))
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.LabelField($"A dependency of {(isValidFrom ? viewFrom.treeModel.root.children.Count() : 0)}", EditorStyles.boldLabel);
                                GUILayout.FlexibleSpace();
                            }
                        }
                        if (isValidFrom) this.drawAssetList(this.dependencyGraphManager.GetTreeViewFrom());

                        using (new EditorGUILayout.HorizontalScope("box", GUILayout.ExpandWidth(true)))
                        {
                            GUILayout.Label(AH_EditorData.Icons.RefTo, GUILayout.Width(32), GUILayout.Height(32));
                            using (new EditorGUILayout.VerticalScope(GUILayout.Height(32)))
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUILayout.LabelField($"Depends on {(isValidTo ? viewTo.treeModel.root.children.Count() : 0)}", EditorStyles.boldLabel);
                                GUILayout.FlexibleSpace();
                            }
                        }
                        if (isValidTo) this.drawAssetList(this.dependencyGraphManager.GetTreeViewTo());

                        //Force flexible size here to make sure the preview area doesn't fill entire window
                        if (!isValidTo && !isValidFrom) GUILayout.FlexibleSpace();
                    }
                }
                else
                {
                    Heureka_WindowStyler.DrawCenteredMessage(window, AH_EditorData.Icons.RefFromWhite, 240f, 110f, "No selection" + Environment.NewLine + "Select asset in project view");
                }
            }
            this.doFooter();
            //Make sure this window has focus to update contents
            this.Repaint();
        }

        private void drawPreview()
        {
            if (this.dependencyGraphManager.SelectedAsset != null)
            {
                var old = this.previewObject;
                this.previewObject = this.dependencyGraphManager.SelectedAsset;
                //if (previewObject != old)
                {
                    this.previewTexture = AssetPreview.GetAssetPreview(this.previewObject); //Asnyc, so we have to do this each frame
                    if (this.previewTexture == null) this.previewTexture = AssetPreview.GetMiniThumbnail(this.previewObject);
                }
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.BeginHorizontal();
                    this.drawHistoryButton(-1);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(this.previewTexture, EditorStyles.boldLabel, /*GUILayout.Width(64),*/ GUILayout.MaxHeight(64), GUILayout.ExpandWidth(true))) EditorGUIUtility.PingObject(this.previewObject);
                    GUILayout.FlexibleSpace();
                    this.drawHistoryButton(1);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void drawHistoryButton(int direction)
        {
            if (!this.dependencyGraphManager.LockedSelection)
            {
                var    style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                string tooltip;
                var    validDirection = this.dependencyGraphManager.HasHistory(direction, out tooltip);

                EditorGUI.BeginDisabledGroup(!validDirection);
                var content                                         = new GUIContent(validDirection ? direction == -1 ? "<" : ">" : string.Empty);
                if (!string.IsNullOrEmpty(tooltip)) content.tooltip = tooltip;

                if (GUILayout.Button(content, style, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false), GUILayout.Width(12)))
                {
                    if (direction == -1)
                        this.dependencyGraphManager.SelectPreviousFromHistory();
                    else if (direction == 1)
                        this.dependencyGraphManager.SelectNextFromHistory();
                    else
                        Debug.LogWarning("Wrong integer. You must select -1 or 1");
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void initIfNeeded()
        {
            if (!this.dependencyGraphManager || !window) Init();

            if (this.dependencyGraphManager.RequiresRefresh()) this.OnSelectionChanged();

            //We dont need to do stuff when in play mode
            if (this.dependencyGraphManager && !this.initialized)
            {
                if (this.searchField == null) this.searchField = new();

                this.dependencyGraphManager.Initialize(this.searchField, this.multiColumnTreeViewRect);
                this.initialized = true;

                InternalEditorUtility.RepaintAllViews();
            }

            //This is an (ugly) fix to make sure we dotn loose our icons due to some singleton issues after play/stop
            if (this.guiContentRefresh.image == null) this.initializeGUIContent();
        }

        private void initializeGUIContent()
        {
            this.titleContent      = Heureka_ResourceLoader.GetContentWithTitle(AH_Window.myPackage, AH_EditorData.IconNames.RefFrom, WINDOWNAME);
            this.guiContentRefresh = Heureka_ResourceLoader.GetContentWithTitle(AH_Window.myPackage, AH_EditorData.IconNames.Refresh, "Refresh data");
        }

        private void doFooter()
        {
            if (this.dependencyGraphManager != null)
            {
                if (!this.dependencyGraphManager.HasSelection) GUILayout.FlexibleSpace();

                var RefreshGUIContent = new GUIContent(this.guiContentRefresh);
                var origColor         = GUI.color;
                if (this.dependencyGraphManager.IsDirty)
                {
                    GUI.color                 = Heureka_WindowStyler.clr_Red;
                    RefreshGUIContent.tooltip = string.Format("{0}{1}", RefreshGUIContent.tooltip, " (Project has changed which means that treeview is out of date)");
                }

                if (AH_UIUtilities.DrawSelectionButton(RefreshGUIContent)) this.dependencyGraphManager.RefreshReferenceGraph();

                GUI.color = origColor;

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                this.seeRefsToInProject   = GUILayout.Toggle(this.seeRefsToInProject, this.contentToggleRefsTo);
                this.seeRefsFromInProject = GUILayout.Toggle(this.seeRefsFromInProject, this.contentToggleRefsFrom);
                //Do we need to repaint projewct view?
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool("AHP_seeRefsToInProject", this.seeRefsToInProject);
                    EditorPrefs.SetBool("AHP_seeRefsFromInProject", this.seeRefsFromInProject);
                    InternalEditorUtility.RepaintAllViews();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void doHeader()
        {
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_lBlue, WINDOWNAME);

            var hasReferenceGraph = this.dependencyGraphManager != null;
            if (hasReferenceGraph)
                if (this.dependencyGraphManager.HasSelection && this.dependencyGraphManager.HasCache())
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(AH_Window.ButtonMaxHeight));
                    this.doSearchBar(this.searchBar);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
        }

        private void drawAssetList(TreeView view)
        {
            if (view != null)
            {
                var rect = EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                view.OnGUI(rect);
                EditorGUILayout.EndVertical();
            }
        }

        /*private bool doSelectionButton(GUIContent content)
        {
            GUIContent btnContent = new GUIContent(content);
            if (AH_SettingsManager.Instance.HideButtonText)
                btnContent.text = null;

            return GUILayout.Button(btnContent, GUILayout.MaxHeight(AH_SettingsManager.Instance.HideButtonText ? AH_Window.ButtonMaxHeight * 2f : AH_Window.ButtonMaxHeight));
        }*/

        private void doSearchBar(Rect rect)
        {
            if (this.searchField != null) this.dependencyGraphManager.SearchString = this.searchField.OnGUI(rect, this.dependencyGraphManager.SearchString);
        }

        private void OnSelectionChanged()
        {
            if (this.dependencyGraphManager != null && !this.dependencyGraphManager.LockedSelection)
            {
                this.dependencyGraphManager.UpdateSelectedAsset(Selection.activeObject ? Selection.activeObject : null);
                this.initialized = false;
            }
        }

        private Rect searchBar => new(this.uiStartPos.x + AH_Window.ButtonMaxHeight, this.uiStartPos.y - (AH_Window.ButtonMaxHeight + 6), this.position.width - this.uiStartPos.x * 2 - AH_Window.ButtonMaxHeight * 2, AH_Window.ButtonMaxHeight);

        private Rect multiColumnTreeViewRect
        {
            get
            {
                var newRect = new Rect(this.uiStartPos.x, this.uiStartPos.y + 20 + (AH_SettingsManager.Instance.HideButtonText ? 20 : 0), this.position.width - this.uiStartPos.x * 2, this.position.height - 90 - (AH_SettingsManager.Instance.HideButtonText ? 20 : 0));
                return newRect;
            }
        }

        private void OnDisable()
        {
            Selection.selectionChanged               -= this.OnSelectionChanged;
            EditorApplication.projectChanged         -= this.EditorApplication_projectChanged;
            EditorApplication.projectWindowItemOnGUI -= this.EditorApplication_ProjectWindowItemCallback;
        }

        private void OnDestroy()
        {
            DestroyImmediate(this.dependencyGraphManager);
        }
    }
}