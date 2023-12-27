namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.Utilities.Extension;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateQuestListItemModel
    {
        public UITemplateQuestPopupPresenter Parent { get; }
        public UITemplateQuestController     Quest  { get; }

        public UITemplateQuestListItemModel(UITemplateQuestPopupPresenter parent, UITemplateQuestController quest)
        {
            this.Parent = parent;
            this.Quest  = quest;
        }
    }

    public class UITemplateQuestListItemView : MonoBehaviour, IUIView
    {
        // Left
        [field: SerializeField] public Image    ImgReward { get; private set; }
        [field: SerializeField] public TMP_Text TxtReward { get; private set; }
        [field: SerializeField] public TMP_Text TxtName   { get; private set; }
        [field: SerializeField] public TMP_Text TxtDesc   { get; private set; }

        // Right
        [field: SerializeField] public TMP_Text TxtProgress { get; private set; }
        [field: SerializeField] public Slider   SldProgress { get; private set; }
        [field: SerializeField] public Button   BtnGo       { get; private set; }
        [field: SerializeField] public Button   BtnClaim    { get; private set; }

        [field: SerializeField] public GameObject[] NormalObjects    { get; private set; }
        [field: SerializeField] public GameObject[] CompletedObjects { get; private set; }
    }

    public class UITemplateQuestListItemPresenter : BaseUIItemPresenter<UITemplateQuestListItemView, UITemplateQuestListItemModel>
    {
        private readonly IGameAssets gameAssets;

        public UITemplateQuestListItemPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
            this.gameAssets = gameAssets;
        }

        public UITemplateQuestListItemModel Model { get; set; }

        public override void BindData(UITemplateQuestListItemModel model)
        {
            this.Model = model;

            // Left
            var record = this.Model.Quest.Record;
            var reward = record.Rewards.Single();
            this.View.ImgReward.sprite = this.gameAssets.LoadAssetAsync<Sprite>(reward.Image).WaitForCompletion();
            this.View.TxtReward.text   = reward.Value.ToString();
            this.View.TxtName.text     = record.Name;
            this.View.TxtDesc.text     = record.Description;

            // Right
            var progressHandler = this.Model.Quest.GetCompleteProgressHandlers().Single();
            this.View.TxtProgress.text  = $"{progressHandler.CurrentProgress}/{progressHandler.MaxProgress}";
            this.View.SldProgress.value = progressHandler.CurrentProgress / progressHandler.MaxProgress;

            this.View.NormalObjects.ForEach(obj => obj.SetActive(this.View.SldProgress.value < 1f));
            this.View.CompletedObjects.ForEach(obj => obj.SetActive(this.View.SldProgress.value >= 1f));

            this.View.BtnGo.onClick.AddListener(this.OnClickGo);
            this.View.BtnClaim.onClick.AddListener(this.OnClickClaim);
        }

        private void OnClickGo()
        {
            this.Model.Parent.CloseViewAsync().Forget();
        }

        private void OnClickClaim()
        {
            this.Model.Quest.CollectReward();
            this.Model.Parent.Refresh();
        }

        public override void Dispose()
        {
            this.View.BtnGo.onClick.RemoveListener(this.OnClickGo);
            this.View.BtnClaim.onClick.RemoveListener(this.OnClickClaim);
        }
    }
}