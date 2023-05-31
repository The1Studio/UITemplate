namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface
{
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;

    public interface ITargetState<TTarget> : IState
    where TTarget : ITarget
    {
        public TTarget Target { get; set; }
    }
}