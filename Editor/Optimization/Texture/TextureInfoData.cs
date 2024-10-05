namespace TheOne.Tool.Optimization.Texture
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "TextureInfoData", menuName = "ScriptableObjects/TextureInfoData", order = 1)]
    public class TextureInfoData : ScriptableObject
    {
        [SerializeField]
        public List<TextureInfo> textureInfos;
    }
}