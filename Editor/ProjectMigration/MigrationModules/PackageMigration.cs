namespace UITemplate.Editor.ProjectMigration.MigrationModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using ServiceImplementation.Configs.Common;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEngine;

    public static class PackageMigration
    {
        private const string OpenUPMRegistryName = "OpenUPM";
        private const string OpenUPMRegistryUrl  = "https://package.openupm.com";

        [NonSerialized]
        private static readonly string[] RequiredScopes =
        {
            "com.google",
            "com.cysharp",
            "com.coffee",
            "org.nuget",
            "com.github-glitchenzo",
            "jp.hadashikick.vcontainer",
            "com.theone"
        };

        [NonSerialized]
        private static readonly Dictionary<string, string> PackagesToAdd = new()
        {
            // {"com.unity.adaptiveperformance", "5.1.0"},
            // {"com.unity.adaptiveperformance.samsung.android", "5.0.0"},
            { "com.google.external-dependency-manager", "1.2.183" },
            {"com.theone.foundation.buildscript", "https://github.com/The1Studio/UnityBuildScript.git?path=Assets/BuildScripts"},
#if APPSFLYER
            { "com.theone.appsflyer-unity-plugin", "https://github.com/The1Studio/appsflyer.git?path=Assets/AppsFlyer#appsflyer_sdk-purchase_sdk" }
#endif
            // add more packages as needed
        };

        [NonSerialized]
        private static readonly Dictionary<string, string> PackagesVersionToUse = new()
        {
            { "com.google.ads.mobile", "9.2.0" },
            { "com.unity.purchasing", "4.12.2" },
        };
        
        [NonSerialized] private static readonly Dictionary<(string, string), string> NameToUnityPackageToImport = new()
        { {("BuildScripts", "BuildScripts"), "https://cdn.builds.the1studio.org/packages/GameVersionRuntime.unitypackage"}
        };

        [NonSerialized]
        private static readonly List<string> PackagesToRemove = new()
        {
            // "com.unity.adaptiveperformance.google.android"
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
            var dependencies = manifest["dependencies"] as JObject;
            if (dependencies == null)
            {
                dependencies             = new JObject();
                manifest["dependencies"] = dependencies;
            }

            var updated = false;
            // Add the new packages
            foreach (var package in PackagesToAdd)
            {
                if (!dependencies.ContainsKey(package.Key) || !dependencies[package.Key].ToString().Equals(package.Value))
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
                if (dependencies.ContainsKey(package.Key) && !dependencies[package.Key].ToString().Equals(package.Value))
                {
                    dependencies[package.Key] = package.Value;
                    updated                   = true;
                }
            }

            if (updated)
            {
                Debug.Log("Updated manifest.json with new packages and removed old packages.");
            }

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
            var scopedRegistries = manifest["scopedRegistries"] as JArray;
            if (scopedRegistries == null)
            {
                scopedRegistries             = new JArray();
                manifest["scopedRegistries"] = scopedRegistries;
            }

            var openUPMRegistry = scopedRegistries.FirstOrDefault(r => r["name"]?.ToString() == OpenUPMRegistryName) as JObject;
            if (openUPMRegistry == null)
            {
                openUPMRegistry = new JObject
                {
                    ["name"]   = OpenUPMRegistryName,
                    ["url"]    = OpenUPMRegistryUrl,
                    ["scopes"] = new JArray()
                };
                scopedRegistries.Add(openUPMRegistry);
            }

            var scopes = openUPMRegistry["scopes"] as JArray;
            if (scopes == null)
            {
                scopes                    = new JArray();
                openUPMRegistry["scopes"] = scopes;
            }

            bool updated = false;
            foreach (var scope in RequiredScopes)
            {
                var trimmedScope = scope.Trim().ToLower();
                if (!scopes.Values<string>().Select(s => s.Trim().ToLower()).Contains(trimmedScope))
                {
                    scopes.Add(scope);
                    updated = true;
                }
            }

            if (updated)
            {
                Debug.Log("Updated manifest.json with new packages and missing OpenUPM scopes.");
            }

            return updated;
        }
    }
}