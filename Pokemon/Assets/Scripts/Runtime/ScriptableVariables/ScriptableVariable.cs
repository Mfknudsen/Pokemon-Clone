#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables
{
    public abstract class ScriptableVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        // ReSharper disable once NotAccessedField.Local
        [SerializeField, TextArea] private string description;

        public abstract void OnBeforeSerialize();
        public abstract void OnAfterDeserialize();
    }
}