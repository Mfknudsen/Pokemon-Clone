#region Packages

using System;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class StructVariable<TGeneric> : ScriptableVariable<TGeneric> where TGeneric : struct
    {
        public TGeneric defaultValue;

        [NonSerialized, ShowInInspector, ReadOnly]
        private TGeneric localValue;

        public TGeneric value
        {
            get => this.localValue;
            set
            {
                if (value.Equals(this.localValue)) return;

                this.localValue = value;

                InvokeEvents(value);

                if (this.debugSetter)
                    Debug.Log(value, this);
            }
        }

        public bool Equals(TGeneric checkAgainst) => value.Equals(checkAgainst);

        public override void OnAfterDeserialize() => this.value = this.defaultValue;

        public override void OnBeforeSerialize()
        {
        }
    }
}