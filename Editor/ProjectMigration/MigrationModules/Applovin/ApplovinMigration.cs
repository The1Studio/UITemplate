namespace UITemplate.Editor.ProjectMigration.MigrationModules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Cysharp.Threading.Tasks;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

#if APPLOVIN
    using AppLovinMax.Scripts.IntegrationManager.Editor;
#endif

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