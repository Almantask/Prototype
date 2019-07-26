using System.Collections;
using System.Collections.Generic;
using Project.Core.Characters;
using Project.Core.Grid;
using UnityEngine;

namespace Project.Core.Abilities {
    public enum TargetType { Self, Enemy, Friendly, Area }
    public abstract class Ability : ScriptableObject {
        [Header("General Settings")]
        public Sprite icon;
        [TextArea] public string description;
        [Space]
        [Space]
        public TargetType targetType;
        [Range(0, 10)] public int damage = 1;
        [Range(1, 10)] public int range = 1;
        [Range(0, 2)] public int cost = 1;
        [Range(0, 4)] public int cooldown = 0;
        public bool endsTurnAfterUse;

        protected Character _character;
        protected Tile _target;

        public virtual void Execute(Character character) {
            _character = character;
        }
        public virtual void OnTargetSelected() {
            this.PostNotification(Events.AbilityTargetSelected, new Events.OnAbilityTargetSelected(this, _target));
        }
        public abstract bool IsTargetValid(ITargetable target);

        public virtual void OnCompleted() {
            _character.ActionPointsLeft -= cost;
            this.PostNotification(Events.AbilityExecutionCompleted, new Events.OnAbilityExecutionCompleted(_character, this));
            if(endsTurnAfterUse) {
                this.PostNotification(Events.CharacterTurnEnded, new Events.OnCharacterTurnEnded(_character));
            } else {

            }
        }

        public virtual void OnCancel() {
            this.PostNotification(Events.AbilityExecutionCancelled, new Events.OnAbilityExecutionCancelled(this));
        }
    }
}