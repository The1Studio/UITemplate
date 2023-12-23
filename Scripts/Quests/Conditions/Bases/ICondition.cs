namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;
    using Newtonsoft.Json;

    public interface ICondition
    {
        internal IProgress SetupProgress();

        public interface IProgress
        {
            [JsonIgnore] internal Type HandlerType { get; }

            public interface IHandler
            {
                internal ICondition Condition { set; }

                internal IProgress Progress { set; }

                public float CurrentProgress { get; }

                public float MaxProgress { get; }

                internal void Initialize();

                internal void Dispose();
            }
        }
    }

    public static class ConditionHandlerExtensions
    {
        public static bool IsSatisfied(this ICondition.IProgress.IHandler handler)
        {
            return handler.CurrentProgress >= handler.MaxProgress;
        }
    }
}