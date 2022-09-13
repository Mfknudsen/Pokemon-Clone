#region Packages

using System;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class StructVariable<TGeneric> : ScriptableVariable<TGeneric> where TGeneric : struct
    {
        [SerializeField] private TGeneric defaultValue;

        [NonSerialized, ShowInInspector, ReadOnly]
        private TGeneric localValue;

        public TGeneric value
        {
            get => this.localValue;
            set
            {
                if (value.Equals(this.localValue) || !ValueAcceptable(value)) return;

                this.localValue = value;

                InvokeEvents(value);

                if (this.debugSetter)
                    Debug.Log(value, this);
            }
        }

        protected override void OnEnable() => this.localValue = this.defaultValue;

        public bool Equals(TGeneric check) => value.Equals(check);

        protected override bool ValueAcceptable(TGeneric value) => true;
    }
}