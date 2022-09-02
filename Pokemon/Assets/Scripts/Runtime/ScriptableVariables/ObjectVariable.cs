#region Package

using System;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ObjectVariable<TGeneric> : ScriptableVariable where TGeneric : class
    {
        public TGeneric defaultValue;

        [NonSerialized] private TGeneric localValue;

        private readonly UnityEvent<TGeneric> valueChangeEventWithValue = new();
        private readonly UnityEvent valueChangeEvent = new();

        public TGeneric value
        {
            get => this.localValue;
            set
            {
                if (value != this.localValue)
                {
                    this.localValue = value;
                    valueChangeEventWithValue.Invoke(this.localValue);
                    valueChangeEvent.Invoke();
                }
                else
                    this.localValue = value;
            }
        }

        public void AddListener(UnityAction<TGeneric> action) =>
            this.valueChangeEventWithValue.AddListener(action);

        public void RemoveListener(UnityAction<TGeneric> action) =>
            this.valueChangeEventWithValue.RemoveListener(action);

        public void AddListener(UnityAction action) =>
            this.valueChangeEvent.AddListener(action);

        public void RemoveListener(UnityAction action) =>
            this.valueChangeEvent.RemoveListener(action);

        public bool Empty()
        {
            return value == null;
        }

        public override void OnAfterDeserialize()
        {
            this.value = this.defaultValue;
        }

        public override void OnBeforeSerialize()
        {
        }
    }
}