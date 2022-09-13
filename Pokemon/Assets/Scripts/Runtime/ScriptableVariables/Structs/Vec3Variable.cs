#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Structs
{
    public sealed class Vec3Variable : ScriptableVariable<Vector3>
    {
        public float x => value.x;
        public float y => value.y;
        public float z => value.z;
    }
}