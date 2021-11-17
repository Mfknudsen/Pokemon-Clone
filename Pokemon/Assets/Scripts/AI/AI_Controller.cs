#region Packages

using System;
using Mfknudsen.AI.States;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI
{
    [RequireComponent(typeof(AI_Sight))]
    public class AI_Controller : SerializedMonoBehaviour
    {
        #region Values

        private AI_Sight sight;

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public NavMeshAgent agent { get; }

        [SerializeField] private AI_State state;

        #endregion

        #region Build in States

        private void Start()
        {
#if UNITY_EDITOR
            if (state == null)
                throw new Exception("AI Controller Require Working State");
#endif

            sight = GetComponent<AI_Sight>();

            state.StartState(this);
        }

        private void Update()
        {
            state.Update();
        }

        #endregion
    }
}