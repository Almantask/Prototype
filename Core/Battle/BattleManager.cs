using System.Collections;
using System.Collections.Generic;
using Project.Core.Characters;
using Project.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core.Battle {
    [RequireComponent(typeof(TurnStateMachine))]
    public class BattleManager : MonoBehaviour {
        private BattleSettings _settings;
        private GameObject _charactersParent;
        public List<Character> Characters { get; private set; }

        private TurnStateMachine stateMachine;

        [SerializeField] private Character _activeCharacter;
        public Character ActiveCharacter { get { return _activeCharacter; } }
        [SerializeField] private int _roundCounter;
        public int RoundCounter { get { return _roundCounter; } }

        private void OnEnable() {
            this.AddObserver(EndTurn, Events.CharacterTurnEnded);
            this.AddObserver(RemoveCharacter, Events.CharacterDeath);
            SceneManager.sceneLoaded += OnBattleLoaded;
        }

        private void OnDestroy() {
            this.RemoveObserver(EndTurn, Events.CharacterTurnEnded);
            this.RemoveObserver(RemoveCharacter, Events.CharacterDeath);
            SceneManager.sceneLoaded -= OnBattleLoaded;
        }

        public void OnBattleLoaded(Scene scene, LoadSceneMode mode) {
            stateMachine = GetComponent<TurnStateMachine>();
            stateMachine.Initialize();
            _settings = GameManager.Settings.battleSettings;
            StartCoroutine(SetupBattle());
        }

        private IEnumerator SetupBattle() {
            GameManager.GridManager.InitializeGrid();
            yield return new WaitForSeconds(0.1f);
            RegisterCharacters();
            yield return new WaitForSeconds(0.5f);
            StartBattle();
        }

        private void StartBattle() {
            StartCoroutine(StartRound());
        }

        private IEnumerator StartRound() {
            this.PostNotification(Events.RoundStarted);
            _roundCounter++;
            CalculateTurnOrder();
            UpdateCharactersBeforeRound();
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(StartNextCharacterTurn());
        }

        private IEnumerator StartNextCharacterTurn() {
            _activeCharacter = FindCharacterForNextTurn();
            _activeCharacter.SetTile(_activeCharacter.Tile);
            yield return new WaitForSeconds(1f);
            this.PostNotification(Events.CharacterTurnStarted, new Events.OnCharacterTurnStarted(_activeCharacter));
            _activeCharacter.SetIndicatorActive(true);
            stateMachine.ChangeState("Move");
            //this.PostNotification(Events.CharacterTurnStarted, new Events.OnCharacterTurnStarted(_activeCharacter));
        }

        private void EndTurn(object sender, object args) {
            _activeCharacter = ((Events.OnCharacterTurnEnded)args).Character;
            _activeCharacter.ActionPointsLeft = 0;
            Debug.Log(_activeCharacter.name + "'s Turn Ended");
            if(FindCharacterForNextTurn() == null) {
                EndRound();
            } else {
                StartCoroutine(StartNextCharacterTurn());
            }
        }

        private void EndRound() {
            this.PostNotification(Events.RoundEnded);
            Debug.Log("Round Ended");
            StartCoroutine(StartRound());
        }

        private void CalculateTurnOrder() {
            Characters.Sort((a, b) => a.Data.Speed.CompareTo(b.Data.Speed));
        }

        private void UpdateCharactersBeforeRound() {
            foreach(var character in Characters) {
                character.UpdateBeforeRound();
            }
        }

        private Character FindCharacterForNextTurn() {
            for(int i = 0; i < Characters.Count; i++) {
                if(Characters[i].ActionPointsLeft > 0) {
                    return Characters[i];
                }
            }
            return null;
        }

        private void RegisterCharacters() {
            _charactersParent = new GameObject("Characters");
            Characters = new List<Character>();
            for(int i = 0; i < _settings.characters.Count; i++) {
                var character = InstantiateCharacter(_settings.characters[i]);
                Characters.Add(character);
            }
            this.PostNotification(Events.CharactersSpawned, new Events.OnCharactersSpawned(Characters));
        }

        private void RemoveCharacter(object sender, object args) {
            var character = ((Events.OnCharacterDeath)args).Character;
            Characters.Remove(character);
            CheckForWin();
        }

        private Character InstantiateCharacter(CharacterData data) {
            var character = Instantiate(data.prefab, data.spawnPosition, Quaternion.identity, _charactersParent.transform);
            character.Initialize(data);
            character.SetTile(GameManager.GridManager.GetTile(data.spawnPosition));
            if(character.Controller == Controller.AI) {
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            return character;
        }

        private bool CheckForWin() {
            var list = new List<Character>();
            foreach(var character in Characters) {
                if(character.Controller == Controller.AI) {
                    list.Add(character);
                }
            }
            if(list.Count > 0) return false;
            this.PostNotification(Events.BattleEnded);
            return true;
        }
    }
}