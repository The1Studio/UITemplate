using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildReportTool
{
    /// <summary>
    /// A collection of file entries in a build report.
    /// Used to display the "Used Assets" and the "Unused Assets".
    /// </summary>
    [Serializable]
    public class AssetList
    {
        // ==================================================================================

        [SerializeField] private SizePart[] _all;

        private int[] _viewOffsets;

        [SerializeField] private SizePart[][] _perCategory;

        [SerializeField] private string[] _labels;

        public SizePart[] All { get => this._all; set => this._all = value; }

        public SizePart[][] PerCategory => this._perCategory;

        public string[] Labels { get => this._labels; set => this._labels = value; }

        // ==================================================================================

        private SizePart[] _topLargest;

        public SizePart[] TopLargest => this._topLargest;

        public int NumberOfTopLargest
        {
            get
            {
                if (this._topLargest == null) return 0;

                return this._topLargest.Length;
            }
        }

        private void PostSetListAll(int numberOfTop)
        {
            var topLargestList = new List<SizePart>();

            // temporarily sort "All" list by raw size so we can get the top largest
            AssetListUtility.SortAssetList(this._all, SortType.RawSize, SortOrder.Descending);

            // in case entries in "all" list is lesser than the numberOfTop value
            var len = Mathf.Min(numberOfTop, this._all.Length);

            for (var n = 0; n < len; ++n) topLargestList.Add(this._all[n]);

            this._topLargest = topLargestList.ToArray();

            // revert "All" list to original sort type
            this.Resort(this._all);
        }

        public void ResortDefault(int numberOfTop)
        {
            this.PostSetListAll(numberOfTop);
        }

        // ==================================================================================
        // Sort Type

        public enum SortType
        {
            None,
            AssetFullPath,
            AssetFilename,
            RawSize,
            ImportedSize,

            /// <summary>
            /// Try imported size. If imported size is unavailable (N/A) use raw size.
            /// </summary>
            ImportedSizeOrRawSize,

            SizeBeforeBuild,
            PercentSize,

            TextureData,
            MeshData,
        }

        public enum SortOrder
        {
            None,
            Ascending,
            Descending,
        }

        private SortType           _lastSortType        = SortType.None;
        private TextureData.DataId _lastTextureSortType = TextureData.DataId.None;
        private MeshData.DataId    _lastMeshSortType    = MeshData.DataId.None;
        private SortOrder          _lastSortOrder       = SortOrder.None;

        public SortType LastSortType => this._lastSortType;

        public SortOrder LastSortOrder => this._lastSortOrder;

        private readonly HashSet<int> _hasListBeenSorted = new();

        public void Resort(SizePart[] assetList)
        {
            if (this._lastSortType != SortType.None && this._lastTextureSortType == TextureData.DataId.None && this._lastMeshSortType == MeshData.DataId.None && this._lastSortOrder != SortOrder.None) AssetListUtility.SortAssetList(assetList, this._lastSortType, this._lastSortOrder);
        }

        public void Sort(TextureData textureData, TextureData.DataId sortType, SortOrder sortOrder, FileFilterGroup fileFilters)
        {
            this._lastTextureSortType = sortType;
            this._lastMeshSortType    = MeshData.DataId.None;
            this._lastSortType        = SortType.TextureData;
            this._lastSortOrder       = sortOrder;

            this._hasListBeenSorted.Clear();

            this._hasListBeenSorted.Add(fileFilters.SelectedFilterIdx);

            // sort only currently displayed list
            if (fileFilters.SelectedFilterIdx == -1)
                AssetListUtility.SortAssetList(this._all, textureData, sortType, sortOrder);
            else
                AssetListUtility.SortAssetList(this._perCategory[fileFilters.SelectedFilterIdx], textureData, sortType, sortOrder);
        }

        public void Sort(MeshData meshData, MeshData.DataId sortType, SortOrder sortOrder, FileFilterGroup fileFilters)
        {
            this._lastTextureSortType = TextureData.DataId.None;
            this._lastMeshSortType    = sortType;
            this._lastSortType        = SortType.MeshData;
            this._lastSortOrder       = sortOrder;

            this._hasListBeenSorted.Clear();

            this._hasListBeenSorted.Add(fileFilters.SelectedFilterIdx);

            // sort only currently displayed list
            if (fileFilters.SelectedFilterIdx == -1)
                AssetListUtility.SortAssetList(this._all, meshData, sortType, sortOrder);
            else
                AssetListUtility.SortAssetList(this._perCategory[fileFilters.SelectedFilterIdx], meshData, sortType, sortOrder);
        }

        public void Sort(SortType sortType, SortOrder sortOrder, FileFilterGroup fileFilters)
        {
            this._lastTextureSortType = TextureData.DataId.None;
            this._lastMeshSortType    = MeshData.DataId.None;
            this._lastSortType        = sortType;
            this._lastSortOrder       = sortOrder;

            this._hasListBeenSorted.Clear();

            this._hasListBeenSorted.Add(fileFilters.SelectedFilterIdx);

            // sort only currently displayed list
            if (fileFilters.SelectedFilterIdx == -1)
                AssetListUtility.SortAssetList(this._all, sortType, sortOrder);
            else
                AssetListUtility.SortAssetList(this._perCategory[fileFilters.SelectedFilterIdx], sortType, sortOrder);

            //SortAssetList(_all, sortType, sortOrder);
            //for (int n = 0, len = _perCategory.Length; n < len; ++n)
            //{
            //	SortAssetList(_perCategory[n], sortType, sortOrder);
            //}
        }

        public void SortIfNeeded(FileFilterGroup fileFilters)
        {
            if (this._lastSortType != SortType.None && this._lastSortOrder != SortOrder.None && !this._hasListBeenSorted.Contains(fileFilters.SelectedFilterIdx))
            {
                if (fileFilters.SelectedFilterIdx == -1)
                {
                    if (this._lastSortType == SortType.TextureData)
                    {
                    }
                    else if (this._lastSortType == SortType.MeshData)
                    {
                    }
                    else
                    {
                        AssetListUtility.SortAssetList(this._all, this._lastSortType, this._lastSortOrder);
                    }
                }
                else
                {
                    if (this._lastSortType == SortType.TextureData)
                    {
                    }
                    else if (this._lastSortType == SortType.MeshData)
                    {
                    }
                    else
                    {
                        AssetListUtility.SortAssetList(this._perCategory[fileFilters.SelectedFilterIdx], this._lastSortType, this._lastSortOrder);
                    }
                }

                this._hasListBeenSorted.Add(fileFilters.SelectedFilterIdx);
            }
        }

        // Queries
        // ==================================================================================

        public List<SizePart> GetAllAsList()
        {
            return this._all.ToList();
        }

        public int AllCount => this._all.Length;

        public double GetTotalSizeInBytes()
        {
            double total = 0;

            for (int n = 0, len = this._all.Length; n < len; ++n)
                if (this._all[n].UsableSize > 0)
                    total += this._all[n].UsableSize;

            return total;
        }

        public int GetViewOffsetForDisplayedList(FileFilterGroup fileFilters)
        {
            if (this._viewOffsets == null || this._viewOffsets.Length == 0) return 0;

            if (fileFilters.SelectedFilterIdx == -1)
                return this._viewOffsets[0]; // _viewOffsets[0] is the "All" list
            else if (this.PerCategory != null && this.PerCategory.Length >= fileFilters.SelectedFilterIdx + 1) return this._viewOffsets[fileFilters.SelectedFilterIdx + 1];

            return 0;
        }

        public SizePart[] GetListToDisplay(FileFilterGroup fileFilters)
        {
            SizePart[] ret = null;
            if (fileFilters.SelectedFilterIdx == -1)
                ret                                                                                                = this.All;
            else if (this.PerCategory != null && this.PerCategory.Length >= fileFilters.SelectedFilterIdx + 1) ret = this.PerCategory[fileFilters.SelectedFilterIdx];

            return ret;
        }

        // Commands
        // ==================================================================================

        public void UnescapeAssetNames()
        {
            for (int n = 0, len = this._all.Length; n < len; ++n) this._all[n].Name = Util.MyHtmlDecode(this._all[n].Name);

            if (this._perCategory != null)
                for (int catIdx = 0, catLen = this._perCategory.Length; catIdx < catLen; ++catIdx)
                for (int n = 0, len = this._perCategory[catIdx].Length; n < len; ++n)
                    this._perCategory[catIdx][n].Name = Util.MyHtmlDecode(this._perCategory[catIdx][n].Name);
        }

        public void SetViewOffsetForDisplayedList(FileFilterGroup fileFilters, int newVal)
        {
            if (fileFilters.SelectedFilterIdx == -1)
                this._viewOffsets[0]                                                                                                                                = newVal; // _viewOffsets[0] is the "All" list
            else if (this.PerCategory != null && this.PerCategory.Length >= fileFilters.SelectedFilterIdx + 1) this._viewOffsets[fileFilters.SelectedFilterIdx + 1] = newVal;
        }

        public void PopulateRawSizes()
        {
            /*long importedSize = -1;
            for (int n = 0, len = _all.Length; n < len; ++n)
            {
                importedSize = BRT_LibCacheUtil.GetImportedFileSize(_all[n].Name);

                _all[n].SizeBytes = importedSize;
                _all[n].Size = BuildReportTool.Util.GetBytesReadable(importedSize);
            }*/
        }

        public void PopulateImportedSizes()
        {
            for (int n = 0, len = this._all.Length; n < len; ++n)
                /*if (BuildReportTool.Util.IsFileAUnityAsset(_all[n].Name))
                {
                    // Scene files/terrain files/scriptable object files/etc. always seem to be only 4kb in the library,
                    // no matter how large the actual file in the assets folder really is.
                    // The 4kb is probably just metadata/reference to the actual file itself.
                    // Makes sense since these file types are "native" to unity, so no importing is necessary.
                    //
                    // In this case, the raw size (size of the file in the assets folder) counts as the imported size
                    // so just use the raw size.

                    _all[n].ImportedSizeBytes = _all[n].RawSizeBytes;
                    _all[n].ImportedSize = _all[n].RawSize;
                }
                else*/
            {
                var importedSize = BRT_LibCacheUtil.GetImportedFileSize(this._all[n].Name);

                this._all[n].ImportedSizeBytes = importedSize;
                this._all[n].ImportedSize      = Util.GetBytesReadable(importedSize);
            }
        }

        public void PopulateSizeInAssetsFolder()
        {
            var projectPath = Util.GetProjectPath(Application.dataPath);
            for (int n = 0, len = this._all.Length; n < len; ++n)
            {
                var assetImportedPath = projectPath + Util.MyHtmlDecode(this._all[n].Name);

                var size = Util.GetFileSizeInBytes(assetImportedPath);
                this._all[n].SizeInAssetsFolderBytes = size;
                this._all[n].SizeInAssetsFolder      = Util.GetBytesReadable(size);
            }
        }

        public void RecalculatePercentages(double totalSize)
        {
            //Debug.Log("Recalculate Percentage Start");

            if (this._all != null)
            {
                // if the all list is available,
                // prefer using that to get the total size

                totalSize = 0;

                for (int n = 0, len = this._all.Length; n < len; ++n) totalSize += this._all[n].UsableSize;
            }

            if (this._all != null)
                for (int n = 0, len = this._all.Length; n < len; ++n)
                {
                    this._all[n].Percentage =
                        Math.Round(this._all[n].UsableSize / totalSize * 100, 2, MidpointRounding.AwayFromZero);
                }
            //Debug.Log("Percentage for: " + n + " " + _all[n].Name + " = " + _all[n].Percentage + " = " + _all[n].UsableSize + " / " + totalSize);
            if (this._perCategory != null)
                for (int catIdx = 0, catLen = this._perCategory.Length; catIdx < catLen; ++catIdx)
                for (int n = 0, len = this._perCategory[catIdx].Length; n < len; ++n)
                {
                    this._perCategory[catIdx][n].Percentage =
                        Math.Round(this._perCategory[catIdx][n].UsableSize / totalSize * 100,
                            2,
                            MidpointRounding.AwayFromZero);
                }

            //Debug.Log("Recalculate Percentage End");
        }

        // Commands: Initialization
        // ==================================================================================

        public void Init(
            SizePart[]      all,
            SizePart[][]    perCategory,
            int             numberOfTop,
            FileFilterGroup fileFilters
        )
        {
            this.All = all;
            this.PostSetListAll(numberOfTop);
            this._perCategory = perCategory;

            this._viewOffsets = new int[1 + this.PerCategory.Length]; // +1 since we need to include the "All" list

            if (this._lastSortType == SortType.None)
                // sort by raw size, descending, by default
                this.Sort(SortType.RawSize, SortOrder.Descending, fileFilters);
            else
                this.Sort(this._lastSortType, this._lastSortOrder, fileFilters);

            this.RefreshFilterLabels(fileFilters);
        }

        public void Init(
            SizePart[]      all,
            SizePart[][]    perCategory,
            int             numberOfTop,
            FileFilterGroup fileFilters,
            SortType        newSortType,
            SortOrder       newSortOrder
        )
        {
            this._lastSortType  = newSortType;
            this._lastSortOrder = newSortOrder;

            this.Init(all, perCategory, numberOfTop, fileFilters);
        }

        public void Reinit(SizePart[] all, SizePart[][] perCategory, int numberOfTop)
        {
            this.All = all;
            this.PostSetListAll(numberOfTop);
            this._perCategory = perCategory;
        }

        public void AssignPerCategoryList(SizePart[][] perCategory)
        {
            this._perCategory = perCategory;
            this._viewOffsets = new int[1 + this._perCategory.Length]; // +1 since we need to include the "All" list
        }

        public void RefreshFilterLabels(FileFilterGroup fileFiltersToUse)
        {
            this._labels    = new string[1 + this.PerCategory.Length];
            this._labels[0] = string.Format("All ({0})", this.All.Length.ToString());
            for (int n = 0, len = fileFiltersToUse.Count; n < len; ++n) this._labels[n + 1] = string.Format("{0} ({1})", fileFiltersToUse[n].Label, this.PerCategory[n].Length.ToString());

            this._labels[this._labels.Length - 1] = string.Format("Unknown ({0})", this.PerCategory[this.PerCategory.Length - 1].Length.ToString());
        }

        // Sum Selection
        // ==================================================================================

        [SerializeField] private Dictionary<string, SizePart> _selectedForSum = new();

        private SizePart _lastSelected;

        // Sum Selection: Queries
        // --------------------------------------------------------------------

        public bool InSumSelection(SizePart b)
        {
            return this._selectedForSum.ContainsKey(b.Name);
        }

        private double GetSizeOfSumSelection()
        {
            double total = 0;
            foreach (var pair in this._selectedForSum)
                if (pair.Value.UsableSize > 0)
                    total += pair.Value.UsableSize;

            return total;
        }

        public double GetPercentageOfSumSelection()
        {
            double total = 0;
            foreach (var pair in this._selectedForSum)
            {
                if (pair.Value.Percentage > 0)
                    if (pair.Value.Percentage > 0)
                        total += pair.Value.Percentage;
            }

            return total;
        }

        public string GetReadableSizeOfSumSelection()
        {
            return Util.GetBytesReadable(this.GetSizeOfSumSelection());
        }

        public bool AtLeastOneSelectedForSum => this._selectedForSum.Count > 0;

        public bool IsNothingSelected => this._selectedForSum.Count <= 0;

        public Dictionary<string, SizePart>.Enumerator GetSelectedEnumerator()
        {
            return this._selectedForSum.GetEnumerator();
        }

        public int GetSelectedCount()
        {
            return this._selectedForSum.Count;
        }

        public SizePart GetLastSelected()
        {
            return this._lastSelected;
        }

        // Sum Selection: Commands
        // --------------------------------------------------------------------

        public void ToggleSumSelection(SizePart b)
        {
            if (this.InSumSelection(b))
                this.RemoveFromSumSelection(b);
            else
                this.AddToSumSelection(b);
        }

        public void RemoveFromSumSelection(SizePart b)
        {
            this._selectedForSum.Remove(b.Name);
        }

        public void AddToSumSelection(SizePart b)
        {
            if (this._selectedForSum.ContainsKey(b.Name))
                // already added
                return;

            this._selectedForSum.Add(b.Name, b);

            this._lastSelected = b;
        }

        public void AddDisplayedRangeToSumSelection(FileFilterGroup fileFilters, int offset, int range)
        {
            var listForSelection = this.GetListToDisplay(fileFilters);

            for (var n = offset; n < offset + range; ++n)
                if (!this.InSumSelection(listForSelection[n]))
                    this.AddToSumSelection(listForSelection[n]);
        }

        public void AddAllDisplayedToSumSelection(FileFilterGroup fileFilters)
        {
            var listForSelection = this.GetListToDisplay(fileFilters);

            for (var n = 0; n < listForSelection.Length; ++n)
                if (!this.InSumSelection(listForSelection[n]))
                    this.AddToSumSelection(listForSelection[n]);
        }

        public void ClearSelection()
        {
            this._selectedForSum.Clear();
        }
    }
} // namespace BuildReportTool