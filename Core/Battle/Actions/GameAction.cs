using System;
using System.Collections;
using UnityEngine;

namespace Project.Core.Actions {
    [Serializable]
    public abstract class GameAction {
        public MonoBehaviour Controller { get; protected set; }
        public int Priority { get; protected set; }
        public int OrderOfPlay { get; protected set; }
        public IEnumerator Action { get; protected set; }

        public bool IsPaused { get; protected set; }
        public bool IsCancelled { get; protected set; }

        public Phase OnPreparePhase { get; protected set; }
        public Phase OnPerformPhase { get; protected set; }

        protected GameAction(int priority, int orderOfPlay, IEnumerator action) {
            Priority = priority;
            OrderOfPlay = orderOfPlay;
            Action = action;
            OnPreparePhase = new Phase(this, OnPrepare);
            OnPerformPhase = new Phase(this, OnPerform);
        }

        public virtual void Pause() {
            IsPaused = true;
        }

        public virtual void Cancel() {
            IsCancelled = true;
        }

        protected abstract void OnPrepare(GameAction action);
        protected abstract void OnPerform(GameAction action);
    }

    public class Phase {
        public GameAction Owner { get; protected set; }
        public Action<GameAction> EventHandler { get; protected set; }

        public Phase(GameAction owner, Action<GameAction> handler) {
            Owner = owner;
            EventHandler = handler;
        }

        public IEnumerator Flow(GameAction action) {
            bool hitKeyFrame = false;
            if(Owner.Action != null) {
                var sequence = Owner.Action;
                while(sequence.MoveNext()) {
                    var isKeyFrame = (sequence.Current is bool) && (bool)sequence.Current;
                    if(isKeyFrame) {
                        hitKeyFrame = true;
                        EventHandler(action);
                    }
                    yield return null;
                }
            }
            if(!hitKeyFrame) {
                EventHandler(action);
            }
        }
    }
}