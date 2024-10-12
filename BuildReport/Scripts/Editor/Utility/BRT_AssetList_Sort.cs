using System;

namespace BuildReportTool
{
    public static partial class AssetListUtility
    {
        public static void SortAssetList(SizePart[] assetList, AssetList.SortType sortType, AssetList.SortOrder sortOrder)
        {
            switch (sortType)
            {
                case AssetList.SortType.RawSize:
                    SortRawSize(assetList, sortOrder);
                    break;
                case AssetList.SortType.ImportedSize:
                    SortImportedSize(assetList, sortOrder);
                    break;
                case AssetList.SortType.ImportedSizeOrRawSize:
                    SortImportedSizeOrRawSize(assetList, sortOrder);
                    break;
                case AssetList.SortType.SizeBeforeBuild:
                    SortSizeBeforeBuild(assetList, sortOrder);
                    break;
                case AssetList.SortType.PercentSize:
                    SortPercentSize(assetList, sortOrder);
                    break;
                case AssetList.SortType.AssetFullPath:
                    SortAssetFullPath(assetList, sortOrder);
                    break;
                case AssetList.SortType.AssetFilename:
                    SortAssetName(assetList, sortOrder);
                    break;
            }
        }

        private static void SortRawSize(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.UsableSize > entry2.UsableSize) return -1;
                        if (entry1.UsableSize < entry2.UsableSize) return 1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameDescending(entry1, entry2);
                    });
            else
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.UsableSize > entry2.UsableSize) return 1;
                        if (entry1.UsableSize < entry2.UsableSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortImportedSizeOrRawSize(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.ImportedSizeOrRawSize > entry2.ImportedSizeOrRawSize) return -1;
                        if (entry1.ImportedSizeOrRawSize < entry2.ImportedSizeOrRawSize) return 1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameDescending(entry1, entry2);
                    });
            else
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.ImportedSizeOrRawSize > entry2.ImportedSizeOrRawSize) return 1;
                        if (entry1.ImportedSizeOrRawSize < entry2.ImportedSizeOrRawSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortImportedSize(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.ImportedSizeBytes > entry2.ImportedSizeBytes) return -1;
                        if (entry1.ImportedSizeBytes < entry2.ImportedSizeBytes) return 1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameDescending(entry1, entry2);
                    });
            else
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.ImportedSizeBytes > entry2.ImportedSizeBytes) return 1;
                        if (entry1.ImportedSizeBytes < entry2.ImportedSizeBytes) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortSizeBeforeBuild(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.SizeInAssetsFolderBytes > entry2.SizeInAssetsFolderBytes) return -1;
                        if (entry1.SizeInAssetsFolderBytes < entry2.SizeInAssetsFolderBytes) return 1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameDescending(entry1, entry2);
                    });
            else
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.SizeInAssetsFolderBytes > entry2.SizeInAssetsFolderBytes) return 1;
                        if (entry1.SizeInAssetsFolderBytes < entry2.SizeInAssetsFolderBytes) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortPercentSize(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.Percentage > entry2.Percentage) return -1;
                        if (entry1.Percentage < entry2.Percentage) return 1;

                        // same percent
                        // sort by asset name for assets with same percent
                        return SortByAssetFullPathDescending(entry1, entry2);
                    });
            else
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        if (entry1.Percentage > entry2.Percentage) return 1;
                        if (entry1.Percentage < entry2.Percentage) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetFullPathAscending(entry1, entry2);
                    });
        }

        private static void SortAssetFullPath(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList, SortByAssetFullPathDescending);
            else
                Array.Sort(assetList, SortByAssetFullPathAscending);
        }

        private static int SortByAssetFullPathDescending(SizePart entry1, SizePart entry2)
        {
            var result = string.Compare(entry1.Name, entry2.Name, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        private static int SortByAssetFullPathAscending(SizePart entry1, SizePart entry2)
        {
            var result = string.Compare(entry1.Name, entry2.Name, StringComparison.OrdinalIgnoreCase);

            // invert the result
            if (result == 1) return -1;
            if (result == -1) return 1;
            return 0;
        }

        private static void SortAssetName(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList, SortByAssetNameDescending);
            else
                Array.Sort(assetList, SortByAssetNameAscending);
        }

        private static int SortByAssetNameDescending(SizePart entry1, SizePart entry2)
        {
            var result = string.Compare(entry1.Name.GetFileNameOnly(),
                entry2.Name.GetFileNameOnly(),
                StringComparison.OrdinalIgnoreCase);

            return result;
        }

        private static int SortByAssetNameAscending(SizePart entry1, SizePart entry2)
        {
            var result = string.Compare(entry1.Name.GetFileNameOnly(),
                entry2.Name.GetFileNameOnly(),
                StringComparison.OrdinalIgnoreCase);

            // invert the result
            if (result == 1) return -1;
            if (result == -1) return 1;

            return 0;
        }
    }
}