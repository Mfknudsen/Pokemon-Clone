#region Packages

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ScriptableVariable<TGeneric> : ScriptableObject
    {
        #region Packages

        // ReSharper disable once NotAccessedField.Local
        [SerializeField, TextArea] private string description;

        [SerializeField] protected bool debugSetter;

        [ShowInInspector, ReadOnly] private UnityEvent<TGeneric> valueChangeEventWithValue;
        [ShowInInspector, ReadOnly] private UnityEvent valueChangeEvent;

        #endregion

        #region Build In States

        protected abstract void OnEnable();

        #endregion

        #region In

        public void AddListener(UnityAction<TGeneric> action)
        {
            this.valueChangeEventWithValue ??= new UnityEvent<TGeneric>();

            this.valueChangeEventWithValue.AddListener(action);
        }

        public void RemoveListener(UnityAction<TGeneric> action)
        {
            this.valueChangeEventWithValue ??= new UnityEvent<TGeneric>();

            this.valueChangeEventWithValue.RemoveListener(action);
        }

        public void AddListener(UnityAction action)
        {
            this.valueChangeEvent ??= new UnityEvent();

            this.valueChangeEvent.AddListener(action);
        }

        public void RemoveListener(UnityAction action)
        {
            this.valueChangeEvent ??= new UnityEvent();

            this.valueChangeEvent.RemoveListener(action);
        }

        #endregion

        #region Internal

        protected void InvokeEvents(TGeneric value)
        {
            this.valueChangeEvent?.Invoke();

            this.valueChangeEventWithValue?.Invoke(value);
        }

        #endregion
    }
}