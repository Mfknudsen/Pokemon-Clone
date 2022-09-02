#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Structs
{
    [CreateAssetMenu(menuName = "Variables/Vector2")]
    public class Vec2Variable : StructVariable<Vector2>
    {
        public float x => value.x;
        public float y => value.y;
    }
}