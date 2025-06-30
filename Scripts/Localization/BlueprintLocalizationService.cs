namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueprintFlow.BlueprintReader;
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
        private readonly Dictionary<IGenericBlueprintReader, List<LocalizableDataRecord>> originalValuesCache = new();

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
            this.CacheOriginalValues();
        }

        public void Dispose()
        {
            this.originalValuesCache.Clear();
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

            this.logger?.Info($"trong: Cached original values for {this.originalValuesCache.Count} blueprints");
        }

        /// <summary>
        /// Localize all fields in all blueprint readers
        /// </summary>
        public async UniTask LocalizeAllBlueprintFields()
        {
            var totalFieldsCount = 0;
            var blueprintCount   = 0;

            foreach (var blueprint in this.originalValuesCache.Keys)
            {
                var localizedFields = await this.LocalizeBlueprintFields(blueprint);
                if (localizedFields > 0)
                {
                    totalFieldsCount += localizedFields;
                    blueprintCount++;
                }
            }
            this.logger?.Info($"Localized {totalFieldsCount} fields in {blueprintCount} blueprints");
        }

        /// <summary>
        /// Localize fields in a specific blueprint reader
        /// </summary>
        private async UniTask<int> LocalizeBlueprintFields(IGenericBlueprintReader blueprint)
        {
            if (blueprint == null) return 0;

            var localizedCount = 0;
            var blueprintType  = blueprint.GetType();

            try
            {
                // Handle ByRow blueprints
                if (this.IsGenericBlueprintByRow(blueprintType))
                {
                    localizedCount += await this.LocalizeByRowBlueprint(blueprint);
                }
                // Handle ByCol blueprints
                else if (this.IsGenericBlueprintByCol(blueprintType))
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
            var blueprintType  = blueprint.GetType();

            var valuesProperty = blueprintType.GetProperty("Values");
            if (valuesProperty == null) return 0;

            var records = valuesProperty.GetValue(blueprint) as IEnumerable;
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

            var localizedCount = 0;
            if (record == null) return localizedCount;
            this.originalValuesCache.TryGetValue(blueprint, out var fieldsData);
            if (fieldsData == null) return localizedCount;
            var localizableMembers = fieldsData
                .FirstOrDefault(data => data.Record == record)?.LocalizableMembers;
            if (localizableMembers == null || localizableMembers.Count == 0) return localizedCount;
            foreach (var member in localizableMembers)
            {
                if (await this.LocalizeField(member))
                {
                    localizedCount++;
                }
            }

            return localizedCount;
        }

        /// <summary>
        /// Localize a specific field
        /// </summary>
        private async UniTask<bool> LocalizeField(LocalizableMember member)
        {

            var localizationKey = member.OriginalValue;

            var localizedValue = this.localizationProvider.GetLocalizedText(localizationKey);

            if (member.Property.CanWrite)
            {
                member.Property.SetValue(member.Record, localizedValue);
                return true;
            }

            return false;
        }
        private List<LocalizableDataRecord> GetLocalizableFieldsData(IGenericBlueprintReader blueprint)
        {
            var fieldsData    = new List<LocalizableDataRecord>();
            var blueprintType = blueprint.GetType();

            if (this.IsGenericBlueprintByRow(blueprintType))
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
            else if (this.IsGenericBlueprintByCol(blueprintType))
            {
                this.CacheRecordFields(blueprint, fieldsData);
            }

            return fieldsData;
        }

        private void CacheRecordFields(object record, List<LocalizableDataRecord> fieldsData)
        {
            var recordType = record.GetType();
            var dataRecord = new LocalizableDataRecord
            {
                Record = record,
            };
            var localizableFields = this.GetLocalizableFields(recordType);

            foreach (var property in localizableFields)
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(record) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        dataRecord.LocalizableMembers.Add(new()
                        {
                            Property      = property,
                            OriginalValue = value,
                            Record        = record,
                        });
                    }
                }
            }

            // Add to fields data
            fieldsData.Add(dataRecord);
        }
        private IEnumerable<PropertyInfo> GetLocalizableFields(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<LocalizableFieldAttribute>() != null);
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

    public class LocalizableDataRecord
    {
        public object                      Record             { get; set; }
        public List<LocalizableMember>     LocalizableMembers { get; set; } = new();
        public List<LocalizableDataRecord> Children           { get; set; } = new();
    }

    public class LocalizableMember
    {
        public PropertyInfo Property      { get; set; }
        public string       OriginalValue { get; set; }
        public object       Record        { get; set; }
    }
}