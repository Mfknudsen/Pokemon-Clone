using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trainer
{
    public class BattleMember : MonoBehaviour
    {
        [Header("Object Reference:")]
        public string name;

        [Header("Team:")]
        public Team team;

        [Header("Move Selection:")]
        public bool selectMove = false;
        public bool hasSelectedMove = false;

        public Pokemon SendOutFirstPokemon(Transform spawnPoint)
        {
            Pokemon pokemon = Instantiate(team.GetPokemonByIndex(0));

            return pokemon;
        }
    }
}
