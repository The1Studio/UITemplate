namespace UITemplate.Scripts.Scenes.Main.Collection
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UITemplate.Scripts.Models;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateCollectionItemView : TViewMono
    {
        [SerializeField] private GameObject objUnlocked, objLocked, objOwned;
        [SerializeField] private Image      imgItem;
        
        public                   Image      ImgItem     => this.imgItem;
        public                   GameObject ObjUnlocked => this.objUnlocked;
        public                   GameObject ObjLocked   => this.objLocked;
        public                   GameObject ObjOwned    => this.objOwned;

        public void Init(ItemData.Status status)
        {
            this.Init();
            switch (status)
            {
                case ItemData.Status.Owned: this.InitItemOwned();
                    break;
                case ItemData.Status.Unlocked: this.InitItemUnLocked();
                    break;
                case ItemData.Status.Locked: this.InitItemLocked();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void Init()
        {
            this.objLocked.SetActive(false);
            this.objOwned.SetActive(false);
            this.objUnlocked.SetActive(false);
        }

        private void InitItemLocked()
        {
            this.objLocked.SetActive(true);
        }

        private void InitItemUnLocked()
        {
            this.objUnlocked.SetActive(true);
        }

        private void InitItemOwned()
        {
            this.objOwned.SetActive(true);
        }
    }

    public class UITemplateCollectionItemModel
    {
        public ItemData ItemData;
        public string   Category;

        public UITemplateCollectionItemModel(ItemData itemData, string category)
        {
            this.ItemData = itemData;
            this.Category = category;
        }
    }

    public class UITemplateCollectionItemPresenter : BaseUIItemPresenter<UITemplateCollectionItemView, UITemplateCollectionItemModel>
    {
        private readonly IGameAssets        gameAssets;
        private readonly UITemplateUserData userData;

        public UITemplateCollectionItemPresenter(IGameAssets gameAssets, UITemplateUserData userData) : base(gameAssets)
        {
            this.gameAssets = gameAssets;
            this.userData   = userData;
        }

        public override void BindData(UITemplateCollectionItemModel param)
        {
            this.View.Init(param.ItemData.CurrentStatus);
        }
    }
}