#region Packages

using System.Collections;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems
{
    public class BattleInitializer : MonoBehaviour
    {
        #region Values

        [SerializeField, Min(4)] private int scanZones;

        [SerializeField, Min(1)] private int checksPerFrame;

        private bool found;

        #endregion

        #region Getters

        public bool HasFoundBattleZone => this.found;

        #endregion

        #region In

        public void FindSetupBattleZone()
        {
            this.found = false;

            this.StartCoroutine(this.FindZone());
        }

        #endregion

        #region Internal

        private IEnumerator FindZone()
        {
            int checkCount = 0;
            while (!this.found)
            {
                int allowed = 0;
                for (int i = 0; i < this.scanZones; i++)
                {
                    if (!this.IsZoneClearOfObstacles())
                        break;

                    if (this.IsZoneReachable())
                        break;

                    allowed++;
                }

                if (allowed == checkCount)
                {
                    this.found = true;
                    break;
                }

                checkCount++;

                if (checkCount < this.checksPerFrame)
                    continue;

                checkCount = 0;
                yield return null;
            }
        }

        private bool IsZoneReachable()
        {
            return true;
        }

        private bool IsZoneClearOfObstacles()
        {
            return true;
        }

        #endregion
    }
}