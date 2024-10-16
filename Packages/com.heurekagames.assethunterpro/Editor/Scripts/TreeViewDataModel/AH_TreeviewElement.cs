﻿using HeurekaGames.Utils;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView
{
    [Serializable]
    public class AH_TreeviewElement : TreeElement, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField] private string absPath;

        //[SerializeField]
        private string relativePath;

        [SerializeField] private string guid;

        //[SerializeField]
        private                  Type   assetType;
        [SerializeField] private string assetTypeSerialized;

        private long assetSize;

        //private string assestSizeStringRepresentation;
        //[SerializeField]
        private long fileSize;

        //[SerializeField]
        //private string fileSizeStringRepresentation;
        [SerializeField] private List<string>                                         scenesReferencingAsset;
        [SerializeField] private bool                                                 usedInBuild;
        [SerializeField] private bool                                                 isFolder;
        [SerializeField] private Dictionary<AH_MultiColumnHeader.AssetShowMode, long> combinedAssetSizeInFolder = new();

        //Dictionary of asset types and their icons (Cant be serialized)
        private static Dictionary<Type, Texture> iconDictionary = new();

        #endregion

        #region Properties

        public string RelativePath
        {
            get
            {
                if (!string.IsNullOrEmpty(this.relativePath))
                    return this.relativePath;
                else
                    return this.relativePath = AssetDatabase.GUIDToAssetPath(this.GUID);
            }
        }

        public string GUID => this.guid;

        public Type AssetType => this.assetType;

        //TODO, make this threaded
        public string AssetTypeSerialized
        {
            get
            {
                if (string.IsNullOrEmpty(this.assetTypeSerialized) && this.assetType != null) this.assetTypeSerialized = Heureka_Serializer.SerializeType(this.assetType);
                return this.assetTypeSerialized;
            }
        }

        public long AssetSize
        {
            get
            {
                if (this.UsedInBuild && this.assetSize == 0)
                {
                    var asset = AssetDatabase.LoadMainAssetAtPath(this.RelativePath);
                    //#if UNITY_2017_1_OR_NEWER
                    if (asset != null)
                        return this.assetSize = Profiler.GetRuntimeMemorySizeLong(asset);
                    else
                        return -1;
                }
                else
                {
                    return this.assetSize;
                }
            }
        }

        public string AssetSizeStringRepresentation => AH_Utils.GetSizeAsString(this.AssetSize);

        //TODO, make this threaded
        public long FileSize
        {
            get
            {
                if (this.fileSize != 0)
                {
                    return this.fileSize;
                }
                else
                {
                    var fileInfo = new System.IO.FileInfo(this.absPath);
                    if (fileInfo.Exists)
                        return this.fileSize = fileInfo != null ? fileInfo.Length : 0;
                    else
                        return -1;
                }
            }
        }

        public string FileSizeStringRepresentation => AH_Utils.GetSizeAsString(this.fileSize);

        public List<string> ScenesReferencingAsset => this.scenesReferencingAsset;

        public int SceneRefCount => this.scenesReferencingAsset != null ? this.scenesReferencingAsset.Count : 0;

        public bool UsedInBuild => this.usedInBuild;

        public bool IsFolder => this.isFolder;

        #endregion

        public AH_TreeviewElement(string absPath, int depth, int id, string relativepath, string assetID, List<string> scenesReferencing, bool isUsedInBuild) : base(System.IO.Path.GetFileName(absPath), depth, id)
        {
            this.absPath = absPath;
            var assetPath = relativepath;
            this.guid                   = AssetDatabase.AssetPathToGUID(assetPath);
            this.scenesReferencingAsset = scenesReferencing;
            this.usedInBuild            = isUsedInBuild;

            //Return if its a folder
            if (this.isFolder = AssetDatabase.IsValidFolder(assetPath)) return;

            //Return if its not an asset
            if (!string.IsNullOrEmpty(this.guid))
            {
                this.assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                this.updateIconDictEntry();
            }
        }

        internal long GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            if (this.combinedAssetSizeInFolder == null) this.combinedAssetSizeInFolder = new();

            if (this.combinedAssetSizeInFolder.ContainsKey(showMode)) return this.combinedAssetSizeInFolder[showMode];

            //TODO store these values instead of calculating each and every time?

            long combinedChildrenSize = 0;
            //Combine the size of all the children
            if (this.hasChildren)
                foreach (AH_TreeviewElement item in this.children)
                {
                    var validAsset = showMode == AH_MultiColumnHeader.AssetShowMode.All || (showMode == AH_MultiColumnHeader.AssetShowMode.Unused && !item.usedInBuild) || (showMode == AH_MultiColumnHeader.AssetShowMode.Used && item.usedInBuild);

                    //Loop thropugh folders and assets thats used not in build
                    if (validAsset || item.isFolder) combinedChildrenSize += item.GetFileSizeRecursively(showMode);
                }

            combinedChildrenSize += this.FileSize;

            //Cache the value
            this.combinedAssetSizeInFolder.Add(showMode, combinedChildrenSize);

            return combinedChildrenSize;
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
            //assetTypeSerialized = "";
        }

        #endregion

        internal bool AssetMatchesState(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            //Test if we want to add this element (We dont want to show "used" when window searches for "unused"
            return this.AssetType != null && (showMode == AH_MultiColumnHeader.AssetShowMode.All || (showMode == AH_MultiColumnHeader.AssetShowMode.Used && this.usedInBuild) || (showMode == AH_MultiColumnHeader.AssetShowMode.Unused && !this.usedInBuild));
        }

        internal bool HasChildrenThatMatchesState(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            if (!this.hasChildren) return false;

            //Check if a valid child exit somewhere in this branch
            foreach (AH_TreeviewElement child in this.children)
            {
                if (child.AssetMatchesState(showMode))
                    return true;
                else if (child.HasChildrenThatMatchesState(showMode))
                    return true;
                else
                    continue;
            }
            return false;
        }

        internal List<string> GetUnusedPathsRecursively()
        {
            var unusedAssetsInFolder = new List<string>();

            //Combine the size of all the children
            if (this.hasChildren)
                foreach (AH_TreeviewElement item in this.children)
                {
                    if (item.isFolder)
                        unusedAssetsInFolder.AddRange(item.GetUnusedPathsRecursively());
                    //Loop thropugh folders and assets thats used not in build
                    else if (!item.usedInBuild) unusedAssetsInFolder.Add(item.RelativePath);
                }
            return unusedAssetsInFolder;
        }

        internal static List<string> GetStoredIconTypes()
        {
            var iconTypesSerialized = new List<string>();
            foreach (var item in iconDictionary) iconTypesSerialized.Add(Heureka_Serializer.SerializeType(item.Key));
            return iconTypesSerialized;
        }

        internal static List<Texture> GetStoredIconTextures()
        {
            var iconTexturesSerialized = new List<Texture>();
            foreach (var item in iconDictionary) iconTexturesSerialized.Add(item.Value);
            return iconTexturesSerialized;
        }

        private void updateIconDictEntry()
        {
            if (this.assetType != null && !iconDictionary.ContainsKey(this.assetType)) iconDictionary.Add(this.assetType, EditorGUIUtility.ObjectContent(null, this.assetType).image);
        }

        internal static void UpdateIconDictAfterSerialization(List<string> serializationHelperListIconTypes, List<Texture> serializationHelperListIconTextures)
        {
            iconDictionary = new();
            for (var i = 0; i < serializationHelperListIconTypes.Count; i++)
            {
                var deserializedType = Heureka_Serializer.DeSerializeType(serializationHelperListIconTypes[i]);
                if (deserializedType != null) iconDictionary.Add(Heureka_Serializer.DeSerializeType(serializationHelperListIconTypes[i]), serializationHelperListIconTextures[i]);
            }
        }

        internal static Texture GetIcon(Type assetType)
        {
            return iconDictionary[assetType];
        }
    }
}