#region Packages

using System;
using System.Linq;
using Mfknudsen.AI.States.Required;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI.States
{
    [Serializable]
    [CreateAssetMenu(menuName = "AI/States/Patrol FromTo Points")]
    public class PatrolFromToPoints : AI_State
    {
        [SerializeField] private PatrolPoint[] patrolPoints;
        [SerializeField] private PatrolPoint toStartFrom;
        private int index;
        private bool returnNow;
        private NavMeshAgent agent;

        #region In

        // ReSharper disable once ParameterHidesMember
        public override void Start(AI_Controller controller)
        {
            base.Start(controller);

            agent = controller.agent;

#if UNITY_EDITOR
            if (toStartFrom == null)
                throw new Exception("Start Point Must Not Be Null");
            if (patrolPoints.Length < 2)
                throw new Exception("Patrol Points Must Contain At Least Two Points");
            if (!patrolPoints.Contains(toStartFrom))
                throw new Exception("Start Point Must Be In Point List");
#endif

            index = ArrayUtility.IndexOf(patrolPoints, toStartFrom);
            SetNewDestination(toStartFrom);
        }

        public override void Update()
        {
            if(!agent.isStopped) return;

            if (returnNow)
                index--;
            else
                index++;
            
            SetNewDestination(patrolPoints[index]);
        }

        public void OverridePointArray(PatrolPoint[] set)
        {
            if (set.Length < 2) return;

            patrolPoints = set;

            PatrolPoint closest = set[0];
            Vector3 agentPosition = agent.transform.position;
            float distance = Vector3.Distance(closest.GetPointTransform().position, agentPosition);

            for (int i = 1; i < set.Length; i++)
            {
                float checkDist = Vector3.Distance(set[i].GetPointTransform().position, agent.transform.position);
                if (distance < checkDist) return;

                distance = checkDist;
                closest = set[i];
            }

            patrolPoints = set;
            SetNewDestination(closest);
        }

        #endregion

        #region Internal

        private void SetNewDestination(PatrolPoint point)
        {
            agent.SetDestination(point.GetPointTransform().position);

            int i = ArrayUtility.IndexOf(patrolPoints, point);

            if (i == 0)
                returnNow = false;
            else if (i == patrolPoints.Length - 1)
                returnNow = true;
        }

        #endregion
    }
}