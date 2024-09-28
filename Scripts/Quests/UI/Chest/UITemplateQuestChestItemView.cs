namespace TheOneStudio.UITemplate.Quests.UI
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Scripts.AssetLibrary;
    using TheOneStudio.UITemplate.Quests.Data;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateQuestChestItemModel
    {
        public UITemplateQuestController Quest { get; }

        public UITemplateQuestChestItemModel(UITemplateQuestController quest)
        {
            this.Quest = quest;
        }
    }

    public class UITemplateQuestChestItemView : MonoBehaviour
    {
        [SerializeField] private Button     btn;
        [SerializeField] private Image      img;
        [SerializeField] private TMP_Text   txt;
        [SerializeField] private Animator   animShake;
        [SerializeField] private GameObject tick;

        private IGameAssets gameAssets;

        private void Awake()
        {
            var container = this.GetCurrentContainer();
            this.gameAssets = container.Resolve<IGameAssets>();

            this.btn.onClick.AddListener(this.OnClick);
        }

        public UITemplateQuestChestItemModel Model { get; set; }

        public void BindData()
        {
            this.SetStatus();

            var reward = this.Model.Quest.Record.Rewards[0];
            this.img.sprite = this.gameAssets.LoadAssetAsync<Sprite>(reward.Image).WaitForCompletion();
            this.txt.text   = reward.Value.ToString();

            var localPosition = this.transform.localPosition;
            localPosition.z              = 0f;
            this.transform.localPosition = localPosition;
            this.transform.localScale    = Vector3.one;
        }

        private void SetStatus()
        {
            switch (this.Model.Quest.Progress.Status)
            {
                case QuestStatus.NotCompleted:
                {
                    this.btn.interactable  = false;
                    this.animShake.enabled = false;
                    this.tick.SetActive(false);
                    break;
                }
                case QuestStatus.NotCollected:
                {
                    this.btn.interactable  = true;
                    this.animShake.enabled = true;
                    this.tick.SetActive(false);
                    break;
                }
                case QuestStatus.All:
                {
                    this.btn.interactable  = false;
                    this.animShake.enabled = false;
                    this.tick.SetActive(true);
                    break;
                }
            }
        }

        private void OnClick()
        {
            this.Model.Quest.CollectReward(this.transform as RectTransform).Forget();
            this.SetStatus();
        }
    }
}