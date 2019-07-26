using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Core.Abilities;

namespace Project.Core.UI {
    public class AbilityButton : MonoBehaviour {
        public Image skillIcon;

        public Ability Ability { get; private set; }

        public void SetAbility(Ability ability) {
            Ability = ability;
            skillIcon.sprite = ability.icon;
        }

        public void OnAbilityButtonPressed() {
            if(!Ability) return;
            this.PostNotification(Events.AbilityButtonPressed, new Events.OnAbilityButtonPressed(Ability));
        }
    }
}