namespace TheOneStudio.UITemplate.Quests.UI
{
    using TheOneStudio.UITemplate.Quests.Data;
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
        [SerializeField] private GameObject objNotCompleted;
        [SerializeField] private GameObject objNotCollected;
        [SerializeField] private GameObject objFinished;
        [SerializeField] private Button     btnClaim;

        private void Awake()
        {
            this.btnClaim.onClick.AddListener(this.OnClickClaim);
        }

        public UITemplateQuestChestItemModel Model { get; set; }

        public void BindData()
        {
            this.SetStatus();

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
                    this.objNotCompleted.SetActive(true);
                    this.objNotCollected.SetActive(false);
                    this.objFinished.SetActive(false);
                    break;
                }
                case QuestStatus.NotCollected:
                {
                    this.objNotCompleted.SetActive(false);
                    this.objNotCollected.SetActive(true);
                    this.objFinished.SetActive(false);
                    break;
                }
                case QuestStatus.All:
                {
                    this.objNotCompleted.SetActive(false);
                    this.objNotCollected.SetActive(false);
                    this.objFinished.SetActive(true);
                    break;
                }
            }
        }

        private void OnClickClaim()
        {
            this.Model.Quest.CollectReward(this.transform as RectTransform);
            this.SetStatus();
        }
    }
}