using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.Characters.Attributes {
    public enum ModifierType { Flat, PercentAdd, PercentMultiply }
    public class Modifier {
        public int Value { get; private set; }
        public ModifierType Type { get; private set; }
        public int Order { get; private set; }
        public object Source { get; private set; }

        public Modifier(int value, ModifierType type, int order, object source) {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public Modifier(int value, ModifierType type) : this(value, type, (int)type, null) { }
        public Modifier(int value, ModifierType type, int order) : this(value, type, order, null) { }
        public Modifier(int value, ModifierType type, object source) : this(value, type, (int)type, source) { }
    }
}