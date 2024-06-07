namespace UITemplate.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using UITemplate.Editor.ProjectMigration;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEngine;

    [InitializeOnLoad]
    public class ProjectAutoMigrationScript
    {
        private const string ProguardUserFilePath = "Assets/Plugins/Android/proguard-user.txt";

        private static readonly string[] RequiredProguardLines =
        {
            "-keep class com.bytebrew.** {*; }",
            "-keep class com.google.unity.ads.**{ *; }"
            // Add more lines as needed
        };

        private static readonly string OpenUPMRegistryName = "OpenUPM";
        private static readonly string OpenUPMRegistryUrl  = "https://package.openupm.com";

        private static readonly string[] RequiredScopes =
        {
            "com.google",
            "com.cysharp",
            "com.coffee",
            "org.nuget",
            "com.github-glitchenzo",
            "com.theone"
        };
        
        private static readonly Dictionary<string, string> PackagesToAdd = new()
        {
            // {"com.unity.adaptiveperformance", "5.1.0"},
            // {"com.unity.adaptiveperformance.samsung.android", "5.0.0"},
            {"com.google.external-dependency-manager", "1.2.180"},
            {"com.svermeulen.extenject", "https://github.com/svermeulen/Yazef.git?path=UnityProject/Assets/Plugins/Zenject/Source"}
            // add more packages as needed
        };

        private static readonly List<string> PackagesToRemove = new()
        {
            // "com.unity.adaptiveperformance.google.android"
        };


        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            CheckAndUpdateProguardFile();
            CheckAndUpdatePackageManagerSettings();
            FolderMigration.RemoveUselessFolder();
            ApplovinMigration.DoMigrate();
            ProjectSettingMigration.APICompatibilityLevel();
        }

        static ProjectAutoMigrationScript()
        {
            EditorApplication.focusChanged += (focus) =>
            {
                if (!focus) return;
                OnProjectLoadedInEditor();
            };
            OnProjectLoadedInEditor();
        }

        private static void CheckAndUpdateProguardFile()
        {
            if (File.Exists(ProguardUserFilePath))
            {
                var  existingLines = File.ReadAllLines(ProguardUserFilePath).ToList();
                bool updated       = false;

                foreach (var line in RequiredProguardLines)
                {
                    if (!existingLines.Contains(line))
                    {
                        existingLines.Add(line);
                        updated = true;
                    }
                }

                if (updated)
                {
                    File.WriteAllLines(ProguardUserFilePath, existingLines);
                    Debug.Log("Updated proguard-user.txt with missing lines.");
                }
            }
            else
            {
                Debug.LogWarning("proguard-user.txt not found at path: " + ProguardUserFilePath);
            }
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
                if (!dependencies.ContainsKey(package.Key))
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
                    updated                   = true;
                }
            }

            if (updated)
            {
                Debug.Log("Updated manifest.json with new packages and removed old packages.");
            }

            return updated;
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