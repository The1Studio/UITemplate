using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_BuildInfoManager : ScriptableObject
    {
        public delegate void BuildInfoSelectionChangedDelegate();

        public BuildInfoSelectionChangedDelegate OnBuildInfoSelectionChanged;

        [SerializeField] private bool                   hasTreeviewSelection = false;
        [SerializeField] private string                 chosenFilePath;
        [SerializeField] private AH_SerializedBuildInfo chosenBuildInfo;
        [SerializeField] private AH_BuildInfoTreeView   treeView;
        [SerializeField] private bool                   projectDirty;
        [SerializeField] private bool                   projectIsClean;

        public AH_BuildInfoTreeView TreeView => this.treeView;

        public bool HasSelection => this.hasTreeviewSelection;

        public bool ProjectDirty { get => this.projectDirty; set => this.projectDirty = value; }

        public void OnEnable()
        {
            this.hideFlags = HideFlags.HideAndDontSave;
        }

        private void UpdateBuildInfoFilePaths()
        {
            //Create new folder if needed
            Directory.CreateDirectory(AH_SerializationHelper.GetBuildInfoFolder());
        }

        public IList<AH_TreeviewElement> GetTreeViewData()
        {
            if (this.treeView != null && this.treeView.treeElements != null && this.treeView.treeElements.Count > 0)
                return this.treeView.treeElements;
            else
                Debug.LogError("Missing Data!!!");

            return null;
        }

        internal void SelectBuildInfo(string filePath)
        {
            this.hasTreeviewSelection = false;
            this.chosenFilePath       = filePath;
            this.chosenBuildInfo      = AH_SerializationHelper.LoadBuildReport(filePath);

            Version reportVersion;
            if (Version.TryParse(this.chosenBuildInfo.versionNumber, out reportVersion))
                if (reportVersion.CompareTo(new("2.2.0")) < 0) //If report is older than 2.2.0 (tweaked datamodel in 2.2.0 from saving 'Paths' to saving 'guids')
                    //Change paths to guids
                    foreach (var item in this.chosenBuildInfo.AssetListUnSorted)
                        item.ChangePathToGUID();

            if (this.chosenBuildInfo == null) return;

            //Make sure JSON is valid
            if (this.populateBuildReport())
            {
                this.hasTreeviewSelection = true;

                if (this.OnBuildInfoSelectionChanged != null) this.OnBuildInfoSelectionChanged();
            }
            else if (!this.projectIsClean)
            {
                EditorUtility.DisplayDialog(
                    "JSON Parse error",
                    "The selected file could not be parsed",
                    "Ok");
            }
        }

        private bool populateBuildReport()
        {
            this.treeView = CreateInstance<AH_BuildInfoTreeView>();
            var success = this.treeView.PopulateTreeView(this.chosenBuildInfo);

            this.projectIsClean = !this.treeView.HasUnused();

            return success;
        }

        internal bool IsMergedReport()
        {
            return this.chosenBuildInfo.IsMergedReport();
        }

        internal void RefreshBuildInfo()
        {
            this.SelectBuildInfo(this.chosenFilePath);
        }

        internal string GetSelectedBuildSize()
        {
            return AH_Utils.GetSizeAsString((long)this.chosenBuildInfo.TotalSize);
        }

        internal string GetSelectedBuildDate()
        {
            return this.chosenBuildInfo.dateTime;
        }

        internal string GetSelectedBuildTarget()
        {
            return this.chosenBuildInfo.buildTargetInfo;
        }

        //Only avaliable in 2018
        #if UNITY_2018_1_OR_NEWER
        internal List<AH_BuildReportFileInfo> GetReportInfoInfo()
        {
            return this.chosenBuildInfo.BuildReportInfoList;
        }
        #endif

        internal AH_SerializedBuildInfo GetSerializedBuildInfo()
        {
            return this.chosenBuildInfo;
        }

        internal bool IsProjectClean()
        {
            return this.projectIsClean;
        }
    }
}