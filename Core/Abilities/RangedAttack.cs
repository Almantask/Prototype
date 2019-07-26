using System.Collections;
using System.Collections.Generic;
using Project.Core.Characters;
using UnityEngine;

namespace Project.Core.Abilities {
    [CreateAssetMenu(menuName = "Abilities/Ranged Attack", fileName = "Ranged Attack")]
    public class RangedAttack : Ability {
        public override void Execute(Character character) {
            base.Execute(character);
        }

        public override bool IsTargetValid(ITargetable target) {
            throw new System.NotImplementedException();
        }

        public override void OnCancel() {
            base.OnCancel();
        }

        public override void OnCompleted() {
            base.OnCompleted();
        }

        public override void OnTargetSelected() {
            base.OnTargetSelected();
        }
    }
}
