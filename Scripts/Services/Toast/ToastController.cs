namespace TheOneStudio.UITemplate.UITemplate.Services.Toast
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class ToastController : MonoBehaviour
    {
        public GameObject      bottomObj;
        public GameObject      topObj;
        public GameObject      centerObj;
        public GameObject      toastObj;
        public TextMeshProUGUI txtToast;

        private GameObject GetParentObj(ToastPosition position)
        {
            return position switch
            {
                ToastPosition.Top => this.topObj,
                ToastPosition.Center => this.centerObj,
                ToastPosition.Bottom => this.bottomObj,
                _ => this.bottomObj
            };
        }

        public async void SetContent(string message, Vector2 offset, ToastPosition position)
        {
            this.toastObj.SetActive(false);
            this.toastObj.transform.SetParent(this.GetParentObj(position).transform);
            this.toastObj.transform.localPosition = Vector3.zero + (Vector3)offset;
            this.txtToast.text                    = message;
            this.toastObj.SetActive(true);
            this.toastObj.transform.DOLocalMoveY(100, 1f);
            await UniTask.Delay(1000);
            this.toastObj.SetActive(false);
        }
    }
}