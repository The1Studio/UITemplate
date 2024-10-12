using HeurekaGames.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_DuplicatesWindow : EditorWindow
    {
        private static readonly string                  WINDOWNAME = "AH Duplicates";
        private static          AH_DuplicatesWindow     window;
        private                 AH_DuplicateDataManager duplicateDataManager;
        private                 Vector2                 scrollPosition;
        private                 int                     scrollStartIndex;
        private                 GUIContent              guiContentRefresh;
        private                 GUIContent              buttonSelectContent;
        private                 GUIContent              labelBtnContent;
        private                 GUIStyle                labelBtnStyle;
        private                 List<float>             scrollviewPositionList = new();
        private                 Rect                    scrollArea;
        private                 int                     scrollEndIndex;

        //Add menu named "Dependency Graph" to the window menu  
        [MenuItem("Tools/Asset Hunter PRO/Find Duplicates")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Find Duplicates")]
        public static void OpenDuplicatesView()
        {
            Init();
        }

        private void OnEnable()
        {
            //Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.projectChanged += this.EditorApplication_projectChanged;
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= this.EditorApplication_projectChanged;
        }

        private void OnGUI()
        {
            this.initIfNeeded();
            this.doHeader();

            if (this.duplicateDataManager != null)
            {
                //If window has no cached data
                if (!this.duplicateDataManager.HasCache)
                {
                    Heureka_WindowStyler.DrawCenteredMessage(window, AH_EditorData.Icons.DuplicateIconWhite, 240f, 110f, "No data" + Environment.NewLine + "Find duplicates");
                    EditorGUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    var origClr = GUI.backgroundColor;

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Find Duplicates", GUILayout.Height(40))) this.duplicateDataManager.RefreshData();
                    GUI.backgroundColor = origClr;
                    EditorGUILayout.EndVertical();
                    return;
                }
                else
                {
                    if (!this.duplicateDataManager.HasDuplicates())
                    {
                        Heureka_WindowStyler.DrawCenteredMessage(window, AH_EditorData.Icons.DuplicateIconWhite, 240f, 110f, "Hurray" + Environment.NewLine + "No duplicates assets" + Environment.NewLine + "in project :)");
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        this.doBody();
                    }
                }
            }
            this.doFooter();
        }

        public static void Init()
        {
            window = GetWindow<AH_DuplicatesWindow>(WINDOWNAME, true);
            if (window.duplicateDataManager == null) window.duplicateDataManager = AH_DuplicateDataManager.instance;

            window.initializeGUIContent();
        }

        public static void Init(Docker.DockPosition dockPosition = Docker.DockPosition.Right)
        {
            Init();

            var mainWindows = Resources.FindObjectsOfTypeAll<AH_Window>();
            if (mainWindows.Length != 0) Docker.Dock(mainWindows[0], window, dockPosition);
        }

        private void initIfNeeded()
        {
            if (!this.duplicateDataManager || !window) Init();

            //This is an (ugly) fix to make sure we dotn loose our icons due to some singleton issues after play/stop
            if (this.guiContentRefresh.image == null) this.initializeGUIContent();
        }

        private void initializeGUIContent()
        {
            this.titleContent      = Heureka_ResourceLoader.GetContentWithTitle(AH_Window.myPackage, AH_EditorData.IconNames.Duplicate, WINDOWNAME);
            this.guiContentRefresh = Heureka_ResourceLoader.GetContentWithTooltip(AH_Window.myPackage, AH_EditorData.IconNames.Refresh, "Refresh data");

            this.buttonSelectContent = new() { };

            this.labelBtnStyle        = new(EditorStyles.label);
            this.labelBtnStyle.border = new(0, 0, 0, 0);

            this.labelBtnContent = new();
        }

        private void EditorApplication_projectChanged()
        {
            this.duplicateDataManager.IsDirty = true;
        }

        private void doHeader()
        {
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Red, WINDOWNAME);
        }

        private void doBody()
        {
            if (this.duplicateDataManager.RequiresScrollviewRebuild) this.scrollviewPositionList = new();

            using (var scrollview = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
            {
                this.scrollPosition = scrollview.scrollPosition;

                //Bunch of stuff to figure which guielements we want to draw inside scrollview (We dont want to draw every single element, only the ones that are infact inside scrollview)
                if (Event.current.type == EventType.Layout)
                {
                    this.scrollStartIndex = this.scrollviewPositionList.FindLastIndex(x => x < this.scrollPosition.y);
                    if (this.scrollStartIndex == -1) this.scrollStartIndex = 0;

                    var scrollMaxY = this.scrollPosition.y + this.scrollArea.height;
                    this.scrollEndIndex = this.scrollviewPositionList.FindLastIndex(x => x <= scrollMaxY) + 1;                                                                                                                           //Add one since we want to make sure the entire height of the guielement is shown
                    if (this.scrollEndIndex > this.scrollviewPositionList.Count - 1) this.scrollEndIndex = this.scrollviewPositionList.Count >= 1 ? this.scrollviewPositionList.Count - 1 : this.duplicateDataManager.Entries.Count - 1; //Dont want out of bounds
                }

                //Insert empty space in the BEGINNING of scrollview
                if (this.scrollStartIndex >= 0 && this.scrollviewPositionList.Count > 0) GUILayout.Space(this.scrollviewPositionList[this.scrollStartIndex]);

                var counter = -1;
                foreach (var kvPair in this.duplicateDataManager.Entries)
                {
                    counter++;
                    if (counter < this.scrollStartIndex)
                        continue;
                    else if (counter > this.scrollEndIndex) break;

                    using (var hScope = new EditorGUILayout.HorizontalScope("box"))
                    {
                        var hScopeSize = hScope.rect;
                        this.buttonSelectContent.image = kvPair.Value.Preview;

                        if (GUILayout.Button(this.buttonSelectContent, EditorStyles.boldLabel, GUILayout.Width(64), GUILayout.MaxHeight(64)))
                        {
                            var assets = kvPair.Value.Paths.Select(x => AssetDatabase.LoadMainAssetAtPath(x)).ToArray();
                            Selection.objects = assets;
                        }

                        //EditorGUILayout.LabelField(kvPair.Key);
                        using (var vScope = new EditorGUILayout.VerticalScope("box"))
                        {
                            foreach (var path in kvPair.Value.Paths)
                            {
                                using (new EditorGUI.DisabledGroupScope(Selection.objects.Any(x => AssetDatabase.GetAssetPath(x) == path)))
                                {
                                    var charCount = (int)vScope.rect.width / 7;
                                    this.labelBtnContent.text    = AH_Utils.ShrinkPathEnd(path.Remove(0, 7), charCount);
                                    this.labelBtnContent.tooltip = path;

                                    if (GUILayout.Button(this.labelBtnContent, this.labelBtnStyle)) Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
                                }
                            }
                        }
                        if (this.duplicateDataManager.RequiresScrollviewRebuild && Event.current.type == EventType.Repaint) this.scrollviewPositionList.Add(hScope.rect.y); //Store Y position of guielement rect
                    }
                }
                //We have succesfully rebuild the scrollview position list
                if (this.duplicateDataManager.RequiresScrollviewRebuild && Event.current.type == EventType.Repaint) this.duplicateDataManager.RequiresScrollviewRebuild = false;

                //Insert empty space at the END of scrollview
                if (this.scrollEndIndex < this.scrollviewPositionList.Count - 1) GUILayout.Space(this.scrollviewPositionList.Last() - this.scrollviewPositionList[this.scrollEndIndex]);
            }
            if (Event.current.type == EventType.Repaint) this.scrollArea = GUILayoutUtility.GetLastRect();
        }

        private void doFooter()
        {
            var RefreshGUIContent = new GUIContent(this.guiContentRefresh);
            var origColor         = GUI.color;
            if (this.duplicateDataManager.IsDirty)
            {
                GUI.color                 = Heureka_WindowStyler.clr_Red;
                RefreshGUIContent.tooltip = string.Format("{0}{1}", RefreshGUIContent.tooltip, " (Project has changed which means that data is out of sync)");
            }

            if (AH_UIUtilities.DrawSelectionButton(RefreshGUIContent)) this.duplicateDataManager.RefreshData();

            GUI.color = origColor;
        }
    }
}