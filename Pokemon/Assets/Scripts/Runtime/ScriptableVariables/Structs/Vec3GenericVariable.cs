#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Structs
{
    public sealed class Vec3GenericVariable : ScriptableVariable<Vector3>
    {
        public float x => this.value.x;
        public float y => this.value.y;
        public float z => this.value.z;
    }
}