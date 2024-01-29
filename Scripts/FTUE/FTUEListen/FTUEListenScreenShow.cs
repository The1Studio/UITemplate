namespace TheOneStudio.UITemplate.UITemplate.FTUE.FTUEListen
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using Zenject;

    public class FTUEListenScreenShow : FTUEBaseListen
    {
        public FTUEListenScreenShow(SignalBus signalBus, UITemplateFTUEBlueprint ftueBlueprint) : base(signalBus, ftueBlueprint) { }
        protected override void InitInternal() { this.SignalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            var currentScreen = obj.ScreenPresenter;

            if (currentScreen == null) return;

            foreach (var ftue in this.FtueBlueprint.Values)
            {
                if (!ftue.EnableTrigger) continue;

                if (currentScreen.GetType().Name.Equals(ftue.ScreenLocation))
                {
                    this.FireFtueTriggerSignal(ftue.Id);
                }
            }
        }
    }
}