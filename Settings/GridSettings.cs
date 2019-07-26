using System.Collections;
using System.Collections.Generic;
using Project.Core.Grid;
using Project.Core.Inputs;
using UnityEngine;

namespace Project.Settings {
    [CreateAssetMenu(menuName = "Settings/Grid Settings", fileName = "GridSettings")]
    public class GridSettings : ScriptableObject {
        [Header("General")]
        public Tile prefab;
        public MouseIndicator mouseIndicator;
        public GameObject wall;

        [Header("Materials")]
        public Material tileMaterial;
        public Material tileFillMaterial;

        [Header("Outline")]
        public List<Material> outlineMaterials;
        public Dictionary<int, Material> OutlineMaterials = new Dictionary<int, Material>();

        [Header("Colors")]
        public Color activeFriendly = new Color(0, 0, 0, 255);
        public Color activeEnemy = new Color(0, 0, 0, 255);
        public Color friendly = new Color(0, 0, 0, 255);
        public Color friendlyHighlighted = new Color(0, 0, 0, 255);
        [Space]
        public Color enemy = new Color(0, 0, 0, 255);
        public Color enemyHighlighted = new Color(0, 0, 0, 255);
        [Space]
        public Color obstacle = new Color(0, 0, 0, 255);
        public Color trap = new Color(0, 0, 0, 255);
        [Space]
        public Color movementRange = new Color(255, 255, 255, 255);
        public Color abilityRange = new Color(255, 255, 255, 255);

        private void OnValidate() {
            for(int i = 0; i < outlineMaterials.Count; i++) {
                if(!OutlineMaterials.ContainsKey(i)) {
                    OutlineMaterials.Add(i, outlineMaterials[i]);
                }
            }
        }

        public Material GetOutlineMaterial(int i) {
            OutlineMaterials.TryGetValue(i, out Material material);
            return material;
        }
    }
}