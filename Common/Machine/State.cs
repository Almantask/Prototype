using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Common.Machine {
    [System.Serializable]
    public abstract class State {
        public string Name { get; private set; }

        protected State(string name) {
            Name = name;
        }

        public virtual void OnEnter() {
            AddObservers();
        }
        public virtual void OnUpdate() {

        }
        public virtual void OnExit() {
            RemoveObservers();
        }
        public virtual void AddObservers() { }
        public virtual void RemoveObservers() { }
    }
}