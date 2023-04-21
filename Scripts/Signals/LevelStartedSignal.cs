namespace TheOneStudio.UITemplate.UITemplate.Signals
{
    using BlueprintFlow.Signals;

    public class LevelStartedSignal:ISignal
    {
        public int Level;
        public LevelStartedSignal(int level) { this.Level = level; }
    }
}