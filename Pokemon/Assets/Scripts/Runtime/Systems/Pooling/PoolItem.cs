#region Packages

using UnityEngine;

#endregion

namespace Runtime.Systems.Pooling
{
    public class PoolItem : MonoBehaviour
    {
        #region Values

        internal PoolHolder pool;

        #endregion

        #region Build In States

        private void OnDisable() =>
            this.pool.Free(this.gameObject);

        #endregion
    }
}