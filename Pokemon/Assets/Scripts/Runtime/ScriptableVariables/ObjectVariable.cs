#region Package

using System;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#endregion

namespace Runtime.ScriptableVariables
{
    public class ObjectVariable<T> : ScriptableVariable where T : Object
    {
        public T defaultValue;

        [NonSerialized] private T localValue;

        private readonly UnityEvent<T> valueChangeEventWithValue = new();
        private readonly UnityEvent valueChangeEvent = new();

        public T value
        {
            get => this.localValue;
            set
            {
                if (value != this.localValue)
                {
                    this.localValue = value;
                    valueChangeEventWithValue.Invoke(this.localValue);
                }
                else
                    this.localValue = value;
            }
        }

        public void AddListener(UnityAction<T> action) => this.valueChangeEventWithValue.AddListener(action);
        public void RemoveListener(UnityAction<T> action) => this.valueChangeEventWithValue.RemoveListener(action);

        public void AddListener(UnityAction action) => this.valueChangeEvent.AddListener(action);
        public void RemoveListener(UnityAction action) => this.valueChangeEvent.RemoveListener(action);
        
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