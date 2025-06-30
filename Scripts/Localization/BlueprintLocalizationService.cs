namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueprintFlow.BlueprintReader;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Utils;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    public class BlueprintLocalizationService
    {
        private readonly SignalBus                            signalBus;
        private readonly LocalizationSettingsProvider      localizationSettingsProvider;
        private readonly IEnumerable<IGenericBlueprintReader> blueprints;
        private readonly ILogger                              logger;

        private readonly Dictionary<IGenericBlueprintReader, List<LocalizableMember>> originalValuesCache = new();

        [Preserve] public BlueprintLocalizationService(
            SignalBus                            signalBus,
            LocalizationSettingsProvider      localizationSettingsProvider,
            IEnumerable<IGenericBlueprintReader> blueprints,
            ILoggerManager                       loggerManager
        )
        {
            this.signalBus            = signalBus;
            this.localizationSettingsProvider = localizationSettingsProvider;
            this.blueprints           = blueprints;
            this.logger               = loggerManager.GetLogger(this);
        }

        private void LoadCacheOriginalValues()
        {
            foreach (var blueprint in this.blueprints)
            {
                var fieldsData = this.GetLocalizableFieldsData(blueprint);

                if (fieldsData.Count > 0)
                {
                    this.originalValuesCache[blueprint] = fieldsData;
                }
            }

            this.signalBus.Fire<LoadedLocalizationBlueprintsSignal>();
            this.logger?.Error($"trong: Cached original values for {this.originalValuesCache.Count} blueprints of {this.blueprints.Count()}");
        }

        public void LocalizeAllBlueprintFields()
        {
            foreach (var blueprint in this.originalValuesCache.Keys)
            {
                this.LocalizeBlueprintFields(blueprint);
            }
        }

        private int LocalizeBlueprintFields(IGenericBlueprintReader blueprint)
        {
            if (blueprint == null) return 0;

            var localizedCount = 0;

            this.LocalizeRecordFields(blueprint);

            return localizedCount;
        }

        private void LocalizeRecordFields(IGenericBlueprintReader blueprint)
        {
            this.originalValuesCache.TryGetValue(blueprint, out var members);

            if (members == null) return;
            foreach (var member in members)
            {
                this.LocalizeField(member);
            }
        }

        private bool LocalizeField(LocalizableMember member)
        {
            var localizationKey = member.OriginalValue;

            var localizedValue = this.localizationSettingsProvider.GetLocalizedText(localizationKey);

            if (!member.Property.CanWrite) return false;
            member.Property.SetValue(member.Record, localizedValue);
            return true;
        }

        private List<LocalizableMember> GetLocalizableFieldsData(IGenericBlueprintReader blueprint)
        {
            var fieldsData    = new List<LocalizableMember>();
            var blueprintType = blueprint.GetType();

            if (BlueprintLocalizeExtensions.IsGenericBlueprintByRow(blueprintType))
            {
                this.ProcessBlueprintCollectionData(blueprint, fieldsData);
            }
            else if (BlueprintLocalizeExtensions.IsGenericBlueprintByCol(blueprintType))
            {
                this.CacheRecordFields(blueprint, fieldsData);
            }
            else
            {
                this.logger?.Error($"trong: Blueprint type {blueprintType.Name} is neither ByRow nor ByCol");
            }

            return fieldsData;
        }
        private void ProcessBlueprintCollectionData(object blueprint, List<LocalizableMember> fieldsData)
        {
            var blueprintType = blueprint.GetType();

            if (BlueprintLocalizeExtensions.IsDictionaryBlueprintByRow(blueprintType))
            {
                this.ProcessDictionaryBlueprintData(blueprint, fieldsData);
            }
            else if (BlueprintLocalizeExtensions.IsListBlueprintByRow(blueprintType))
            {
                this.ProcessListBlueprintData(blueprint, fieldsData);
            }
            else
            {
                this.logger?.Error($"Blueprint type {blueprintType.Name} is not a recognized collection type");
            }
        }

        private void ProcessDictionaryBlueprintData(object blueprint, List<LocalizableMember> fieldsData)
        {
            if (blueprint is not IDictionary dictionary) return;
            this.logger?.Info($" Dictionary with {dictionary.Count} items");
            foreach (DictionaryEntry kvp in dictionary)
            {
                var record = kvp.Value;
                this.CacheRecordFields(record, fieldsData);
            }
        }

        private void ProcessListBlueprintData(object blueprint, List<LocalizableMember> fieldsData)
        {
            if (blueprint is not IEnumerable enumerable) return;
            var records = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            this.logger?.Info($"List with {records.Cast<object>().Count()} items");
            foreach (var record in records)
            {
                this.CacheRecordFields(record, fieldsData);
            }
        }

        private void CacheRecordFields(object record, List<LocalizableMember> fieldsData)
        {
            var recordType        = record.GetType();
            var localizableFields = BlueprintLocalizeExtensions.GetLocalizableFields(recordType).ToList();

            this.logger?.Info($"Processing record type: {recordType.Name}, found {localizableFields.Count()} localizable fields");

            foreach (var property in localizableFields)
            {
                if (property.PropertyType != typeof(string)) continue;
                var value = property.GetValue(record) as string;
                this.logger?.Info($"Property {property.Name} value: '{value}'");

                if (string.IsNullOrEmpty(value)) continue;
                fieldsData.Add(new()
                {
                    Property      = property,
                    OriginalValue = value,
                    Record        = record,
                });
                this.logger?.Info($"Added localizable member: {property.Name} = '{value}'");
            }

            this.CacheNestedBlueprintCollectionFields(record, fieldsData);
        }

        private void CacheNestedBlueprintCollectionFields(object record, List<LocalizableMember> fieldsData)
        {
            var recordType = record.GetType();
            var properties = recordType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (!typeof(IBlueprintCollection).IsAssignableFrom(propertyType)) continue;

                var nestedBlueprint = property.GetValue(record);
                this.ProcessBlueprintCollectionData(nestedBlueprint, fieldsData);
            }
        }


    }

    public class LocalizableMember
    {
        public PropertyInfo Property      { get; set; }
        public string       OriginalValue { get; set; }
        public object       Record        { get; set; }
    }
}