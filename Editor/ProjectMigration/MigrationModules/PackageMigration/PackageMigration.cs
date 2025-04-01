namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core.AnalyticServices;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ServiceImplementation.Configs.Common;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEngine;

    public static class PackageMigration
    {
        public class Config
        {
            public List<Registry>                   Registries                 { get; set; }
            public Dictionary<string, string>       PackagesToAdd              { get; set; }
            public Dictionary<string, string>       PackagesVersionToUse       { get; set; }
            public Dictionary<string, UnityPackage> NameToUnityPackageToImport { get; set; }
            public List<string>                     PackagesToRemove           { get; set; }
            
            public class Registry
            {
                public string       Name   { get; set; }
                public string       Url    { get; set; }
                public List<string> Scopes { get; set; }
            }
            
            public class UnityPackage
            {
                public string Path { get; set; }
                public string Url  { get; set; }
            }
        }

        [NonSerialized] private static Config config;
        public static void Migrate()
        {
            var configTextAsset = Resources.Load<TextAsset>("PackageMigationConfig");
            if (configTextAsset == null)
            {
                Debug.LogError("PackageMigationConfig.json not found in Resources folder");
                return;
            }

            var configJson = configTextAsset.text;
            // Deserialize the JSON string into the Config object
            config = JsonConvert.DeserializeObject<Config>(configJson);
#if APPSFLYER
            config.PackagesToAdd.Add("com.theone.appsflyer-unity-plugin", AnalyticConfig.AppsflyerPackageGitURL);
#endif
#if BYTEBREW
            config.PackagesToAdd.Add("com.bytebrew.unitysdk", AnalyticConfig.ByteBrewPackageGitURL);
#endif
#if UNITY_6000_0_OR_NEWER
            config.PackagesToAdd.Add("com.unity.addressables.android", "1.0.6");
#endif
            CheckAndUpdatePackageManagerSettings();
            ImportUnityPackage();
        }

        private static void ImportUnityPackage()
        {
            foreach (var (name, unityPackage) in config.NameToUnityPackageToImport)
            {
                if (AssetDatabase.IsValidFolder(unityPackage.Path)) continue;
                UnityPackageHelper.DownloadThenImportPackage(unityPackage.Url, name).Forget();
            }
        }

        private static bool UpdatePackageDependencies(JObject manifest)
        {
            if (manifest["dependencies"] is not JObject dependencies)
            {
                dependencies             = new();
                manifest["dependencies"] = dependencies;
            }

            var updated = false;
            // Add the new packages
            foreach (var package in config.PackagesToAdd)
            {
                if (!dependencies.ContainsKey(package.Key) || !dependencies[package.Key]!.ToString().Equals(package.Value))
                {
                    dependencies[package.Key] = package.Value;
                    updated                   = true;
                }
            }

            // Remove the packages
            foreach (var package in config.PackagesToRemove)
            {
                if (dependencies.ContainsKey(package))
                {
                    dependencies.Remove(package);
                    updated = true;
                }
            }

            // Change package version
            foreach (var package in config.PackagesVersionToUse)
            {
                if (dependencies.ContainsKey(package.Key) && !dependencies[package.Key]!.ToString().Equals(package.Value))
                {
                    dependencies[package.Key] = package.Value;
                    updated                   = true;
                }
            }

            if (updated) Debug.Log("Updated manifest.json with new packages and removed old packages.");

            return updated;
        }

        private static void CheckAndUpdatePackageManagerSettings()
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest     = JObject.Parse(manifestJson);

            if (UpdatePackageDependencies(manifest) || UpdateScopedRegistries(manifest))
            {
                File.WriteAllText(manifestPath, manifest.ToString());
                Client.Resolve();
            }
        }

        private static bool UpdateScopedRegistries(JObject manifest)
        {
            if (manifest["scopedRegistries"] is not JArray scopedRegistries)
            {
                scopedRegistries             = new();
                manifest["scopedRegistries"] = scopedRegistries;
            }

            var updated = false;

            foreach (var registry in config.Registries)
            {
                if (scopedRegistries.FirstOrDefault(jRegistry => jRegistry["name"]?.ToString() == registry.Name) is not JObject jRegistry)
                {
                    if (registry.Scopes.Count == 0) continue;
                    scopedRegistries.Add(JObject.FromObject(registry));
                    updated = true;
                    continue;
                }

                if (jRegistry["url"]?.ToString() != registry.Url)
                {
                    jRegistry["url"] = registry.Url;
                    updated          = true;
                }

                var otherRegistryScopes = config.Registries.Where(otherRegistry => otherRegistry.Name != registry.Name).SelectMany(otherRegistry => otherRegistry.Scopes).ToArray();
                var oldScopes           = jRegistry["scopes"]?.Values<string>().Select(scope => scope.Trim().ToLower()).ToArray() ?? Array.Empty<string>();
                var newScopes           = oldScopes.Union(registry.Scopes).Except(otherRegistryScopes).ToArray();
                if (oldScopes.Except(newScopes).Union(newScopes.Except(oldScopes)).Any())
                {
                    jRegistry["scopes"] = JArray.FromObject(newScopes);
                    updated             = true;
                }

                if (!jRegistry["scopes"]!.Any())
                {
                    jRegistry.Remove();
                    updated = true;
                }
            }

            if (updated) Debug.Log("Updated manifest.json with new packages and missing OpenUPM scopes.");

            return updated;
        }
    }
}
