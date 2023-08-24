namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateHomeSimpleScreenView : BaseView
    {
        public Button                      PlayButton;
        public Button                      LevelButton;
        public Button                      ShopButton;
        public UITemplateSettingButtonView SettingButtonView;
    }

    [ScreenInfo(nameof(UITemplateHomeSimpleScreenView))]
    public class UITemplateHomeSimpleScreenPresenter : UITemplateBaseScreenPresenter<UITemplateHomeSimpleScreenView>
    {
        public UITemplateHomeSimpleScreenPresenter(SignalBus signalBus, DiContainer diContainer, IScreenManager screenManager) :
            base(signalBus)
        {
            this.diContainer                       = diContainer;
            this.ScreenManager                     = screenManager;
        }

        protected override async void OnViewReady()
        {
            base.OnViewReady();
            await this.OpenViewAsync();
            this.diContainer.Inject(this.View.SettingButtonView);
            this.View.PlayButton.onClick.AddListener(this.OnClickPlay);
            this.View.LevelButton?.onClick.AddListener(this.OnClickLevel);
            this.View.ShopButton?.onClick.AddListener(this.OnClickShop);
        }

        protected virtual void OnClickLevel()
        {
        }

        protected virtual void OnClickShop() { }

        protected virtual void OnClickPlay()
        {
        }

        public override UniTask BindData()
        {
            return UniTask.CompletedTask;
        }

        #region inject

        protected readonly DiContainer                       diContainer;
        protected readonly IScreenManager                    ScreenManager;

        #endregion
    }
}