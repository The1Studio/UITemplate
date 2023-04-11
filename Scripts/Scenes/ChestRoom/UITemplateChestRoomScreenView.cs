namespace TheOneStudio.UITemplate.UITemplate.Scenes.ChestRoom
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using TheOneStudio.UITemplate.UITemplate.Blueprints.Gacha;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateChestRoomScreenView : BaseView
    {
        public List<UITemplateChestItemView> ChestItemViewList; // List of chest item view
        public Button                        NoThankButton;     // No thank button
        public Button                        WatchAdButton;     // Watch ad button
        public List<GameObject>              KeyObjectList;     // Open button
        public GameObject                    KeyGroupObject;
    }

    public class UITemplateChestRoomScreenPresenter : UITemplateBaseScreenPresenter<UITemplateChestRoomScreenView>
    {
        #region region

        private readonly UITemplateGachaChestRoomBlueprint uiTemplateGachaChestRoomBlueprint;

        #endregion
        public UITemplateChestRoomScreenPresenter(SignalBus signalBus, UITemplateGachaChestRoomBlueprint uiTemplateGachaChestRoomBlueprint) : base(signalBus)
        {
            this.uiTemplateGachaChestRoomBlueprint = uiTemplateGachaChestRoomBlueprint;
        }

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.NoThankButton.onClick.AddListener(this.OnClickNoThankButton);
            this.View.WatchAdButton.onClick.AddListener(this.OnClickWatchAdButton);
            foreach (var uiTemplateChestItemView in this.View.ChestItemViewList)
            {
                uiTemplateChestItemView.ChestButton.onClick.AddListener(() => this.OnClickChestButton(uiTemplateChestItemView));
            }
        }
        
        private void OnClickChestButton(UITemplateChestItemView uiTemplateChestItemView)
        {
        }

        private void OnClickWatchAdButton() { this.ResetKeys(); }

        private void OnClickNoThankButton() { }

        public override UniTask BindData()
        {
            foreach (var uiTemplateChestItemView in this.View.ChestItemViewList)
            {
                uiTemplateChestItemView.Init();
            }

            this.ResetKeys();

            return UniTask.CompletedTask;
        }

        private void ResetKeys()
        {
            this.View.NoThankButton.gameObject.SetActive(false);
            this.View.WatchAdButton.gameObject.SetActive(false);
            this.View.KeyGroupObject.SetActive(true);

            foreach (var keyObject in this.View.KeyObjectList)
            {
                keyObject.SetActive(true);
            }
        }
    }
}