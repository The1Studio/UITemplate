namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Collection.Base
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Models;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class BaseItemCollectionModel
    {
        public int                Index              { get; set; }
        public UITemplateItemData UITemplateItemInventoryData { get; set; }
        public string             Category           { get; set; }
        public Action             OnNotEnoughMoney   { get; set; }
    }

    public abstract class BaseItemCollectionView : TViewMono
    {
        public GameObject      UnlockedObj;
        public GameObject      LockedObj;
        public GameObject      OwnedObj;
        public GameObject      ChoosedObj;
        public Image           ItemImage;
        public TextMeshProUGUI PriceText;
        public Button          SelectButton;
        public Button          BuyItemButton;
    }

    public abstract class BaseItemCollectionPresenter<TView, TModel> : BaseUIItemPresenter<TView, TModel> where TView : BaseItemCollectionView where TModel : BaseItemCollectionModel
    {
        protected BaseItemCollectionPresenter(IGameAssets gameAssets) : base(gameAssets)
        {
        }

        protected abstract string CategoryType { get; }
    }
}