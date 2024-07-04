namespace UITemplate.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEngine;

    public static class AssetSearcher
    {
        public static Dictionary<TType, HashSet<GameObject>> GetAllAssetInAddressable<TType>() where TType : Object
        {
            var allAssetInAddressable = new Dictionary<TType, HashSet<GameObject>>();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings)
            {
                var totalSteps  = settings.groups.Sum(group => group.entries.Count);
                var currentStep = 0;
                foreach (var group in settings.groups)
                {
                    foreach (var entry in group.entries)
                    {
                        EditorUtility.DisplayProgressBar("Queries all asset", "Processing Addressables", currentStep / (float)totalSteps);

                        var path       = AssetDatabase.GUIDToAssetPath(entry.guid);
                        var mainObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (mainObject == null) continue;

                        foreach (var type in GetAllDependencies<TType>(path))
                        {
                            allAssetInAddressable.GetOrAdd(type, () => new HashSet<GameObject>()).Add(mainObject);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
            }

            return allAssetInAddressable;
        }

        public static List<TType> GetAllDependencies<TType>(string path, bool recursive = true) where TType : Object
        {
            var dependencies         = new List<string>(AssetDatabase.GetDependencies(path, recursive));
            return dependencies.Select(depPath => AssetDatabase.LoadAssetAtPath<TType>(depPath)).Where(depO => depO != null).ToList();
        }
        
        public static List<T> GetAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            List<T>  assets = new List<T>();
            string[] guids  = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T      asset     = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}