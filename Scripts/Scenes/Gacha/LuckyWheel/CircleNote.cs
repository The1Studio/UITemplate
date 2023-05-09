namespace TheOneStudio.UITemplate.UITemplate.Scenes.Gacha.LuckyWheel
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class CircleNote : MonoBehaviour
    {
        public  Image     imgIconColor;
        private Coroutine coroutine;
        private float     currentTime = 0.5f;

        public void ChangeColor(float startTime)
        {
            if (!this.isActiveAndEnabled) return;
            
            if (this.coroutine != null)
            {
                this.currentTime = startTime;

                return;
            }

            this.coroutine = this.StartCoroutine(this.Run());
        }

        private IEnumerator Run()
        {
            while (true)
            {
                yield return new WaitForSeconds(this.currentTime);
                this.imgIconColor.color = new Color(Random.Range(0, 255f) / 255f, Random.Range(0, 255f) / 255f, Random.Range(0, 255f) / 255f);
            }
        }

        private void OnDisable()
        {
            if (this.coroutine != null)
            {
                this.StopCoroutine(this.coroutine);
                this.coroutine = null;
            }
        }
    }
}