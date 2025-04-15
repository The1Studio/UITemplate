namespace TheOneStudio.UITemplate.UITemplate.Services.Toast
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public enum ToastPosition
    {
        Bottom,
        Top,
        Center,
    }

    public class ToastController : MonoBehaviour
    {
        [SerializeField] private GameObject      bottomObj;
        [SerializeField] private GameObject      topObj;
        [SerializeField] private GameObject      centerObj;
        [SerializeField] private GameObject      toastObj;
        [SerializeField] private TextMeshProUGUI txtToast;

        private GameObject GetParentObj(ToastPosition position)
        {
            return position switch
            {
                ToastPosition.Top    => this.topObj,
                ToastPosition.Center => this.centerObj,
                ToastPosition.Bottom => this.bottomObj,
                _                    => this.bottomObj,
            };
        }

        public void ShowToast(string message, float offsetX = 0, float offsetY = 0, ToastPosition position = ToastPosition.Center, float duration = 1f, float endYPos = 100)
        {
            this.toastObj.SetActive(false);
            this.toastObj.transform.DOKill();
            this.toastObj.transform.SetParent(this.GetParentObj(position).transform);
            this.toastObj.transform.localPosition = Vector3.zero + new Vector3(offsetX, offsetY, 0);
            this.txtToast.text                    = message;

            this.toastObj.SetActive(true);
            this.toastObj.transform.DOLocalMoveY(endYPos, duration).SetUpdate(true).OnComplete(() =>
            {
                this.toastObj.SetActive(false);
            });
        }
    }
}