using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Core.Grid {
    [System.Serializable]
    public class Edge {
        public Edge(Vector3 position) {
            Position = position;
        }

        public Vector3 Position { get; }

        public GameObject wall;

        [SerializeField] private bool walled;
        public bool Walled { get => walled; set => walled = value; }
    }
}