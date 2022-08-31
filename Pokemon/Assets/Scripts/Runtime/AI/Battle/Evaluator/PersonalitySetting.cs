#region Packages

using System;
using UnityEngine;

#endregion

namespace Runtime.AI.Battle.Evaluator
{
    [Serializable]
    public class PersonalitySetting
    {
        public float aggressionLevel = 1;
        [SerializeField] private float aggressionIncrease;

        public float setupLevel = 1;
        [SerializeField] private float setupIncrease;

        public float survivalLevel = 1;
        [SerializeField] private float survivalIncrease;

        public void Tick()
        {
            aggressionLevel += aggressionIncrease;
            setupLevel += setupIncrease;
            survivalLevel += survivalIncrease;
        }
    }
}