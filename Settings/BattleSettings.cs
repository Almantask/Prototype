using System.Collections.Generic;
using Project.Core.Characters;
using UnityEngine;

namespace Project.Settings {
    [CreateAssetMenu(menuName = "Settings/Battle Settings", fileName = "BattleSettings")]
    public class BattleSettings : ScriptableObject {
        public List<CharacterData> characters = new List<CharacterData>();
    }
}