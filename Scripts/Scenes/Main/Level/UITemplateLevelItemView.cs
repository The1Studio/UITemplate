namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Level
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class UITemplateLevelItemModel : LevelData
    {
        public UITemplateLevelItemModel(UITemplateLevelRecord record, int level, Models.LevelData.Status levelStatus, int starCount) : base(record)
        {
            base.LevelStatus = levelStatus;
            base.StarCount   = starCount;
            base.Level       = level;
        }
    }

    public class UITemplateLevelItemView : TViewMono
    {
        public TMP_Text LevelText;
        public Image    BackgroundImage;
        public Button   LevelButton;

        [SerializeField] private Sprite LockedSprite;
        [SerializeField] private Sprite NowSprite;
        [SerializeField] private Sprite PassedSprite;
        [SerializeField] private Sprite SkippedSprite;

        public virtual void InitView(UITemplateLevelItemModel data, UITemplateLevelData levelData)
        {
            this.LevelText.text         = data.Level.ToString();
            this.BackgroundImage.sprite = this.GetStatusBackground(data.LevelStatus);
            if (data.Level == levelData.CurrentLevel) this.BackgroundImage.sprite = this.NowSprite;
            this.LevelButton.interactable = data.LevelStatus != Models.LevelData.Status.Locked;
        }

        private Sprite GetStatusBackground(Models.LevelData.Status levelStatus) =>
            levelStatus switch
            {
                Models.LevelData.Status.Locked => this.LockedSprite,
                Models.LevelData.Status.Passed => this.PassedSprite,
                Models.LevelData.Status.Skipped => this.SkippedSprite,
                _ => throw new System.ArgumentOutOfRangeException(nameof(levelStatus), levelStatus, null)
            };
    }

    public class UITemplateLevelItemPresenter : BaseUIItemPresenter<UITemplateLevelItemView, UITemplateLevelItemModel>
    {
        #region inject

        private readonly IGameAssets gameAssets;

        private readonly SignalBus           signalBus;
        private readonly IScreenManager      screenManager;
        private          UITemplateLevelData levelData;

        #endregion

        private UITemplateLevelItemModel _model;
        public UITemplateLevelItemPresenter(IGameAssets gameAssets, SignalBus signalBus, IScreenManager screenManager, UITemplateLevelData levelData) : base(gameAssets)
        {
            this.signalBus     = signalBus;
            this.screenManager = screenManager;
            this.levelData     = levelData;
        }
        public override void BindData(UITemplateLevelItemModel param)
        {
            this._model = param;
            this.View.InitView(param, this.levelData);
            this.View.LevelButton.onClick.RemoveAllListeners();
            this.View.LevelButton.onClick.AddListener(this.OnClick);
        }

        private void OnClick() { Debug.Log("ClickLevelButton"); }
    }
}