namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine;

    public class DummyStoreRatingService : IStoreRatingService
    {
        #region Inject

        private readonly ILogService logService;

        #endregion

        public DummyStoreRatingService(ILogService logService) { this.logService = logService; }

        public UniTask LaunchStoreRating()
        {
            this.logService.LogWithColor("Launch Rating", Color.cyan);
            return UniTask.CompletedTask;
        }
    }
}