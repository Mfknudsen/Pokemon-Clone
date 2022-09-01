#region Packages

using System;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableVariables
{
    public class StructVariable<T> : ScriptableObject, ISerializationCallbackReceiver where T : struct
    {
        public T defaultValue;

        public T value
        {
            get => this.localValue;
            set
            {
                if (!value.Equals(this.localValue))
                {
                    this.localValue = value;
                    valueChangeEvent.Invoke(this.localValue);
                }
                else
                    this.localValue = value;
            }
        }
        
        [NonSerialized]
        private T localValue;

        private readonly UnityEvent<T> valueChangeEvent = new();

        public void AddListener(UnityAction<T> action) => this.valueChangeEvent.AddListener(action);
        public void RemoveListener(UnityAction<T> action) => this.valueChangeEvent.RemoveListener(action);

        public void OnAfterDeserialize()
        {
            this.value = this.defaultValue;
        }

        public void OnBeforeSerialize()
        {
        }
    }
}