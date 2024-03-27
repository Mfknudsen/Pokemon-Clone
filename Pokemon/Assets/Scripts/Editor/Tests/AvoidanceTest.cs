#region Libraries

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Editor.Common;
using Runtime.AI.Navigation;
using Runtime.Core;
using Sirenix.OdinInspector;
using UnityEditor;
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

        private void OnDrawGizmos()
        {
            foreach (NavTriangle t in this.navMesh.Triangles)
            {
                Vector3 center = t.Center(this.navMesh);

                Vector3 a = this.navMesh.VertByIndex(t.GetA),
                    b = this.navMesh.VertByIndex(t.GetB),
                    c = this.navMesh.VertByIndex(t.GetC);
                Debug.DrawLine(a, b);
                Debug.DrawLine(b, c);
                Debug.DrawLine(c, a);

                Handles.Label(center, t.ID.ToString());
            }

            List<UnitAgent> allAgents = new List<UnitAgent>();
            allAgents.AddRange(this.team1);
            allAgents.AddRange(this.team2);

            foreach (UnitAgent u in allAgents)
            {
                //if (!u.name.Equals("Avoidance Test Unit_27"))
                //continue;

                bool inTriangle = false;
                foreach (NavTriangle t in this.navMesh.Triangles)
                    //if (t.ID != 151)
                    //continue;
                    if (MathC.PointWithinTriangle2D(u.transform.position.XZ(),
                            this.navMesh.SimpleVertices[t.GetA],
                            this.navMesh.SimpleVertices[t.GetB],
                            this.navMesh.SimpleVertices[t.GetC]))
                        inTriangle = true;

                if (inTriangle)
                    continue;

                Draw.DrawCircle(u.transform.position, .5f, Color.red, 16);
                Handles.Label(u.transform.position + Vector3.up, u.name);
            }
        }

        private IEnumerator Start()
        {
            UnitNavigation.SetNavMesh(this.navMesh);

            while (this.team1.Any(agent => !agent.IsOnNavMesh()) || this.team2.Any(agent => !agent.IsOnNavMesh()))
                yield return null;

            yield break;

            for (int i = 0; i < this.team1.Length; i++)
            {
                this.team1[i].MoveTo(this.team2[i].transform.position);
                this.team2[i].MoveTo(this.team1[i].transform.position);
            }
        }
    }
}