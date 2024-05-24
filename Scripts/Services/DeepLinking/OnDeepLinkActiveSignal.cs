namespace TheOneStudio.UITemplate.UITemplate.Services.DeepLinking
{
    public class OnDeepLinkActiveSignal
    {
        public string Url { get; set; }

        public OnDeepLinkActiveSignal(string url) { this.Url = url; }
    }
}