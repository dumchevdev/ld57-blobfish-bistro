using System;
using System.Collections.Generic;

namespace Game.Runtime._Game.Scripts.Runtime.StateMachine
{
    public class StateMachine<T> : IDisposable where T : class
    {
        private readonly T _context;
        private readonly Dictionary<Type, State<T>> _states = new();
        
        private State<T> _currentState;
    
        public StateMachine(T context)
        {
            _context = context;
        }
    
        public void AddState<S>(S state) where S : State<T>
        {
            state.SetContext(_context);
            _states[typeof(S)] = state;
        }
    
        public void ChangeState<S>() where S : State<T>
        {
            if (_states.TryGetValue(typeof(S), out var newState))
            {
                _currentState?.OnExit();
                _currentState = newState;
                _currentState.OnEnter();
            }
            else
            {
                throw new ArgumentException($"State {typeof(S)} not registered in state machine");
            }
        }

        public void Dispose()
        {
            _currentState?.OnExit();
        }
    }
}