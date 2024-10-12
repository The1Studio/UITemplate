#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	#define UNITY_5_2_AND_LESSER
#endif

using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace BuildReportTool
{
    public partial class BuildInfo
    {
        public struct SceneInBuild
        {
            public bool   Enabled;
            public string Path;
        }

        // Queries
        // ==================================================================================

        public bool HasContents =>
            // build sizes can't be empty (they are always there when you build)
            !string.IsNullOrEmpty(this.ProjectName) && this.BuildSizes != null && this.BuildSizes.Length > 0;

        public bool IsUnityVersionAtLeast(int majorAtLeast, int minorAtLeast, int patchAtLeast)
        {
            return DldUtil.UnityVersion.IsUnityVersionAtLeast(this.UnityVersion, majorAtLeast, minorAtLeast, patchAtLeast);
        }

        public bool IsUnityVersionAtMost(int majorAtMost, int minorAtMost, int patchAtMost)
        {
            return DldUtil.UnityVersion.IsUnityVersionAtMost(this.UnityVersion, majorAtMost, minorAtMost, patchAtMost);
        }

        public string SuitableTitle
        {
            get
            {
                if (this.UnityBuildSettings != null && this.UnityBuildSettings.HasValues && !string.IsNullOrEmpty(this.UnityBuildSettings.ProductName)) return this.UnityBuildSettings.ProductName;

                return this.ProjectName;
            }
        }

        public string GetTimeReadable()
        {
            if (!string.IsNullOrEmpty(this.BuildTimeGotReadable)) return this.BuildTimeGotReadable;

            if (!string.IsNullOrEmpty(this.TimeGotReadable)) return this.TimeGotReadable;

            return this.TimeGot.ToString(ReportGenerator.TIME_OF_BUILD_FORMAT);
        }

        public string UnityVersionDisplayed
        {
            get
            {
                if (this.UnityBuildSettings != null && this.UnityBuildSettings.HasValues) return this.UnityVersion + (this.UnityBuildSettings.UsingAdvancedLicense ? " Pro" : "");

                return this.UnityVersion;
            }
        }

        public string GetDefaultFilename()
        {
            return Util.GetBuildInfoDefaultFilename(this.ProjectName, this.BuildType, this.BuildTimeGot);
        }

        public string GetAccompanyingAssetDependenciesFilename()
        {
            return Util.GetAssetDependenciesDefaultFilename(this.ProjectName, this.BuildType, this.BuildTimeGot);
        }

        // old size values were only TotalBuildSize and CompressedBuildSize
        public bool HasOldSizeValues =>
            string.IsNullOrEmpty(this.UnusedTotalSize) && string.IsNullOrEmpty(this.UsedTotalSize) && string.IsNullOrEmpty(this.StreamingAssetsSize) && string.IsNullOrEmpty(this.WebFileBuildSize) && string.IsNullOrEmpty(this.AndroidApkFileBuildSize) && string.IsNullOrEmpty(this.AndroidObbFileBuildSize);

        public bool HasUsedAssets => this.UsedAssets != null;

        public bool HasUnusedAssets => this.UnusedAssets != null;

        public bool HasStreamingAssets => this.StreamingAssetsSize != "0 B";

        // Commands
        // ==================================================================================

        public void UnescapeAssetNames()
        {
            if (this.UsedAssets != null) this.UsedAssets.UnescapeAssetNames();

            if (this.UnusedAssets != null) this.UnusedAssets.UnescapeAssetNames();
        }

        public void RecategorizeAssetLists()
        {
            var fileFiltersToUse = this.FileFilters;

            if (Options.ShouldUseConfiguredFileFilters()) fileFiltersToUse = FiltersUsed.GetProperFileFilterGroupToUse();
            //Debug.Log("going to use configured file filters instead... loaded: " + (fileFiltersToUse != null));
            if (this.UsedAssets != null)
            {
                this.UsedAssets.AssignPerCategoryList(
                    ReportGenerator.SegregateAssetSizesPerCategory(this.UsedAssets.All, fileFiltersToUse));

                this.UsedAssets.RefreshFilterLabels(fileFiltersToUse);

                this.UsedAssets.ResortDefault(Options.NumberOfTopLargestUsedAssetsToShow);
            }

            if (this.UnusedAssets != null)
            {
                this.UnusedAssets.AssignPerCategoryList(
                    ReportGenerator.SegregateAssetSizesPerCategory(this.UnusedAssets.All, fileFiltersToUse));

                this.UnusedAssets.RefreshFilterLabels(fileFiltersToUse);

                this.UnusedAssets.ResortDefault(Options.NumberOfTopLargestUnusedAssetsToShow);
            }
        }

        public void RecategorizeUsedAssets()
        {
            if (this.UsedAssets == null) return;

            var fileFiltersToUse = this.FileFilters;

            if (Options.ShouldUseConfiguredFileFilters()) fileFiltersToUse = FiltersUsed.GetProperFileFilterGroupToUse();
            //Debug.Log("going to use configured file filters instead... loaded: " + (fileFiltersToUse != null));
            this.UsedAssets.AssignPerCategoryList(
                ReportGenerator.SegregateAssetSizesPerCategory(this.UsedAssets.All, fileFiltersToUse));

            this.UsedAssets.RefreshFilterLabels(fileFiltersToUse);

            this.UsedAssets.ResortDefault(Options.NumberOfTopLargestUsedAssetsToShow);
        }

        public void RecategorizeUnusedAssets()
        {
            if (this.UnusedAssets == null) return;

            var fileFiltersToUse = this.FileFilters;

            if (Options.ShouldUseConfiguredFileFilters()) fileFiltersToUse = FiltersUsed.GetProperFileFilterGroupToUse();
            //Debug.Log("going to use configured file filters instead... loaded: " + (fileFiltersToUse != null));
            this.UnusedAssets.AssignPerCategoryList(
                ReportGenerator.SegregateAssetSizesPerCategory(this.UnusedAssets.All, fileFiltersToUse));

            this.UnusedAssets.RefreshFilterLabels(fileFiltersToUse);

            this.UnusedAssets.ResortDefault(Options.NumberOfTopLargestUnusedAssetsToShow);
        }

        private void CalculateUsedAssetsDerivedSizes()
        {
            if (this.UsedAssets != null)
                for (int n = 0, len = this.UsedAssets.All.Length; n < len; ++n)
                    this.UsedAssets.All[n].DerivedSize = Util.GetApproxSizeFromString(this.UsedAssets.All[n].Size);
        }

        public void SortSizes()
        {
            System.Array.Sort(this.BuildSizes,
                delegate(SizePart b1, SizePart b2)
                {
                    if (b1.Percentage > b2.Percentage)
                        return -1;
                    else if (b1.Percentage < b2.Percentage)
                        return 1;
                    // if percentages are equal, check actual file size (approximate values)
                    else if (b1.DerivedSize > b2.DerivedSize)
                        return -1;
                    else if (b1.DerivedSize < b2.DerivedSize) return 1;
                    return 0;
                });
        }

        /// <summary>
        /// This is called right after generating a build report.
        /// </summary>
        public void FixReport()
        {
            #if UNITY_5_2_AND_LESSER
			// this bug has already been fixed since Unity 5.2.1
			// so we only execute this for Unity 5.2.0 and below

			if (!DldUtil.UnityVersion.IsUnityVersionAtLeast(5, 2, 1))
			{
				// --------------------------------------------------------------------------------
				// fix imported sizes of Resources files

				for (int n = 0; n < UsedAssets.All.Length; ++n)
				{
					if (BuildReportTool.Util.IsFileInAPath(UsedAssets.All[n].Name, "/Resources/"))
					{
						UsedAssets.All[n].ImportedSizeBytes = BRT_LibCacheUtil.GetImportedFileSize(UsedAssets.All[n].Name);
						UsedAssets.All[n].ImportedSize =
							BuildReportTool.Util.GetBytesReadable(UsedAssets.All[n].ImportedSizeBytes);

						UsedAssets.All[n].RawSizeBytes = UsedAssets.All[n].ImportedSizeBytes;
						UsedAssets.All[n].RawSize = UsedAssets.All[n].ImportedSize;

						UsedAssets.All[n].DerivedSize = 0;
						UsedAssets.All[n].Percentage = -1;
					}
				}

				UsedAssets.ResortDefault(BuildReportTool.Options.NumberOfTopLargestUsedAssetsToShow);

				// --------------------------------------------------------------------------------
				// recalculate percentages

				var totalSizePart = BuildSizes.FirstOrDefault(part => part.IsTotal);
				if (totalSizePart != null && System.Math.Abs(totalSizePart.DerivedSize) < 0.01)
				{
					var totalSize = GetTotalSize();
					ChangeTotalSize(totalSize);
				}

				// add textures, meshes, sounds, and animations that are in resources folder to the build size
				// since they are not included anymore in Unity 5

				var resourcesTextureSizeSum =
					GetSizeSumForUsedAssets("/Resources/", BuildReportTool.Util.IsTextureFile);
				AddToSize("Textures", resourcesTextureSizeSum);

				var resourcesMeshSizeSum = GetSizeSumForUsedAssets("/Resources/", BuildReportTool.Util.IsMeshFile);
				AddToSize("Meshes", resourcesMeshSizeSum);

				var resourcesSoundsSizeSum = GetSizeSumForUsedAssets("/Resources/", BuildReportTool.Util.IsSoundFile);
				AddToSize("Sounds", resourcesSoundsSizeSum);

				var resourcesAnimationsSizeSum =
					GetSizeSumForUsedAssets("/Resources/", BuildReportTool.Util.IsAnimationFile);
				AddToSize("Animations", resourcesAnimationsSizeSum);

				AddToTotalSize(resourcesTextureSizeSum);
				AddToTotalSize(resourcesMeshSizeSum);
				AddToTotalSize(resourcesSoundsSizeSum);
				AddToTotalSize(resourcesAnimationsSizeSum);

				RecalculatePercentages();

				// sort sizes again since we modified them
				SortSizes();
			}
            #else
            // newer versions of Unity (2017 and up)
            // has a bug where the total size reported is actually the final build's size,
            // instead of the total of the sizes indicated in the build log.
            // so we recalculate the percentages.
            // this is most noticeable when the percentages
            // indicated don't really total up to 100, not even close to 90
            this.RecalculatePercentages();

            // sort sizes again since we modified them
            this.SortSizes();
            #endif
        }

        // Events
        // ==================================================================================

        public void OnBeforeSave()
        {
        }

        public void OnAfterLoad()
        {
            if (this.HasContents)
            {
                this.CalculateUsedAssetsDerivedSizes();
                this.UnescapeAssetNames();
                this.RecategorizeAssetLists();
            }
        }

        // Helper methods
        // ==================================================================================

        private double GetTotalSize()
        {
            if (this.BuildSizes == null) return 0;

            double totalSize = 0;
            for (int n = 0, len = this.BuildSizes.Length; n < len; ++n)
                if (!this.BuildSizes[n].IsTotal)
                    totalSize += this.BuildSizes[n].DerivedSize;

            return totalSize;
        }

        private void RecalculatePercentages()
        {
            //Debug.Log("RecalculatePercentages() called");

            var totalSize = this.GetTotalSize();

            //Debug.LogFormat("BuildSizes total: {0}", totalSize);

            for (int n = 0, len = this.BuildSizes.Length; n < len; ++n)
            {
                this.BuildSizes[n].Percentage = System.Math.Round(this.BuildSizes[n].UsableSize / totalSize * 100,
                    2,
                    System.MidpointRounding.AwayFromZero);
            }

            // note: only Used Assets are shown the percentages so we
            // don't bother recalculating percentage for Unused Assets
            if (this.UsedAssets != null) this.UsedAssets.RecalculatePercentages(totalSize);

            this.ChangeTotalSize(totalSize);
        }

        private long GetSizeSumForUsedAssets(string assetFolderName, System.Func<string, bool> fileTypePredicate)
        {
            if (this.UsedAssets == null || this.UsedAssets.All == null) return 0;

            return this.UsedAssets.All.Where(part =>
                    Util.IsFileInAPath(part.Name, assetFolderName) && fileTypePredicate(part.Name))
                .Sum(part => BRT_LibCacheUtil.GetImportedFileSize(part.Name));
        }

        private static void AddToSize(SizePart buildSize, long sizeToAdd)
        {
            if (buildSize != null)
            {
                buildSize.DerivedSize += sizeToAdd;
                buildSize.Size        =  Util.GetBytesReadable(buildSize.DerivedSize);
            }
        }

        private void AddToSize(string buildSizeName, long sizeToAdd)
        {
            if (sizeToAdd == 0) return;

            var buildSize = this.BuildSizes.FirstOrDefault(part => part.Name == buildSizeName);

            if (buildSize != null)
                //Debug.LogFormat("{0} size before: {1}", buildSizeName, buildSize.DerivedSize);
                AddToSize(buildSize, sizeToAdd);
            //Debug.LogFormat("{0} size after: {1}", buildSizeName, buildSize.DerivedSize);
        }

        private void AddToTotalSize(long sizeToAdd)
        {
            if (sizeToAdd == 0) return;

            var buildSize = this.BuildSizes.FirstOrDefault(part => part.IsTotal);

            if (buildSize != null)
            {
                //Debug.LogFormat("total size before: {0}", buildSize.DerivedSize);

                AddToSize(buildSize, sizeToAdd);

                this.UsedTotalSize = buildSize.Size;

                //Debug.LogFormat("total size after: {0}", buildSize.DerivedSize);
            }
        }

        private void ChangeTotalSize(double newSize)
        {
            if (System.Math.Abs(newSize) < 0.01)
                // disallow zero total size
                return;

            var totalSize = this.BuildSizes.FirstOrDefault(part => part.IsTotal);

            if (totalSize != null)
            {
                //Debug.LogFormat("total size before: {0}", totalSize.DerivedSize);

                totalSize.DerivedSize = newSize;
                totalSize.Size        = Util.GetBytesReadable(totalSize.DerivedSize);

                this.UsedTotalSize = totalSize.Size;

                //Debug.LogFormat("total size after: {0}", totalSize.DerivedSize);
            }
        }

        // temp variables that are not serialized into the XML file
        // ==================================================================================

        // only used while generating the build report or opening one

        /// <summary>
        /// Needed for ParseDLLs
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public ApiCompatibilityLevel MonoLevel;

        /// <summary>
        /// Needed for ParseDLLs
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StrippingLevel CodeStrippingLevel;

        public void SetScenes(EditorBuildSettingsScene[] newScenes)
        {
            var len = newScenes.Length;
            this.ScenesInBuild = new SceneInBuild[len];
            for (var n = 0; n < len; ++n)
            {
                this.ScenesInBuild[n].Enabled = newScenes[n].enabled;
                this.ScenesInBuild[n].Path    = newScenes[n].path;
            }
        }

        public void SetScenes(string[] newScenes)
        {
            var len = newScenes.Length;
            this.ScenesInBuild = new SceneInBuild[len];
            for (var n = 0; n < len; ++n)
            {
                this.ScenesInBuild[n].Enabled = true;
                this.ScenesInBuild[n].Path    = newScenes[n];
            }
        }

        private int _unusedAssetsBatchIdx;

        public int UnusedAssetsBatchIdx => this._unusedAssetsBatchIdx;

        /// <summary>
        /// Last asset number that each batch displays.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public List<int> UnusedAssetsBatchFinalNum = new();

        public void ResetUnusedAssetsBatchData()
        {
            this._unusedAssetsBatchIdx = 0;
            this.UnusedAssetsBatchFinalNum.Clear();
        }

        public void MoveUnusedAssetsBatchNumToNext()
        {
            ++this._unusedAssetsBatchIdx;
        }

        public void MoveUnusedAssetsBatchNumToPrev()
        {
            if (this._unusedAssetsBatchIdx == 0) return;

            --this._unusedAssetsBatchIdx;
        }

        // ---------------------------------------

        /// <summary>
        /// Full path where this Build Report is saved in the local storage.
        /// </summary>
        private string _savedPath;

        /// <inheritdoc cref="_savedPath"/>
        public string SavedPath => this._savedPath;

        public void SetSavedPath(string val)
        {
            this._savedPath = val.Replace("\\", "/");
        }

        // ---------------------------------------

        private BuildTarget _buildTarget;

        public BuildTarget BuildTargetUsed => this._buildTarget;

        public void SetBuildTargetUsed(BuildTarget val)
        {
            this._buildTarget = val;
        }

        // ---------------------------------------

        private bool _refreshRequest;

        public void FlagOkToRefresh()
        {
            this._refreshRequest = true;
        }

        public void FlagFinishedRefreshing()
        {
            this._refreshRequest = false;
        }

        public bool RequestedToRefresh => this._refreshRequest;
    }
}