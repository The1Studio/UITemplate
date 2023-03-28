namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TMPro;
    using UnityEngine;
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
        public TMP_Text RankText;
        public TMP_Text NameText;
        public Image    FlagImage;
        public Image    BackGround;


        public Sprite OtherSpriteBg;
        public Sprite YourSpriteBg;
        
        public void SetRank(int rank)
        {
            this.RankText.text = $"#{rank}";
        }
    }

    public class UITemplateLeaderboardItemPresenter : BaseUIItemPresenter<UITemplateLeaderboardItemView, UITemplateLeaderboardItemModel>
    {
        public UITemplateLeaderboardItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(UITemplateLeaderboardItemModel param)
        {
            this.View.SetRank(param.Rank);
            this.View.NameText.text                     = param.Name;
            this.View.FlagImage.sprite                  = param.CountryFlag;
            this.View.BackGround.sprite                 = param.IsYou ? this.View.YourSpriteBg : this.View.OtherSpriteBg;
            this.View.GetComponent<CanvasGroup>().alpha = param.IsYou ? 0 : 1;
        }
    }
}