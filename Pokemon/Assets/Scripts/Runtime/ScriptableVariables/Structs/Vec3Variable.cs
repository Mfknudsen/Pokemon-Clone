#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Structs
{
    public class Vec3Variable : StructVariable<Vector3>
    {
        public float x => value.x;
        public float y => value.y;
        public float z => value.z;
    }
}