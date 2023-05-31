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
        protected readonly SignalBus   signalBus;
        protected readonly ILogService logService;
        
        public           IState      CurrentState { get; private set; }

        private readonly Dictionary<Type, IState> typeToState;

        protected StateMachine(List<IState> listState, SignalBus signalBus, ILogService logService)
        {
            this.signalBus   = signalBus;
            this.logService  = logService;
            this.typeToState = listState.ToDictionary(state => state.GetType(), state => state);
        }

        public void TransitionTo(Type stateType)
        {
            if (this.CurrentState != null)
            {
                this.CurrentState.Exit();
                this.logService.Log($"Exit {this.CurrentState.GetType().Name} State!!!");
            }
            
            if (!this.typeToState.TryGetValue(stateType, out var nextState)) return;

            this.CurrentState = nextState;
            nextState.Enter();
            this.logService.Log($"Enter {stateType.Name} State!!!");

        }
    }
}