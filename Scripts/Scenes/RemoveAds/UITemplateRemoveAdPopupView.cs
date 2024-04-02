namespace TheOneStudio.UITemplate.UITemplate.Scenes.RemoveAdsBottomBar
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using TMPro;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateRemoveAdPopupView : BaseView
    {
        public TMP_Text priceText;
        public Button   btnRemoveAds;
    }

    [PopupInfo(nameof(UITemplateRemoveAdPopupView))]
    public class UITemplateRemoveAdPopupPresenter : UITemplateBasePopupPresenter<UITemplateRemoveAdPopupView>
    {
        public UITemplateRemoveAdPopupPresenter(SignalBus signalBus) : base(signalBus) { }
        public override UniTask BindData() { return UniTask.CompletedTask; }
    }
}