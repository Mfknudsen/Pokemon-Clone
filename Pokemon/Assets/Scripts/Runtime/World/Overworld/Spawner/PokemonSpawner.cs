#region Libraries

using Runtime.AI;
using Runtime.Pokémon;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Runtime.Core;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Spawner
{
    public sealed class PokemonSpawner : MonoBehaviour
    {
        #region Values

#if UNITY_EDITOR
        [SerializeField] private string spawnerName;
#endif


        [SerializeField, Required] private PokemonSpawnList spawnList;

        [SerializeField] private SpawnLocation[] spawnLocations;

        [SerializeField] private int maxActiveEntities;

        [SerializeField] private float spawnInterval;

        private readonly List<PokemonUnit> currentActiveEntities = new List<PokemonUnit>();

        private Timer checkTimer;

        private Transform aiParent;

        #endregion

        #region Build In States

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.spawnList != null)
                this.name = this.spawnerName + " Spawner - " + this.spawnList.name;
        }
#endif

        private void OnEnable()
        {
            Transform root = this.transform.root;
            for (int i = 0; root.childCount > 0; i++)
            {
                if (!root.GetChild(i).name.Equals("AI"))
                    continue;

                this.aiParent = root.GetChild(i);
                break;
            }

            this.checkTimer = new Timer(Random.Range(this.spawnInterval - 1f, this.spawnInterval + 1f), this.CheckState);
        }

        private void OnDisable() => this.checkTimer?.Stop();

        #endregion

        [Button]
        private void Test()
        {
            Transform root = this.transform.root;
            for (int i = 0; root.childCount > 0; i++)
            {
                if (!root.GetChild(i).name.Equals("AI"))
                    continue;

                this.aiParent = root.GetChild(i);
                break;
            }

            this.SpawnOverWorldPokemon();
        }

        private void CheckState()
        {
            if (this.currentActiveEntities.Count < this.maxActiveEntities)
                this.SpawnOverWorldPokemon();

            this.checkTimer = new Timer(Random.Range(this.spawnInterval - 1f, this.spawnInterval + 1f), this.CheckState);
        }


        private void SpawnOverWorldPokemon()
        {
            Pokemon toSpawn = this.spawnList.GetPokemonPrefab();

            SpawnLocation[] allowedFrom = this.spawnLocations.Where(l => l.Allowed(toSpawn)).ToArray();

            if (allowedFrom.Length == 0)
            {
                Debug.LogWarning("0");
                return;
            }

            SpawnTypeResult locationResult = allowedFrom[Random.Range(0, allowedFrom.Length)].GetSpawnResult;

            PokemonUnit unitBase = toSpawn.InstantiateUnitPrefab(PokemonState.Wild, locationResult.Position, locationResult.Rotation, this.aiParent);
            this.currentActiveEntities.Add(unitBase);
            unitBase.AddDisableEventListener(() => this.currentActiveEntities.Remove(unitBase));
        }
    }
}