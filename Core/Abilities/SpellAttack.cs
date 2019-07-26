using System.Collections;
using System.Collections.Generic;
using Project.Core.Characters;
using Project.Core.Grid;
using Project.Core.VFX;
using UnityEngine;

namespace Project.Core.Abilities {
    [CreateAssetMenu(menuName = "Abilities/Spell Attack", fileName = "Spell Attack")]
    public class SpellAttack : Ability {
        public Projectile projectile;

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
                        return true;
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
            throw new System.NotImplementedException();
        }

        public override void OnCompleted() {
            base.OnCompleted();
        }

        public override void OnTargetSelected() {
            base.OnTargetSelected();
            _character.StartCoroutine(CastSpellAttack());
        }

        private IEnumerator CastSpellAttack() {
            yield return _character.StartCoroutine(_character.CastSpell(this, _target));
        }
    }
}