namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.CollectionNew
{
    using System;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using TheOneStudio.UITemplate.UITemplate.Services;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    public class TopButtonItemModel
    {
        public int                        Index         { get; set; }
        public int                        SelectedIndex { get; set; }
        public string                     Title         { get; set; }
        public Sprite                     Icon          { get; set; }
        public Action<TopButtonItemModel> OnSelected    { get; set; }
    }

    public class TopButtonBarItemView : TViewMono
    {
        public Button   btnChoose, btnNormal;
        public Image    imgIcon;
        public TMP_Text txtTittle;

        public Action OnButtonClick;

        private void Awake()
        {
            this.btnChoose.onClick.AddListener(() =>
            {
                this.OnButtonClick?.Invoke();
            });
            this.btnNormal.onClick.AddListener(() =>
            {
                this.OnButtonClick?.Invoke();
            });
        }
    }

    public class TopButtonPresenter : BaseUIItemPresenter<TopButtonBarItemView, TopButtonItemModel>
    {
        private readonly UITemplateSoundServices soundServices;

        [Preserve]
        public TopButtonPresenter(IGameAssets gameAssets, UITemplateSoundServices soundServices) : base(gameAssets)
        {
            this.soundServices = soundServices;
        }

        public override async void BindData(TopButtonItemModel param)
        {
            if (this.View.txtTittle != null)
            {
                this.View.txtTittle.text = param.Title;
            }

            if (this.View.imgIcon != null)
            {
                this.View.imgIcon.sprite = param.Icon;
            }

            this.View.btnChoose.gameObject.SetActive(param.Index == param.SelectedIndex);
            this.View.btnNormal.gameObject.SetActive(param.Index != param.SelectedIndex);

            this.View.OnButtonClick = () => { param.OnSelected?.Invoke(param); };
        }

        public override void Dispose() { base.Dispose(); }
    }
}