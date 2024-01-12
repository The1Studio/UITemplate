namespace TheOneStudio.UITemplate.Quests.Conditions
{
    using System;

    public abstract class BaseCondition : ICondition
    {
        ICondition.IProgress ICondition.SetupProgress() => this.SetupProgress();

        protected abstract ICondition.IProgress SetupProgress();

        public abstract class BaseProgress : ICondition.IProgress
        {
            Type ICondition.IProgress.HandlerType => this.HandlerType;

            protected abstract Type HandlerType { get; }

            public abstract class BaseHandler<TCondition, TProgress> : ICondition.IProgress.IHandler
                where TCondition : ICondition
                where TProgress : ICondition.IProgress
            {
                ICondition ICondition.IProgress.IHandler.          Condition { set => this.Condition = (TCondition)value; }
                ICondition.IProgress ICondition.IProgress.IHandler.Progress  { set => this.Progress = (TProgress)value; }

                float ICondition.IProgress.IHandler.CurrentProgress => this.CurrentProgress;
                float ICondition.IProgress.IHandler.MaxProgress     => this.MaxProgress;

                void ICondition.IProgress.IHandler.Initialize() => this.Initialize();
                void ICondition.IProgress.IHandler.Dispose()    => this.Dispose();

                protected TCondition Condition { get; private set; }
                protected TProgress  Progress  { get; private set; }

                protected abstract float CurrentProgress { get; }
                protected abstract float MaxProgress     { get; }

                protected virtual void Initialize() { }
                protected virtual void Dispose()    { }
            }
        }
    }
}