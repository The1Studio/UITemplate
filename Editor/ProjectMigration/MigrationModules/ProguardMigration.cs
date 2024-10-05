namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System.IO;
    using System.Linq;
    using UnityEngine;

    public static class ProguardMigration
    {
        private const string ProguardUserFilePath = "Assets/Plugins/Android/proguard-user.txt";
        
        private static readonly string[] RequiredProguardLines =
        {
            "-keep class com.bytebrew.** {*; }",
            "-keep class com.google.unity.ads.**{ *; }"
            // Add more lines as needed
        };


        public static void CheckAndUpdateProguardFile()
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
    }
}