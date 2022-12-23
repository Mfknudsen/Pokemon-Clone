#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.ScriptableEvents
{
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        [SerializeField, TextArea] private string description;

        private UnityEvent<T> eventValue;

        public void AddListener(UnityAction<T> action) => this.eventValue.AddListener(action);

        public void RemoveListener(UnityAction<T> action) => this.eventValue.RemoveListener(action);

        public void Trigger(T value) => this.eventValue.Invoke(value);
    }
}