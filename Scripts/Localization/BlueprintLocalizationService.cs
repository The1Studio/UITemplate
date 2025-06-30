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
            var fieldsData = new List<LocalizableMember>();
            var blueprintType = blueprint.GetType();

            this.logger?.Info($"GetLocalizableFieldsData for {blueprintType.Name}");
            this.logger?.Info($"IsGenericBlueprintByRow (IBlueprintCollection): {this.IsGenericBlueprintByRow(blueprintType)}");
            this.logger?.Info($"IsGenericBlueprintByCol: {this.IsGenericBlueprintByCol(blueprintType)}");

            if (this.IsGenericBlueprintByRow(blueprintType))
            {
                this.ProcessBlueprintCollectionData(blueprint, fieldsData);
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

        /// <summary>
        /// Process IBlueprintCollection data (can be Dictionary or List based)
        /// </summary>
        private void ProcessBlueprintCollectionData(object blueprint, List<LocalizableMember> fieldsData)
        {
            var blueprintType = blueprint.GetType();
            this.logger?.Info($"trong: Processing IBlueprintCollection: {blueprintType.Name}");

            if (this.IsDictionaryBlueprintByRow(blueprintType))
            {
                this.ProcessDictionaryBlueprintData(blueprint, fieldsData);
            }
            else if (this.IsListBlueprintByRow(blueprintType))
            {
                this.ProcessListBlueprintData(blueprint, fieldsData);
            }
            else
            {
                // For GenericBlueprintReaderByRow, check for Values property
                this.ProcessGenericBlueprintReaderData(blueprint, fieldsData);
            }
        }

        /// <summary>
        /// Process Dictionary-based BlueprintByRow<TKey, TRecord>
        /// </summary>
        private void ProcessDictionaryBlueprintData(object blueprint, List<LocalizableMember> fieldsData)
        {
            this.logger?.Info($"trong: Processing Dictionary-based blueprint");

            if (blueprint is IDictionary dictionary)
            {
                this.logger?.Info($"trong: Dictionary with {dictionary.Count} items");
                var recordCount = 0;
                foreach (DictionaryEntry kvp in dictionary)
                {
                    var record = kvp.Value;
                    recordCount++;
                    this.logger?.Info($"trong: Processing dictionary record {recordCount}: {record.GetType().Name}");
                    this.CacheRecordFields(record, fieldsData);
                }
                this.logger?.Info($"trong: Processed {recordCount} dictionary records total");
            }
        }

        /// <summary>
        /// Process List-based BlueprintByRow<TRecord>
        /// </summary>
        private void ProcessListBlueprintData(object blueprint, List<LocalizableMember> fieldsData)
        {
            this.logger?.Info($"trong: Processing List-based blueprint");

            if (blueprint is IEnumerable enumerable)
            {
                var recordCount = 0;
                foreach (var record in enumerable)
                {
                    recordCount++;
                    this.logger?.Info($"trong: Processing list record {recordCount}: {record.GetType().Name}");
                    this.CacheRecordFields(record, fieldsData);
                }
                this.logger?.Info($"trong: Processed {recordCount} list records total");
            }
        }

        /// <summary>
        /// Process GenericBlueprintReaderByRow (has Values property)
        /// </summary>
        private void ProcessGenericBlueprintReaderData(object blueprint, List<LocalizableMember> fieldsData)
        {
            var blueprintType = blueprint.GetType();
            var valuesProperty = blueprintType.GetProperty("Values");

            this.logger?.Info($"trong: Processing GenericBlueprintReader: {blueprintType.Name}");
            this.logger?.Info($"Values property found: {valuesProperty != null}");

            if (valuesProperty != null)
            {
                var valuesObject = valuesProperty.GetValue(blueprint);
                this.logger?.Info($"trong: Values object type: {valuesObject?.GetType().Name ?? "null"}");

                if (valuesObject != null)
                {
                    // Recursively process the Values object (which should be IBlueprintCollection)
                    if (typeof(IBlueprintCollection).IsAssignableFrom(valuesObject.GetType()))
                    {
                        this.logger?.Info($"trong: Values object is IBlueprintCollection, processing recursively");
                        this.ProcessBlueprintCollectionData(valuesObject, fieldsData);
                    }
                    else
                    {
                        this.logger?.Warning($"trong: Values object is not IBlueprintCollection: {valuesObject.GetType().Name}");
                    }
                }
                else
                {
                    this.logger?.Warning($"trong: Values property returned null for blueprint {blueprintType.Name}");
                }
            }
            else
            {
                this.logger?.Warning($"trong: GenericBlueprintReader {blueprintType.Name} has no Values property!");
            }
        }

        private void CacheRecordFields(object record, List<LocalizableMember> fieldsData)
        {
            var recordType = record.GetType();
            var localizableFields = this.GetLocalizableFields(recordType).ToList();


            this.logger?.Info($"trong: Processing record type: {recordType.Name}, found {localizableFields.Count()} localizable fields");

            foreach (var property in localizableFields)
            {
                if (property.PropertyType != typeof(string)) continue;
                var value = property.GetValue(record) as string;
                this.logger?.Info($"trong: Property {property.Name} value: '{value}'");

                if (string.IsNullOrEmpty(value)) continue;
                fieldsData.Add(new()
                {
                    Property      = property,
                    OriginalValue = value,
                    Record        = record,
                });
                this.logger?.Info($"trong: Added localizable member: {property.Name} = '{value}'");
            }

            this.CacheNestedBlueprintCollectionFields(record, fieldsData);
        }

        /// <summary>
        /// Cache nested IBlueprintCollection fields recursively
        /// </summary>
        private void CacheNestedBlueprintCollectionFields(object record, List<LocalizableMember> fieldsData)
        {
            var recordType = record.GetType();
            var properties = recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                // Check if this property implements IBlueprintCollection
                if (typeof(IBlueprintCollection).IsAssignableFrom(propertyType))
                {
                    this.logger?.Info($"trong: Found nested IBlueprintCollection property: {property.Name} of type {propertyType.Name}");

                    var nestedBlueprint = property.GetValue(record);
                    if (nestedBlueprint != null)
                    {
                        this.logger?.Info($"trong: Processing nested blueprint collection: {property.Name}");
                        this.ProcessBlueprintCollectionData(nestedBlueprint, fieldsData);
                    }
                    else
                    {
                        this.logger?.Info($"trong: Nested blueprint collection {property.Name} is null");
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
            // Check if type implements IBlueprintCollection
            return typeof(IBlueprintCollection).IsAssignableFrom(type);
        }

        private bool IsGenericBlueprintByCol(Type type)
        {
            // Check if type implements IGenericBlueprintReader but NOT IBlueprintCollection
            return typeof(IGenericBlueprintReader).IsAssignableFrom(type) &&
                   !typeof(IBlueprintCollection).IsAssignableFrom(type);
        }

        /// <summary>
        /// Check if type is Dictionary-based BlueprintByRow (has TKey, TRecord generics)
        /// </summary>
        private bool IsDictionaryBlueprintByRow(Type type)
        {
            if (!type.IsGenericType) return false;

            var genericArgs = type.GetGenericArguments();
            // BlueprintByRow<TKey, TRecord> has 2 generic arguments
            return genericArgs.Length == 2 && typeof(IBlueprintCollection).IsAssignableFrom(type);
        }

        /// <summary>
        /// Check if type is List-based BlueprintByRow (has only TRecord generic)
        /// </summary>
        private bool IsListBlueprintByRow(Type type)
        {
            if (!type.IsGenericType) return false;

            var genericArgs = type.GetGenericArguments();
            // BlueprintByRow<TRecord> has 1 generic argument
            return genericArgs.Length == 1 && typeof(IBlueprintCollection).IsAssignableFrom(type);
        }
    }

    public class LocalizableMember
    {
        public PropertyInfo Property      { get; set; }
        public string       OriginalValue { get; set; }
        public object       Record        { get; set; }
    }
}