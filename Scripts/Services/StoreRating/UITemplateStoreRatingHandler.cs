namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using UnityEngine;

    public class UITemplateStoreRatingHandler
    {
        #region MyRegion

        private readonly IStoreRatingService storeRatingService;

        #endregion

        private const string StoreRatingLocalDataKey = "LD_StoreRating";

        public UITemplateStoreRatingHandler(IStoreRatingService storeRatingService) { this.storeRatingService = storeRatingService; }

        public void LaunchStoreRating()
        {
            this.storeRatingService.LaunchStoreRating();
            PlayerPrefs.SetString(StoreRatingLocalDataKey, "TRUE");
        }
        public bool IsRated  => PlayerPrefs.HasKey(StoreRatingLocalDataKey);
    }
}