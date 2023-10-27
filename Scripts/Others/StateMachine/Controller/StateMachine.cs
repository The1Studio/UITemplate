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
        protected readonly ILogService              LogService;
        protected readonly SignalBus                SignalBus;
        protected readonly Dictionary<Type, IState> TypeToState;

        protected StateMachine(
            List<IState>           listState,
            SignalBus              signalBus,
            ILogService            logService
        )
        {
            this.SignalBus              = signalBus;
            this.LogService             = logService;
            this.TypeToState            = listState.ToDictionary(state => state.GetType(), state => state);
        }

        public IState CurrentState { get; private set; }

        public void TransitionTo<T>() where T : class, IState
        {
            this.TransitionTo(typeof(T));
        }

        public void TransitionTo<TState, TModel>(TModel model) where TState : class, IState<TModel>
        {
            this.CurrentState?.Exit();
            this.CurrentState = this.TypeToState.GetValueOrDefault(typeof(TState));
            if (this.CurrentState is not TState nextState) return;
            nextState.Model = model;
            nextState.Enter();
        }

        public virtual void TransitionTo(Type stateType)
        {
            if (this.CurrentState != null)
            {
                this.CurrentState.Exit();
                this.LogService.Log($"Exit {this.CurrentState.GetType().Name} State!!!");
            }

            if (!this.TypeToState.TryGetValue(stateType, out var nextState)) return;

            this.CurrentState = nextState;
            nextState.Enter();
            this.LogService.Log($"Enter {stateType.Name} State!!!");
        }

        public void Dispose()
        {
        }
    }
}