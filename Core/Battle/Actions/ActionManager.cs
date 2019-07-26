using System.Collections;
using System.Collections.Generic;
using Project.Common;
using UnityEngine;

namespace Project.Core.Actions {
    public class ActionManager : MonoBehaviour {
        private GameAction _currentAction;
        private IEnumerator _currentSequence;
        private List<GameAction> _actions;

        public bool InProcess { get { return _currentSequence != null; } }

        private void AddActionToQueue(GameAction action) {
            _actions.Add(action);
            _actions.Sort(SortActions);
        }

        private IEnumerator ProcessActions() {
            while(_actions.Count > 0) {
                _currentAction = _actions[0];
                _currentSequence = _currentAction.Action;
                yield return StartCoroutine(_currentSequence);
            }
            _currentAction = null;
            _currentSequence = null;
        }

        private int SortActions(GameAction x, GameAction y) {
            if(x.Priority != y.Priority) {
                return y.Priority.CompareTo(x.Priority);
            }
            return x.OrderOfPlay.CompareTo(y.OrderOfPlay);
        }
    }
}