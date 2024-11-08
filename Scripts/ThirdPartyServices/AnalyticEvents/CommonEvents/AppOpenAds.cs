namespace TheOneStudio.UITemplate.UITemplate.ThirdPartyServices.AnalyticEvents.CommonEvents
{
    using Core.AnalyticServices.Data;

    public class AppOpenCalled : IEvent
    {
        public string placement;

        public AppOpenCalled(string place)
        {
            this.placement = place;
        }
    }

    public class AppOpenEligible : IEvent
    {
        public string placement;

        public AppOpenEligible(string place)
        {
            this.placement = place;
        }
    }

    public class AppOpenLoadFailed : IEvent
    {
    }

    public class AppOpenLoaded : IEvent
    {
    }

    public class AppOpenFullScreenContentClosed : IEvent
    {
        public string placement;

        public AppOpenFullScreenContentClosed(string place)
        {
            this.placement = place;
        }
    }

    public class AppOpenFullScreenContentFailed : IEvent
    {
        public string placement;

        public AppOpenFullScreenContentFailed(string place)
        {
            this.placement = place;
        }
    }

    public class AppOpenFullScreenContentOpened : IEvent
    {
        public string placement;

        public AppOpenFullScreenContentOpened(string place)
        {
            this.placement = place;
        }
    }

    public class AppOpenClicked : IEvent
    {
        public string placement;

        public AppOpenClicked(string place)
        {
            this.placement = place;
        }
    }

    public enum AppOpenPlacement
    {
        FirstOpen,
        ResumeApp,
    }
}