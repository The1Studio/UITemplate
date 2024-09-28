namespace TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;

    public abstract class FTUEBaseListen : IInitializable
    {
        protected readonly SignalBus               SignalBus;
        protected readonly UITemplateFTUEBlueprint FtueBlueprint;

        protected FTUEBaseListen(SignalBus signalBus,UITemplateFTUEBlueprint ftueBlueprint)
        {
            this.SignalBus     = signalBus;
            this.FtueBlueprint = ftueBlueprint;
        }

        public             void Initialize() { this.InitInternal(); }
        protected abstract void InitInternal();

        protected void FireFtueTriggerSignal(string ftueId)
        {
            this.SignalBus.Fire(new FTUETriggerSignal(ftueId));
        }
    }
}