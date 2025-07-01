namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PackageMigrationConfig
    {
        [JsonProperty("base")]
        public Config Base { get; set; }

        [JsonProperty("variants")]
        public Dictionary<string, Config> Variants { get; set; }

        public class Config
        {
            [JsonProperty("registries")]
            public List<Registry> Registries { get; set; }

            [JsonProperty("packagesToAdd")]
            public Dictionary<string, string> PackagesToAdd { get; set; }

            [JsonProperty("packagesVersionToUse")]
            public Dictionary<string, string> PackagesVersionToUse { get; set; }

            [JsonProperty("nameToUnityPackageToImport")]
            public Dictionary<string, UnityPackage> NameToUnityPackageToImport { get; set; }

            [JsonProperty("packagesToRemove")]
            public List<string> PackagesToRemove { get; set; }

            [JsonProperty("webGLPackagesToRemove")]
            public List<string> WebGLPackagesToRemove { get; set; }

            public class Registry
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("url")]
                public string Url { get; set; }

                [JsonProperty("scopes")]
                public List<string> Scopes { get; set; }
            }

            public class UnityPackage
            {
                [JsonProperty("path")]
                public string Path { get; set; }

                [JsonProperty("url")]
                public string Url { get; set; }
            }

            public static Config Merge(Config baseConfig, Config variantConfig)
            {
                if (variantConfig == null) return baseConfig;

                var mergedConfig = new Config
                {
                    Registries = new List<Registry>(baseConfig.Registries ?? new List<Registry>()),
                    PackagesToAdd = new Dictionary<string, string>(baseConfig.PackagesToAdd ?? new Dictionary<string, string>()),
                    PackagesVersionToUse = new Dictionary<string, string>(baseConfig.PackagesVersionToUse ?? new Dictionary<string, string>()),
                    NameToUnityPackageToImport = new Dictionary<string, UnityPackage>(baseConfig.NameToUnityPackageToImport ?? new Dictionary<string, UnityPackage>()),
                    PackagesToRemove = new List<string>(baseConfig.PackagesToRemove ?? new List<string>()),
                    WebGLPackagesToRemove = new List<string>(baseConfig.WebGLPackagesToRemove ?? new List<string>())
                };

                // Merge registries - replace existing ones with same name
                if (variantConfig.Registries != null)
                {
                    foreach (var variantRegistry in variantConfig.Registries)
                    {
                        var existingIndex = mergedConfig.Registries.FindIndex(r => r.Name == variantRegistry.Name);
                        if (existingIndex >= 0)
                        {
                            mergedConfig.Registries[existingIndex] = variantRegistry;
                        }
                        else
                        {
                            mergedConfig.Registries.Add(variantRegistry);
                        }
                    }
                }

                // Merge packages to add
                if (variantConfig.PackagesToAdd != null)
                {
                    foreach (var package in variantConfig.PackagesToAdd)
                    {
                        mergedConfig.PackagesToAdd[package.Key] = package.Value;
                    }
                }

                // Merge package versions - variant overrides base
                if (variantConfig.PackagesVersionToUse != null)
                {
                    foreach (var package in variantConfig.PackagesVersionToUse)
                    {
                        mergedConfig.PackagesVersionToUse[package.Key] = package.Value;
                    }
                }

                // Merge unity packages to import
                if (variantConfig.NameToUnityPackageToImport != null)
                {
                    foreach (var package in variantConfig.NameToUnityPackageToImport)
                    {
                        mergedConfig.NameToUnityPackageToImport[package.Key] = package.Value;
                    }
                }

                // Merge packages to remove
                if (variantConfig.PackagesToRemove != null)
                {
                    foreach (var package in variantConfig.PackagesToRemove)
                    {
                        if (!mergedConfig.PackagesToRemove.Contains(package))
                        {
                            mergedConfig.PackagesToRemove.Add(package);
                        }
                    }
                }

                // Merge WebGL packages to remove
                if (variantConfig.WebGLPackagesToRemove != null)
                {
                    foreach (var package in variantConfig.WebGLPackagesToRemove)
                    {
                        if (!mergedConfig.WebGLPackagesToRemove.Contains(package))
                        {
                            mergedConfig.WebGLPackagesToRemove.Add(package);
                        }
                    }
                }

                return mergedConfig;
            }
        }
    }
} 