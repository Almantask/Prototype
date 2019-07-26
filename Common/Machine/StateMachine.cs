using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Common.Machine {
    public abstract class StateMachine<T> : MonoBehaviour where T : State {
        [SerializeField] private T _currentState;
        public T CurrentState { get { return _currentState; } protected set { Transition(value); } }
        public Dictionary<string, T> States = new Dictionary<string, T>();

        private bool _inTransition;

        public abstract void Initialize();

        protected void AddState(T state) {
            if(!States.ContainsKey(state.Name)) States.Add(state.Name, state);
        }
        protected void RemoveState(string name) {
            if(States.ContainsKey(name)) States.Remove(name);
        }
        protected T GetState(string name) {
            States.TryGetValue(name, out T state);
            return state;
        }
        public virtual void ChangeState(string name) {
            CurrentState = GetState(name);
        }

        protected virtual void Update() {
            if(CurrentState != null) {
                CurrentState.OnUpdate();
            }
        }

        protected virtual void Transition(T target) {
            if(target == null) return;
            if(_inTransition || _currentState == target) return;
            _inTransition = true;
            if(_currentState != null) _currentState.OnExit();
            _currentState = target;
            if(_currentState != null) _currentState.OnEnter();
            _inTransition = false;
        }
    }
}