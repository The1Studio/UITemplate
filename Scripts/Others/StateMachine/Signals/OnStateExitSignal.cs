namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Signals
{
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;

    public class OnStateExitSignal
    {
        public IState State { get; }

        public OnStateExitSignal(IState state) { this.State = state; }
    }
}