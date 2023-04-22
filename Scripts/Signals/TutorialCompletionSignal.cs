namespace TheOneStudio.UITemplate.UITemplate.Scripts.Signals
{
    public class TutorialCompletionSignal
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