namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using DG.Tweening;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class UITemplateLeaderboardItemModel
    {
        public int    Rank;
        public Sprite CountryFlag;
        public string Name;
        public bool   IsYou;

        public UITemplateLeaderboardItemModel(int rank, Sprite countryFlag, string name, bool isYou)
        {
            this.Rank        = rank;
            this.CountryFlag = countryFlag;
            this.Name        = name;
            this.IsYou       = isYou;
        }
    }

    public class UITemplateLeaderboardItemView : TViewMono
    {
        public TMP_Text   RankText;
        public TMP_Text   NameText;
        public Image      FlagImage;
        public Image      BackGround;
        public TMP_Text   RankUpText;
        public GameObject RankUpObject;

        public Sprite OtherSpriteBg;
        public Sprite YourSpriteBg;

        public void SetRank(int rank) { this.RankText.text = $"#{rank}"; }
        public void SetRankUp(int rankUp) { this.RankUpText.text = rankUp.ToString(); }
        public void ShowRankUP()
        {
            this.RankUpObject.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public class UITemplateLeaderboardItemPresenter : BaseUIItemPresenter<UITemplateLeaderboardItemView, UITemplateLeaderboardItemModel>
    {
        [Preserve]
        public UITemplateLeaderboardItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(UITemplateLeaderboardItemModel param)
        {
            this.View.SetRank(param.Rank);
            this.View.NameText.text     = param.Name;
            this.View.NameText.text     = param.Name;
            this.View.NameText.fontSize = param.IsYou ? 50 : 30;
            this.View.FlagImage.sprite  = param.CountryFlag;
            this.View.FlagImage.gameObject.SetActive(!param.IsYou);
            this.View.RankUpObject.gameObject.SetActive(param.IsYou);
            this.View.RankUpObject.transform.localScale = Vector3.zero;

            this.View.BackGround.sprite                 = param.IsYou ? this.View.YourSpriteBg : this.View.OtherSpriteBg;
            this.View.GetComponent<CanvasGroup>().alpha = param.IsYou ? 0 : 1;
        }
    }
}