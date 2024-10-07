namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules.Applovin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
#if APPLOVIN
    using AppLovinMax.Scripts.IntegrationManager.Editor;
#endif
    using Cysharp.Threading.Tasks;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class ApplovinMigration
    {
        /// <summary>
        /// This method is responsible for migrating the AppLovin SDK.
        /// It fetches the latest version information from the AppLovin server and compares it with the current version.
        /// If the current version is lesser than the latest version, it prompts the user to upgrade.
        /// After the upgrade process, it changes the version of the iosPods in all Dependencies.xml files according to a predefined dictionary.
        /// </summary>
        public static async void DoMigrate()
        {
#if APPLOVIN
            // URL to fetch the latest version information of the AppLovin SDK
            var url = $"https://unity.applovin.com/max/1.0/integration_manager_info?plugin_version={MaxSdk.Version}";

            // Send a GET request to the URL
            using var www       = UnityWebRequest.Get(url);
            var       operation = www.SendWebRequest();

            // Wait for the request to complete
            await operation;

            // If the request was not successful, exit the method
            if (www.result != UnityWebRequest.Result.Success) return;

            PluginData pluginData;
            try
            {
                // Parse the response text into a PluginData object
                pluginData = JsonUtility.FromJson<PluginData>(www.downloadHandler.text);
            }
            catch (Exception exception)
            {
                // If an error occurred while parsing the response text, log the error and set pluginData to null
                Console.WriteLine(exception);
                pluginData = null;
            }

            if (pluginData != null)
            {
                // Get the current version of the AppLovin SDK
                var appLovinMax = pluginData.AppLovinMax;

                // Update the current versions of the AppLovin SDK and its adapters
                AppLovinIntegrationManager.UpdateCurrentVersions(appLovinMax, AppLovinIntegrationManager.PluginParentDirectory);

                // Compare the current version of the AppLovin SDK with the latest version
                var maxVersionComparisonResult = MaxSdkUtils.CompareVersions(MaxSdk.Version, appLovinMax.LatestVersions.Unity);

                // If the current version is lesser than the latest version
                if (maxVersionComparisonResult == MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    // Show a dialog with a message to upgrade AppLovin
                    EditorUtility.DisplayDialog(
                        "Upgrade AppLovin",
                        $"Your current version of AppLovin is {MaxSdk.Version}. The latest version is {appLovinMax.LatestVersions.Unity}. Please upgrade AppLovin to the latest version.",
                        "OK"
                    );

                    // Start the process of downloading the latest version of the AppLovin SDK
                    AppLovinEditorCoroutine.StartCoroutine(AppLovinIntegrationManager.Instance.DownloadPlugin(appLovinMax, false));

                    // Show the AppLovin Integration Manager window
                    AppLovinIntegrationManagerWindow.ShowManager();

                    // Show a dialog with a message to upgrade the AppLovin adapters
                    EditorUtility.DisplayDialog(
                        "Upgrade AppLovin Adapters",
                        "One or more of your AppLovin adapters need to be upgraded. Please upgrade to the latest version.",
                        "OK"
                    );
                }
            }

            // Change the version of the iosPods in all Dependencies.xml files according to the predefined dictionary
            ChangeIosPodVersionInAllFiles();

#endif
        }

        // Define a global dictionary to store the iosPodName and newVersion pairs
        private static readonly Dictionary<string, string> IOSPodVersions = new()
        {
            { "AppLovinMediationGoogleAdManagerAdapter", "11.10.0.0" },
            // Add other iosPodName and newVersion pairs as needed
        };

        private static void ChangeIosPodVersionInAllFiles()
        {
            const string rootPath      = "Assets/MaxSdk/Mediation";
            const string searchPattern = "Dependencies.xml";

            var filePaths = Directory.GetFiles(rootPath, searchPattern, SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
            {
                var doc = new XmlDocument();
                doc.Load(filePath);

                var iosPodNodes = doc.SelectNodes("//iosPod");

                if (iosPodNodes != null)
                    foreach (XmlNode iosPodNode in iosPodNodes)
                    {
                        // Check if the name attribute of the iosPod node is in the dictionary
                        if (iosPodNode.Attributes != null && IOSPodVersions.ContainsKey(iosPodNode.Attributes["name"].Value))
                        {
                            // Change the version attribute to the new version from the dictionary
                            iosPodNode.Attributes["version"].Value = IOSPodVersions[iosPodNode.Attributes["name"].Value];
                        }
                    }

                doc.Save(filePath);
            }
        }
    }
}