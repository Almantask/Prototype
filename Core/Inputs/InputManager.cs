using System;
using System.Collections;
using System.Collections.Generic;
using Project.Core.Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Core.Inputs {
    public class InputManager : MonoBehaviour {
        public Tile HoverTile { get; private set; }

        private void OnEnable() {
            this.AddObserver(OnCharacterTurnStarted, Events.CharacterTurnStarted);
            this.AddObserver(OnCharacterTurnEnded, Events.CharacterTurnEnded);
        }

        private void OnDisable() {
            this.RemoveObserver(OnCharacterTurnStarted, Events.CharacterTurnStarted);
            this.RemoveObserver(OnCharacterTurnEnded, Events.CharacterTurnEnded);
        }

        private bool _isInputsOn = true;
        private bool _canSearchTile;
        private MouseIndicator _indicator;

        private void Update() {
            if(!_isInputsOn) return;
            CheckInputs();
            if(_canSearchTile) {
                FindHoverTile();
            }
        }

        private void FindHoverTile() {
            bool isOverUI = EventSystem.current.IsPointerOverGameObject();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Tile previousTile = HoverTile;
            if(Physics.Raycast(ray, out RaycastHit hit, 100) && !isOverUI) {
                HoverTile = GameManager.GridManager.GetTile(hit.point);
                if(!HoverTile) return;
                _indicator.transform.position = HoverTile.Position;
                if(!previousTile || previousTile != HoverTile) {
                    HoverTile.SetAsHighlighted(true);
                } else {
                    previousTile = null;
                }
                if(HoverTile.Character) {
                    _indicator.SetIndicatorEnabled(false);
                } else {
                    _indicator.SetIndicatorEnabled(true);
                }
                if(HoverTile && Input.GetMouseButtonDown(0)) {
                    this.PostNotification(Events.TileLeftClicked, new Events.OnTileLeftClicked(HoverTile));
                }
                if(HoverTile && Input.GetMouseButtonDown(1)) {
                    this.PostNotification(Events.TileRightClicked, new Events.OnTileRightClicked(HoverTile));
                }
            } else {
                if(HoverTile) {
                    HoverTile = null;
                }
            }
            if(previousTile) {
                previousTile.SetAsHighlighted(false);
            }
        }

        private void OnCharacterTurnStarted(object sender, object args) {
            _canSearchTile = true;
            InstatiateIndicator();
            _indicator.gameObject.SetActive(true);
        }

        private void OnCharacterTurnEnded(object arg1, object arg2) {
            _indicator.gameObject.SetActive(false);
        }

        private void InstatiateIndicator() {
            if(_indicator) return;
            var prefab = GameManager.Settings.gridSettings.mouseIndicator;
            _indicator = Instantiate(prefab);
            _indicator.gameObject.SetActive(false);
        }

        private void CheckInputs() {
            if(Input.GetKeyDown(KeyCode.Alpha1)) {
                this.PostNotification(Events.AbilityShortcutPressed, new Events.OnAbilityShortcutPressed(0));
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) {
                this.PostNotification(Events.AbilityShortcutPressed, new Events.OnAbilityShortcutPressed(1));
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)) {
                this.PostNotification(Events.AbilityShortcutPressed, new Events.OnAbilityShortcutPressed(2));
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)) {
                this.PostNotification(Events.AbilityShortcutPressed, new Events.OnAbilityShortcutPressed(3));
            }
            if(Input.GetKeyDown(KeyCode.R)) {
                this.PostNotification(Events.AbilityShortcutPressed, new Events.OnAbilityShortcutPressed(4));
            }
            if(Input.GetKeyDown(KeyCode.Escape)) {
                this.PostNotification(Events.EscapeButtonPressed);
            }
            if(Input.GetKeyDown(KeyCode.Space)) {
                this.PostNotification(Events.EndTurnButtonPressed);
            }
        }
    }
}