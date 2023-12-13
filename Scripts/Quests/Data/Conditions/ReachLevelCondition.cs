namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using Newtonsoft.Json;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine;

    public class ReachLevelCondition : ICondition
    {
        [JsonProperty] [field: SerializeField] private int Level { get; set; }

        ICondition.IProgress ICondition.SetupProgress() => new Progress();

        private class Progress : ICondition.IProgress
        {
            Type ICondition.IProgress.HandlerType => typeof(Handler);

            private class Handler : ConditionProgressHandler<ReachLevelCondition, Progress>
            {
                private readonly UITemplateLevelDataController levelDataController;

                public Handler(UITemplateLevelDataController levelDataController)
                {
                    this.levelDataController = levelDataController;
                }

                public override float CurrentProgress => this.levelDataController.CurrentLevel < this.Condition.Level ? 0f : 1f;
                public override float MaxProgress     => 1;
            }
        }
    }
}