#region Packages

using UnityEngine;

#endregion

namespace Runtime.ScriptableVariables.Structs
{
    [CreateAssetMenu(menuName = "Variables/Bool")]
    public sealed class BoolVariable : ScriptableVariable<bool>
    {
    }
}