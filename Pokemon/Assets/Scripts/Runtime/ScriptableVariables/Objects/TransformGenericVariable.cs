#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Objects
{
    [CreateAssetMenu(menuName = "Variables/Transform")]
    public sealed class TransformGenericVariable : ScriptableVariable<Transform>
    {
        public Vector3 Position => this.value.position;
        public Vector3 Forward => this.value.forward;
        public Vector3 Up => this.value.up;
        public Vector3 Right => this.value.right;
        public Quaternion Rotation => this.value.rotation;
        public Vector3 Euler => this.value.rotation.eulerAngles;
    }
}