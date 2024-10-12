using HeurekaGames.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.DependencyGraph
{
    [Serializable]
    public class AH_DepGraphElement : TreeElement, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField] private string relativePath;

        /*[SerializeField]
        private string assetName;*/
        [SerializeField] private Type    assetType;
        private                  Texture icon;
        [SerializeField] private string  assetTypeSerialized;

        #endregion

        #region Properties

        public string RelativePath => this.relativePath;

        public string AssetName => this.m_Name;

        public Type AssetType => this.assetType;

        public Texture Icon => this.icon;

        public string AssetTypeSerialized => this.assetTypeSerialized;

        #endregion

        public AH_DepGraphElement(string name, int depth, int id, string relativepath) : base(name, depth, id)
        {
            this.relativePath = relativepath;
            var stringSplit = relativepath.Split('/');
            //this.assetName = stringSplit.Last();
            this.assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(relativepath);
            if (this.assetType != null) this.assetTypeSerialized = Heureka_Serializer.SerializeType(this.assetType);
            this.icon = UnityEditor.EditorGUIUtility.ObjectContent(null, this.assetType).image;
        }

        #region Serialization callbacks

        //TODO Maybe we can store type infos in BuildInfoTreeView instead of on each individual element, might be performance heavy

        //Store serializable string so we can retrieve type after serialization
        public void OnBeforeSerialize()
        {
            if (this.assetType != null) this.assetTypeSerialized = Heureka_Serializer.SerializeType(this.assetType);
        }

        //Set type from serialized property
        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(this.AssetTypeSerialized)) this.assetType = Heureka_Serializer.DeSerializeType(this.AssetTypeSerialized);
        }

        #endregion
    }
}