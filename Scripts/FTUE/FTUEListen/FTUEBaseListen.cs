namespace TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen
{
    using Cysharp.Threading.Tasks;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.FTUE.Signal;
    using Zenject;

    public abstract class FTUEBaseListen : IInitializable
    {
        protected readonly SignalBus                SignalBus;
        protected readonly UITemplateFTUEBlueprint  FtueBlueprint;
        protected readonly UITemplateFTUEController ftueController;

        protected FTUEBaseListen(SignalBus signalBus, UITemplateFTUEBlueprint ftueBlueprint, UITemplateFTUEController ftueController)
        {
            this.SignalBus      = signalBus;
            this.FtueBlueprint  = ftueBlueprint;
            this.ftueController = ftueController;
        }

        public void Initialize() { this.InitInternal(); }

        protected abstract void InitInternal();

        protected void FireFtueTriggerSignal(string ftueId)
        {
            this.ftueController.DoDeactiveCurrentFTUEStep();
            this.SignalBus.Fire(new FTUETriggerSignal(ftueId));
        }
    }
}