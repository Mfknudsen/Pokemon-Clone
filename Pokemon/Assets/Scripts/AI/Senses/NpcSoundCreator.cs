using UnityEngine;

namespace Mfknudsen.AI.Senses
{
    public class NpcSoundCreator : MonoBehaviour
    {
        #region Values

        [SerializeField] private SoundType soundType;
        [SerializeField] private float distance;

        #endregion

        #region In

        public void Trigger()
        {
        }

        #endregion
    }
}