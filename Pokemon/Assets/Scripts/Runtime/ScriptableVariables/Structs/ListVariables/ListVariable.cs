#region Packages

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Runtime.ScriptableVariables.Structs.ListVariables
{
    public abstract class ListVariable<TGeneric> : ScriptableVariable<List<TGeneric>>
    {
        #region Build In States

        protected override void OnEnable() =>
            this.localValue = this.defaultValue.ToList();

        #endregion

        #region In

        public void AddElement(TGeneric element, bool allowMultiplySameInstances = false)
        {
            this.value ??= new List<TGeneric>();

            if (!allowMultiplySameInstances && this.value.Contains(element)) return;

            this.value.Add(element);
            this.InvokeEvents(this.value);
        }

        public void RemoveElement(TGeneric element)
        {
            this.value ??= new List<TGeneric>();

            this.value.Remove(element);
            this.InvokeEvents(this.value);
        }

        #endregion
    }
}