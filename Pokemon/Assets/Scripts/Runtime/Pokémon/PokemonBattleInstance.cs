using System.Collections.Generic;
using Runtime.Battle.Actions;
using UnityEngine;

namespace Runtime.Pokémon
{
    public struct PokemonBattleInstance 
    {
         public int spotIndex;
         public GameObject prefab;
         public GameObject spawnedObject;
         public Animator anim;
         public bool gettingSwitched, inBattle;
         public BattleAction battleAction;
         public bool gettingRevived;
         public int[] multipliers;
         public List<Ability> instantiatedAbilities;
        
        public PokemonBattleInstance(Pokemon pokemon)
        {
            multipliers = new int[6];
            spotIndex = 0;
            prefab = null;
            spawnedObject = null;
            anim = null;
            gettingSwitched = false;
            inBattle = false;
            battleAction = null;
            gettingRevived = false;
            instantiatedAbilities = null;
        }
    }
}
