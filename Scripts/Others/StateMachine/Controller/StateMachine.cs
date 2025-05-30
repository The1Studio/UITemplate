namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOne.Logging;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Signals;

    public abstract class StateMachine : IStateMachine, ITickable
    {
        #region inject

        protected readonly ILogger                  Logger;
        protected readonly SignalBus                signalBus;
        protected readonly Dictionary<Type, IState> TypeToState;

        #endregion

        protected StateMachine(
            List<IState>   listState,
            ILoggerManager loggerManager,
            SignalBus      signalBus
        )
        {
            this.Logger      = loggerManager.GetLogger(this);
            this.signalBus   = signalBus;
            this.TypeToState = listState.ToDictionary(state => state.GetType(), state => state);
        }

        public IState CurrentState { get; private set; }

        public void TransitionTo<T>() where T : class, IState
        {
            this.TransitionTo(typeof(T));
        }

        public void TransitionTo<TState, TModel>(TModel model) where TState : class, IState<TModel>
        {
            var stateType = typeof(TState);
            if (!this.TypeToState.TryGetValue(stateType, out var nextState)) return;

            if (nextState is not TState nextStateT) return;
            nextStateT.Model = model;

            this.InternalStateTransition(nextState);
        }

        public virtual void TransitionTo(Type stateType)
        {
            if (!this.TypeToState.TryGetValue(stateType, out var nextState)) return;

            this.InternalStateTransition(nextState);
        }

        private void InternalStateTransition(IState nextState)
        {
            if (this.CurrentState != null)
            {
                this.CurrentState.Exit();
                this.signalBus.Fire(new OnStateExitSignal(this.CurrentState));
                this.Logger.Info($"Exit {this.CurrentState.GetType().Name} State!!!");
            }

            this.CurrentState = nextState;
            this.signalBus.Fire(new OnStateEnterSignal(this.CurrentState));
            this.Logger.Info($"Enter {nextState.GetType().Name} State!!!");
            nextState.Enter();
        }

        public void Tick()
        {
            if (this.CurrentState is not ITickable tickableState) return;
            tickableState.Tick();
        }
    }
}