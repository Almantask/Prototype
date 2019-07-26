using System.Collections;
using System.Collections.Generic;
using Project.Core.Abilities;
using Project.Core.Characters;
using UnityEngine;

namespace Project.Core.Grid {
    public class Tile : MonoBehaviour, ITargetable {
        public static Vector3[] Directions = {
            new Vector3(-1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, 0, 1),
            new Vector3(-1, 0, 0), new Vector3(1, 0, 0),
            new Vector3(-1, 0, -1), new Vector3(0, 0, -1), new Vector3(1, 0, -1)
        };

        public Vector3 Position { get; private set; }
        public List<Tile> Neighbours = new List<Tile>(8);
        public List<Edge> Edges = new List<Edge>(4);
        public bool Taken { get; set; }

        public Renderer Renderer { get; private set; }

        public Character Character { get; private set; }
        public Obstacle Obstacle { get; private set; }

        public bool Highlighted { get; private set; }

        public void Initialize(Vector3 position) {
            Position = position;
            Renderer = GetComponentInChildren<Renderer>();
            Renderer.enabled = false;
        }

        public void SetCharacter(Character character) {
            Character = character;
            Taken = character;
            UpdateMaterial();
        }

        public void SetObstacle(Obstacle obstacle) {
            Obstacle = obstacle;
            Taken = obstacle;
            UpdateMaterial();
        }

        public void SetAsHighlighted(bool highlighted) {
            Highlighted = highlighted;
            if(Character) {
                Highlight();
            }
        }

        private void Highlight() {
            var settings = GameManager.Settings.gridSettings;
            if(Highlighted) {
                Renderer.material = settings.tileFillMaterial;
                switch(Character.Controller) {
                    case Controller.HUMAN:
                        if(Character == GameManager.BattleManager.ActiveCharacter) {
                            Renderer.enabled = false;
                        } else {
                            Renderer.material.SetColor("_BaseColor", settings.friendlyHighlighted);
                            Renderer.enabled = true;
                        }
                        break;
                    case Controller.AI:
                        if(Character == GameManager.BattleManager.ActiveCharacter) {
                            Renderer.enabled = false;
                        } else {
                            Renderer.material.SetColor("_BaseColor", settings.enemyHighlighted);
                            Renderer.enabled = true;
                        }
                        break;
                }
            } else {
                Renderer.material = settings.tileMaterial;
                switch(Character.Controller) {
                    case Controller.HUMAN:
                        if(Character == GameManager.BattleManager.ActiveCharacter) {
                            Renderer.enabled = false;
                        } else {
                            Renderer.material.SetColor("_BaseColor", settings.friendly);
                            Renderer.enabled = true;
                        }
                        break;
                    case Controller.AI:
                        if(Character == GameManager.BattleManager.ActiveCharacter) {
                            Renderer.enabled = false;
                        } else {
                            Renderer.material.SetColor("_BaseColor", settings.enemy);
                            Renderer.enabled = true;
                        }
                        break;
                }
            }
        }

        public void UpdateMaterial() {
            var settings = GameManager.Settings.gridSettings;
            if(Character) {
                switch(Character.Controller) {
                    case Controller.HUMAN:
                        if(Character == GameManager.BattleManager.ActiveCharacter) {
                            Renderer.enabled = false;
                        } else {
                            Renderer.material.SetColor("_BaseColor", settings.friendly);
                            Renderer.enabled = true;
                        }
                        break;
                    case Controller.AI:
                        if(Character == GameManager.BattleManager.ActiveCharacter) {
                            Renderer.enabled = false;
                        } else {
                            Renderer.material.SetColor("_BaseColor", settings.enemy);
                            Renderer.enabled = true;
                        }
                        break;
                }
            } else if(Obstacle) {
                Renderer.enabled = true;
            } else {
                Renderer.enabled = false;
            }
        }

        public Vector3 GetDirection(Tile other) {
            return other.Position - Position;
        }

        public bool IsConnected(Tile other) {
            var direction = GetDirection(other);
            if(direction == Directions[0]) {
                if(Edges[0].Walled || Edges[1].Walled) return false;
                if(other.Edges[2].Walled || other.Edges[3].Walled) return false;
                return true;
            }
            if(direction == Directions[1]) {
                if(Edges[0].Walled) return false;
                if(other.Edges[3].Walled) return false;
                return true;
            }
            if(direction == Directions[2]) {
                if(Edges[0].Walled || Edges[2].Walled) return false;
                if(other.Edges[1].Walled || other.Edges[3].Walled) return false;
                return true;
            }
            if(direction == Directions[3]) {
                if(Edges[1].Walled) return false;
                if(other.Edges[2].Walled) return false;
                return true;
            }
            if(direction == Directions[4]) {
                if(Edges[2].Walled) return false;
                if(other.Edges[1].Walled) return false;
                return true;
            }
            if(direction == Directions[5]) {
                if(Edges[3].Walled || Edges[1].Walled) return false;
                if(other.Edges[2].Walled || other.Edges[0].Walled) return false;
                return true;
            }
            if(direction == Directions[6]) {
                if(Edges[3].Walled) return false;
                if(other.Edges[0].Walled) return false;
                return true;
            }
            if(direction == Directions[7]) {
                if(Edges[3].Walled || Edges[2].Walled) return false;
                if(other.Edges[1].Walled || other.Edges[0].Walled) return false;
                return true;
            }
            return false;
        }

        public bool IsTraversable() {
            return !Taken;
        }

        public int GetMovementCost(Tile other) {
            var direction = GetDirection(other);
            if(direction == Directions[0] || direction == Directions[2] || direction == Directions[5] || direction == Directions[7]) {
                return 7;
            }
            if(direction == Directions[1] || direction == Directions[3] || direction == Directions[4] || direction == Directions[6]) {
                return 5;
            }
            return int.MaxValue;
        }
    }
}