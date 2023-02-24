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


    public class UITemplateLevelItemView : TViewMono
    {
        public TMP_Text LevelText;
        public Image    BackgroundImage;
        public Button   LevelButton;

        [SerializeField]
        private Sprite LockedSprite;

        [SerializeField]
        private Sprite NowSprite;

        [SerializeField]
        private Sprite PassedSprite;

        [SerializeField]
        private Sprite SkippedSprite;

        public virtual void InitView(LevelData data, UITemplateUserLevelData userLevelData)
        {
            this.LevelText.text         = data.Level.ToString();
            this.BackgroundImage.sprite = this.GetStatusBackground(data.LevelStatus);
            if (data.Level == userLevelData.CurrentLevel) this.BackgroundImage.sprite = this.NowSprite;
            this.LevelButton.interactable = data.Level <= userLevelData.CurrentLevel;
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

    public class UITemplateLevelItemPresenter : BaseUIItemPresenter<UITemplateLevelItemView, LevelData>
    {
        #region inject

        private readonly IGameAssets gameAssets;

        private readonly SignalBus           signalBus;
        private readonly IScreenManager      screenManager;
        private          UITemplateUserLevelData userLevelData;

        #endregion

        private LevelData _model;
        public UITemplateLevelItemPresenter(IGameAssets gameAssets, SignalBus signalBus, IScreenManager screenManager, UITemplateUserLevelData userLevelData) : base(gameAssets)
        {
            this.signalBus     = signalBus;
            this.screenManager = screenManager;
            this.userLevelData     = userLevelData;
        }
        public override void BindData(LevelData param)
        {
            this._model = param;
            this.View.InitView(param, this.userLevelData);
            this.View.LevelButton.onClick.RemoveAllListeners();
            this.View.LevelButton.onClick.AddListener(this.OnClick);
        }

        protected virtual void OnClick()
        {
            #region test
            
            this.userLevelData.LevelToLevelData[this.userLevelData.CurrentLevel].LevelStatus = LevelData.Status.Passed;
            this.userLevelData.LevelToLevelData[this.userLevelData.CurrentLevel].StarCount = Random.Range(1, 4);

            #endregion
        }
    }
}