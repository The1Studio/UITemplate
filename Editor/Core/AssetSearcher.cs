namespace TheOne.Tool.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DG.DemiEditor;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;
    using UnityEngine;

    public static class AssetSearcher
    {
        public static Dictionary<TType, HashSet<Object>> GetAllAssetInAddressable<TType>() where TType : Object
        {
            var allAssetInAddressable = new Dictionary<TType, HashSet<Object>>();
            var settings              = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return allAssetInAddressable;

            var totalSteps  = settings.groups.Sum(group => group.entries.Count);
            var currentStep = 0;
            foreach (var group in settings.groups)
            foreach (var entry in group.entries)
            {
                EditorUtility.DisplayProgressBar("Queries all asset", "Processing Addressables", currentStep++ / (float)totalSteps);
                ProcessEntry<TType>(entry, allAssetInAddressable);
            }

            EditorUtility.ClearProgressBar();
            return allAssetInAddressable;
        }
        
        public static HashSet<TType> GetAllComponentInAddressable<TType>() where TType : Object
        {
            var gameObjects               = GetAllAssetInAddressable<GameObject>().Keys;
            var allComponentInAddressable = gameObjects.SelectMany(go => go.GetComponentsInChildren<TType>(true)).ToHashSet();
            //check if them belong to a same prefab
            var prefabSet = allComponentInAddressable.Select(comp =>
            {
                var nearestPrefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(comp);
                return nearestPrefabInstanceRoot == null ? null : PrefabUtility.GetCorrespondingObjectFromOriginalSource(nearestPrefabInstanceRoot);
            }).Where(prefab => prefab != null).ToHashSet();
            
            allComponentInAddressable = prefabSet.SelectMany(prefab => prefab.GetComponentsInChildren<TType>(true).Where(comp => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(comp) == PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab))).ToHashSet();
            
            return allComponentInAddressable;
        }

        private static void ProcessEntry<TType>(AddressableAssetEntry entry, Dictionary<TType, HashSet<Object>> allAssetInAddressable) where TType : Object
        {
            var path       = AssetDatabase.GUIDToAssetPath(entry.guid);
            var mainObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (mainObject is TType mainObjectTType) allAssetInAddressable.GetOrAdd(mainObjectTType, () => new HashSet<Object>()).Add(mainObject);

            GetAllDependencies<TType>(path).ForEach(type => allAssetInAddressable.GetOrAdd(type, () => new HashSet<Object>()).Add(mainObject));
        }

        public static List<TType> GetAllDependencies<TType>(Object objectDeped, bool recursive = true) where TType : Object
        {
            var path = AssetDatabase.GetAssetPath(objectDeped);
            return GetAllDependencies<TType>(path, recursive);
        }

        public static List<TType> GetAllDependencies<TType>(string path, bool recursive = true) where TType : Object
        {
            return AssetDatabase.GetDependencies(path, recursive)
                .Where(AssetDatabase.IsOpenForEdit)
                .Select(AssetDatabase.LoadAssetAtPath<TType>)
                .Where(dep => dep != null)
                .ToList();
        }

        public static List<Object> GetAllAssetsInGroup(string groupName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) return new();

            return settings.groups
                    .FirstOrDefault(g => g.Name == groupName)?.entries
                    .Select(e => AssetDatabase.GUIDToAssetPath(e.guid))
                    .Select(AssetDatabase.LoadAssetAtPath<Object>)
                    .Where(asset => asset != null)
                    .ToList()
                ?? new List<Object>();
        }

        public static bool IsAssetAddressable(Object obj, out AddressableAssetGroup addressableGroup)
        {
            return IsAssetAddressable(AssetDatabase.GetAssetPath(obj), out addressableGroup);
        }

        public static bool IsAssetAddressable(string assetPath, out AddressableAssetGroup addressableGroup)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            addressableGroup = settings?.groups
                .SelectMany(g => g.entries, (g, e) => new { Group = g, Entry = e })
                .FirstOrDefault(x => x.Entry.guid == AssetDatabase.AssetPathToGUID(assetPath))?.Group;

            return addressableGroup != null;
        }

        public static List<T> GetAllAssetsOfType<T>() where T : Object
        {
            var assets = new List<T>();
            var guids  = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset     = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) assets.Add(asset);
            }

            return assets;
        }

        public static void MoveAssetToGroup(Object asset, string targetGroupName)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Asset path could not be found.");
                return;
            }

            MoveAssetToGroup(assetPath, targetGroupName);
        }

        private static void MoveAssetToGroup(string assetPath, string targetGroupName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");
                return;
            }

            var guid  = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                Debug.LogError($"Asset not found in Addressables for path: {assetPath}");
                return;
            }

            var targetGroup = settings.groups.Find(g => g.Name == targetGroupName);
            if (targetGroup == null)
            {
                // Create the group if it doesn't exist
                targetGroup = settings.CreateGroup(targetGroupName, false, false, false, null);
                targetGroup.AddSchema<BundledAssetGroupSchema>();
                targetGroup.AddSchema<ContentUpdateGroupSchema>();
            }

            AddressableAssetSettingsDefaultObject.Settings.MoveEntry(entry, targetGroup);

            // Save changes
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        }

        public static bool CreateFolderIfNotExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }

            return false;
        }

        //Move targetObject to assetsSpritesBuildinui directory
        public static void MoveToNewFolder(Object targetObject, string assetsSpritesBuildinui)
        {
            var assetPath = AssetDatabase.GetAssetPath(targetObject);
            var newPath   = Path.Combine(assetsSpritesBuildinui, Path.GetFileName(assetPath));
            var moveError = AssetDatabase.MoveAsset(assetPath, newPath);
            if (!moveError.IsNullOrEmpty()) Debug.LogError(moveError);
        }
    }
}