#region Packages

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.World
{
    public class Waypoints : MonoBehaviour
    {
        #region Values

        private List<Transform> points = new();

        #endregion

        #region Build In States

        private void OnDrawGizmos()
        {
            if (transform.childCount < 2)
                return;

            for (int i = 0; i < transform.childCount - 1; i++)
                Debug.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position, Color.red);
        }

        private void Start()
        {
            foreach (Transform t in transform) this.points.Add(t);
        }

        #endregion

        #region Getters

        public Transform[] GetPoints()
        {
            return this.points.ToArray();
        }

        #endregion
    }
}