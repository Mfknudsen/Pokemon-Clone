#region Packages

using UnityEngine;

#endregion

namespace Runtime.World.Overworld
{
    public class ConnectionsPoint : MonoBehaviour
    {
        [SerializeField] private string connectTo;

        private void OnValidate()
        {
            name = "ConnectionPoint - " + connectTo;
        }
    }
}