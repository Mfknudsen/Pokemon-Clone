#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Objects
{
    [CreateAssetMenu(menuName = "Variables/Transform")]
    public sealed class TransformVariable : ScriptableVariable<Transform>
    {
    }
}
