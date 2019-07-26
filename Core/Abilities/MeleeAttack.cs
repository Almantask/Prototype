using System.Collections;
using System.Collections.Generic;
using Project.Core.Characters;
using Project.Core.Grid;
using UnityEngine;

namespace Project.Core.Abilities {
    [CreateAssetMenu(menuName = "Abilities/Melee Attack", fileName = "Melee Attack")]
    public class MeleeAttack : Ability {
        public override void Execute(Character character) {
            base.Execute(character);
        }

        public override bool IsTargetValid(ITargetable target) {
            _target = (Tile)target;
            switch(targetType) {
                case TargetType.Self:
                    break;
                case TargetType.Enemy:
                    if(_target.Character && _target.Character.Controller == Controller.AI) {
                        if(_character.AbilityRange.Contains(_target)) return true;
                        return false;
                    }
                    break;
                case TargetType.Friendly:
                    break;
                case TargetType.Area:
                    break;
            }
            return false;
        }

        public override void OnCancel() {
            base.OnCancel();
        }

        public override void OnCompleted() {
            base.OnCompleted();
        }

        public override void OnTargetSelected() {
            base.OnTargetSelected();
            _character.StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine() {
            yield return _character.StartCoroutine(_character.Attack(this, _target));
        }
    }
}
