namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.View;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TheOneStudio.UITemplate.Quests.Data;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Utils;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateQuestPopupModel
    {
        public string Tag { get; }

        public UITemplateQuestPopupModel(string tag = "")
        {
            this.Tag = tag;
        }
    }

    public class UITemplateQuestPopupView : BaseView
    {
        [field: SerializeField] public Button                  BtnClose          { get; private set; }
        [field: SerializeField] public Transform               ItemViewContainer { get; private set; }
        [field: SerializeField] public UITemplateQuestItemView ItemViewPrefab    { get; private set; }
    }

    [PopupInfo(nameof(UITemplateQuestPopupView))]
    public class UITemplateQuestPopupPresenter : UITemplateBasePopupPresenter<UITemplateQuestPopupView, UITemplateQuestPopupModel>
    {
        #region Inject

        private readonly UITemplateQuestManager questManager;
        private readonly ObjectPoolManager      objectPoolManager;

        public UITemplateQuestPopupPresenter(
            SignalBus              signalBus,
            ILogService            logService,
            UITemplateQuestManager questManager,
            ObjectPoolManager      objectPoolManager
        ) : base(signalBus, logService)
        {
            this.questManager      = questManager;
            this.objectPoolManager = objectPoolManager;
        }

        #endregion

        protected override void OnViewReady()
        {
            base.OnViewReady();
            this.View.BtnClose.onClick.AddListener(this.OnClickClose);
        }

        public override UniTask BindData(UITemplateQuestPopupModel _)
        {
            this.questManager.GetAllControllers()
                .Where(controller => controller.Record.Tags.Contains(this.Model.Tag))
                .Where(controller => controller.Progress.Status is QuestStatus.NotCompleted or QuestStatus.NotCollected)
                .ForEach(controller =>
                {
                    var itemView = this.objectPoolManager.Spawn(this.View.ItemViewPrefab, this.View.ItemViewContainer);
                    itemView.Model = new UITemplateQuestItemModel(
                        controller: controller,
                        onClickGo: this.OnClickClose,
                        onClickClaim: () =>
                        {
                            this.Dispose();
                            this.BindData(this.Model).Forget();
                        }
                    );
                    itemView.OnSpawn();
                });
            return UniTask.CompletedTask;
        }

        private void OnClickClose()
        {
            this.CloseViewAsync().Forget();
        }

        public override void Dispose()
        {
            this.objectPoolManager.RecycleAll(this.View.ItemViewPrefab);
        }
    }
}