namespace TheOneStudio.UITemplate.Quests.Data.Conditions
{
    public abstract class ConditionProgressHandler<TCondition, TProgress> : ICondition.IProgress.IHandler
        where TCondition : ICondition
        where TProgress : ICondition.IProgress
    {
        ICondition ICondition.IProgress.IHandler.          Condition { set => this.Condition = (TCondition)value; }
        ICondition.IProgress ICondition.IProgress.IHandler.Progress  { set => this.Progress = (TProgress)value; }

        protected TCondition Condition { get; private set; }
        protected TProgress  Progress  { get; private set; }

        public abstract float CurrentProgress { get; }
        public abstract float MaxProgress     { get; }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}