namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.LogService;
    using UnityEngine;
    using UnityEngine.Scripting;

    public class DummyStoreRatingService : IStoreRatingService
    {
        #region Inject

        private readonly ILogService logService;

        #endregion

        [Preserve]
        public DummyStoreRatingService(ILogService logService) { this.logService = logService; }

        public UniTask LaunchStoreRating()
        {
            this.logService.LogWithColor("Launch Rating", Color.cyan);
            return UniTask.CompletedTask;
        }
    }
}