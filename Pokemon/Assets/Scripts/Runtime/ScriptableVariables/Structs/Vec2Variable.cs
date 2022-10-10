#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Structs
{
    [CreateAssetMenu(menuName = "Variables/Vector2")]
    public sealed class Vec2Variable : ScriptableVariable<Vector2>
    {
        public float x => this.value.x;
        public float y => this.value.y;
    }
}