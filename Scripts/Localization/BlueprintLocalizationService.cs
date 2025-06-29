namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueprintFlow.BlueprintControlFlow;
    using BlueprintFlow.BlueprintReader;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using UnityEngine.Scripting;

    /// <summary>
    /// Service responsible for managing blueprint localization
    /// Automatically localizes fields marked with [LocalizableField] attribute
    /// </summary>
    public class BlueprintLocalizationService : IInitializable, IDisposable
    {
        private readonly SignalBus signalBus;
        private readonly UnityStringTableLocalizationProvider localizationProvider;
        private readonly BlueprintReaderManager blueprintReaderManager;
        private readonly ILogger logger;

        // Cache original values before localization
        private readonly Dictionary<object, Dictionary<string, string>> originalValuesCache;

        [Preserve]
        public BlueprintLocalizationService(
            SignalBus signalBus,
            UnityStringTableLocalizationProvider localizationProvider,
            BlueprintReaderManager blueprintReaderManager,
            ILoggerManager loggerManager)
        {
            this.signalBus = signalBus;
            this.localizationProvider = localizationProvider;
            this.blueprintReaderManager = blueprintReaderManager;
            this.logger = loggerManager.GetLogger(this);
            this.originalValuesCache = new Dictionary<object, Dictionary<string, string>>();
        }

        public void Initialize()
        {
            this.logger?.Info("Initializing Blueprint Localization Service");

            // Subscribe to language change signals
            this.signalBus.Subscribe<LanguageChangedSignal>(this.OnLanguageChanged);

            // Cache original values from all blueprints
            this.CacheOriginalValues();

            this.logger?.Info("Blueprint Localization Service initialized");
        }

        public void Dispose()
        {
            this.signalBus?.Unsubscribe<LanguageChangedSignal>(this.OnLanguageChanged);
            this.originalValuesCache.Clear();
        }

        /// <summary>
        /// Change language and localize all blueprint fields
        /// </summary>
        /// <param name="languageCode">Target language code</param>
        public async UniTask ChangeLanguageAsync(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                this.logger?.Error("Language code cannot be null or empty");
                return;
            }

            var oldLanguage = this.localizationProvider.CurrentLanguage;

            this.logger?.Info($"Changing language from '{oldLanguage}' to '{languageCode}'");

            // Fire language changing signal
            this.signalBus.Fire(new LanguageChangingSignal
            {
                NewLanguage = languageCode,
                CurrentLanguage = oldLanguage
            });

            try
            {
                // Load new language data
                await this.localizationProvider.LoadLanguageAsync(languageCode);

                // Localize all blueprint fields
                var localizedCount = await this.LocalizeAllBlueprintFields();

                // Fire language changed signal
                this.signalBus.Fire(new LanguageChangedSignal
                {
                    NewLanguage = languageCode,
                    OldLanguage = oldLanguage
                });

                // Fire localization completed signal
                this.signalBus.Fire(new BlueprintLocalizationCompletedSignal
                {
                    Language = languageCode,
                    LocalizedFieldsCount = localizedCount.fieldsCount,
                    BlueprintCount = localizedCount.blueprintCount
                });

                this.logger?.Info($"Language changed successfully. Localized {localizedCount.fieldsCount} fields in {localizedCount.blueprintCount} blueprints");
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Failed to change language to '{languageCode}'");
                this.logger?.Exception(ex);
                throw;
            }
        }

        /// <summary>
        /// Handle language changed signal
        /// </summary>
        private async void OnLanguageChanged(LanguageChangedSignal signal)
        {
            // This is called after language has already been changed
            // We can add additional logic here if needed
            this.logger?.Info($"Language changed signal received: {signal.OldLanguage} -> {signal.NewLanguage}");
        }

        /// <summary>
        /// Cache original values from all blueprint fields before any localization
        /// </summary>
        private void CacheOriginalValues()
        {
            this.logger?.Info("Caching original values from blueprints");

            var blueprints = this.GetAllBlueprintReaders();
            var cachedCount = 0;

            foreach (var blueprint in blueprints)
            {
                var fieldsData = this.GetLocalizableFieldsData(blueprint);

                if (fieldsData.Count > 0)
                {
                    this.originalValuesCache[blueprint] = fieldsData;
                    cachedCount += fieldsData.Count;
                }
            }

            this.logger?.Info($"Cached {cachedCount} original values from {this.originalValuesCache.Count} blueprints");
        }

        /// <summary>
        /// Localize all fields in all blueprint readers
        /// </summary>
        private async UniTask<(int fieldsCount, int blueprintCount)> LocalizeAllBlueprintFields()
        {
            var totalFieldsCount = 0;
            var blueprintCount = 0;

            var blueprints = this.GetAllBlueprintReaders();

            foreach (var blueprint in blueprints)
            {
                var localizedFields = await this.LocalizeBlueprintFields(blueprint);
                if (localizedFields > 0)
                {
                    totalFieldsCount += localizedFields;
                    blueprintCount++;
                }
            }

            return (totalFieldsCount, blueprintCount);
        }

        /// <summary>
        /// Localize fields in a specific blueprint reader
        /// </summary>
        private async UniTask<int> LocalizeBlueprintFields(IGenericBlueprintReader blueprint)
        {
            if (blueprint == null) return 0;

            var localizedCount = 0;
            var blueprintType = blueprint.GetType();

            try
            {
                // Handle ByRow blueprints
                if (IsGenericBlueprintByRow(blueprintType))
                {
                    localizedCount += await this.LocalizeByRowBlueprint(blueprint);
                }
                // Handle ByCol blueprints
                else if (IsGenericBlueprintByCol(blueprintType))
                {
                    localizedCount += await this.LocalizeByColBlueprint(blueprint);
                }
            }
            catch (Exception ex)
            {
                this.logger?.Error($"Failed to localize blueprint: {blueprintType.Name}");
                this.logger?.Exception(ex);
            }

            return localizedCount;
        }

        /// <summary>
        /// Localize ByRow blueprint (key-value pairs)
        /// </summary>
        private async UniTask<int> LocalizeByRowBlueprint(IGenericBlueprintReader blueprint)
        {
            var localizedCount = 0;
            var blueprintType = blueprint.GetType();

            // Get the Values property (collection of records)
            var valuesProperty = blueprintType.GetProperty("Values");
            if (valuesProperty == null) return 0;

            var records = valuesProperty.GetValue(blueprint) as System.Collections.IEnumerable;
            if (records == null) return 0;

            foreach (var record in records)
            {
                localizedCount += await this.LocalizeRecordFields(record, blueprint);
            }

            return localizedCount;
        }

        /// <summary>
        /// Localize ByCol blueprint (column-based properties)
        /// </summary>
        private async UniTask<int> LocalizeByColBlueprint(IGenericBlueprintReader blueprint)
        {
            return await this.LocalizeRecordFields(blueprint, blueprint);
        }

        /// <summary>
        /// Localize fields in a record object
        /// </summary>
        private async UniTask<int> LocalizeRecordFields(object record, IGenericBlueprintReader blueprint)
        {
            if (record == null) return 0;

            var localizedCount = 0;
            var recordType = record.GetType();
            var localizableFields = this.GetLocalizableFields(recordType);

            foreach (var field in localizableFields)
            {
                try
                {
                    if (await this.LocalizeField(record, field, blueprint))
                    {
                        localizedCount++;
                    }
                }
                catch (Exception ex)
                {
                    this.logger?.Error($"Failed to localize field '{field.Name}' in '{recordType.Name}'");
                    this.logger?.Exception(ex);
                }
            }

            return localizedCount;
        }

        /// <summary>
        /// Localize a specific field
        /// </summary>
        private async UniTask<bool> LocalizeField(object record, PropertyInfo property, IGenericBlueprintReader blueprint)
        {
            if (property.PropertyType != typeof(string)) return false;

            var attribute = property.GetCustomAttribute<LocalizableFieldAttribute>();
            if (attribute == null) return false;

            // Get original value
            var originalValue = this.GetOriginalFieldValue(record, property.Name, blueprint);
            if (string.IsNullOrEmpty(originalValue)) return false;

            // Determine localization key
            var localizationKey = !string.IsNullOrEmpty(attribute.LocalizationKey)
                ? attribute.LocalizationKey
                : originalValue; // Use original value as key

            // Get localized value
            var localizedValue = this.localizationProvider.GetLocalizedText(localizationKey);
            
            // Use original value as fallback if localization not found
            if (localizedValue == localizationKey)
            {
                localizedValue = originalValue;
            }

            // Set localized value
            if (property.CanWrite)
            {
                property.SetValue(record, localizedValue);
                this.logger?.Debug($"Localized field '{property.Name}': '{originalValue}' -> '{localizedValue}'");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get original field value from cache
        /// </summary>
        private string GetOriginalFieldValue(object record, string fieldName, IGenericBlueprintReader blueprint)
        {
            if (this.originalValuesCache.TryGetValue(blueprint, out var fieldsData))
            {
                var key = $"{record.GetType().Name}.{fieldName}";
                if (fieldsData.TryGetValue(key, out var originalValue))
                {
                    return originalValue;
                }
            }

            // Fallback: get current value
            var property = record.GetType().GetProperty(fieldName);
            return property?.GetValue(record) as string ?? string.Empty;
        }

        /// <summary>
        /// Get localizable fields data for caching
        /// </summary>
        private Dictionary<string, string> GetLocalizableFieldsData(IGenericBlueprintReader blueprint)
        {
            var fieldsData = new Dictionary<string, string>();
            var blueprintType = blueprint.GetType();

            if (IsGenericBlueprintByRow(blueprintType))
            {
                var valuesProperty = blueprintType.GetProperty("Values");

                if (valuesProperty?.GetValue(blueprint) is IEnumerable records)
                {
                    foreach (var record in records)
                    {
                        this.CacheRecordFields(record, fieldsData);
                    }
                }
            }
            else if (IsGenericBlueprintByCol(blueprintType))
            {
                this.CacheRecordFields(blueprint, fieldsData);
            }

            return fieldsData;
        }

        /// <summary>
        /// Cache fields from a record
        /// </summary>
        private void CacheRecordFields(object record, Dictionary<string, string> fieldsData)
        {
            var recordType = record.GetType();
            var localizableFields = this.GetLocalizableFields(recordType);

            foreach (var property in localizableFields)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(record) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var key = $"{recordType.Name}.{property.Name}";
                        fieldsData[key] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Get all properties marked with LocalizableField attribute
        /// </summary>
        private IEnumerable<PropertyInfo> GetLocalizableFields(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<LocalizableFieldAttribute>() != null);
        }

        /// <summary>
        /// Get all blueprint readers from the manager
        /// </summary>
        private IEnumerable<IGenericBlueprintReader> GetAllBlueprintReaders()
        {
            // Use reflection to get the blueprints field from BlueprintReaderManager
            var blueprintsField = typeof(BlueprintReaderManager)
                .GetField("blueprints", BindingFlags.NonPublic | BindingFlags.Instance);

            if (blueprintsField?.GetValue(this.blueprintReaderManager) is IEnumerable<IGenericBlueprintReader> blueprints)
            {
                return blueprints;
            }

            this.logger?.Warning("Could not access blueprints from BlueprintReaderManager");
            return Enumerable.Empty<IGenericBlueprintReader>();
        }

        private static bool IsGenericBlueprintByRow(Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.Contains("GenericBlueprintReaderByRow"))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        private static bool IsGenericBlueprintByCol(Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.Contains("GenericBlueprintReaderByCol"))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}