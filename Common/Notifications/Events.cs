using System;
using System.Collections.Generic;
using Project.Core.Abilities;
using Project.Core.Characters;
using Project.Core.Grid;
using Project.Core.VFX;
using UnityEngine;

public static class Events {
    #region Game Events
    public const string GameStarted = "GameStarted";
    #endregion

    #region Input Events
    public const string TileLeftClicked = "TileLeftClicked";
    public const string TileRightClicked = "TileRightClicked";
    public const string AbilityButtonPressed = "AbilityButtonPressed";
    public const string EscapeButtonPressed = "EscapeButtonPressed";
    public const string AbilityShortcutPressed = "AbilityShortcutPressed";
    public const string EndTurnButtonPressed = "EndTurnButtonPressed";
    //Event Arguments
    public class OnTileLeftClicked : EventArgs {
        public Tile Tile;

        public OnTileLeftClicked(Tile tile) {
            Tile = tile;
        }
    }
    public class OnTileRightClicked : EventArgs {
        public Tile Tile;

        public OnTileRightClicked(Tile tile) {
            Tile = tile;
        }
    }
    public class OnAbilityButtonPressed : EventArgs {
        public Ability Ability;

        public OnAbilityButtonPressed(Ability ability) {
            Ability = ability;
        }
    }

    public class OnAbilityShortcutPressed : EventArgs {
        public int Key;

        public OnAbilityShortcutPressed(int key) {
            Key = key;
        }
    }
    #endregion

    #region Battle Events
    public const string BattleLoaded = "BattleLoaded";
    public const string CharactersSpawned = "CharactersSpawned";

    public const string BattleStarted = "BattleStarted";
    public const string BattleEnded = "BattleEnded";

    public const string RoundStarted = "RoundStarted";
    public const string RoundEnded = "RoundEnded";

    public const string CharacterTurnStarted = "CharacterTurnStarted";
    public const string CharacterTurnEnded = "CharacterTurnEnded";

    public const string MoveCompleted = "MoveCompleted";

    public const string AbilityExecutionStarted = "AbilityExecutionStarted";
    public const string AbilityTargetSelected = "AbilityTargetSelected";
    public const string AbilityExecutionCancelled = "AbilityExecutionCancelled";
    public const string AbilityExecutionCompleted = "AbilityExecutionCompleted";

    public const string ProjectileCollision = "ProjectileCollision";
    public const string DamageTaken = "DamageTaken";

    public const string CharacterDeath = "CharacterDeath";

    //Event Arguments
    public class OnCharactersSpawned : EventArgs {
        public List<Character> Characters = new List<Character>();

        public OnCharactersSpawned(List<Character> character) {
            Characters = character;
        }
    }

    public class OnCharacterTurnStarted : EventArgs {
        public Character Character;

        public OnCharacterTurnStarted(Character character) {
            Character = character;
        }
    }

    public class OnCharacterTurnEnded : EventArgs {
        public Character Character;

        public OnCharacterTurnEnded(Character character) {
            Character = character;
        }
    }

    public class OnMoveCompleted : EventArgs {
        public Character Character;

        public OnMoveCompleted(Character character) {
            Character = character;
        }
    }

    public class OnAbilityExecutionStarted : EventArgs {
        public Character Character;
        public Ability Ability;

        public OnAbilityExecutionStarted(Character character, Ability ability) {
            Character = character;
            Ability = ability;
        }
    }

    public class OnAbilityTargetSelected : EventArgs {
        public Ability Ability;
        public Tile Tile;

        public OnAbilityTargetSelected(Ability ability, Tile tile) {
            Ability = ability;
            Tile = tile;
        }
    }

    public class OnAbilityExecutionCancelled : EventArgs {
        public Ability Ability;

        public OnAbilityExecutionCancelled(Ability ability) {
            Ability = ability;
        }
    }

    public class OnAbilityExecutionCompleted : EventArgs {
        public Character Character;
        public Ability Ability;

        public OnAbilityExecutionCompleted(Character character, Ability ability) {
            Character = character;
            Ability = ability;
        }
    }

    public class OnProjectileCollision : EventArgs {
        public Projectile Projectile;
        public Collider Collider;

        public OnProjectileCollision(Projectile projectile, Collider collider) {
            Projectile = projectile;
            Collider = collider;
        }
    }

    public class OnDamageTaken : EventArgs {
        public Character Character;
        public int Damage;

        public OnDamageTaken(Character character, int damage) {
            Character = character;
            Damage = damage;
        }
    }

    public class OnCharacterDeath : EventArgs {
        public Character Character;

        public OnCharacterDeath(Character character) {
            Character = character;
        }
    }
    #endregion
}