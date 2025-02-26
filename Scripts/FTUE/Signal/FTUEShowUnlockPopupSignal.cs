namespace TheOneStudio.UITemplate.UITemplate.FTUE.Signal
{
    using System;

    public class FTUEShowUnlockPopupSignal
    {
        public Action OnClose         { get; }
        public string ItemId          { get; }
        public string ItemName        { get; }
        public string NextScreenName  { get; }
        public string DestinationName { get; }

        public FTUEShowUnlockPopupSignal(Action onClose, string itemId, string itemName, string nextScreenName, string destinationName)
        {
            this.OnClose         = onClose;
            this.ItemId          = itemId;
            this.ItemName        = itemName;
            this.NextScreenName  = nextScreenName;
            this.DestinationName = destinationName;
        }
    }
}