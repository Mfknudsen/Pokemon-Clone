#region Packages

using System;
using UnityEngine;

#endregion

namespace Runtime.Pokémon
{
    [Serializable]
    public struct Stats
    {
        [SerializeField]
        private int hp,
            attack,
            defence,
            spAtk,
            spDef,
            speed,
            accuracy,
            evasion,
            critical;

        public int this[Stat stat]
        {
            get
            {
                return stat switch
                {
                    Stat.HP => hp,
                    Stat.Attack => attack,
                    Stat.Defence => defence,
                    Stat.SpAtk => spAtk,
                    Stat.SpDef => spDef,
                    Stat.Speed => speed,
                    Stat.Accuracy => accuracy,
                    Stat.Evasion => evasion,
                    Stat.Critical => critical,
                    _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
                };
            }
        }
    }
}