#region Packages

using System.Collections;
using Runtime.Systems.Operation;
using UnityEngine;

#endregion

namespace Runtime.Weathers._Extra
{
    public class ThunderStormLighting : MonoBehaviour, IOperation
    {
        private bool done;

        public bool IsOperationDone()
        {
            return this.done;
        }

        public IEnumerator Operation()
        {
            this.done = false;

            this.done = true;

            yield break;
        }

        public void OperationEnd()
        {
            Destroy(gameObject);
        }
    }
}