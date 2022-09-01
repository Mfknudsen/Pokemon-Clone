#region Packages

using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableVariables
{
    public class StructVariable<TGeneric> : ScriptableVariable where TGeneric : struct
    {
        public TGeneric defaultValue;

        [NonSerialized, ShowInInspector, ReadOnly]
        private TGeneric localValue;

        private readonly UnityEvent<TGeneric> valueChangeEventWithParam = new();
        private readonly UnityEvent valueChangeEvent = new();

        public TGeneric value
        {
            get => this.localValue;
            set
            {
                if (!value.Equals(this.localValue))
                {
                    this.localValue = value;
                    valueChangeEventWithParam.Invoke(this.localValue);
                    valueChangeEvent.Invoke();
                }
                else
                    this.localValue = value;
            }
        }

        public void AddListener(UnityAction<TGeneric> action) =>
            this.valueChangeEventWithParam.AddListener(action);

        public void RemoveListener(UnityAction<TGeneric> action) =>
            this.valueChangeEventWithParam.RemoveListener(action);

        public void AddListener(UnityAction action) =>
            this.valueChangeEvent.AddListener(action);

        public void RemoveListener(UnityAction action) =>
            this.valueChangeEvent.RemoveListener(action);

        public bool Equals(TGeneric checkAgainst)
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