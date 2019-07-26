using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.UI {
    public class ActionBar : MonoBehaviour {
        private Dictionary<int, AbilityButton> _actions;
        public AbilityButton[] abilities;

        private void OnEnable() {
            AddButtons();
            this.AddObserver(RebuildActionBar, Events.CharacterTurnStarted);
            this.AddObserver(OnAbilityKeyPressed, Events.AbilityShortcutPressed);
        }

        private void OnDestroy() {
            this.RemoveObserver(RebuildActionBar, Events.CharacterTurnStarted);
            this.RemoveObserver(OnAbilityKeyPressed, Events.AbilityShortcutPressed);
        }

        private void AddButtons() {
            _actions = new Dictionary<int, AbilityButton>();
            for(int i = 0; i < abilities.Length; i++) {
                _actions.Add(i, abilities[i]);
            }
        }

        private void RebuildActionBar(object sender, object args) {
            var character = ((Events.OnCharacterTurnStarted)args).Character;
            for(int i = 0; i < character.Data.abilities.Count; i++) {
                var ability = character.Data.abilities[i];
                abilities[i].SetAbility(ability);
            }
            foreach(var button in abilities) {
                if(button.Ability == null) {
                    button.skillIcon.enabled = false;
                }
            }
        }

        private void OnAbilityKeyPressed(object sender, object args) {
            var index = ((Events.OnAbilityShortcutPressed)args).Key;
            _actions.TryGetValue(index, out AbilityButton button);
            button.OnAbilityButtonPressed();
        }
    }
}