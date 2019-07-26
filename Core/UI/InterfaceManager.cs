using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core.UI {
    public class InterfaceManager : MonoBehaviour {
        private BattleUI _battleUI;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnBattleLoaded;
            this.AddObserver(OnMoveCompleted, Events.MoveCompleted);
            this.AddObserver(OnAbilityExecutionCompleted, Events.AbilityExecutionCompleted);
            this.AddObserver(OnCharacterSpawned, Events.CharactersSpawned);
            this.AddObserver(OnCharacterTurnStarted, Events.CharacterTurnStarted);
            this.AddObserver(OnDamageTaken, Events.DamageTaken);
            this.AddObserver(OnBattleEnded, Events.BattleEnded);
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnBattleLoaded;
            this.RemoveObserver(OnMoveCompleted, Events.MoveCompleted);
            this.RemoveObserver(OnAbilityExecutionCompleted, Events.AbilityExecutionCompleted);
            this.RemoveObserver(OnCharacterSpawned, Events.CharactersSpawned);
            this.RemoveObserver(OnCharacterTurnStarted, Events.CharacterTurnStarted);
            this.RemoveObserver(OnDamageTaken, Events.DamageTaken);
            this.RemoveObserver(OnBattleEnded, Events.BattleEnded);
        }

        private void OnBattleLoaded(Scene scene, LoadSceneMode mode) {
            _battleUI = FindObjectOfType<BattleUI>();
            _battleUI.gameObject.SetActive(false);
        }

        private void OnBattleEnded(object sender, object args) {
            _battleUI.gameObject.SetActive(true);
        }

        private void OnCharacterSpawned(object sender, object args) {
            var characters = ((Events.OnCharactersSpawned)args).Characters;
            foreach(var c in characters) {
                c.resourcesBar.SetCharacter(c);
            }
        }

        private void OnCharacterTurnStarted(object sender, object args) {
            var character = ((Events.OnCharacterTurnStarted)args).Character;
            character.resourcesBar.ResetAP(character);
        }

        private void OnMoveCompleted(object sender, object args) {
            var character = ((Events.OnMoveCompleted)args).Character;
            character.resourcesBar.DecreaseAP();
        }

        private void OnAbilityExecutionCompleted(object sender, object args) {
            var character = ((Events.OnAbilityExecutionCompleted)args).Character;
            character.resourcesBar.DecreaseAP();
        }

        private void OnDamageTaken(object sender, object args) {
            var character = ((Events.OnDamageTaken)args).Character;
            var damage = ((Events.OnDamageTaken)args).Damage;
            character.resourcesBar.ModifyHealth();
        }
    }
}