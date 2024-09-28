#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.UITemplate.Scenes.Loading
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities;

    public class UITemplateLoadingSceneInstaller : BaseSceneInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.InitScreenManually<UITemplateLoadingScreenPresenter>();
        }
    }
}
#endif