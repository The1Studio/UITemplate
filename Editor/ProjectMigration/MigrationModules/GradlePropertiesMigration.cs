namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Changes in the gradle.properties file.
    /// </summary>
    public class GradlePropertiesMigration
    {
        private const string GradlePropertiesPath = "Assets/Plugins/Android/gradleTemplate.properties";

        private static Dictionary<string, string> PropertyToValue = new()
        {
            {"org.gradle.daemon", "false"},
            {"org.gradle.caching", "true"},
            {"org.gradle.console", "plain"},
            {"org.gradle.logging.level", "info"},
            {"android.useAndroidX", "true"},
            {"android.enableJetifier", "true"},
        };

        // Check if the gradle.properties file exists and update the properties
        public static void CheckAndUpdateGradleProperties()
        {
            if (System.IO.File.Exists(GradlePropertiesPath))
            {
                var lines   = System.IO.File.ReadAllLines(GradlePropertiesPath).ToList();
                var updated = false;

                foreach (var property in PropertyToValue)
                {
                    var propertyLine      = $"{property.Key}={property.Value}";
                    var existingLineIndex = lines.FindIndex(line => line.StartsWith(property.Key));

                    if (existingLineIndex >= 0)
                    {
                        if (!lines[existingLineIndex].Equals(propertyLine))
                        {
                            // Update the existing property value
                            lines[existingLineIndex] = propertyLine;
                            updated                  = true;
                        }
                    }
                    else
                    {
                        // Add the new property
                        lines.Add(propertyLine);
                        updated = true;
                    }
                }

                if (updated)
                {
                    System.IO.File.WriteAllLines(GradlePropertiesPath, lines);
                    UnityEngine.Debug.Log("Updated gradle.properties with missing properties.");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("gradle.properties not found at path: " + GradlePropertiesPath);
            }
        }
    }
}