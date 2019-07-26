using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Settings {
    [CreateAssetMenu(menuName = "Settings/Game Settings", fileName = "Settings")]
    public class GameSettings : ScriptableObject {
        public GridSettings gridSettings;
        public BattleSettings battleSettings;
    }
}