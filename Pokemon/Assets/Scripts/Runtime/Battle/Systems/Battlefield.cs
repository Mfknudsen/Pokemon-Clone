using UnityEngine;

namespace Runtime.Battle.Systems
{
    public class Battlefield : MonoBehaviour
    {
        [SerializeField] private Transform allyField, enemyField;

        #region Getters

        public Transform GetAllyField()
        {
            return this.allyField;
        }

        public Transform GetEnemyField()
        {
            return this.enemyField;
        }

        #endregion
    }
}
