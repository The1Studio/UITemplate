namespace TheOneStudio.UITemplate.UITemplate.Scripts.Signals
{
    using BlueprintFlow.Signals;

    public class TutorialCompletionSignal : ISignal
    {
        public bool   Success;
        public string TutorialId;

        public TutorialCompletionSignal(bool success, string tutorialId)
        {
            this.Success    = success;
            this.TutorialId = tutorialId;
        }
    }
}