#region Packages

using System.Collections.Generic;
using Runtime.AI;
using Runtime.Common;
using Runtime.Pokémon;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

namespace Runtime.World.Overworld.Spawner
{
    public class PokemonSpawner : MonoBehaviour
    {
        [SerializeField] private Pokemon[] list;
        [SerializeField] private int maxActiveEntities;
        [SerializeField] private float spawnInterval;
        [SerializeField] private float radius;

        private readonly List<UnitBase> currentActiveEntities = new();

        private Timer checkTimer;

        #region Build In States

        private void OnEnable() => CheckState();

        private void OnDisable() => checkTimer?.Stop();

        #endregion

        private void CheckState()
        {
            if (this.currentActiveEntities.Count < this.maxActiveEntities)
                SpawnOverWorldPokemon();

            checkTimer = new Timer(Random.Range(this.spawnInterval - 1f, this.spawnInterval + 1f), CheckState);
        }

        private Pokemon SelectPokemonFromList() => this.list[Random.Range(0, this.list.Length)];

        private Vector3 RandomPositionFromArea() => transform.position + new Vector3(
            Random.Range(-this.radius, this.radius),
            0,
            Random.Range(-this.radius, this.radius));

        private void SpawnOverWorldPokemon()
        {
            Pokemon toSpawn = SelectPokemonFromList();

            GameObject obj = Instantiate(
                toSpawn.GetPokemonPrefab(),
                RandomPositionFromArea(),
                Quaternion.LookRotation(Vector3.up * Random.Range(0, 360)));

            UnitBase unitBase = obj.GetFirstComponentTowardsRoot<UnitBase>();
            currentActiveEntities.Add(unitBase);
            unitBase.AddDisableEventListener(() => this.currentActiveEntities.Remove(unitBase));
        }
    }
}