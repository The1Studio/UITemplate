namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Level
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

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

        public virtual void InitView(LevelData data, UITemplateLevelDataController userLevelData)
        {
            this.LevelText.text         = data.Level.ToString();
            this.BackgroundImage.sprite = this.GetStatusBackground(data.LevelStatus);
            if (data.Level == userLevelData.GetCurrentLevelData.Level) this.BackgroundImage.sprite = this.NowSprite;
            this.LevelButton.interactable = data.LevelStatus != LevelData.Status.Locked;
        }

        private Sprite GetStatusBackground(LevelData.Status levelStatus)
        {
            return levelStatus switch
            {
                LevelData.Status.Locked  => this.LockedSprite,
                LevelData.Status.Passed  => this.PassedSprite,
                LevelData.Status.Skipped => this.SkippedSprite,
                _                        => throw new ArgumentOutOfRangeException(nameof(levelStatus), levelStatus, null)
            };
        }
    }

    public class UITemplateLevelItemPresenter : BaseUIItemPresenter<UITemplateLevelItemView, LevelData>
    {
        [Preserve]
        public UITemplateLevelItemPresenter(IGameAssets gameAssets, UITemplateLevelDataController userLevelData) : base(gameAssets)
        {
            this.userLevelData = userLevelData;
        }

        public override void BindData(LevelData param)
        {
            this.View.InitView(param, this.userLevelData);
            this.View.LevelButton.onClick.RemoveAllListeners();
            this.View.LevelButton.onClick.AddListener(this.OnClick);
        }

        protected virtual void OnClick()
        {
            #region test

            var currentLevel = this.userLevelData.GetCurrentLevelData.Level;
            this.userLevelData.GetLevelData(currentLevel).LevelStatus = LevelData.Status.Passed;
            this.userLevelData.GetLevelData(currentLevel).StarCount   = Random.Range(1, 4);

            #endregion
        }

        #region inject

        private readonly IGameAssets gameAssets;

        private readonly UITemplateLevelDataController userLevelData;

        #endregion
    }
}