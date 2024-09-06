#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Signals;

    public class UITemplateMainSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.DeclareSignals();
            this.Container.InitScreenManually<UITemplateHomeSimpleScreenPresenter>();
        }

        private void DeclareSignals()
        {
            this.Container.DeclareSignal<OnUpdateCurrencySignal>();
        }
    }
}
#endif