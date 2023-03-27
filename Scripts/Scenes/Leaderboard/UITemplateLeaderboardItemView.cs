namespace TheOneStudio.UITemplate.UITemplate.Scenes.Leaderboard
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;

    public class UITemplateLeaderboardItemModel
    {
        public int    Rank;
        public string Country;
        public string Name;
        public bool   IsYou;
    }
    
    public class UITemplateLeaderboardItemView : TViewMono
    {
        
    }

    public class UITemplateLeaderboardItemPresenter : BaseUIItemPresenter<UITemplateLeaderboardItemView, UITemplateLeaderboardItemModel>
    {
        public UITemplateLeaderboardItemPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
        }
        
        public override void BindData(UITemplateLeaderboardItemModel param)
        {
        }
    }
}