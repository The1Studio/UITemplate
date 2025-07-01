# Package Migration Configuration

This folder contains the configuration files for package migration across different Unity versions.

## File Structure

- `PackageMigrationConfig.json` - **Unified configuration file** (contains base + all variants)
- `PackageMigrationConfig_base.json` - Legacy base config (kept for reference)
- `PackageMigrationConfig_unity2022_variant.json` - Legacy variant config (kept for reference)
- `PackageMigrationConfig_unity6.json` - Legacy config (kept for reference)
- `PackageMigrationConfig_unity2022.json` - Legacy config (kept for reference)

## New Unified Structure

The system now uses a **single JSON file** with base configuration and variants:

```json
{
  "base": {
    // Base configuration (Unity 6)
  },
  "variants": {
    "unity2022": {
      // Unity 2022 specific overrides
    },
    "unity2021": {
      // Unity 2021 specific overrides
    }
  }
}
```

## How It Works

1. **Base Config**: Contains the complete configuration for Unity 6
2. **Variants**: Contains only the differences for specific Unity versions
3. **Automatic Selection**: The system automatically selects the appropriate variant based on Unity version
4. **Smart Merging**: Base config is merged with the selected variant

## Unity Version Mapping

- **Unity 6+**: Uses base config as-is
- **Unity 2022.3+**: Uses base + `unity2022` variant
- **Unity 2021.3+**: Uses base + `unity2021` variant
- **Older versions**: Uses base config

## Maintenance

### Adding New Unity Versions

1. Add a new variant to `PackageMigrationConfig.json`:
   ```json
   "variants": {
     "unity2020": {
       "packagesVersionToUse": {
         "com.google.ads.mobile": "8.0.0"
       }
     }
   }
   ```

2. Update `GetUnityVariantKey()` in `PackageMigration.cs`:
   ```csharp
   #elif UNITY_2020_3_OR_NEWER
       return "unity2020";
   ```

### Updating Package Versions

- **For Unity 6**: Update the `base` section
- **For specific Unity version**: Update the corresponding variant
- **For multiple versions**: Update base and remove from variants if needed

### Adding New Packages

- **For Unity 6 only**: Add to `base` section
- **For specific Unity version**: Add to the corresponding variant
- **For multiple versions**: Add to base config

## Key Differences Between Unity Versions

### Unity 2022 vs Base (Unity 6)
- **Registries**: TheOne registry doesn't include "com.bytebrew" scope
- **Package Versions**:
  - `com.google.ads.mobile`: 10.1.0 → 9.4.0
  - AppLovin adapter versions are different
  - Includes `com.applovin.mediation.adapters.yandex.android`

### Unity 2021 vs Base (Unity 6)
- **Package Versions**:
  - `com.google.ads.mobile`: 10.1.0 → 8.6.0
  - AppLovin adapter versions are older

## Tools and Utilities

### Menu Items (TheOne > Package Migration)

1. **Show All Variants** - Displays the structure of all variants
2. **Compare Base vs Unity 2022** - Shows differences between base and Unity 2022
3. **Compare Base vs Unity 2021** - Shows differences between base and Unity 2021
4. **Show Current Unity Version Config** - Shows the merged config for current Unity version
5. **Validate All Variants** - Validates that all variants can be merged correctly

## Benefits

1. **🎯 Single File Management**: All configurations in one place
2. **📊 Clear Structure**: Easy to see base vs variants
3. **🔄 Automatic Selection**: No manual file selection needed
4. **🛠️ Easy Maintenance**: Add new Unity versions with minimal effort
5. **✅ Validation Tools**: Built-in tools to verify configurations
6. **📈 Scalable**: Easy to add more Unity versions

## Migration from Old System

The old system with separate files is still supported for backward compatibility, but the new unified system is recommended for new projects and easier maintenance. 