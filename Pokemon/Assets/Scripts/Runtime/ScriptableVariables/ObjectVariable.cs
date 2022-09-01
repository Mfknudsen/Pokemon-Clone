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

        public T value
        {
            get => this.localValue;
            set
            {
                if (value != this.localValue)
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
