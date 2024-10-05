/// <summary>
/// This namespace contains classes related to the migration of the LevelPlay project.
/// </summary>

namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules.LevelPlay
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using UnityEngine;

    /// <summary>
    /// This class contains methods for migrating the LevelPlay project.
    /// </summary>
    public static class LevelPlayMigration
    {
        /// <summary>
        /// A dictionary that maps the names of iosPods to their new versions.
        /// </summary>
        private static readonly Dictionary<string, string> IOSPodVersions = new()
        {
            // { "IronSourceMaioAdapter", "4.1.11.11" },
            // Add other iosPodName and newVersion pairs as needed
        };
        
        /// <summary>
        /// This method is the entry point for the migration of the LevelPlay project.
        /// It changes the version of the iosPods in all AdapterDependencies.xml files according to the predefined dictionary.
        /// </summary>
        public static void DoMigration()
        {
#if IRONSOURCE
            ChangeIosPodVersionInAllFiles();
            MolocoMigration.UpdateIosPostProcess();
#endif
        }
        

        /// <summary>
        /// This method changes the version of the iosPods in all AdapterDependencies.xml files according to the predefined dictionary.
        /// It searches for all XML files with names ending in AdapterDependencies.xml in the Assets/LevelPlay/Editor directory and its subdirectories.
        /// </summary>
        private static void ChangeIosPodVersionInAllFiles()
        {
            var dependencies = GetAllDepedenciesDoc();

            foreach (var (doc, filePath) in dependencies)
            {
                var iosPodNodes = doc.SelectNodes("//iosPod");

                var updated = false;
                if (iosPodNodes != null)
                {
                    foreach (XmlNode iosPodNode in iosPodNodes)
                    {
                        // Check if the name attribute of the iosPod node is in the dictionary
                        var ipodName = iosPodNode.Attributes["name"].Value;
                        if (iosPodNode.Attributes != null && IOSPodVersions.ContainsKey(ipodName) && !iosPodNode.Attributes["version"].Value.Equals(IOSPodVersions[ipodName]))
                        {
                            // Change the version attribute to the new version from the dictionary
                            iosPodNode.Attributes["version"].Value = IOSPodVersions[ipodName];
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    doc.Save(filePath);
                    Debug.Log($"Updated {filePath}");
                }
            }
        }

        private static (XmlDocument, string)[] GetAllDepedenciesDoc()
        {
            var rootPath      = "Assets/LevelPlay/Editor";
            var searchPattern = "*AdapterDependencies.xml";

            var filePaths = Directory.GetFiles(rootPath, searchPattern, SearchOption.AllDirectories);
            return filePaths.Select(filePath =>
            {
                var doc = new XmlDocument();
                doc.Load(filePath);
                return (doc, filePath);
            }).ToArray();
        }

        public static bool IsIOSHasAdapters(string adapterName)
        {
            var depedencies = GetAllDepedenciesDoc();

            foreach (var (doc, filePath) in depedencies)
            {
                var iosPodNodes = doc.SelectNodes("//iosPod");

                if (iosPodNodes != null)
                {
                    foreach (XmlNode iosPodNode in iosPodNodes)
                    {
                        // Check if the name attribute of the iosPod node is in the dictionary
                        if (iosPodNode.Attributes != null && adapterName.Equals(iosPodNode.Attributes["name"].Value))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}