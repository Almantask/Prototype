using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.Characters.Attributes {
    public enum EmotionType { Rage, Anger, Fear, Terror, Vigilance, Anticipation, Distraction, Surprise, Ecstasy, Joy, Sadness, Grief }
    [System.Serializable]
    public class Emotion {
        public EmotionType Type;
        public int Value;
        public Dictionary<Attribute, Modifier> Modifiers;

        public Emotion(EmotionType type, int value) {
            Type = type;
            Value = value;
            Modifiers = new Dictionary<Attribute, Modifier>();
        }

        public Modifier CreateModifier(int value, ModifierType type) {
            var modifier = new Modifier(value, type, this);
            return modifier;
        }

        public Modifier GetModifier(Attribute attribute) {
            Modifiers.TryGetValue(attribute, out Modifier modifier);
            return modifier;
        }

        public void AddModifier(Attribute attribute, Modifier modifier) {
            if(Modifiers.ContainsKey(attribute)) return;
            Modifiers.Add(attribute, modifier);
        }

        public bool RemoveModifier(Attribute attribute) {
            return Modifiers.Remove(attribute);
        }

        public void Modify() {
            foreach(var attribute in Modifiers.Keys) {
                var modifier = GetModifier(attribute);
                if(modifier == null) continue;
                attribute.AddModifier(modifier);
            }
        }
    }
}