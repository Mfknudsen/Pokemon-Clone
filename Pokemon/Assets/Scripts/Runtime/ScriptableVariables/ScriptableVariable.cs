#region Packages

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ScriptableVariable<TGeneric> : ScriptableObject
    {
        #region Values

        [SerializeField, TextArea] private string description;

        [SerializeField] private TGeneric defaultValue;

        [NonSerialized, ShowInInspector, ReadOnly]
        private TGeneric localValue;

        public TGeneric value
        {
            get => this.localValue;
            set
            {
                if (value.Equals(this.localValue) || !this.ValueAcceptable(value)) return;

                this.localValue = value;

                this.InvokeEvents(value);

                if (this.debugSetter)
                    Debug.Log(value, this);
            }
        }

        [SerializeField] protected bool debugSetter;

        private UnityEvent<TGeneric, TGeneric> valueChangeEventWithHistory;
        private UnityEvent<TGeneric> valueChangeEventWithValue;
        private UnityEvent valueChangeEvent;

        #endregion

        #region Build In States

        protected virtual void OnEnable() => this.localValue = this.defaultValue;

        #endregion

        #region Getters

        public string getDescription => this.description;

        #endregion

        #region In

        public void AddListener(UnityAction<TGeneric, TGeneric> action)
        {
            this.valueChangeEventWithHistory ??= new UnityEvent<TGeneric, TGeneric>();

            this.valueChangeEventWithHistory.AddListener(action);
        }

        public void RemoveListener(UnityAction<TGeneric, TGeneric> action)
        {
            this.valueChangeEventWithHistory?.RemoveListener(action);
        }

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
            if (this.valueChangeEvent == null) return;

            this.valueChangeEvent.RemoveListener(action);

            if (this.valueChangeEvent.GetPersistentEventCount() == 0)
                this.valueChangeEvent = null;
        }

        #endregion

        #region Internal

        private void InvokeEvents(TGeneric toCheck)
        {
            this.valueChangeEvent?.Invoke();

            this.valueChangeEventWithValue?.Invoke(toCheck);
        }

        protected virtual bool ValueAcceptable(TGeneric item) => true;

        #endregion
    }
}