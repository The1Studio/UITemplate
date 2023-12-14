namespace UITemplate.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEngine;

    public static class AddressableSearcherTool
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

                        var dependencies = new List<string>(AssetDatabase.GetDependencies(path, true));

                        var allMeshInAddressable = dependencies.Select(depPath => AssetDatabase.LoadAssetAtPath<TType>(depPath));

                        foreach (var type in allMeshInAddressable)
                        {
                            if (type == null) continue;
                            allAssetInAddressable.GetOrAdd(type, () => new HashSet<GameObject>()).Add(mainObject);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
            }

            return allAssetInAddressable;
        }
    }
}