namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BottomBarNavigatorTabButtonView : MonoBehaviour
    {
        public GameObject lockObj;
        public Button     Button;
        public TMP_Text   TabName;
        public Image      TabIcon;
        public Material   grayScaleMat;

        protected bool IsActive = false;

        public void Init()
        {
            this.SetActive(false);
            this.TabName.transform.localScale = Vector3.zero;
        }

        public void SetActive(bool isActive)
        {
            if (isActive == this.IsActive) return;

            this.IsActive = isActive;
            this.SetAnimation();
        }

        protected virtual void SetAnimation()
        {
            const float duration = 0.3f;

            if (this.IsActive)
            {
                this.TabIcon.rectTransform.DOAnchorPosY(-30, duration).SetEase(Ease.OutBounce).SetUpdate(true);
                this.TabIcon.transform.DOScale(1.3f, duration).SetEase(Ease.OutBounce).SetUpdate(true);
                this.TabName.transform.DOScale(1, duration).SetEase(Ease.OutBack).SetUpdate(true);
            }
            else
            {
                this.TabIcon.rectTransform.DOAnchorPosY(-100, duration).SetEase(Ease.InBack).SetUpdate(true);
                this.TabIcon.transform.DOScale(1f, duration).SetEase(Ease.InBack).SetUpdate(true);
                this.TabName.transform.DOScale(0, duration).SetEase(Ease.InBack).SetUpdate(true);
            }
        }

        public virtual void ActivateLockButton(bool isActivate)
        {
            if (this.lockObj == null) return;
            this.lockObj.SetActive(isActivate);
            if (this.grayScaleMat == null) return;
            this.TabIcon.material = isActivate ? this.grayScaleMat : null;
        }
    }
}