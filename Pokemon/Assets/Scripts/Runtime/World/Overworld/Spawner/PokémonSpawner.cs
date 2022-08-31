#region Libraries

using Runtime.Pokémon;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Spawner
{
    public class PokémonSpawner : MonoBehaviour
    {
        [SerializeField]
        private PokémonSpawnList[] list;
        [SerializeField]
        private int maxActiveEntities;

        private int currentActiveEntities = 0;
        
        private void Update()
        {
            if (currentActiveEntities < maxActiveEntities)
            {
                Pokemon pokemon = SelectPokemonFromList();
                
                if(pokemon == null) return;
                
                SpawnOverWorldPokemon(pokemon);
            }
        }

        private Pokemon SelectPokemonFromList()
        {
            Pokemon result = null;
            
            return result;
        }

        private Vector3 RandomPositionFromArea()
        {
            
            
            return Vector3.zero;
        }
        
        private void SpawnOverWorldPokemon(Pokemon toSpawn)
        {
            GameObject obj = Instantiate(toSpawn.GetPokemonPrefab(),
                 RandomPositionFromArea(), Quaternion.identity);

        }
    }
}
