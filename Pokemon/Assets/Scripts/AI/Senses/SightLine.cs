#region Packages

using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Senses
{
    #region Enums

    public enum SightType
    {
        Cone,
        Box
    }

    #endregion

    public class SightLine : MonoBehaviour
    {
        #region Values
        
        private List<GameObject> objectsInSight = new();

        #endregion

        #region Build In States

        private void Start()
        {
            GetComponent<BehaviourTreeOwner>().blackboard.SetVariableValue("sightLine", this);
        }

        #endregion

        #region Getters

        public GameObject[] GetInSight()
        {
            return objectsInSight.ToArray();
        }

        #endregion
    }
}