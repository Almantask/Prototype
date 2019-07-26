using System.Collections;
using System.Collections.Generic;
using Project.Core.Abilities;
using UnityEngine;

namespace Project.Core.Characters {
    [CreateAssetMenu(menuName = "New Character", fileName = "New Character")]
    public class CharacterData : ScriptableObject {
        [Header("General")]
        public Sprite Portrait;

        //TODO Replace with a proper stat system.
        [Header("Stats")]
        [Range(0, 10)] public int Health = 4;
        [Range(0, 10)] public int Damage = 1;
        [Space]
        [Range(0, 100)] public int Accuracy = 75;
        [Range(0, 100)] public int Deflection = 20;
        [Range(0, 100)] public int Critical = 10;
        [Space]
        [Range(0, 100)] public int Protection = 0;
        [Range(0, 100)] public int Reflex = 20;
        [Range(0, 100)] public int Resistance = 20;
        [Space]
        [Range(0, 100)] public int Focus = 40;
        [Range(0, 100)] public int Resolve = 20;
        [Range(-5, 10)] public int Speed = 5;

        [Header("Turn")]
        [Range(0, 2)] public int ActionsPerTurn = 2;

        [Header("Battle")]
        public Controller controller;
        public Character prefab;
        public Vector3 spawnPosition;

        public List<Ability> abilities = new List<Ability>();
    }
}