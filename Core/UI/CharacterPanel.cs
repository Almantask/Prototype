using Project.Core.Characters.Attributes;
using TMPro;
using UnityEngine;

namespace Project.Core.UI {
    public class CharacterPanel : MonoBehaviour {
        [Header("General")]
        public TextMeshProUGUI CharacterName;

        [Header("Attributes")]
        public TextMeshProUGUI ACC;
        public TextMeshProUGUI DEF;
        public TextMeshProUGUI LCK;
        public TextMeshProUGUI ARM;
        public TextMeshProUGUI REF;
        public TextMeshProUGUI RST;
        public TextMeshProUGUI FCS;
        public TextMeshProUGUI RES;
        public TextMeshProUGUI SPD;

        private void OnEnable() {
            this.AddObserver(SetCharacterPanel, Events.CharacterTurnStarted);
        }

        private void OnDisable() {
            this.RemoveObserver(SetCharacterPanel, Events.CharacterTurnStarted);
        }

        private void SetCharacterPanel(object sender, object args) {
            var character = ((Events.OnCharacterTurnStarted)args).Character;
            CharacterName.SetText(character.name);
            ACC.text = character.GetAttribute(AttributeType.Accuracy).Value.ToString();
            DEF.text = character.GetAttribute(AttributeType.Deflection).Value.ToString();
            LCK.text = character.GetAttribute(AttributeType.Critical).Value.ToString();
            ARM.text = character.GetAttribute(AttributeType.Protection).Value.ToString();
            REF.text = character.GetAttribute(AttributeType.Reflex).Value.ToString();
            RST.text = character.GetAttribute(AttributeType.Resistance).Value.ToString();
            FCS.text = character.GetAttribute(AttributeType.Focus).Value.ToString();
            RES.text = character.GetAttribute(AttributeType.Resolve).Value.ToString();
            SPD.text = character.GetAttribute(AttributeType.Speed).Value.ToString();
        }
    }
}