using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.Characters.Attributes {
    public enum AttributeType { Health, Damage, Accuracy, Deflection, Critical, Protection, Reflex, Resistance, Focus, Resolve, Speed }
    [System.Serializable]
    public class Attribute {
        public AttributeType Type;
        public int InitialValue;

        public Attribute(AttributeType type, int value) {
            Type = type;
            InitialValue = value;
            Modifiers = new List<Modifier>();
        }

        public int Value { get { return CalculateFinalValue(); } }

        protected List<Modifier> Modifiers { get; private set; }

        public virtual void AddModifier(Modifier modifier) {
            Modifiers.Add(modifier);
            Modifiers.Sort(ByOrder);
        }

        public virtual bool RemoveModifier(Modifier modifier) {
            if(Modifiers.Remove(modifier)) {
                return true;
            }
            return false;
        }

        public virtual bool RemoveModifiersBySource(object source) {
            bool removed = false;
            for(int i = Modifiers.Count - 1; i >= 0; i--) {
                if(Modifiers[i].Source == source) {
                    Modifiers.RemoveAt(i);
                    removed = true;
                }
            }
            return removed;
        }

        protected int CalculateFinalValue() {
            int final = InitialValue;
            int percentSum = 0;
            for(int i = 0; i < Modifiers.Count; i++) {
                var modifier = Modifiers[i];
                switch(modifier.Type) {
                    case ModifierType.Flat:
                        final += modifier.Value;
                        break;
                    case ModifierType.PercentAdd:
                        percentSum += modifier.Value;
                        if(i + 1 >= Modifiers.Count || Modifiers[i + 1].Type != ModifierType.PercentAdd) {
                            final *= 1 + percentSum;
                            percentSum = 0;
                        }
                        break;
                    case ModifierType.PercentMultiply:
                        final *= 1 + modifier.Value;
                        break;
                }
            }
            return Mathf.FloorToInt(final);
        }

        protected int ByOrder(Modifier a, Modifier b) {
            if(a.Order < b.Order) {
                return -1;
            }
            if(a.Order > b.Order) {
                return 1;
            }
            return 0;
        }
    }
}