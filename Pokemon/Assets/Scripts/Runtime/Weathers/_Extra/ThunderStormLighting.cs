#region Packages

using System.Collections;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Weathers._Extra
{
    public class ThunderStormLighting : MonoBehaviour, IOperation
    {
        private bool done;

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            done = true;

            yield break;
        }

        public void End()
        {
            Destroy(gameObject);
        }
    }
}