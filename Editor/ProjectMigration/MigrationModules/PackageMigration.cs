namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using ServiceImplementation.Configs.Common;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEngine;

    public static class PackageMigration
    {
        private static readonly Registry[] Registries =
        {
            new(
                name: "OpenUPM",
                url: "https://package.openupm.com",
                scopes: new string[]
                {
                    "com.google",
                    "com.cysharp",
                    "com.coffee",
                    "com.github-glitchenzo",
                    "jp.hadashikick.vcontainer",
                    "org.nuget",
                    "com.adjust",
                }
            ),
            new(
                name: "NuGet",
                url: "https://unitynuget-registry.azurewebsites.net/",
                scopes: new string[]
                {
                }
            ),
            new(
                name: "TheOne",
                url: "https://upm.the1studio.org/",
                scopes: new string[]
                {
                    "com.theone",
                }
            ),
            new(
                name: "AppLovin MAX Unity",
                url: "https://unity.packages.applovin.com",
                scopes: new string[]
                {
                    "com.applovin.mediation.ads",
                    "com.applovin.mediation.adapters",
                    "com.applovin.mediation.dsp",
                }
            ),
        };

        [NonSerialized]
        private static readonly Dictionary<string, string> PackagesToAdd = new()
        {
            // {"com.unity.adaptiveperformance", "5.1.0"},
            // {"com.unity.adaptiveperformance.samsung.android", "5.0.0"},
            { "com.google.external-dependency-manager", "1.2.183" },
            { "com.theone.foundation.buildscript", "https://github.com/The1Studio/UnityBuildScript.git?path=Assets/BuildScripts" },
            //need to use this method because of the purchase connector, if we can import the purchase connector through UPM then we can change it
            #if APPSFLYER
            { "com.theone.appsflyer-unity-plugin", "https://github.com/The1Studio/appsflyer.git?path=Assets/AppsFlyer#appsflyer_sdk-purchase_sdk" },
            #endif
            #if BYTEBREW
            { "com.bytebrew.unitysdk", "https://github.com/The1Studio/ByteBrewUnitySDK.git?path=UPMPackage#" },
            #endif
            #if UNITY_6000_0_OR_NEWER
            { "com.unity.addressables.android", "1.0.4" },
            #endif
            // add more packages as needed
        };

        // Add the package version to use here if you want to use a specific version
        [NonSerialized]
        private static readonly Dictionary<string, string> PackagesVersionToUse = new()
        {
            { "com.google.ads.mobile", "9.5.0" },
            { "com.unity.purchasing", "4.12.2" },
            { "com.cysharp.unitask", "2.5.10" },
            { "jp.hadashikick.vcontainer", "1.16.8" },
            { "com.coffee.ui-effect", "5.2.0" },
            { "com.adjust.sdk", "https://github.com/The1Studio/adjust.git?path=Assets/Adjust#" },
        };

        [NonSerialized]
        private static readonly Dictionary<(string, string), string> NameToUnityPackageToImport = new()
        {
            { ("BuildScripts", "Assets/BuildScripts"), "https://cdn.builds.the1studio.org/packages/GameVersionRuntime.unitypackage" },
        };

        [NonSerialized]
        public static readonly List<string> PackagesToRemove = new()
        {
            // "com.unity.adaptiveperformance.google.android"
            "appsflyer-unity-plugin"
        };

        public static void ImportUnityPackage()
        {
            foreach (var ((name, path), url) in NameToUnityPackageToImport)
            {
                if (AssetDatabase.IsValidFolder(path)) continue;
                UnityPackageHelper.DownloadThenImportPackage(url, name).Forget();
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
            foreach (var package in PackagesToAdd)
            {
                if (!dependencies.ContainsKey(package.Key) || !dependencies[package.Key]!.ToString().Equals(package.Value))
                {
                    dependencies[package.Key] = package.Value;
                    updated                   = true;
                }
            }

            // Remove the packages
            foreach (var package in PackagesToRemove)
            {
                if (dependencies.ContainsKey(package))
                {
                    dependencies.Remove(package);
                    updated = true;
                }
            }

            // Change package version
            foreach (var package in PackagesVersionToUse)
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

        public static void CheckAndUpdatePackageManagerSettings()
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

            foreach (var registry in Registries)
            {
                if (scopedRegistries.FirstOrDefault(jRegistry => jRegistry["name"]?.ToString() == registry.name) is not JObject jRegistry)
                {
                    if (registry.scopes.Length == 0) continue;
                    scopedRegistries.Add(JObject.FromObject(registry));
                    updated = true;
                    continue;
                }
                if (jRegistry["url"]?.ToString() != registry.url)
                {
                    jRegistry["url"] = registry.url;
                    updated          = true;
                }
                var otherRegistryScopes = Registries.Where(otherRegistry => otherRegistry.name != registry.name).SelectMany(otherRegistry => otherRegistry.scopes).ToArray();
                var oldScopes           = jRegistry["scopes"]?.Values<string>().Select(scope => scope.Trim().ToLower()).ToArray() ?? Array.Empty<string>();
                var newScopes           = oldScopes.Union(registry.scopes).Except(otherRegistryScopes).ToArray();
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

        private readonly struct Registry
        {
            public readonly string   name;
            public readonly string   url;
            public readonly string[] scopes;

            public Registry(string name, string url, string[] scopes)
            {
                this.name   = name;
                this.url    = url;
                this.scopes = scopes;
            }
        }
    }
}