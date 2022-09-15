#region Packages

using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public class ManagerUpdater : MonoBehaviour
    {
        private readonly Manager toUpdate;

        public ManagerUpdater(Manager toUpdate)
        {
            this.toUpdate = toUpdate;

            if (toUpdate != null)
            {
                Debug.LogWarning("No Manager");
                Destroy(gameObject);
                return;
            }

            toUpdate.SetHolder(this);
        }

        private void Update() => toUpdate.UpdateManager();
        private void FixedUpdate() => toUpdate.FixedUpdateManager();
    }
}