namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.LogService;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;
    using Zenject;

    public abstract class StateMachine : IStateMachine
    {
        protected readonly SignalBus                SignalBus;
        protected readonly ILogService              LogService;
        protected readonly Dictionary<Type, IState> TypeToState;

        public IState CurrentState { get; private set; }

        protected StateMachine(List<IState> listState, SignalBus signalBus, ILogService logService)
        {
            this.SignalBus   = signalBus;
            this.LogService  = logService;
            this.TypeToState = listState.ToDictionary(state => state.GetType(), state => state);
        }

        public void TransitionTo(Type stateType)
        {
            this.CurrentState?.Exit();

            if (!this.TypeToState.TryGetValue(stateType, out var nextState)) return;

            this.CurrentState = nextState;
            nextState.Enter();
        }
    }
}