using System.Collections;
using System.Collections.Generic;
using Project.Settings;
using UnityEngine;

namespace Project.Core.Inputs {
    public class MouseIndicator : MonoBehaviour {
        private GridSettings _settings;
        private Renderer _renderer;

        private void OnEnable() {
            _settings = GameManager.Settings.gridSettings;
            _renderer = GetComponentInChildren<Renderer>();
        }

        public void SetIndicatorEnabled(bool enabled) {
            _renderer.enabled = enabled;
        }
    }
}