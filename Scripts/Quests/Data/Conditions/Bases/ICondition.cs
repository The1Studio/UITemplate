namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    using System;
    using Newtonsoft.Json;

    public interface ICondition
    {
        public IProgress SetupProgress();

        public interface IProgress
        {
            [JsonIgnore] public Type HandlerType { get; }

            public interface IHandler
            {
                internal ICondition Condition { set; }

                internal IProgress Progress { set; }

                public float CurrentProgress { get; }

                public float MaxProgress { get; }

                public void Initialize();

                public void Dispose();
            }
        }
    }

    public static class ConditionProgressHandlerExtensions
    {
        public static bool IsSatisfied(this ICondition.IProgress.IHandler handler)
        {
            return handler.CurrentProgress >= handler.MaxProgress;
        }
    }
}