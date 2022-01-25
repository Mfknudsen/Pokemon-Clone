#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers._Extra
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