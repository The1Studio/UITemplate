namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Signals
{
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;

    public class OnStateEnterSignal
    {
        public IState State { get; }

        public OnStateEnterSignal(IState state) { this.State = state; }
    }
}