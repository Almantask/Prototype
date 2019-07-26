using System;
using System.Collections;
using System.Collections.Generic;
using Project.Common.Machine;
using Project.Core.Abilities;
using Project.Core.Characters;
using UnityEngine;

namespace Project.Core.Battle {
    public class TurnStateMachine : StateMachine<TurnState> {
        public Character CurrentCharacter { get; private set; }
        public Ability CurrentAbility { get; private set; }

        private void OnEnable() {
            this.AddObserver(OnCharacterTurnStarted, Events.CharacterTurnStarted);
            this.AddObserver(OnCharacterTurnEnded, Events.CharacterTurnEnded);
            this.AddObserver(OnTileLeftClicked, Events.TileLeftClicked);
            this.AddObserver(OnTileRightClicked, Events.TileRightClicked);
            this.AddObserver(OnAbilityButtonPressed, Events.AbilityButtonPressed);
            this.AddObserver(OnEscapeButtonPressed, Events.EscapeButtonPressed);
            this.AddObserver(OnEndTurnButtonPressed, Events.EndTurnButtonPressed);
        }

        private void OnDisable() {
            this.RemoveObserver(OnCharacterTurnStarted, Events.CharacterTurnStarted);
            this.RemoveObserver(OnCharacterTurnEnded, Events.CharacterTurnEnded);
            this.RemoveObserver(OnTileLeftClicked, Events.TileLeftClicked);
            this.RemoveObserver(OnTileRightClicked, Events.TileRightClicked);
            this.RemoveObserver(OnAbilityButtonPressed, Events.AbilityButtonPressed);
            this.RemoveObserver(OnEscapeButtonPressed, Events.EscapeButtonPressed);
            this.RemoveObserver(OnEndTurnButtonPressed, Events.EndTurnButtonPressed);
        }

        public override void Initialize() {
            AddState(new MoveState("Move", this));
            AddState(new ExecutionState("Execution", this));
            AddState(new IdleState("Idle", this));
        }

        public void OnTileLeftClicked(object sender, object args) {
            CurrentState.OnTileLeftClicked();
        }

        public void OnTileRightClicked(object sender, object args) {
            CurrentState.OnTileRightClicked();
        }

        public void OnAbilityButtonPressed(object sender, object args) {
            CurrentAbility = ((Events.OnAbilityButtonPressed)args).Ability;
            CurrentState.OnAbilityButtonPressed();
        }

        public void OnEscapeButtonPressed(object sender, object args) {
            CurrentAbility = null;
            CurrentState.OnEscapeButtonPressed();
        }

        private void OnEndTurnButtonPressed(object sender, object args) {
            CurrentState.OnEndTurnButtonPressed();
        }

        private void OnCharacterTurnStarted(object sender, object args) {
            CurrentCharacter = ((Events.OnCharacterTurnStarted)args).Character;
        }

        private void OnCharacterTurnEnded(object sender, object args) {
            CurrentCharacter = null;
        }
    }
}