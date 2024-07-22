namespace UITemplate.Editor.TheOneWindowTools.ListAndOptimize
{
    using System;
    using System.Collections.Generic;
    using UITemplate.Editor.Optimization.TextureOptimization;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "TextureInfoData", menuName = "ScriptableObjects/TextureInfoData", order = 1)]
    public class TextureInfoData : ScriptableObject
    {
        [SerializeField]
        public List<TextureInfo> textureInfos;
    }
}