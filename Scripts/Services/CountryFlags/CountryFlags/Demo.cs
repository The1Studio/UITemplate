namespace TheOneStudio.UITemplate.UITemplate.Services.CountryFlags.CountryFlags
{
    using TheOneStudio.UITemplate.UITemplate.Services.CountryFlags.CountryFlags.Scripts;
    using UnityEngine;
    using UnityEngine.UI;

    public class Demo : MonoBehaviour
    {
        private void Start()
        {
            this.GetComponent<Image>().sprite = FindObjectOfType<CountryFlags>().GetLocalDeviceFlagByDeviceLang();
        }

        public void _Next()
        {
            this.GetComponent<Image>().sprite = FindObjectOfType<CountryFlags>().GetRandomFlag();
        }
    }
}