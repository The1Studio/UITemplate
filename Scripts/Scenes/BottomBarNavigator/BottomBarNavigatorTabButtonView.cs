namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BottomBarNavigatorTabButtonView : MonoBehaviour
    {
        public Button   Button;
        public TMP_Text TabName;
        public Image    TabIcon;

        public void SetActive(bool isActive)
        {
            this.TabIcon.rectTransform.DOMoveY(-30, 0.3f).SetEase(Ease.InOutBack);
            this.TabName.transform.DOScale(1.3f, 0.3f).SetEase(Ease.OutBounce);
        }
    }
}