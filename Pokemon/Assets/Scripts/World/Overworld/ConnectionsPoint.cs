#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld
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