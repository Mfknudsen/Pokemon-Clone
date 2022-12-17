#region Packages

using System.Collections.Generic;
using Runtime.AI;
using Runtime.Common;
using Runtime.Pokémon;
using UnityEngine;

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

        private void OnEnable() => this.CheckState();

        private void OnDisable() => this.checkTimer?.Stop();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, .25f);
            Gizmos.DrawSphere(this.transform.position, this.radius);
        }

        #endregion

        private void CheckState()
        {
            if (this.currentActiveEntities.Count < this.maxActiveEntities) this.SpawnOverWorldPokemon();

            this.checkTimer = new Timer(Random.Range(this.spawnInterval - 1f, this.spawnInterval + 1f), this.CheckState);
        }

        private Pokemon SelectPokemonFromList() => this.list[Random.Range(0, this.list.Length)];

        private Vector3 RandomPositionFromArea() =>
            this.transform.position + new Vector3(
            Random.Range(-this.radius, this.radius),
            0,
            Random.Range(-this.radius, this.radius));

        private void SpawnOverWorldPokemon()
        {
            Pokemon toSpawn = this.SelectPokemonFromList();

            if (toSpawn.GetVisuelPrefab() == null)
            {
                Debug.LogWarning("Pokemon need visual prefab to be spawned");
                return;
            }
            
            GameObject obj = Instantiate(
                toSpawn.GetVisuelPrefab(), this.RandomPositionFromArea(),
                Quaternion.LookRotation(Vector3.up * Random.Range(0, 360)));

            UnitBase unitBase = obj.GetFirstComponentTowardsRoot<UnitBase>();
            this.currentActiveEntities.Add(unitBase);
            unitBase.AddDisableEventListener(() => this.currentActiveEntities.Remove(unitBase));
        }
    }
}