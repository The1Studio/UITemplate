namespace UITemplate.Scripts.Scenes.Main.Level
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TMPro;
    using UITemplate.Scripts.Blueprints;
    using UITemplate.Scripts.Models;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;


    public class UITemplateLevelItemModel : LevelData
    {
        public UITemplateLevelItemModel(UITemplateLevelRecord record, int level, Models.LevelData.Status levelStatus, int starCount ) : base(record)
        {
            base.LevelStatus = levelStatus;
            base.StarCount   = starCount;
            base.Level       = level;
        }
        
    }

    public class UITemplateLevelItemView : TViewMono
    {
        public TMP_Text         LevelText;
        public Image            BackgroundImage;
        public Button           LevelButton;

        [SerializeField] private Sprite LockedSprite;
        [SerializeField] private Sprite NowSprite;
        [SerializeField] private Sprite PassedSprite;
        [SerializeField] private Sprite SkippedSprite;

        [Inject] private UITemplateLevelData levelData;
        public virtual void InitView(UITemplateLevelItemModel data)
        {
            this.LevelText.text           = data.Level.ToString();
            this.BackgroundImage.sprite   = this.GetStatusBackground(data.LevelStatus);
            if (data.Level == this.levelData.CurrentLevel) this.BackgroundImage.sprite = this.NowSprite;
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

        [Inject] private readonly SignalBus      signalBus;
        [Inject] private readonly IScreenManager screenManager;
        // [Inject] private readonly ILocalData     levelData;

        #endregion

        private UITemplateLevelItemModel _model;
        public UITemplateLevelItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }
        public override void BindData(UITemplateLevelItemModel param)
        {
            this._model = param;
            this.View.InitView(param);
            this.View.LevelButton.onClick.RemoveAllListeners();
            this.View.LevelButton.onClick.AddListener(this.OnClick);
        }

        private void OnClick() { Debug.Log("ClickLevelButton"); }
    }
}