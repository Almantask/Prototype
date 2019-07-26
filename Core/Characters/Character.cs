using System;
using System.Collections;
using System.Collections.Generic;
using Project.Core.Abilities;
using Project.Core.Grid;
using Project.Core.VFX;
using UnityEngine;
using Project.Core.Characters.Attributes;
using Attribute = Project.Core.Characters.Attributes.Attribute;
using Project.Core.UI;

namespace Project.Core.Characters {
    public class Character : MonoBehaviour {

        public CharacterData Data { get; private set; }
        public Controller Controller { get; private set; }
        public int ActionPointsLeft { get; set; }
        public Tile Tile { get; private set; }

        public List<Vector3> Path { get; private set; }
        public List<Tile> MovementRange { get; set; }
        public List<Tile> AbilityRange { get; set; }

        public bool SearchingPath { get; set; }
        public bool HadMovedThisTurn { get; private set; }
        private int _movementSpeed = 5;

        public bool IsMoving { get; private set; }
        public bool IsRotating { get; private set; }
        public bool IsDead { get; private set; }
        public bool OnOverwatch { get; private set; }
        public bool ReachedDestination { get; private set; }

        [Header("Dependencies")]
        public ResourcesBar resourcesBar;

        public GameObject turnIndicator;
        public LineRenderer pathIndicator;
        private Animator _animator;

        private Quaternion _facingRotation;

        [Header("Stats")]
        public Dictionary<AttributeType, Attribute> Stats;

        public void Initialize(CharacterData data) {
            _animator = GetComponent<Animator>();
            _facingRotation = transform.rotation;
            Data = data;
            Controller = data.controller;
            name = Data.name;
            ActionPointsLeft = data.ActionsPerTurn;
            SetAttributes();
        }

        public void SetAttributes() {
            Stats = new Dictionary<AttributeType, Attribute>();
            SetAttribute(AttributeType.Health, Data.Health);
            SetAttribute(AttributeType.Damage, Data.Damage);
            SetAttribute(AttributeType.Accuracy, Data.Accuracy);
            SetAttribute(AttributeType.Deflection, Data.Deflection);
            SetAttribute(AttributeType.Critical, Data.Critical);
            SetAttribute(AttributeType.Protection, Data.Protection);
            SetAttribute(AttributeType.Reflex, Data.Reflex);
            SetAttribute(AttributeType.Resistance, Data.Resistance);
            SetAttribute(AttributeType.Focus, Data.Focus);
            SetAttribute(AttributeType.Resolve, Data.Resolve);
            SetAttribute(AttributeType.Speed, Data.Speed);
        }

        private void SetAttribute(AttributeType attributeType, int value) {
            if(Stats.ContainsKey(attributeType)) return;
            Stats.Add(attributeType, new Attribute(attributeType, value));
        }

        public Attribute GetAttribute(AttributeType type) {
            Stats.TryGetValue(type, out Attribute attribute);
            return attribute;
        }

        private void UpdateAttributes() {
            foreach(var key in Stats.Keys) {
                // Debug.Log(gameObject.name + "'s " + key.ToString() + " is " + Stats[key].Value + ".");
            }
        }

        public void SetTile(Tile tile) {
            if(!tile) {
                Tile.SetCharacter(null);
                return;
            }
            Tile = tile;
            tile.SetCharacter(this);
        }

        public void SetIndicatorActive(bool active) {
            turnIndicator.SetActive(active);
            turnIndicator.GetComponent<Renderer>().material.color = GameManager.Settings.gridSettings.activeFriendly;
        }

        public void UpdateBeforeRound() {
            UpdateAttributes();
            HadMovedThisTurn = false;
            ActionPointsLeft = Data.ActionsPerTurn;
        }

        public void ShowMovementRange(bool show) {
            foreach(var tile in MovementRange) {
                tile.Renderer.enabled = show;
            }
        }

        public void ShowAbilityRange(bool show) {
            foreach(var tile in AbilityRange) {
                tile.Renderer.enabled = show;
                if(tile.Character) {
                    tile.UpdateMaterial();
                }
            }
        }

        public void SetPath(List<Vector3> path) {
            Path = path;
            if(Path.Count < 0) return;
            pathIndicator.positionCount = Path.Count;
            for(int i = 0; i < Path.Count; i++) {
                pathIndicator.SetPosition(i, Path[i]);
            }
        }

        public void ShowPath(bool show) {
            pathIndicator.gameObject.SetActive(show);
        }

        public IEnumerator Move() {
            SearchingPath = false;
            IsMoving = true;
            _animator.SetBool("Moving", IsMoving);
            foreach(var point in Path) {
                while(transform.position != point) {
                    transform.LookAt(point);
                    transform.position = Vector3.MoveTowards(transform.position, point, Time.deltaTime * _movementSpeed);
                    yield return null;
                }
            }
            ReachedDestination = true;
            HadMovedThisTurn = true;
            SetTile(GameManager.GridManager.GetTile(transform.position));
            IsMoving = false;
            _animator.SetBool("Moving", IsMoving);
            this.PostNotification(Events.MoveCompleted, new Events.OnMoveCompleted(this));
        }

        public IEnumerator Rotate(Transform targetTransform) {
            IsRotating = true;
            var direction = targetTransform.position - transform.position;
            var dot = Vector3.Dot(direction, transform.right);
            if(dot > 0) {
                _animator.SetBool("RotatingRight", IsRotating);
            } else {
                _animator.SetBool("RotatingLeft", IsRotating);
            }
            var rotation = Quaternion.LookRotation(direction);
            while(transform.rotation != rotation) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * _movementSpeed * 40);
                yield return null;
            }
            IsRotating = false;
            _animator.SetBool("RotatingRight", IsRotating);
            _animator.SetBool("RotatingLeft", IsRotating);
        }

        public IEnumerator Attack(Ability ability, ITargetable target) {
            yield return StartCoroutine(Rotate(((Tile)target).transform));
            _animator.SetTrigger("Attack1");
            yield return new WaitForSeconds(0.1f);
            var defender = ((Tile)target).Character;
            defender.GetAttribute(AttributeType.Health).AddModifier(new Modifier(-ability.damage, ModifierType.Flat));
            this.PostNotification(Events.DamageTaken, new Events.OnDamageTaken(defender, -ability.damage));
            CheckHealth(defender);
            yield return new WaitForSeconds(0.1f);
            ability.OnCompleted();
        }

        public IEnumerator CastSpell(Ability ability, ITargetable target) {
            yield return StartCoroutine(Rotate(((Tile)target).transform));
            _animator.SetTrigger("Cast");
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(SendProjectile(ability, (Tile)target));
        }

        public void Hit() {

        }

        public void Shoot() {
            //StartCoroutine(SendProjectile(GameManager.InputManager.HoverTile));
        }

        public void FootL() {

        }

        public void FootR() {

        }

        private IEnumerator SendProjectile(Ability ability, Tile target) {
            var skill = (SpellAttack)ability;
            Projectile obj = Instantiate(skill.projectile, transform.position, Quaternion.identity);
            obj.transform.localScale = Vector3.one * 0.25f;
            while(obj.transform.position != target.Position) {
                obj.transform.LookAt(target.Position);
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, target.Position, Time.deltaTime * 10);
                yield return null;
            }
            while(!obj.Hit) {
                yield return null;
            }
            var defender = target.Character;
            defender.GetAttribute(AttributeType.Health).AddModifier(new Modifier(-ability.damage, ModifierType.Flat));
            this.PostNotification(Events.DamageTaken, new Events.OnDamageTaken(defender, -ability.damage));
            yield return new WaitForSeconds(0.1f);
            CheckHealth(defender);
            ability.OnCompleted();
        }

        private void CheckHealth(Character character) {
            var health = character.GetAttribute(AttributeType.Health).Value;
            if(health <= 0) {
                character.StartCoroutine(Die(character));
            }
        }

        private IEnumerator Die(Character character) {
            character._animator.SetTrigger("Die");
            this.PostNotification(Events.CharacterDeath, new Events.OnCharacterDeath(character));
            yield return new WaitForSeconds(3f);
            Destroy(character.gameObject);
        }
    }
}