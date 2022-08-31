#region Packages

using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#endregion

namespace Runtime.Common.ScriptableVariables
{
    [Serializable]
    public abstract class ScriptableVariable<T> : ScriptableObject, ISerializationCallbackReceiver where T : Object
    {
        public T value
        {
            get => valueHolder;
            set
            {
                if (value != valueHolder)
                {
                    valueHolder = value;
                    changeEvent.Invoke(valueHolder);
                }
                else
                    valueHolder = value;
            }
        }

        [SerializeField] private T valueHolder;

        private readonly UnityEvent<T> changeEvent = new();

        public void AddListener(UnityAction<T> action)
        {
            changeEvent.AddListener(action);
        }

        public void RemoveListener(UnityAction<T> action)
        {
            changeEvent.RemoveListener(action);
        }

        public abstract void OnBeforeSerialize();
        public abstract void OnAfterDeserialize();
    }
}