#region Packages

using System;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableVariables
{
    public class StructVariable<T> : ScriptableVariable where T : struct
    {
        public T defaultValue;

        [NonSerialized] private T localValue;

        private readonly UnityEvent<T> valueChangeEventWithParam = new();
        private readonly UnityEvent valueChangeEvent = new();

        public T value
        {
            get => this.localValue;
            set
            {
                if (!value.Equals(this.localValue))
                {
                    this.localValue = value;
                    valueChangeEventWithParam.Invoke(this.localValue);
                }
                else
                    this.localValue = value;
            }
        }
        
        public void AddListener(UnityAction<T> action) => this.valueChangeEventWithParam.AddListener(action);
        public void RemoveListener(UnityAction<T> action) => this.valueChangeEventWithParam.RemoveListener(action);

        public void AddListener(UnityAction action) => this.valueChangeEvent.AddListener(action);
        public void RemoveListener(UnityAction action) => this.valueChangeEvent.RemoveListener(action);

        public bool Equals(T checkAgainst)
        {
            return value.Equals(checkAgainst);
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