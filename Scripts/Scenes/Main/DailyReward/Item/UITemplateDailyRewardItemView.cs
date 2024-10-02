namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateDailyRewardItemModel
    {
        public UITemplateDailyRewardRecord DailyRewardRecord { get; set; }
        public UITemplateRewardRecord      RewardRecord      { get; set; }
        public RewardStatus                RewardStatus      { get; set; }
        public bool                        IsGetWithAds      { get; set; }
    }

    public class UITemplateDailyRewardItemView : TViewMono
    {
        [BoxGroup("Reward")] [SerializeField] private GameObject      objReward;
        [BoxGroup("Reward")] [SerializeField] private Image           imgReward;
        [BoxGroup("Reward")] [SerializeField] private TextMeshProUGUI txtValue;
        [BoxGroup("Reward")] [SerializeField] private GameObject      objLock;

        public GameObject      ObjReward => this.objReward;
        public Image           ImgReward => this.imgReward;
        public TextMeshProUGUI TxtValue  => this.txtValue;
        public GameObject      ObjLock   => this.objLock;

        public void UpdateIconRectTransform(Vector2? position, Vector2? size)
        {
            var rectTransform = this.objReward.GetComponent<RectTransform>();

            if (position is not null)
            {
                rectTransform.anchoredPosition = position.Value;
            }

            if (size is not null)
            {
                rectTransform.sizeDelta = size.Value;
            }
        }
    }

    public class UITemplateDailyRewardItemPresenter : BaseUIItemPresenter<UITemplateDailyRewardItemView, UITemplateDailyRewardItemModel>
    {
        public UITemplateDailyRewardItemModel Model { get; set; }

        #region inject

        private readonly UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper;

        #endregion

        [Preserve]
        public UITemplateDailyRewardItemPresenter(IGameAssets gameAssets, UITemplateDailyRewardItemViewHelper dailyRewardItemViewHelper) : base(gameAssets) { this.dailyRewardItemViewHelper = dailyRewardItemViewHelper; }

        public override void BindData(UITemplateDailyRewardItemModel param)
        {
            this.Model = param;
            this.dailyRewardItemViewHelper.BindDataItem(param, this.View, this);
        }

        public override void Dispose() { this.dailyRewardItemViewHelper.DisposeItem(this); }
    }
}