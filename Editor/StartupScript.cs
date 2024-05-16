using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace UITemplate.Editor
{
    [InitializeOnLoad]  
    public class StartupScript
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

        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            CheckAndUpdateProguardFile();
            CheckAndUpdatePackageManagerSettings();
        }

        static StartupScript()
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
                File.WriteAllText(manifestPath, manifest.ToString());
                Debug.Log("Updated manifest.json with missing OpenUPM scopes.");
            }
        }

        [System.Serializable]
        private class Manifest
        {
            public List<ScopedRegistry> scopedRegistries = new();
        }

        [System.Serializable]
        private class ScopedRegistry
        {
            public string   name;
            public string   url;
            public string[] scopes;
        }
    }
}
