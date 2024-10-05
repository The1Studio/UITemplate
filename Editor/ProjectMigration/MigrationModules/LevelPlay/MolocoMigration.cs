namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules.LevelPlay
{
    using System.IO;
    using ServiceImplementation.Configs.Common;
    using UnityEngine;

    public static class MolocoMigration
    {
        private const string MolocoIpodName = "IronSourceMolocoAdapter";

        public static void UpdateIosPostProcess()
        {
            var levelplayEditor = "LevelPlay/Editor";

            var embedPath  = Path.Combine(Application.dataPath, levelplayEditor, "EmbedMoloco.cs");
            var macosxPath = Path.Combine(Application.dataPath, levelplayEditor, "__MACOSX");

            if (!LevelPlayMigration.IsIOSHasAdapters(MolocoIpodName))
            {
                if (File.Exists(embedPath))
                {
                    File.Delete(embedPath);
                }

                var macosXEmbedPath = Path.Combine(macosxPath, "._embedMoloco.cs");
                if (Directory.Exists(macosxPath) && File.Exists(macosXEmbedPath))
                {
                    if (Directory.GetFiles(macosxPath, "*.*", SearchOption.AllDirectories).Length <= 1)
                    {
                        Directory.Delete(macosxPath, true);
                        File.Delete($"{macosxPath}.meta");
                    }
                    else
                    {
                        File.Delete(macosXEmbedPath);
                    }
                }
            }
            else
            {
                if (File.Exists(embedPath))
                {
                    return;
                }

                UnityPackageHelper.DownloadThenUnZip("https://cdn.builds.the1studio.org/SDK/LevelPlay/EmbedMoloco.cs_.zip", "EmbedMoloco", levelplayEditor).Forget();
            }
        }
    }
}