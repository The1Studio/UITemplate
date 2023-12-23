namespace TheOneStudio.UITemplate.Quests.UI
{
    using System;
    using System.Linq;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.Utilities.Extension;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateQuestItemModel
    {
        public UITemplateQuestController Controller   { get; }
        public Action                    OnClickGo    { get; }
        public Action                    OnClickClaim { get; }

        public UITemplateQuestItemModel(
            UITemplateQuestController controller,
            Action                    onClickGo,
            Action                    onClickClaim
        )
        {
            this.Controller   = controller;
            this.OnClickGo    = onClickGo;
            this.OnClickClaim = onClickClaim;
        }
    }

    public class UITemplateQuestItemView : MonoBehaviour
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
        public UITemplateQuestItemView Construct(IGameAssets gameAssets)
        {
            this.gameAssets = gameAssets;
            return this;
        }

        public UITemplateQuestItemModel Model { get; set; }

        public void OnSpawn()
        {
            // Left
            var record = this.Model.Controller.Record;
            var reward = record.Rewards.Single();
            this.imgReward.sprite = this.gameAssets.LoadAssetAsync<Sprite>(reward.Image).WaitForCompletion();
            this.txtReward.text   = reward.Value.ToString();
            this.txtName.text     = record.Name;
            this.txtDesc.text     = record.Description;

            // Right
            var progressHandler = this.Model.Controller.GetCompleteProgressHandlers().Single();
            this.txtProgress.text  = $"{progressHandler.CurrentProgress}/{progressHandler.MaxProgress}";
            this.sldProgress.value = progressHandler.CurrentProgress / progressHandler.MaxProgress;

            this.normalObjects.ForEach(obj => obj.SetActive(this.sldProgress.value < 1f));
            this.completedObjects.ForEach(obj => obj.SetActive(this.sldProgress.value >= 1f));

            this.transform.localScale = Vector3.one;
        }

        private void OnClickGo()
        {
            this.Model.OnClickGo();
        }

        private void OnClickClaim()
        {
            this.Model.Controller.CollectReward();
            this.Model.OnClickClaim();
        }
    }
}