#region Packages

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ScriptableVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        [ShowInInspector, TextArea] private string description;
        
        public abstract void OnBeforeSerialize();
        public abstract void OnAfterDeserialize();
    }
}
