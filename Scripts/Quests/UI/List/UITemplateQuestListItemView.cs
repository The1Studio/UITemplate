namespace TheOneStudio.UITemplate.Quests.UI
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.Utilities.Extension;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateQuestListItemModel
    {
        public UITemplateQuestController Quest { get; }

        public UITemplateQuestListItemModel(UITemplateQuestController quest)
        {
            this.Quest = quest;
        }
    }

    public class UITemplateQuestListItemView : MonoBehaviour
    {
        // Left
        [SerializeField] private Image    imgReward;
        [SerializeField] private TMP_Text txtReward;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtDesc;

        // Right
        [SerializeField] private TMP_Text txtProgress;
        [SerializeField] private Slider   sldProgress;
        [SerializeField] private Button   btnGo;
        [SerializeField] private Button   btnClaim;

        [SerializeField] private GameObject[] normalObjects;
        [SerializeField] private GameObject[] completedObjects;

        private void Awake()
        {
            this.btnGo.onClick.AddListener(this.OnClickGo);
            this.btnClaim.onClick.AddListener(this.OnClickClaim);

            ZenjectUtils.GetCurrentContainer().Inject(this);
        }

        private IGameAssets gameAssets;

        [Inject]
        public void Construct(IGameAssets gameAssets)
        {
            this.gameAssets = gameAssets;
        }

        public UITemplateQuestPopupPresenter Parent { get; set; }

        public UITemplateQuestListItemModel Model { get; set; }

        public void BindData()
        {
            // Left
            var record = this.Model.Quest.Record;
            var reward = record.Rewards.Single();
            this.imgReward.sprite = this.gameAssets.LoadAssetAsync<Sprite>(reward.Image).WaitForCompletion();
            this.txtReward.text   = reward.Value.ToString();
            this.txtName.text     = record.Name;
            this.txtDesc.text     = record.Description;

            // Right
            var progressHandler = this.Model.Quest.GetCompleteProgressHandlers().Single();
            this.txtProgress.text  = $"{progressHandler.CurrentProgress}/{progressHandler.MaxProgress}";
            this.sldProgress.value = progressHandler.CurrentProgress / progressHandler.MaxProgress;

            this.normalObjects.ForEach(obj => obj.SetActive(this.sldProgress.value < 1f));
            this.completedObjects.ForEach(obj => obj.SetActive(this.sldProgress.value >= 1f));

            var localPosition = this.transform.localPosition;
            localPosition.z              = 0f;
            this.transform.localPosition = localPosition;
            this.transform.localScale    = Vector3.one;
        }

        private void OnClickGo()
        {
            this.Parent.CloseViewAsync().Forget();
        }

        private void OnClickClaim()
        {
            this.Model.Quest.CollectReward();
            this.Parent.Refresh();
        }
    }
}