#region Packages

using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Runtime.ValueAttributes
{
    [CreateAssetMenu(menuName = "Attribute")]
    public class ObjectAttribute<T> : ScriptableObject
    {
        private T Value
        {
            get => value;
            set
            {
                this.value = value;

                onChange.Invoke(this.value);
            }
        }

        private T value;

        public UnityEvent<T> onChange = new();

        public void Add(UnityAction<T> action)
        {
            onChange.AddListener(action);
        }

        public void Remove(UnityAction<T> action)
        {
            onChange.RemoveListener(action);
        }
    }
}