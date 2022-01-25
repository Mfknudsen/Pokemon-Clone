#region Libraries

using System;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.Spawner
{
    [CreateAssetMenu(menuName = "Pokemon/Spawn List", fileName = "New Spawn List")]
    public class PokémonSpawnList : ScriptableObject
    {
        [SerializeField]
        private PokémonSpawnListEntity[] list;
    }

    [Serializable]
    public struct PokémonSpawnListEntity
    {
        public Pokemon pokemon;
        public float encounterRate;
        public int maxActiveEntities;
    }
}
