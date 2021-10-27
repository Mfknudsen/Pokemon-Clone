#region Packages

using System;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    [Serializable]
    public class PersonalitySetting
    {
        public float aggressionLevel = 1;
        [SerializeField] private float aggressionIncrease = 0;

        public float setupLevel = 1;
        [SerializeField] private float setupIncrease = 0;

        public float survivalLevel = 1;
        [SerializeField] private float survivalIncrease = 0;

        public void Tick()
        {
            aggressionLevel += aggressionIncrease;
            setupLevel += setupIncrease;
            survivalLevel += survivalIncrease;
        }
    }
}