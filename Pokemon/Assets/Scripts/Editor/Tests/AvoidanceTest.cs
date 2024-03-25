#region Libraries

using System.Collections;
using System.Linq;
using Runtime.AI.Navigation;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Editor.Tests
{
    public sealed class AvoidanceTest : MonoBehaviour
    {
        #region Values

        [SerializeField] private UnitAgent[] team1, team2;

        [SerializeField] [Required] private NavigationMesh navMesh;

        #endregion

        private IEnumerator Start()
        {
            UnitNavigation.SetNavMesh(this.navMesh);

            while (this.team1.Any(agent => !agent.IsOnNavMesh()) || this.team2.Any(agent => !agent.IsOnNavMesh()))
                yield return null;

            for (int i = 0; i < this.team1.Length; i++)
            {
                this.team1[i].MoveTo(this.team2[i].transform.position);
                this.team2[i].MoveTo(this.team1[i].transform.position);
            }
        }
    }
}