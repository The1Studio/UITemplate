namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueprintFlow.BlueprintReader;
    using BlueprintFlow.Signals;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    /// <summary>
    /// Service responsible for managing blueprint localization
    /// Automatically localizes fields marked with [LocalizableField] attribute
    /// </summary>
    public class BlueprintLocalizationService : IInitializable, IDisposable
    {
        private readonly SignalBus                            signalBus;
        private readonly StringTableLocalizationProvider      localizationProvider;
        private readonly IEnumerable<IGenericBlueprintReader> blueprints;
        private readonly ILogger                              logger;

        // Cache original values before localization
        private readonly Dictionary<IGenericBlueprintReader, List<LocalizableMember>> originalValuesCache = new();

        [Preserve] public BlueprintLocalizationService(
            SignalBus                            signalBus,
            StringTableLocalizationProvider      localizationProvider,
            IEnumerable<IGenericBlueprintReader> blueprints,
            ILoggerManager                       loggerManager
        )
        {
            this.signalBus            = signalBus;
            this.localizationProvider = localizationProvider;
            this.blueprints           = blueprints;
            this.logger               = loggerManager.GetLogger(this);
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<LoadBlueprintDataSucceedSignal>(this.CacheOriginalValues);
        }

        public void Dispose()
        {
            this.originalValuesCache.Clear();
            this.signalBus.Unsubscribe<LoadBlueprintDataSucceedSignal>(this.CacheOriginalValues);
        }

        /// <summary>
        /// Cache original values from all blueprint fields before any localization
        /// </summary>
        private void CacheOriginalValues()
        {
            foreach (var blueprint in this.blueprints)
            {
                var fieldsData = this.GetLocalizableFieldsData(blueprint);

                if (fieldsData.Count > 0)
                {
                    this.originalValuesCache[blueprint] = fieldsData;
                }
            }

            this.logger?.Info($"trong: Cached original values for {this.originalValuesCache.Count} blueprints of {this.blueprints.Count()}");
        }

        /// <summary>
        /// Localize all fields in all blueprint readers
        /// </summary>
        public void LocalizeAllBlueprintFields()
        {
            var totalFieldsCount = 0;
            var blueprintCount   = 0;

            foreach (var blueprint in this.originalValuesCache.Keys)
            {
                var localizedFields = this.LocalizeBlueprintFields(blueprint);
                if (localizedFields > 0)
                {
                    totalFieldsCount += localizedFields;
                    blueprintCount++;
                }
            }
            this.logger?.Info($"trong: Localized {totalFieldsCount} fields in {blueprintCount} blueprints");
        }

        /// <summary>
        /// Localize fields in a specific blueprint reader
        /// </summary>
        private int LocalizeBlueprintFields(IGenericBlueprintReader blueprint)
        {
            if (blueprint == null) return 0;

            var localizedCount = 0;

            this.LocalizeRecordFields(blueprint);

            return localizedCount;
        }
        /// <summary>
        /// Localize fields in a record object
        /// </summary>
        private int LocalizeRecordFields(IGenericBlueprintReader blueprint)
        {

            var localizedCount = 0;
            this.originalValuesCache.TryGetValue(blueprint, out var members);

            if (members != null)
                foreach (var member in members)
                {
                    if (this.LocalizeField(member))
                    {
                        localizedCount++;
                    }
                }

            return localizedCount;
        }

        /// <summary>
        /// Localize a specific field
        /// </summary>
        private bool LocalizeField(LocalizableMember member)
        {

            var localizationKey = member.OriginalValue;

            var localizedValue = this.localizationProvider.GetLocalizedText(localizationKey);

            if (!member.Property.CanWrite) return false;
            member.Property.SetValue(member.Record, localizedValue);
            return true;

        }
        private List<LocalizableMember> GetLocalizableFieldsData(IGenericBlueprintReader blueprint)
        {
            var fieldsData    = new List<LocalizableMember>();
            var blueprintType = blueprint.GetType();

            this.logger?.Info($"GetLocalizableFieldsData for {blueprintType.Name}");
            this.logger?.Info($"IsGenericBlueprintByRow: {this.IsGenericBlueprintByRow(blueprintType)}");
            this.logger?.Info($"IsGenericBlueprintByCol: {this.IsGenericBlueprintByCol(blueprintType)}");

            if (this.IsGenericBlueprintByRow(blueprintType))
            {
                var valuesProperty = blueprintType.GetProperty("Values");
                this.logger?.Info($"Values property found: {valuesProperty != null}");

                if (valuesProperty != null)
                {
                    var valuesObject = valuesProperty.GetValue(blueprint);
                    this.logger?.Info($"trong: Values object type: {valuesObject?.GetType().Name ?? "null"}");
                    this.logger?.Info($"trong: Values object is null: {valuesObject == null}");

                    if (valuesObject != null)
                    {
                        // For GenericBlueprintReaderByRow, Values is typically a Dictionary<TKey, TRecord>
                        // We need to access the Values property of the dictionary to get the records
                        if (valuesObject is IDictionary dictionary)
                        {
                            this.logger?.Info($"trong: Values is Dictionary with {dictionary.Count} items");
                            var recordCount = 0;
                            foreach (var kvp in dictionary)
                            {
                                // kvp is DictionaryEntry, kvp.Value is the actual record
                                var record = ((DictionaryEntry)kvp).Value;
                                recordCount++;
                                this.CacheRecordFields(record, fieldsData);
                            }
                            this.logger?.Info($"trong: Processed {recordCount} records total");
                        }
                        else if (valuesObject is IEnumerable records)
                        {
                            this.logger?.Info($"trong: Values is IEnumerable");
                            foreach (var record in records)
                            {
                                // If it's KeyValuePair, get the Value
                                var actualRecord = record;
                                var recordType = record.GetType();
                                if (recordType.IsGenericType && recordType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                                {
                                    var valueProperty = recordType.GetProperty("Value");
                                    actualRecord = valueProperty?.GetValue(record);
                                    this.logger?.Info($"trong: Extracted Value from KeyValuePair: {actualRecord?.GetType().Name ?? "null"}");
                                }

                                if (actualRecord != null)
                                {
                                    this.CacheRecordFields(actualRecord, fieldsData);
                                }
                            }
                        }
                        else
                        {
                            this.logger?.Info($"trong: Values property exists but is not Dictionary or IEnumerable. Type: {valuesObject.GetType().Name}");
                        }
                    }
                    else
                    {
                        this.logger?.Warning($"trong: Values property returned null for blueprint {blueprintType.Name}");
                    }
                }
                else
                {
                    this.logger?.Info($"trong: Blueprint {blueprintType.Name} should be ByRow but has no Values property!");
                }
            }
            else if (this.IsGenericBlueprintByCol(blueprintType))
            {
                this.logger?.Info("trong: Processing as ByCol blueprint");
                this.CacheRecordFields(blueprint, fieldsData);
            }
            else
            {
                this.logger?.Info($"trong: Blueprint type {blueprintType.Name} is neither ByRow nor ByCol");
            }

            return fieldsData;
        }

        private void CacheRecordFields(object record, List<LocalizableMember> fieldsData)
        {
            var recordType = record.GetType();
            var localizableFields = this.GetLocalizableFields(recordType);


            this.logger?.Info($"trong: Processing record type: {recordType.Name}, found {localizableFields.Count()} localizable fields");

            foreach (var property in localizableFields)
            {
                this.logger?.Info($"trong: Checking property: {property.Name}, type: {property.PropertyType.Name}");

                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(record) as string;
                    this.logger?.Info($"trong: Property {property.Name} value: '{value}'");

                    if (!string.IsNullOrEmpty(value))
                    {
                        fieldsData.Add(new()
                        {
                            Property      = property,
                            OriginalValue = value,
                            Record        = record,
                        });
                        this.logger?.Info($"trong: Added localizable member: {property.Name} = '{value}'");
                    }
                }
            }
        }
        private IEnumerable<PropertyInfo> GetLocalizableFields(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            this.logger?.Debug($"trong: Type {type.Name} has {properties.Length} total properties");

            var localizableFields = properties
                .Where(p => p.GetCustomAttribute<LocalizableFieldAttribute>() != null).ToList();

            this.logger?.Debug($"trong: Found {localizableFields.Count} localizable fields in {type.Name}");
            foreach (var field in localizableFields)
            {
                this.logger?.Debug($"  - {field.Name} (type: {field.PropertyType.Name})");
            }

            return localizableFields;
        }

        private bool IsGenericBlueprintByRow(Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.Contains("GenericBlueprintReaderByRow")) return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        private bool IsGenericBlueprintByCol(Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.Contains("GenericBlueprintReaderByCol")) return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }

    public class LocalizableMember
    {
        public PropertyInfo Property      { get; set; }
        public string       OriginalValue { get; set; }
        public object       Record        { get; set; }
    }
}