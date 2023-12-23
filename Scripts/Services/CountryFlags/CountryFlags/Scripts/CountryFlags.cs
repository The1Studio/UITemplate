namespace TheOneStudio.UITemplate.UITemplate.Services.CountryFlags.CountryFlags.Scripts
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Utilities.Utils;

    public class CountryFlags : MonoBehaviour
    {
        public List<Sprite> listFlagSpite;
        public List<string> listCountryCode = new();

        private          Sprite                     spFlagDefault;
        private readonly Dictionary<string, Sprite> dictFlags = new();

        private static readonly List<string> ListPreferCountry   = new() { "us", "kr", "vn", "jp", "in", "sa", "es", "pt", "tr", "pl", "th", "ru" };
        private const           int          RATE_PREFER_COUNTRY = 85;

        private void Awake() { this.InitialResources(); }

        public void InitialResources()
        {
            if (this.listCountryCode.Count != 0) return;

            foreach (var sp in this.listFlagSpite)
            {
                this.dictFlags.Add(sp.name, sp);
                this.listCountryCode.Add(sp.name);
            }

            this.spFlagDefault = this.dictFlags[RegionHelper.COUNTRY_CODE_DEFAULT];
        }

        //
        // Summary:
        //     Get the sprite country flags of operating system is running in.
        public Sprite GetLocalDeviceFlagByDeviceLang()
        {
            var countryCode = RegionHelper.GetCountryCodeByDeviceLang();
            return this.dictFlags.TryGetValue(countryCode, out var spriteReturn) ? spriteReturn : this.spFlagDefault;
        }

        //
        // Summary:
        //     Get the sprite country flags of country code
        public Sprite GetFlag(string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode)) return this.spFlagDefault;
            return this.dictFlags.TryGetValue(countryCode, out var spriteReturn) ? spriteReturn : this.spFlagDefault;
        }

        //
        // Summary:
        //     Get random the sprite country flags
        public Sprite GetRandomFlag() { return this.dictFlags.TryGetValue(this.RandomCountryCode(), out var spriteReturn) ? spriteReturn : this.spFlagDefault; }

        public string RandomCountryCode() { return this.listCountryCode[Random.Range(0, this.listCountryCode.Count)]; }

        //
        // Summary:
        //     Get random the sprite country flags
        public Sprite GetRandomFlag(out string countryCode)
        {
            var randCountryCode = Random.Range(0, 100) < RATE_PREFER_COUNTRY
                ? ListPreferCountry[Random.Range(0, ListPreferCountry.Count)]
                : this.listCountryCode[Random.Range(0, this.listCountryCode.Count)];

            countryCode = randCountryCode;
            return this.dictFlags.TryGetValue(randCountryCode, out var spriteReturn) ? spriteReturn : this.spFlagDefault;
        }

        public static void ApplyFlagToImage(Image imgTarget, Sprite spFlag)
        {
            var rectTarget = imgTarget.GetComponent<RectTransform>();
            var width      = rectTarget.sizeDelta.x;
            var height     = width * 0.75f; // 3/4 is standard ratio flag

            imgTarget.sprite     = spFlag;
            rectTarget.sizeDelta = new Vector2(width, height);
        }
    }
}