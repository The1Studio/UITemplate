namespace UITemplate.Editor.ProjectMigration
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEngine;
    using UnityEngine.Networking;

    [InitializeOnLoad]
    public class FirebasePackagesUpdater : MonoBehaviour
    {
        private const string PackagesDirectory = "Packages/GoogleDependency";

        private const string BaseUrl = "https://dl.google.com/games/registry/unity/";

        private static readonly List<string> RequirePackages = new()
        {
            "com.google.firebase.analytics",
            "com.google.firebase.app",
            "com.google.firebase.crashlytics",
            "com.google.firebase.messaging",
            "com.google.firebase.remote-config"
        };

        static FirebasePackagesUpdater() { UpdatePackageToVersion(); }

        private static void UpdatePackageToVersion()
        {
            Debug.Log($"Update {RequirePackages.Count} Firebase packages to version 12.0.0");

            UpdateFirebasePackages(RequirePackages, "12.0.0");
        }

        private static async void UpdateFirebasePackages(List<string> packages, string version)
        {
            var packagesDirectoryPath = Path.Combine(Application.dataPath, "..", PackagesDirectory);
            var existingPackages = Directory.GetFiles(packagesDirectoryPath, "*.tgz")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
            
            // Check if all the packages of the required version already exist
            if (packages.All(package => existingPackages.Any(p => p.StartsWith(package) && p == package + "-" + version)))
            {
                Debug.Log("All packages of the required version already exist. No need to update.");
                return;
            }
            
            // Show a dialog box that blocks user interaction
            EditorUtility.DisplayDialog($"Update firebase packages to version: {version}", "Please wait while the packages are being downloaded...", "OK");

            // Disable the "OK" button
            EditorUtility.DisplayProgressBar("Download in progress", "Please wait while the packages are being downloaded...", 0);

            var completedDownloads = 0;
            
            foreach (var package in packages)
            {
                // Check if the package of the same version already exists
                var expectedPackageName = package + "-" + version;
                if (existingPackages.Any(p => p.StartsWith(package) && p == expectedPackageName))
                {
                    Debug.Log($"Package {package} of version {version} already exists. Skipping download.");
                    continue;
                }
                
                // Get the old version of the package from the existing files
                var oldPackageFile = existingPackages.FirstOrDefault(p => p.Contains(package));
                if (oldPackageFile != null)
                {
                    Debug.Log($"Deleting old package {oldPackageFile}");
                    // Delete the old package file
                    var oldPackagePath = Path.Combine(packagesDirectoryPath, oldPackageFile + ".tgz");
                    if (File.Exists(oldPackagePath))
                    {
                        File.Delete(oldPackagePath);
                    }
                }

                // Download the new package file
                var packageUrl = BaseUrl + package + "/" + package + "-" + version + ".tgz";
                using (var request = UnityWebRequest.Get(packageUrl))
                {
                    await request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Failed to download .tgz file: " + request.error);
                        continue;
                    }

                    var downloadPath = Path.Combine(Application.dataPath, "..", PackagesDirectory, Path.GetFileName(packageUrl));
                    await File.WriteAllBytesAsync(downloadPath, request.downloadHandler.data);

                    // Update the progress
                    completedDownloads++;
                    float progress = (float)completedDownloads / packages.Count;
                    EditorUtility.DisplayProgressBar("Download in progress", $"Downloading {completedDownloads} of {packages.Count} packages...", progress);
                }
            }
            
            // Enable the "OK" button when the download is complete
            EditorUtility.ClearProgressBar();

            UpdateManifestJson(packages, version);
        }

        private static void UpdateManifestJson(List<string> packages, string version)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest     = JObject.Parse(manifestJson);

            var dependencies = manifest["dependencies"] as JObject;
            if (dependencies == null)
            {
                dependencies             = new JObject();
                manifest["dependencies"] = dependencies;
            }

            foreach (var package in packages)
            {
                dependencies[package] = "file:GoogleDependency/" + package + "-" + version + ".tgz";
            }

            File.WriteAllText(manifestPath, manifest.ToString());
            Client.Resolve();
        }
    }
}