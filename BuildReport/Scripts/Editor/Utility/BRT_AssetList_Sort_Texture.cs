using System;

namespace BuildReportTool
{
    public static partial class AssetListUtility
    {
        public static void SortAssetList(SizePart[] assetList, TextureData textureData, TextureData.DataId textureSortType, AssetList.SortOrder sortOrder)
        {
            switch (textureSortType)
            {
                case TextureData.DataId.TextureType:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.TextureType);
                    break;
                case TextureData.DataId.IsSRGB:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.IsSRGB);
                    break;
                case TextureData.DataId.AlphaSource:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.AlphaSource);
                    break;
                case TextureData.DataId.AlphaIsTransparency:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.AlphaIsTransparency);
                    break;
                case TextureData.DataId.IgnorePngGamma:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.IgnorePngGamma);
                    break;
                case TextureData.DataId.NPotScale:
                    SortNPotScale(assetList, textureData, sortOrder);
                    break;
                case TextureData.DataId.IsReadable:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.IsReadable);
                    break;
                case TextureData.DataId.MipMapGenerated:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.MipMapGenerated);
                    break;
                case TextureData.DataId.MipMapFilter:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.MipMapFilter);
                    break;
                case TextureData.DataId.StreamingMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.StreamingMipMaps);
                    break;
                case TextureData.DataId.BorderMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.BorderMipMaps);
                    break;
                case TextureData.DataId.PreserveCoverageMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.PreserveCoverageMipMaps);
                    break;
                case TextureData.DataId.FadeOutMipMaps:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.FadeOutMipMaps);
                    break;
                case TextureData.DataId.SpriteImportMode:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.SpriteImportMode);
                    break;
                case TextureData.DataId.SpritePackingTag:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.SpritePackingTag);
                    break;
                case TextureData.DataId.SpritePixelsPerUnit:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.SpritePixelsPerUnit);
                    break;
                case TextureData.DataId.QualifiesForSpritePacking:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.QualifiesForSpritePacking);
                    break;
                case TextureData.DataId.WrapMode:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapMode);
                    break;
                case TextureData.DataId.WrapModeU:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeU);
                    break;
                case TextureData.DataId.WrapModeV:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeV);
                    break;
                case TextureData.DataId.WrapModeW:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.WrapModeW);
                    break;
                case TextureData.DataId.FilterMode:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.FilterMode);
                    break;
                case TextureData.DataId.AnisoLevel:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.AnisoLevel);
                    break;
                case TextureData.DataId.MaxTextureSize:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownMaxTextureSize());
                    break;
                case TextureData.DataId.TextureResizeAlgorithm:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownTextureResizeAlgorithm());
                    break;
                case TextureData.DataId.TextureFormat:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownTextureFormat());
                    break;
                case TextureData.DataId.CompressionType:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionType());
                    break;
                case TextureData.DataId.CompressionIsCrunched:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionIsCrunched());
                    break;
                case TextureData.DataId.CompressionQuality:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetShownCompressionQuality());
                    break;
                case TextureData.DataId.ImportedWidthAndHeight:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetImportedPixelCount());
                    break;
                case TextureData.DataId.RealWidthAndHeight:
                    SortTextureData(assetList, textureData, sortOrder, entry => entry.GetRealPixelCount());
                    break;
            }
        }

        private static int CompareNPotScale(string nPotScale1, string nPotScale2)
        {
            var nPotScale1IsNoneNot = nPotScale1 == TextureData.NPOT_SCALE_NONE_NOT_POT;
            var nPotScale2IsNoneNot = nPotScale1 == TextureData.NPOT_SCALE_NONE_NOT_POT;

            if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot) return -1;
            if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot) return 1;

            return string.Compare(nPotScale1, nPotScale2, StringComparison.Ordinal);
        }

        // =============================================================================================================

        private static void SortByInt(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        var sortResult = entry2.GetIntAuxData().CompareTo(entry1.GetIntAuxData());
                        if (sortResult != 0) return sortResult;

                        // same texture data
                        // sort by asset size for assets with texture data
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
                        var sortResult = entry1.GetIntAuxData().CompareTo(entry2.GetIntAuxData());
                        if (sortResult != 0) return sortResult;

                        // same texture data
                        // sort by asset size for assets with same texture data
                        if (entry1.UsableSize > entry2.UsableSize) return 1;
                        if (entry1.UsableSize < entry2.UsableSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortByFloat(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        var sortResult = entry1.GetFloatAuxData().CompareTo(entry2.GetFloatAuxData());
                        if (sortResult != 0) return sortResult;

                        // same texture data
                        // sort by asset size for assets with texture data
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
                        var sortResult = entry2.GetFloatAuxData().CompareTo(entry1.GetFloatAuxData());
                        if (sortResult != 0) return sortResult;

                        // same texture data
                        // sort by asset size for assets with same texture data
                        if (entry1.UsableSize > entry2.UsableSize) return 1;
                        if (entry1.UsableSize < entry2.UsableSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortByText(SizePart[] assetList, AssetList.SortOrder sortOrder)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        var sortTextureTypeResult = string.Compare(entry1.GetTextAuxData(), entry2.GetTextAuxData(), StringComparison.OrdinalIgnoreCase);
                        if (sortTextureTypeResult != 0) return sortTextureTypeResult;

                        // same texture type
                        // sort by asset size for assets with same texture types
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
                        var sortTextureTypeResult = string.Compare(entry2.GetTextAuxData(), entry1.GetTextAuxData(), StringComparison.OrdinalIgnoreCase);
                        if (sortTextureTypeResult != 0) return sortTextureTypeResult;

                        // same texture type
                        // sort by asset size for assets with same texture types
                        if (entry1.UsableSize > entry2.UsableSize) return 1;
                        if (entry1.UsableSize < entry2.UsableSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        private static void SortByText(SizePart[] assetList, AssetList.SortOrder sortOrder, Func<string, string, int> compare)
        {
            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        var sortTextureTypeResult = compare(entry1.GetTextAuxData(), entry2.GetTextAuxData());
                        if (sortTextureTypeResult != 0) return sortTextureTypeResult;

                        // same texture type
                        // sort by asset size for assets with same texture types
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
                        var sortTextureTypeResult = compare(entry2.GetTextAuxData(), entry1.GetTextAuxData());
                        if (sortTextureTypeResult != 0) return sortTextureTypeResult;

                        // same texture type
                        // sort by asset size for assets with same texture types
                        if (entry1.UsableSize > entry2.UsableSize) return 1;
                        if (entry1.UsableSize < entry2.UsableSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }

        // =============================================================================================================

        private static void SortTextureData(SizePart[] assetList, TextureData textureData, AssetList.SortOrder sortOrder, Func<TextureData.Entry, bool> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                var boolValue                                                = 0;
                if (textureEntries.ContainsKey(assetList[n].Name)) boolValue = func(textureEntries[assetList[n].Name]) ? 1 : 0;

                assetList[n].SetIntAuxData(boolValue);
            }

            SortByInt(assetList, sortOrder);
        }

        private static void SortTextureData(SizePart[] assetList, TextureData textureData, AssetList.SortOrder sortOrder, Func<TextureData.Entry, string> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                string textData                                             = null;
                if (textureEntries.ContainsKey(assetList[n].Name)) textData = func(textureEntries[assetList[n].Name]);

                assetList[n].SetTextAuxData(textData);
            }

            SortByText(assetList, sortOrder);
        }

        private static void SortTextureData(SizePart[] assetList, TextureData textureData, AssetList.SortOrder sortOrder, Func<TextureData.Entry, float> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                float floatValue                                              = 0;
                if (textureEntries.ContainsKey(assetList[n].Name)) floatValue = func(textureEntries[assetList[n].Name]);

                assetList[n].SetFloatAuxData(floatValue);
            }

            SortByFloat(assetList, sortOrder);
        }

        private static void SortTextureData(SizePart[] assetList, TextureData textureData, AssetList.SortOrder sortOrder, Func<TextureData.Entry, int> func)
        {
            var textureEntries = textureData.GetTextureData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                var intValue                                                = 0;
                if (textureEntries.ContainsKey(assetList[n].Name)) intValue = func(textureEntries[assetList[n].Name]);

                assetList[n].SetIntAuxData(intValue);
            }

            SortByInt(assetList, sortOrder);
        }

        // NPotScale sort is special: we want the "None (Not Power of 2)" values to go at top, ignoring alphabetical order for that special value
        private static void SortNPotScale(SizePart[] assetList, TextureData textureData, AssetList.SortOrder sortOrder)
        {
            var textureEntries = textureData.GetTextureData();

            for (var n = 0; n < assetList.Length; ++n)
            {
                string textData                                             = null;
                if (textureEntries.ContainsKey(assetList[n].Name)) textData = textureEntries[assetList[n].Name].NPotScale;

                assetList[n].SetTextAuxData(textData);
            }

            if (sortOrder == AssetList.SortOrder.Descending)
                Array.Sort(assetList,
                    delegate(SizePart entry1, SizePart entry2)
                    {
                        var nPotScale1 = entry1.GetTextAuxData();
                        var nPotScale2 = entry2.GetTextAuxData();

                        // put non-power-of-2 at top
                        var nPotScale1IsNoneNot = nPotScale1 == TextureData.NPOT_SCALE_NONE_NOT_POT;
                        var nPotScale2IsNoneNot = nPotScale2 == TextureData.NPOT_SCALE_NONE_NOT_POT;
                        if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot) return -1;
                        if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot) return 1;

                        // at this point, entry1 and entry 2 are not non-power-of-2 (or both of them are), so compare them as usual
                        var sortTextureTypeResult = string.Compare(nPotScale1, nPotScale2, StringComparison.OrdinalIgnoreCase);
                        if (sortTextureTypeResult != 0) return sortTextureTypeResult;

                        // same nPotScale type
                        // sort by asset size for assets with same nPotScale types
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
                        var nPotScale1 = entry1.GetTextAuxData();
                        var nPotScale2 = entry2.GetTextAuxData();

                        // put non-power-of-2 at bottom
                        var nPotScale1IsNoneNot = nPotScale1 == TextureData.NPOT_SCALE_NONE_NOT_POT;
                        var nPotScale2IsNoneNot = nPotScale2 == TextureData.NPOT_SCALE_NONE_NOT_POT;
                        if (nPotScale1IsNoneNot && !nPotScale2IsNoneNot) return 1;
                        if (!nPotScale1IsNoneNot && nPotScale2IsNoneNot) return -1;

                        // at this point, entry1 and entry 2 are not non-power-of-2 (or both of them are), so compare them as usual
                        var sortTextureTypeResult = string.Compare(nPotScale2, nPotScale1, StringComparison.OrdinalIgnoreCase);
                        if (sortTextureTypeResult != 0) return sortTextureTypeResult;

                        // same nPotScale type
                        // sort by asset size for assets with same nPotScale types
                        if (entry1.UsableSize > entry2.UsableSize) return 1;
                        if (entry1.UsableSize < entry2.UsableSize) return -1;

                        // same size
                        // sort by asset name for assets with same sizes
                        return SortByAssetNameAscending(entry1, entry2);
                    });
        }
    }
}