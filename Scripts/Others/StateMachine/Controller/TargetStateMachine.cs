namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Controller
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;
    using UnityEngine.Assertions;
    using Zenject;

    public abstract class TargetStateMachine<TTarget> : StateMachine
    where TTarget : ITarget
    {
        protected TargetStateMachine(TTarget target, List<IState> listState, SignalBus signalBus, ILogService logService) : base(listState, signalBus, logService)
        {
            foreach (var state in listState)
            {
                Assert.IsTrue(state is ITargetState<TTarget>, $"State {state.GetType()} must implement {nameof(ITargetState<TTarget>)}");
            }

            target.StateMachine = this;
            foreach (var state in this.TypeToState.Values)
            {
                if (state is ITargetState<TTarget> targetState)
                {
                    targetState.Target = target;
                }
            }
        }
    }
}