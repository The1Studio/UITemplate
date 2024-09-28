namespace TheOneStudio.UITemplate.Quests.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.Quests.Conditions;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class UITemplateQuestProgress : ILocalData
    {
        public Dictionary<string, Quest> Storage { get; } = new Dictionary<string, Quest>();

        void ILocalData.Init() { }

        public sealed class Quest
        {
            [JsonProperty] public QuestStatus Status { get; internal set; }

            [JsonProperty] public IReadOnlyList<ICondition.IProgress> StartProgress    { get; }
            [JsonProperty] public IReadOnlyList<ICondition.IProgress> ShowProgress     { get; }
            [JsonProperty] public IReadOnlyList<ICondition.IProgress> CompleteProgress { get; }
            [JsonProperty] public IReadOnlyList<ICondition.IProgress> ResetProgress    { get; }

            [JsonConstructor]
            private Quest()
            {
                this.StartProgress    = new List<ICondition.IProgress>();
                this.ShowProgress     = new List<ICondition.IProgress>();
                this.CompleteProgress = new List<ICondition.IProgress>();
                this.ResetProgress    = new List<ICondition.IProgress>();
            }

            public Quest(QuestRecord record)
            {
                this.StartProgress    = record.StartConditions.Select(condition => condition.SetupProgress()).ToList();
                this.ShowProgress     = record.ShowConditions.Select(condition => condition.SetupProgress()).ToList();
                this.CompleteProgress = record.CompleteConditions.Select(condition => condition.SetupProgress()).ToList();
                this.ResetProgress    = record.ResetConditions.Select(condition => condition.SetupProgress()).ToList();
                if (this.StartProgress.Count is 0)
                {
                    this.Status |= QuestStatus.Started;
                }
                if (this.ShowProgress.Count is 0)
                {
                    this.Status |= QuestStatus.Shown;
                }
            }
        }
    }
}