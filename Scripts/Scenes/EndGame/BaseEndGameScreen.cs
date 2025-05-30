namespace TheOneStudio.UITemplate.UITemplate.Scenes.EndGame
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using UnityEngine.UI;

    public abstract class BaseEndGameScreenView : BaseView
    {
        public Button btnNext;
    }

    public abstract class BaseEndGameScreenPresenter<TView> : UITemplateBaseScreenPresenter<TView> where TView : BaseEndGameScreenView
    {
        protected readonly UITemplateSoundServices    SoundServices;
        protected readonly UITemplateAdServiceWrapper UITemplateAdService;

        protected BaseEndGameScreenPresenter(
            SignalBus                  signalBus,
            ILoggerManager             loggerManager,
            UITemplateAdServiceWrapper uiTemplateAdService,
            UITemplateSoundServices    soundServices
        ) : base(signalBus, loggerManager)
        {
            this.UITemplateAdService = uiTemplateAdService;
            this.SoundServices       = soundServices;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.btnNext.onClick.AddListener(this.OnClickNext);
        }

        public override UniTask BindData()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnClickNext()
        {
        }
    }
}