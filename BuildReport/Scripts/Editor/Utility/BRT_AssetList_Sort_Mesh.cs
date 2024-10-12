using System;

namespace BuildReportTool
{
    public static partial class AssetListUtility
    {
        public static void SortAssetList(SizePart[] assetList, MeshData meshData, MeshData.DataId meshSortType, AssetList.SortOrder sortOrder)
        {
            switch (meshSortType)
            {
                case MeshData.DataId.MeshFilterCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.MeshFilterCount);
                    break;
                case MeshData.DataId.SkinnedMeshRendererCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.SkinnedMeshRendererCount);
                    break;
                case MeshData.DataId.SubMeshCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.SubMeshCount);
                    break;
                case MeshData.DataId.VertexCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.VertexCount);
                    break;
                case MeshData.DataId.TriangleCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.TriangleCount);
                    break;
                case MeshData.DataId.AnimationType:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.AnimationType);
                    break;
                case MeshData.DataId.AnimationClipCount:
                    SortMeshData(assetList, meshData, sortOrder, entry => entry.AnimationClipCount);
                    break;
            }
        }

        private static void SortMeshData(SizePart[] assetList, MeshData meshData, AssetList.SortOrder sortOrder, Func<MeshData.Entry, int> func)
        {
            var meshEntries = meshData.GetMeshData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                var intValue                                             = 0;
                if (meshEntries.ContainsKey(assetList[n].Name)) intValue = func(meshEntries[assetList[n].Name]);

                assetList[n].SetIntAuxData(intValue);
            }

            SortByInt(assetList, sortOrder);
        }

        private static void SortMeshData(SizePart[] assetList, MeshData meshData, AssetList.SortOrder sortOrder, Func<MeshData.Entry, string> func)
        {
            var meshEntries = meshData.GetMeshData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                string textData                                          = null;
                if (meshEntries.ContainsKey(assetList[n].Name)) textData = func(meshEntries[assetList[n].Name]);

                assetList[n].SetTextAuxData(textData);
            }

            SortByText(assetList, sortOrder);
        }
    }
}