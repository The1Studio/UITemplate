namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using Core.AnalyticServices;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.Wido;
    using UnityEngine.UI;
    using Zenject;

    internal class UITemplateBaseScreenUtils
    {
        private readonly IAnalyticServices       analyticService;
        private readonly UITemplateSoundServices soundServices;

        public UITemplateBaseScreenUtils()
        {
            var diContainer = ZenjectUtils.GetCurrentContainer();
            this.analyticService = diContainer.Resolve<IAnalyticServices>();
            this.soundServices   = diContainer.Resolve<UITemplateSoundServices>();
        }

        public static UITemplateBaseScreenUtils Instance { get; set; }

        public static void Init()
        {
            Instance ??= new UITemplateBaseScreenUtils();
        }

        public static void ReInit()
        {
            Instance = new UITemplateBaseScreenUtils();
        }

        private void OnClickButton(string screenName, Button button)
        {
            Init();
            this.soundServices.PlaySoundClick();
            this.analyticService.Track(new ButtonClick(screenName, button.gameObject.name));
        }

        public void BindOnClickButton(string screenName, Button[] buttons)
        {
            foreach (var button in buttons) button.onClick.AddListener(() => this.OnClickButton($"{SceneDirector.CurrentSceneName}/{screenName}", button));
        }
    }

    public abstract class UITemplateBaseScreenPresenter<TView> : BaseScreenPresenter<TView> where TView : IScreenView
    {
        protected UITemplateBaseScreenPresenter(SignalBus signalBus) : base(signalBus)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
        }
    }

    public abstract class UITemplateBaseScreenPresenter<TView, TModel> : BaseScreenPresenter<TView, TModel> where TView : IScreenView
    {
        protected UITemplateBaseScreenPresenter(SignalBus signalBus, ILogService logger) : base(signalBus, logger)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
        }
    }

    public abstract class UITemplateBasePopupPresenter<TView> : BasePopupPresenter<TView> where TView : IScreenView
    {
        protected UITemplateBasePopupPresenter(SignalBus signalBus) : base(signalBus)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
        }
    }

    public abstract class UITemplateBasePopupPresenter<TView, TModel> : BasePopupPresenter<TView, TModel> where TView : IScreenView
    {
        protected UITemplateBasePopupPresenter(SignalBus signalBus, ILogService logger) : base(signalBus, logger)
        {
            UITemplateBaseScreenUtils.Init();
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            UITemplateBaseScreenUtils.Instance.BindOnClickButton(this.View.GetType().Name, this.View.RectTransform.GetComponentsInChildren<Button>());
        }
    }
}