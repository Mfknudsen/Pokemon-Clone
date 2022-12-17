#region Packages

using System.Collections.Generic;

#endregion

namespace Runtime.ScriptableVariables.Structs.ListVariables
{
    public abstract class ListVariable<TGeneric> : ScriptableVariable<List<TGeneric>>
    {
        public void AddElement(TGeneric element, bool allowMultiplySameInstances = false)
        {
            this.value ??= new List<TGeneric>();

            if (allowMultiplySameInstances || !this.value.Contains(element))
                this.value.Add(element);
        }

        public void RemoveElement(TGeneric element)
        {
            this.value ??= new List<TGeneric>();
            
            this.value.Remove(element);
        }
    }
}