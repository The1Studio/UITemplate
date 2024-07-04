namespace UITemplate.Editor.TheOneWindowTools.ListAndOptimize
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TextureInfoData", menuName = "ScriptableObjects/TextureInfoData", order = 1)]
    public class TextureInfoData : ScriptableObject
    {
        public List<TextureInfo> textureInfos;
    }
}