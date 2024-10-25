namespace TheOneStudio.UITemplate.Quests.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;
    using Newtonsoft.Json;
    using TheOne.Data.Conversion;
    using TheOne.Extensions;
    using TheOneStudio.UITemplate.Quests.Conditions;
    using TheOneStudio.UITemplate.Quests.Rewards;
    using TheOneStudio.UITemplate.Quests.TargetHandler;
    using UnityEngine.Scripting;

    [Preserve]
    [BlueprintReader("UITemplateQuest")]
    public sealed class UITemplateQuestBlueprint : GenericBlueprintReaderByRow<string, QuestRecord>
    {
        public sealed class ListConverter : Converter
        {
            private static readonly HashSet<Type> SupportedTypes = new() { typeof(List<IReward>), typeof(List<ICondition>), typeof(List<IRedirectTarget>) };

            private readonly Dictionary<Type, IReadOnlyDictionary<string, Type>> typeMap = new();

            protected override bool CanConvert(Type type) => SupportedTypes.Contains(type);

            protected override object GetDefaultValue(Type type)
            {
                return Activator.CreateInstance(type, 0);
            }

            protected override object ConvertFromString(string list, Type listType)
            {
                var result  = (IList)Activator.CreateInstance(listType);
                var typeMap = this.GetTypeMap(listType.GetGenericArguments()[0]);
                foreach (var str in list.Split(";"))
                {
                    var index = str.IndexOf(":", StringComparison.Ordinal);
                    result.Add(JsonConvert.DeserializeObject(str[(index + 1)..], typeMap[str[..index]]));
                }
                return result;
            }

            protected override string ConvertToString(object list, Type listType)
            {
                var typeMap = this.GetTypeMap(listType.GetGenericArguments()[0]);
                return string.Join(";", ((IEnumerable)list).Cast<object>().Select(obj =>
                {
                    var typeStr = typeMap.First(kv => kv.Value == obj.GetType()).Key;
                    return $"{typeStr}:{JsonConvert.SerializeObject(obj)}";
                }));
            }

            private IReadOnlyDictionary<string, Type> GetTypeMap(Type baseType)
            {
                return this.typeMap.GetOrAdd(
                    baseType,
                    () =>
                    {
                        var postFix = baseType.Name[1..];
                        return baseType.GetDerivedTypes().ToDictionary(type =>
                            type.Name.EndsWith(postFix)
                                ? type.Name[..^postFix.Length]
                                : type.Name
                        );
                    }
                );
            }
        }
    }

    [Preserve]
    [CsvHeaderKey(nameof(Id))]
    public sealed class QuestRecord
    {
        public string          Id          { get; [Preserve] private set; }
        public string          Name        { get; [Preserve] private set; }
        public string          Description { get; [Preserve] private set; }
        public string          Image       { get; [Preserve] private set; }
        public HashSet<string> Tags        { get; [Preserve] private set; }

        public List<IReward>         Rewards            { get; [Preserve] private set; }
        public List<ICondition>      StartConditions    { get; [Preserve] private set; }
        public List<ICondition>      ShowConditions     { get; [Preserve] private set; }
        public List<ICondition>      CompleteConditions { get; [Preserve] private set; }
        public List<ICondition>      ResetConditions    { get; [Preserve] private set; }
        public List<IRedirectTarget> Target             { get; [Preserve] private set; }

        public bool HasTag(string tag)
        {
            return this.Tags.Contains(tag);
        }
    }
}