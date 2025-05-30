namespace TheOneStudio.UITemplate.UITemplate.Services.StoreRating
{
    using Cysharp.Threading.Tasks;
    using TheOne.Extensions;
    using TheOne.Logging;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = TheOne.Logging.ILogger;

    public class DummyStoreRatingService : IStoreRatingService
    {
        #region Inject

        private readonly ILogger logger;

        #endregion

        [Preserve]
        public DummyStoreRatingService(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
        }

        public UniTask LaunchStoreRating()
        {
            this.logger.Info("Launch Rating".WithColor(Color.cyan));
            return UniTask.CompletedTask;
        }
    }
}