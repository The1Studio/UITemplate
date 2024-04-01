namespace TheOneStudio.UITemplate.UITemplate.Scenes.RemoveAdsBottomBar
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using Zenject;

    public class UITemplateRemoveAdPopupView : BaseView
    {
    }
    
    public class UITemplateRemoveAdPopupPresenter : UITemplateBasePopupPresenter<UITemplateRemoveAdPopupView>
    {
        public UITemplateRemoveAdPopupPresenter(SignalBus signalBus) : base(signalBus)
        {
        }
        public override UniTask BindData()
        {
            return UniTask.CompletedTask;
        }
    }
}