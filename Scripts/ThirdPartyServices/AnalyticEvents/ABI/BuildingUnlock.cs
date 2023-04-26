namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.ABI
{
    using Core.AnalyticServices.Data;

    public class BuildingUnlock : IEvent
    {
        public bool IsSuccess;

        public BuildingUnlock(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }
    }
}