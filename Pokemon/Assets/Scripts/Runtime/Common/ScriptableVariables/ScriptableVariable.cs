#region Packages

using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#endregion

namespace Runtime.Common.ScriptableVariables
{
    [Serializable]
    public class ScriptableVariable<T> : ScriptableObject where T : Object
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

        public static ScriptableVariable<TGeneric> ConstructNew<TGeneric>(TGeneric startValue) where TGeneric : Object
        {
            ScriptableVariable<TGeneric> result = new()
            {
                value = startValue
            };

            return result;
        }
        
        public void AddListener(UnityAction<T> action)
        {
            changeEvent.AddListener(action);
        }

        public void RemoveListener(UnityAction<T> action)
        {
            changeEvent.RemoveListener(action);
        }
    }

    [Serializable]
    public class ScriptableStruct<T> : ScriptableObject where T : struct
    {
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

        [SerializeField] private T valueHolder;

        private readonly UnityEvent<T> changeEvent = new();

        public static ScriptableStruct<TGeneric> ConstructNew<TGeneric>(TGeneric startValue) where TGeneric : struct
        {
            ScriptableStruct<TGeneric> result = new()
            {
                value = startValue
            };

            return result;
        }

        public void AddListener(UnityAction<T> action)
        {
            changeEvent.AddListener(action);
        }

        public void RemoveListener(UnityAction<T> action)
        {
            changeEvent.RemoveListener(action);
        }
    }
}