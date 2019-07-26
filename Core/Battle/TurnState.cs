using System.Collections;
using System.Collections.Generic;
using Project.Common.Machine;
using Project.Core.Abilities;
using Project.Core.Characters;
using Project.Core.Grid;
using Project.Core.Inputs;
using UnityEngine;

namespace Project.Core.Battle {
    [System.Serializable]
    public abstract class TurnState : State {
        protected TurnState(string name, TurnStateMachine fsm) : base(name) {
            FSM = fsm;
        }

        public Character Character { get; protected set; }
        public TurnStateMachine FSM { get; protected set; }

        public abstract void OnTileLeftClicked();
        public abstract void OnTileRightClicked();
        public abstract void OnAbilityButtonPressed();
        public abstract void OnEscapeButtonPressed();
        public abstract void OnEndTurnButtonPressed();
    }

    public class MoveState : TurnState {
        public MoveState(string name, TurnStateMachine fsm) : base(name, fsm) { }

        private GridManager _gridManager;
        private InputManager _inputManager;
        private Tile _target;
        private List<Vector3> _path;

        public override void AddObservers() {
            base.AddObservers();
        }

        public override void RemoveObservers() {
            base.RemoveObservers();
        }

        public override void OnEnter() {
            base.OnEnter();
            _gridManager = GameManager.GridManager;
            _inputManager = GameManager.InputManager;
            Character = GameManager.BattleManager.ActiveCharacter;
            Character.MovementRange = GameManager.GridManager.FindMovementRange(Character.Tile, 5);
            _gridManager.SetOutlineMaterials(Character.MovementRange, true);
            Character.ShowMovementRange(true);
            Character.SearchingPath = true;
        }

        public override void OnExit() {
            Character.ShowMovementRange(false);
            Character.MovementRange = new List<Tile>();
            Character.SetPath(new List<Vector3>());
            base.OnExit();
        }

        public override void OnUpdate() {
            base.OnUpdate();
            FindPath();
        }

        private void FindPath() {
            if(Character.SearchingPath) {
                _target = _inputManager.HoverTile;
                if(!_target) return;
                if(!Character.MovementRange.Contains(_target)) {
                    Character.ShowPath(false);
                    return;
                }
                _path = _gridManager.SmoothPath(_gridManager.FindPath(Character.Tile, _target));
                _path.Add(_inputManager.HoverTile.Position);
                Character.SetPath(_path);
                Character.ShowPath(true);
            }
        }

        public override void OnTileRightClicked() {
            Character.ShowMovementRange(false);
            Character.ShowPath(false);
            Character.SetTile(null);
            Character.StartCoroutine(Character.Move());
            Character.ActionPointsLeft--;
            Character.SetTile(_target);
        }

        public override void OnTileLeftClicked() {
            //Tile Actions Here.
        }

        public override void OnAbilityButtonPressed() {
            FSM.ChangeState("Execution");
        }

        public override void OnEscapeButtonPressed() {
            //TODO Open Escape Menu
        }

        public override void OnEndTurnButtonPressed() {
            this.PostNotification(Events.CharacterTurnEnded, new Events.OnCharacterTurnEnded(Character));
            FSM.ChangeState("Idle");
        }
    }

    public class ExecutionState : TurnState {
        public ExecutionState(string name, TurnStateMachine fsm) : base(name, fsm) { }

        private GridManager _gridManager;
        private InputManager _inputManager;
        private Tile _target;
        private Ability _ability;

        public override void AddObservers() {
            base.AddObservers();
        }

        public override void RemoveObservers() {
            base.RemoveObservers();
        }

        public override void OnEnter() {
            base.OnEnter();
            _gridManager = GameManager.GridManager;
            _inputManager = GameManager.InputManager;
            Character = GameManager.BattleManager.ActiveCharacter;
            _ability = FSM.CurrentAbility;
            Character.AbilityRange = _gridManager.FindAbilityRange(Character.Tile, _ability.range);
            _gridManager.SetOutlineMaterials(Character.AbilityRange, false);
            Character.ShowAbilityRange(true);
        }

        public override void OnExit() {
            base.OnExit();
            Character.ShowAbilityRange(false);
            Character.AbilityRange = new List<Tile>();
        }

        public override void OnAbilityButtonPressed() {
            FSM.StartCoroutine(AbilityButtonPressedCoroutine());
        }

        private IEnumerator AbilityButtonPressedCoroutine() {
            Character.ShowAbilityRange(false);
            Character.AbilityRange = new List<Tile>();
            _ability = FSM.CurrentAbility;
            Character.AbilityRange = _gridManager.FindAbilityRange(Character.Tile, _ability.range);
            _gridManager.SetOutlineMaterials(Character.AbilityRange, false);
            Character.ShowAbilityRange(true);
            yield return new WaitForSeconds(0.25f);
        }

        public override void OnEscapeButtonPressed() {
            if(Character.HadMovedThisTurn) {
                Character.ShowAbilityRange(false);
            } else {
                FSM.ChangeState("Move");
            }
        }

        public override void OnTileLeftClicked() {
            ITargetable target = _inputManager.HoverTile;
            _ability.Execute(Character);
            if(_ability.IsTargetValid(target)) {
                _ability.OnTargetSelected();
            }
        }

        public override void OnTileRightClicked() {
            throw new System.NotImplementedException();
        }

        public override void OnEndTurnButtonPressed() {
            this.PostNotification(Events.CharacterTurnEnded, new Events.OnCharacterTurnEnded(Character));
            FSM.ChangeState("Idle");
        }
    }

    public class IdleState : TurnState {
        public IdleState(string name, TurnStateMachine fsm) : base(name, fsm) {
        }

        public override void OnAbilityButtonPressed() {
            throw new System.NotImplementedException();
        }

        public override void OnEndTurnButtonPressed() {
            throw new System.NotImplementedException();
        }

        public override void OnEscapeButtonPressed() {
            throw new System.NotImplementedException();
        }

        public override void OnTileLeftClicked() {
            throw new System.NotImplementedException();
        }

        public override void OnTileRightClicked() {
            throw new System.NotImplementedException();
        }
    }
}