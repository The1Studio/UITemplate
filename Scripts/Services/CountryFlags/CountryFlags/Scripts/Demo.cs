using UnityEngine;
using UnityEngine.UI;

namespace GenifyStudio.Scripts.Libs.Leaderboard.CountryFlags.Scripts
{
    public class Demo : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Image>().sprite = FindObjectOfType<CountryFlags>().GetLocalDeviceFlagByDeviceLang();
        }

        public void _Next()
        {
            GetComponent<Image>().sprite = FindObjectOfType<CountryFlags>().GetRandomFlag();
        }
    }
}