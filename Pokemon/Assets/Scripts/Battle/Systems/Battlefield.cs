using UnityEngine;

namespace Mfknudsen.Battle.Systems
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField] private Transform allyField, enemyField;

        #region Getters

        public Transform GetAllyField()
        {
            return allyField;
        }

        public Transform GetEnemyField()
        {
            return enemyField;
        }

        #endregion
    }
}
