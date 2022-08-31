using System;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Common.ScriptableVariables
{
    [Serializable]
    public abstract class ScriptableStruct<T> : ScriptableObject, ISerializationCallbackReceiver where T : struct
    {
        [SerializeField] private T initialValue;

        public T value
        {
            get => valueHolder;
            set
            {
                if (!value.Equals(valueHolder))
                {
                    valueHolder = value;
                    changeEvent.Invoke(valueHolder);
                }
                else
                    valueHolder = value;
            }
        }

        [NonSerialized] private T valueHolder;

        private readonly UnityEvent<T> changeEvent = new();

        public void AddListener(UnityAction<T> action)
        {
            changeEvent.AddListener(action);
        }

        public void RemoveListener(UnityAction<T> action)
        {
            changeEvent.RemoveListener(action);
        }

        public virtual void OnAfterDeserialize()
        {
            this.value = this.initialValue;
        }

        public virtual void OnBeforeSerialize()
        {
        }
    }
}