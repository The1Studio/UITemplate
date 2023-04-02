namespace TheOneStudio.UITemplate.UITemplate.Scripts.Services
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using Zenject;

    public class UITemplateScreenShowServices : IInitializable
    {
        private readonly List<IUITemplateScreenShow> screenShows;
        private readonly SignalBus                   signalBus;

        public UITemplateScreenShowServices(List<IUITemplateScreenShow> screenShows, SignalBus signalBus)
        {
            this.screenShows = screenShows;
            this.signalBus   = signalBus;
        }

        public void Initialize() { this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShow); }

        private void OnScreenShow(ScreenShowSignal obj)
        {
            foreach (var s in this.screenShows)
            {
                if (obj.ScreenPresenter.GetType().Name.Equals(s.ScreenPresenter.Name))
                {
                    s.OnProcessScreenShow();
                }
            }
        }
    }
}