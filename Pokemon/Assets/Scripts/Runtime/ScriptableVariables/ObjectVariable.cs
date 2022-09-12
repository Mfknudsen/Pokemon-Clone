#region Packages

using System;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ObjectVariable<TGeneric> : ScriptableVariable<TGeneric> where TGeneric : class
    {
        [SerializeField] private TGeneric defaultValue;

        [NonSerialized, ShowInInspector, ReadOnly]
        private TGeneric localValue;

        public TGeneric value
        {
            get => this.localValue;
            set
            {
                if (value == this.localValue) return;

                this.localValue = value;

                InvokeEvents(value);

                if (this.debugSetter)
                    Debug.Log(value);
            }
        }

        protected override void OnEnable() => this.localValue = this.defaultValue;

        public bool Empty() => value == null;
    }
}