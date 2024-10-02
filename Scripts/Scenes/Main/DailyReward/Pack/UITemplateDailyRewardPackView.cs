namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Pack
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using TheOneStudio.UITemplate.UITemplate.Scenes.Main.DailyReward.Item;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class UITemplateDailyRewardPackModel
    {
        public Action<UITemplateDailyRewardPackPresenter> OnClick           { get; set; }
        public RewardStatus                               RewardStatus      { get; set; }
        public UITemplateDailyRewardRecord                DailyRewardRecord { get; set; }
        public bool                                       IsGetWithAds      { get; set; }

        public UITemplateDailyRewardPackModel(UITemplateDailyRewardRecord dailyRewardRecord, RewardStatus rewardStatus, Action<UITemplateDailyRewardPackPresenter> click)
        {
            this.DailyRewardRecord = dailyRewardRecord;
            this.RewardStatus      = rewardStatus;
            this.OnClick           = click;
        }
    }

    public class UITemplateDailyRewardPackView : TViewMono
    {
        [BoxGroup("View"), SerializeField] private UITemplateDailyRewardItemAdapter dailyRewardItemAdapter;
        [BoxGroup("View"), SerializeField] private Button                           btnClaim;
        [BoxGroup("View"), SerializeField] private TextMeshProUGUI                  txtDayLabel;
        [BoxGroup("View"), SerializeField] private GameObject                       objClaimed;
        [BoxGroup("View"), SerializeField] private GameObject                       objClaimedCheckIcon;
        [BoxGroup("View"), SerializeField] private GameObject                       objClaimByAds;

        [BoxGroup("View/Background"), SerializeField]
        private Image imgBackground;

        [BoxGroup("View/Background"), SerializeField]
        private Sprite sprBgNormal;

        [BoxGroup("View/Background"), SerializeField]
        private Sprite sprBgCurrentDay;

        [BoxGroup("View/PackImage"), SerializeField]
        private Image packImg;

        [BoxGroup("Feature/CoverPack")]
        [SerializeField]
        private bool coverPackWhenAllItemsHidden;

        [BoxGroup("Feature/CoverPack")]
        [SerializeField]
        private Image coverImg;

        public UITemplateDailyRewardItemAdapter DailyRewardItemAdapter      => this.dailyRewardItemAdapter;
        public Button                           BtnClaim                    => this.btnClaim;
        public TextMeshProUGUI                  TxtDayLabel                 => this.txtDayLabel;
        public GameObject                       ObjClaimed                  => this.objClaimed;
        public GameObject                       ObjClaimedCheckIcon         => this.objClaimedCheckIcon;
        public GameObject                       ObjClaimByAds               => this.objClaimByAds;
        public Image                            ImgBackground               => this.imgBackground;
        public Sprite                           SprBgNormal                 => this.sprBgNormal;
        public Sprite                           SprBgCurrentDay             => this.sprBgCurrentDay;
        public Image                            PackImg                     => this.packImg;
        public bool                             CoverPackWhenAllItemsHidden => this.coverPackWhenAllItemsHidden;
        public Image                            CoverImg                    => this.coverImg;
        public Action                           OnClickClaimButton          { get; set; }

        private void Awake()
        {
            if (this.btnClaim != null)
            {
                this.btnClaim.onClick.AddListener(() =>
                {
                    this.OnClickClaimButton?.Invoke();
                });
            }
        }
    }

    public class UITemplateDailyRewardPackPresenter : BaseUIItemPresenter<UITemplateDailyRewardPackView, UITemplateDailyRewardPackModel>
    {
        public UITemplateDailyRewardPackModel Model { get; set; }

        #region inject

        private readonly UITemplateDailyRewardPackViewHelper dailyRewardPackViewHelper;

        #endregion

        [Preserve]
        public UITemplateDailyRewardPackPresenter(
            IGameAssets                         gameAssets,
            UITemplateDailyRewardPackViewHelper dailyRewardPackViewHelper
        ) : base(gameAssets)
        {
            this.dailyRewardPackViewHelper = dailyRewardPackViewHelper;
        }

        public override void BindData(UITemplateDailyRewardPackModel param)
        {
            this.Model = param;

            this.dailyRewardPackViewHelper.BindDataItem(param, this.View, this);

            if (!string.IsNullOrEmpty(this.Model.DailyRewardRecord.PackImage)) return;
            var models = param.DailyRewardRecord.Reward.Values
                              .Select(item => new UITemplateDailyRewardItemModel
                              {
                                  DailyRewardRecord = this.Model.DailyRewardRecord,
                                  RewardRecord      = item,
                                  RewardStatus      = this.Model.RewardStatus,
                                  IsGetWithAds      = this.Model.IsGetWithAds
                              })
                              .ToList();
            this.View.DailyRewardItemAdapter.InitItemAdapter(models).Forget();

            this.CoverPack(models);
        }

        private void CoverPack(IEnumerable<UITemplateDailyRewardItemModel> itemModels)
        {
            if(!this.View.CoverPackWhenAllItemsHidden) return;

            if (this.Model.RewardStatus == RewardStatus.Claimed)
            {
                this.View.CoverImg.gameObject.SetActive(false);
                return;
            }

            var isAllItemHidden = itemModels.All(im => !im.RewardRecord.SpoilReward);
            this.View.CoverImg.gameObject.SetActive(isAllItemHidden);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.dailyRewardPackViewHelper.DisposeItem(this);
        }

        public void ClaimReward() { this.dailyRewardPackViewHelper.OnClaimReward(this); }
    }
}