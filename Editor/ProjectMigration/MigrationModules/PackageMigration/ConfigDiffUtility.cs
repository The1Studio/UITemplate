namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;

    public static class ConfigDiffUtility
    {
        [MenuItem("TheOne/Package Migration/Show All Variants")]
        public static void ShowAllVariants()
        {
            var config = LoadUnifiedConfig();
            if (config == null) return;

            Debug.Log("=== Package Migration Config Structure ===\n");
            Debug.Log($"Base Config: {(config.Base != null ? "✓ Loaded" : "✗ Missing")}");
            
            if (config.Variants != null && config.Variants.Any())
            {
                Debug.Log($"Variants ({config.Variants.Count}):");
                foreach (var variant in config.Variants)
                {
                    Debug.Log($"  - {variant.Key}: {(variant.Value != null ? "✓ Loaded" : "✗ Invalid")}");
                }
            }
            else
            {
                Debug.Log("Variants: None defined");
            }
        }

        [MenuItem("TheOne/Package Migration/Compare Base vs Unity 2022")]
        public static void CompareBaseVsUnity2022()
        {
            var config = LoadUnifiedConfig();
            if (config?.Base == null) return;

            if (config.Variants == null || !config.Variants.ContainsKey("unity2022"))
            {
                Debug.LogError("Unity 2022 variant not found in config");
                return;
            }

            var differences = CompareConfigs(config.Base, config.Variants["unity2022"]);
            
            Debug.Log("=== Base vs Unity 2022 Differences ===\n");
            if (differences.Any())
            {
                foreach (var diff in differences)
                {
                    Debug.Log(diff);
                }
            }
            else
            {
                Debug.Log("No differences found between base and Unity 2022 variant");
            }
        }

        [MenuItem("TheOne/Package Migration/Compare Base vs Unity 2021")]
        public static void CompareBaseVsUnity2021()
        {
            var config = LoadUnifiedConfig();
            if (config?.Base == null) return;

            if (config.Variants == null || !config.Variants.ContainsKey("unity2021"))
            {
                Debug.LogError("Unity 2021 variant not found in config");
                return;
            }

            var differences = CompareConfigs(config.Base, config.Variants["unity2021"]);
            
            Debug.Log("=== Base vs Unity 2021 Differences ===\n");
            if (differences.Any())
            {
                foreach (var diff in differences)
                {
                    Debug.Log(diff);
                }
            }
            else
            {
                Debug.Log("No differences found between base and Unity 2021 variant");
            }
        }

        [MenuItem("TheOne/Package Migration/Show Current Unity Version Config")]
        public static void ShowCurrentUnityVersionConfig()
        {
            var config = LoadUnifiedConfig();
            if (config?.Base == null) return;

            string variantKey = GetCurrentUnityVariantKey();
            PackageMigrationConfig.Config variantConfig = null;

            if (!string.IsNullOrEmpty(variantKey) && config.Variants != null && 
                config.Variants.ContainsKey(variantKey))
            {
                variantConfig = config.Variants[variantKey];
            }

            var mergedConfig = PackageMigrationConfig.Config.Merge(config.Base, variantConfig);
            
            Debug.Log($"=== Current Unity Version Config ({variantKey ?? "base"}) ===\n");
            Debug.Log($"Registries: {mergedConfig.Registries?.Count ?? 0}");
            Debug.Log($"Packages to Add: {mergedConfig.PackagesToAdd?.Count ?? 0}");
            Debug.Log($"Package Versions: {mergedConfig.PackagesVersionToUse?.Count ?? 0}");
            Debug.Log($"Unity Packages to Import: {mergedConfig.NameToUnityPackageToImport?.Count ?? 0}");
            Debug.Log($"Packages to Remove: {mergedConfig.PackagesToRemove?.Count ?? 0}");
            Debug.Log($"WebGL Packages to Remove: {mergedConfig.WebGLPackagesToRemove?.Count ?? 0}");
        }

        [MenuItem("TheOne/Package Migration/Validate All Variants")]
        public static void ValidateAllVariants()
        {
            var config = LoadUnifiedConfig();
            if (config?.Base == null) return;

            Debug.Log("=== Validating All Variants ===\n");
            bool allValid = true;

            if (config.Variants != null)
            {
                foreach (var variant in config.Variants)
                {
                    try
                    {
                        var mergedConfig = PackageMigrationConfig.Config.Merge(config.Base, variant.Value);
                        Debug.Log($"✅ {variant.Key}: Valid");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"❌ {variant.Key}: Invalid - {e.Message}");
                        allValid = false;
                    }
                }
            }

            if (allValid)
            {
                Debug.Log("✅ All variants are valid!");
            }
            else
            {
                Debug.LogError("❌ Some variants have issues!");
            }
        }

        private static PackageMigrationConfig LoadUnifiedConfig()
        {
            var textAsset = Resources.Load<TextAsset>("PackageMigrationConfig");
            if (textAsset == null)
            {
                Debug.LogError("PackageMigrationConfig.json not found in Resources folder");
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<PackageMigrationConfig>(textAsset.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize PackageMigrationConfig: {e.Message}");
                return null;
            }
        }

        private static string GetCurrentUnityVariantKey()
        {
#if UNITY_6000_0_OR_NEWER
            return null; // Unity 6 uses base config as-is
#elif UNITY_2022_3_OR_NEWER
            return "unity2022";
#elif UNITY_2021_3_OR_NEWER
            return "unity2021";
#else
            return null; // Default to base config for older versions
#endif
        }

        private static List<string> CompareConfigs(PackageMigrationConfig.Config config1, PackageMigrationConfig.Config config2)
        {
            var differences = new List<string>();

            // Compare registries
            differences.AddRange(CompareRegistries(config1.Registries, config2.Registries, "Registries"));

            // Compare packages to add
            differences.AddRange(CompareDictionaries(config1.PackagesToAdd, config2.PackagesToAdd, "PackagesToAdd"));

            // Compare package versions
            differences.AddRange(CompareDictionaries(config1.PackagesVersionToUse, config2.PackagesVersionToUse, "PackagesVersionToUse"));

            // Compare unity packages to import
            differences.AddRange(CompareUnityPackages(config1.NameToUnityPackageToImport, config2.NameToUnityPackageToImport, "NameToUnityPackageToImport"));

            // Compare packages to remove
            differences.AddRange(CompareLists(config1.PackagesToRemove, config2.PackagesToRemove, "PackagesToRemove"));

            // Compare WebGL packages to remove
            differences.AddRange(CompareLists(config1.WebGLPackagesToRemove, config2.WebGLPackagesToRemove, "WebGLPackagesToRemove"));

            return differences;
        }

        private static List<string> CompareRegistries(List<PackageMigrationConfig.Config.Registry> registries1, List<PackageMigrationConfig.Config.Registry> registries2, string sectionName)
        {
            var differences = new List<string>();
            
            var dict1 = registries1?.ToDictionary(r => r.Name) ?? new Dictionary<string, PackageMigrationConfig.Config.Registry>();
            var dict2 = registries2?.ToDictionary(r => r.Name) ?? new Dictionary<string, PackageMigrationConfig.Config.Registry>();

            var allNames = dict1.Keys.Union(dict2.Keys).OrderBy(k => k);

            foreach (var name in allNames)
            {
                if (!dict1.ContainsKey(name))
                {
                    differences.Add($"[{sectionName}] Added registry: {name}");
                }
                else if (!dict2.ContainsKey(name))
                {
                    differences.Add($"[{sectionName}] Removed registry: {name}");
                }
                else
                {
                    var reg1 = dict1[name];
                    var reg2 = dict2[name];

                    if (reg1.Url != reg2.Url)
                    {
                        differences.Add($"[{sectionName}] {name}.Url: {reg1.Url} → {reg2.Url}");
                    }

                    var scopes1 = reg1.Scopes?.OrderBy(s => s).ToList() ?? new List<string>();
                    var scopes2 = reg2.Scopes?.OrderBy(s => s).ToList() ?? new List<string>();

                    if (!scopes1.SequenceEqual(scopes2))
                    {
                        var added = scopes2.Except(scopes1);
                        var removed = scopes1.Except(scopes2);
                        
                        if (added.Any())
                            differences.Add($"[{sectionName}] {name}.Scopes added: {string.Join(", ", added)}");
                        if (removed.Any())
                            differences.Add($"[{sectionName}] {name}.Scopes removed: {string.Join(", ", removed)}");
                    }
                }
            }

            return differences;
        }

        private static List<string> CompareDictionaries(Dictionary<string, string> dict1, Dictionary<string, string> dict2, string sectionName)
        {
            var differences = new List<string>();
            
            dict1 ??= new Dictionary<string, string>();
            dict2 ??= new Dictionary<string, string>();

            var allKeys = dict1.Keys.Union(dict2.Keys).OrderBy(k => k);

            foreach (var key in allKeys)
            {
                if (!dict1.ContainsKey(key))
                {
                    differences.Add($"[{sectionName}] Added: {key} = {dict2[key]}");
                }
                else if (!dict2.ContainsKey(key))
                {
                    differences.Add($"[{sectionName}] Removed: {key} = {dict1[key]}");
                }
                else if (dict1[key] != dict2[key])
                {
                    differences.Add($"[{sectionName}] {key}: {dict1[key]} → {dict2[key]}");
                }
            }

            return differences;
        }

        private static List<string> CompareUnityPackages(Dictionary<string, PackageMigrationConfig.Config.UnityPackage> dict1, Dictionary<string, PackageMigrationConfig.Config.UnityPackage> dict2, string sectionName)
        {
            var differences = new List<string>();
            
            dict1 ??= new Dictionary<string, PackageMigrationConfig.Config.UnityPackage>();
            dict2 ??= new Dictionary<string, PackageMigrationConfig.Config.UnityPackage>();

            var allKeys = dict1.Keys.Union(dict2.Keys).OrderBy(k => k);

            foreach (var key in allKeys)
            {
                if (!dict1.ContainsKey(key))
                {
                    differences.Add($"[{sectionName}] Added: {key}");
                }
                else if (!dict2.ContainsKey(key))
                {
                    differences.Add($"[{sectionName}] Removed: {key}");
                }
                else
                {
                    var pkg1 = dict1[key];
                    var pkg2 = dict2[key];

                    if (pkg1.Path != pkg2.Path)
                    {
                        differences.Add($"[{sectionName}] {key}.Path: {pkg1.Path} → {pkg2.Path}");
                    }

                    if (pkg1.Url != pkg2.Url)
                    {
                        differences.Add($"[{sectionName}] {key}.Url: {pkg1.Url} → {pkg2.Url}");
                    }
                }
            }

            return differences;
        }

        private static List<string> CompareLists(List<string> list1, List<string> list2, string sectionName)
        {
            var differences = new List<string>();
            
            list1 ??= new List<string>();
            list2 ??= new List<string>();

            var sorted1 = list1.OrderBy(s => s).ToList();
            var sorted2 = list2.OrderBy(s => s).ToList();

            if (!sorted1.SequenceEqual(sorted2))
            {
                var added = sorted2.Except(sorted1);
                var removed = sorted1.Except(sorted2);
                
                if (added.Any())
                    differences.Add($"[{sectionName}] Added: {string.Join(", ", added)}");
                if (removed.Any())
                    differences.Add($"[{sectionName}] Removed: {string.Join(", ", removed)}");
            }

            return differences;
        }
    }
} 