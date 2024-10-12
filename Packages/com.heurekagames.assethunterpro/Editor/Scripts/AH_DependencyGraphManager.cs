using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.DependencyGraph
{
    [Serializable]
    public class AH_DependencyGraphManager : ScriptableSingleton<AH_DependencyGraphManager>, ISerializationCallbackReceiver
    {
        [SerializeField] public  bool                             IsDirty = true;
        [SerializeField] private TreeViewState                    treeViewStateFrom;
        [SerializeField] private TreeViewState                    treeViewStateTo;
        [SerializeField] private MultiColumnHeaderState           multiColumnHeaderStateFrom;
        [SerializeField] private MultiColumnHeaderState           multiColumnHeaderStateTo;
        [SerializeField] private AH_DepGraphTreeviewWithModel     TreeViewModelFrom;
        [SerializeField] private AH_DepGraphTreeviewWithModel     TreeViewModelTo;
        [SerializeField] private Dictionary<string, List<string>> referencedFrom = new();
        [SerializeField] private Dictionary<string, List<string>> referenceTo    = new();

        #region serializationHelpers

        [SerializeField] private List<string>         _keysFrom          = new();
        [SerializeField] private List<AH_WrapperList> _wrapperValuesFrom = new();

        [SerializeField] private List<string>         _keysTo          = new();
        [SerializeField] private List<AH_WrapperList> _wrapperValuesTo = new();

        #endregion

        [SerializeField] private string             selectedAssetGUID       = "";
        [SerializeField] private string             selectedAssetObjectName = "";
        [SerializeField] private UnityEngine.Object selectedAssetObject;

        [SerializeField] private List<string> selectionHistory      = new();
        [SerializeField] private int          selectionHistoryIndex = 0;

        private bool lockedSelection;

        public bool LockedSelection
        {
            get => this.lockedSelection;
            set
            {
                this.lockedSelection = value;

                //Update currently selected
                if (this.lockedSelection == false)
                {
                    this.requiresRefresh = true;
                    this.UpdateSelectedAsset(Selection.activeObject);
                }
            }
        }

        public bool TraversingHistory { get; set; }

        //Force window to refresh selection
        private bool requiresRefresh;

        //We clear history when project changes, as there are problems in identifying if history points to deleted assets
        internal void ResetHistory()
        {
            var obsoleteAssets = this.selectionHistory.FindAll(x => AssetDatabase.LoadMainAssetAtPath(x) == null);
            //Remove the objets that are no longer in asset db
            this.selectionHistory.RemoveAll(x => obsoleteAssets.Contains(x));

            var duplicateCount = obsoleteAssets.Count;
            for (var i = this.selectionHistory.Count - 1; i >= 0; i--)
                //Find identical IDs directly after each other
            {
                if (i > 0 && this.selectionHistory[i] == this.selectionHistory[i - 1])
                {
                    this.selectionHistory.RemoveAt(i);
                    duplicateCount++;
                }
            }
            //Reset history index to match new history
            this.selectionHistoryIndex -= duplicateCount;
        }

        public string SearchString
        {
            get => this.treeViewStateFrom.searchString;
            set
            {
                var tmp = this.treeViewStateFrom.searchString;

                this.treeViewStateFrom.searchString = this.treeViewStateTo.searchString = value;
                if (tmp != value)
                {
                    this.TreeViewModelTo.Reload();
                    this.TreeViewModelFrom.Reload();
                }
            }
        }

        //Return selected asset
        public UnityEngine.Object SelectedAsset => this.selectedAssetObject;

        public void OnEnable()
        {
            this.hideFlags = HideFlags.HideAndDontSave;
        }

        public void Initialize(SearchField searchField, Rect multiColumnTreeViewRect)
        {
            var referenceID = 0;
            this.initTreeview(ref this.treeViewStateFrom, ref this.multiColumnHeaderStateFrom, multiColumnTreeViewRect, ref this.TreeViewModelFrom, searchField, this.referencedFrom, this.selectedAssetGUID, ref referenceID);
            this.initTreeview(ref this.treeViewStateTo, ref this.multiColumnHeaderStateTo, multiColumnTreeViewRect, ref this.TreeViewModelTo, searchField, this.referenceTo, this.selectedAssetGUID, ref referenceID);

            this.requiresRefresh = false;
        }

        public void RefreshReferenceGraph()
        {
            this.referenceTo    = new();
            this.referencedFrom = new();

            var paths     = AssetDatabase.GetAllAssetPaths();
            var pathCount = paths.Length;

            for (var i = 0; i < pathCount; i++)
            {
                var path = paths[i];
                if (AssetDatabase.IsValidFolder(path) || !path.StartsWith("Assets")) //Slow, could be done recusively
                    continue;

                if (EditorUtility.DisplayCancelableProgressBar("Creating Reference Graph", path, (float)i / (float)pathCount))
                {
                    this.referenceTo    = new();
                    this.referencedFrom = new();
                    break;
                }

                var allRefs       = AssetDatabase.GetDependencies(path, false);
                var assetPathGuid = AssetDatabase.AssetPathToGUID(path);

                var newList = allRefs.Where(val => val != path).Select(val => AssetDatabase.AssetPathToGUID(val)).ToList();

                //Store everything reference by this asset
                if (newList.Count > 0) this.referenceTo.Add(assetPathGuid, newList);

                //Foreach asset refenced by this asset, store the connection
                foreach (var reference in allRefs)
                {
                    var refGuid = AssetDatabase.AssetPathToGUID(reference);

                    if (!this.referencedFrom.ContainsKey(refGuid)) this.referencedFrom.Add(refGuid, new());

                    this.referencedFrom[refGuid].Add(assetPathGuid);
                }
            }

            this.IsDirty = false;
            EditorUtility.ClearProgressBar();
        }

        private void initTreeview(ref TreeViewState _treeViewState, ref MultiColumnHeaderState _headerState, Rect _rect, ref AH_DepGraphTreeviewWithModel _treeView, SearchField _searchField, Dictionary<string, List<string>> referenceDict, string assetGUID, ref int referenceID)
        {
            bool hasValidReferences;
            var  treeModel = new TreeModel<AH_DepGraphElement>(this.getTreeViewData(referenceDict, assetGUID, out hasValidReferences, ref referenceID));

            // Check if it already exists (deserialized from window layout file or scriptable object)
            if (_treeViewState == null) _treeViewState = new();

            var firstInit   = _headerState == null;
            var headerState = AH_DepGraphTreeviewWithModel.CreateDefaultMultiColumnHeaderState(_rect.width);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(_headerState, headerState)) MultiColumnHeaderState.OverwriteSerializedFields(_headerState, headerState);
            _headerState = headerState;

            var multiColumnHeader = new AH_DepGraphHeader(_headerState);
            //if (firstInit)
            multiColumnHeader.ResizeToFit();

            _treeView                            =  new(_treeViewState, multiColumnHeader, treeModel);
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
        }

        internal void UpdateTreeData(ref AH_DepGraphTreeviewWithModel _treeView, Dictionary<string, List<string>> referenceDict, string assetGUID, ref int referenceID)
        {
            bool hasValidReferences;
            _treeView.treeModel.SetData(this.getTreeViewData(referenceDict, assetGUID, out hasValidReferences, ref referenceID));
        }

        internal bool HasCache()
        {
            return this.referencedFrom.Count > 0 && this.referenceTo.Count > 0;
        }

        internal bool HasHistory(int direction, out string tooltip)
        {
            var testIndex  = this.selectionHistoryIndex + direction;
            var validIndex = testIndex >= 0 && testIndex < this.selectionHistory.Count;
            tooltip = validIndex ? AssetDatabase.LoadMainAssetAtPath(this.selectionHistory[testIndex])?.name : string.Empty;
            //Validate that history contains that index
            return testIndex >= 0 && testIndex < this.selectionHistory.Count;
        }

        public bool HasSelection => !string.IsNullOrEmpty(this.selectedAssetGUID);

        internal bool RequiresRefresh()
        {
            return this.requiresRefresh;
        }

        internal AH_DepGraphTreeviewWithModel GetTreeViewTo()
        {
            return this.TreeViewModelTo;
        }

        internal AH_DepGraphTreeviewWithModel GetTreeViewFrom()
        {
            return this.TreeViewModelFrom;
        }

        internal Dictionary<string, List<string>> GetReferencesTo()
        {
            return this.referenceTo;
        }

        internal string GetSelectedName()
        {
            return this.selectedAssetObjectName;
        }

        internal Dictionary<string, List<string>> GetReferencesFrom()
        {
            return this.referencedFrom;
        }

        public IList<AH_DepGraphElement> getTreeViewData(Dictionary<string, List<string>> referenceDict, string assetGUID, out bool success, ref int referenceID)
        {
            var treeElements = new List<AH_DepGraphElement>();

            var depth = -1;

            var root = new AH_DepGraphElement("Root", depth, -1, "");
            treeElements.Add(root);

            var referenceQueue = new Stack<string>(); //Since we are creating a tree we want the same asset to be referenced in any branch, but we do NOT want circular references

            var references = referenceDict.ContainsKey(assetGUID) ? referenceDict[assetGUID] : null;
            if (references != null)
                foreach (var item in references)
                    this.addElement(treeElements, referenceDict, item, ref depth, ref referenceID, ref referenceQueue);

            success = treeElements.Count > 2; //Did we find any references (Contains more thatn 'root' and 'self')
            TreeElementUtility.ListToTree(treeElements);

            EditorUtility.ClearProgressBar();
            return treeElements;
        }

        private void addElement(List<AH_DepGraphElement> treeElements, Dictionary<string, List<string>> referenceDict, string assetGUID, ref int depth, ref int id, ref Stack<string> referenceStack)
        {
            var path      = AssetDatabase.GUIDToAssetPath(assetGUID);
            var pathSplit = path.Split('/');

            if (referenceStack.Contains(path)) return;

            depth++;

            treeElements.Add(new( /*path*/pathSplit.Last(), depth, id++, path));
            referenceStack.Push(path); //Add to stack to keep track of circular refs in branch

            var references = referenceDict.ContainsKey(assetGUID) ? referenceDict[assetGUID] : null;
            if (references != null)
                foreach (var item in references)
                    this.addElement(treeElements, referenceDict, item, ref depth, ref id, ref referenceStack);
            depth--;

            referenceStack.Pop();
        }

        public void SelectPreviousFromHistory()
        {
            this.selectionHistoryIndex--;
            this.SelectFromHistory(this.selectionHistoryIndex);
        }

        public void SelectNextFromHistory()
        {
            this.selectionHistoryIndex++;
            this.SelectFromHistory(this.selectionHistoryIndex);
        }

        private void SelectFromHistory(int index)
        {
            this.TraversingHistory = true;
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(this.selectionHistory[this.selectionHistoryIndex]);
        }

        internal void UpdateSelectedAsset(UnityEngine.Object activeObject)
        {
            var invalid = activeObject == null || AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));

            if (invalid)
            {
                this.selectedAssetGUID   = this.selectedAssetObjectName = string.Empty;
                this.selectedAssetObject = null;
            }
            else
            {
                this.selectedAssetGUID       = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(activeObject));
                this.selectedAssetObjectName = activeObject.name;
                this.selectedAssetObject     = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(this.selectedAssetGUID));

                if (!this.TraversingHistory) this.addToHistory();
            }

            this.TraversingHistory = false;
        }

        private void addToHistory()
        {
            //Remove the part of the history branch that are no longer needed
            if (this.selectionHistory.Count - 1 > this.selectionHistoryIndex) this.selectionHistory.RemoveRange(this.selectionHistoryIndex, this.selectionHistory.Count - this.selectionHistoryIndex);

            var path = AssetDatabase.GUIDToAssetPath(this.selectedAssetGUID);

            if (this.selectionHistory.Count == 0 || path != this.selectionHistory.Last())
            {
                this.selectionHistory.Add(AssetDatabase.GUIDToAssetPath(this.selectedAssetGUID));
                this.selectionHistoryIndex = this.selectionHistory.Count - 1;
            }
        }

        public void OnBeforeSerialize()
        {
            this._keysFrom.Clear();
            this._wrapperValuesFrom.Clear();

            foreach (var kvp in this.referencedFrom)
            {
                this._keysFrom.Add(kvp.Key);
                this._wrapperValuesFrom.Add(new(kvp.Value));
            }

            this._keysTo.Clear();
            this._wrapperValuesTo.Clear();

            foreach (var kvp in this.referenceTo)
            {
                this._keysTo.Add(kvp.Key);
                this._wrapperValuesTo.Add(new(kvp.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            this.referencedFrom = new();
            for (var i = 0; i != Math.Min(this._keysFrom.Count, this._wrapperValuesFrom.Count); i++) this.referencedFrom.Add(this._keysFrom[i], this._wrapperValuesFrom[i].list);

            this.referenceTo = new();
            for (var i = 0; i != Math.Min(this._keysTo.Count, this._wrapperValuesTo.Count); i++) this.referenceTo.Add(this._keysTo[i], this._wrapperValuesTo[i].list);
        }
    }
}