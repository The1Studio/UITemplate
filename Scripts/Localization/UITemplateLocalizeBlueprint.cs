namespace TheOneStudio.UITemplate.UITemplate.Localization
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BlueprintFlow.BlueprintReader;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Utils;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    public sealed class UITemplateLocalizeBlueprint
    {
        private readonly SignalBus                              signalBus;
        private readonly UITemplateLocalizationSettingsProvider uiTemplateLocalizationSettingsProvider;
        private readonly IEnumerable<IGenericBlueprintReader>   blueprints;
        private readonly ILogger                                logger;

        private readonly Dictionary<IGenericBlueprintReader, List<LocalizableMember>> originalValuesCache = new();

        [Preserve]
        public UITemplateLocalizeBlueprint(
            SignalBus                              signalBus,
            UITemplateLocalizationSettingsProvider uiTemplateLocalizationSettingsProvider,
            IEnumerable<IGenericBlueprintReader>   blueprints,
            ILoggerManager                         loggerManager
        )
        {
            this.signalBus                              = signalBus;
            this.uiTemplateLocalizationSettingsProvider = uiTemplateLocalizationSettingsProvider;
            this.blueprints                             = blueprints;
            this.logger                                 = loggerManager.GetLogger(this);
        }

        public UniTask LoadCacheOriginalValues()
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
            this.logger?.Info($"Cached original values for {this.originalValuesCache.Count} blueprints of {this.blueprints.Count()}");

            return UniTask.CompletedTask;
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

            var localizedValue = this.uiTemplateLocalizationSettingsProvider.GetLocalizedText(localizationKey);

            if (!member.Property.CanWrite) return false;
            member.Property.SetValue(member.Record, localizedValue);
            return true;
        }

        private List<LocalizableMember> GetLocalizableFieldsData(IGenericBlueprintReader blueprint)
        {
            var fieldsData    = new List<LocalizableMember>();
            var blueprintType = blueprint.GetType();

            if (UITemplateBlueprintLocalizeExtensions.IsGenericBlueprintByRow(blueprintType))
            {
                this.ProcessBlueprintCollectionData(blueprint, fieldsData);
            }
            else if (UITemplateBlueprintLocalizeExtensions.IsGenericBlueprintByCol(blueprintType))
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

            if (UITemplateBlueprintLocalizeExtensions.IsDictionaryBlueprintByRow(blueprintType))
            {
                this.ProcessDictionaryBlueprintData(blueprint, fieldsData);
            }
            else if (UITemplateBlueprintLocalizeExtensions.IsListBlueprintByRow(blueprintType))
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
            var localizableFields = UITemplateBlueprintLocalizeExtensions.GetLocalizableFields(recordType).ToList();

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