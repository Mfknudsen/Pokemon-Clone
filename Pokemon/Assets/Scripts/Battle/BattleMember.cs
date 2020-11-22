using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trainer
{
    public class BattleMember : MonoBehaviour
    {
        [Header("Object Reference:")]
        [SerializeField] private string memberName = "";

        [Header("Team:")]
        [SerializeField] private Team team = null;

        [Header("Move Selection:")]
        public bool selectMove = false;
        public bool hasSelectedMove = false;

        #region Getters
        public string GetName()
        {
            return memberName;
        }

        public Team GetTeam()
        {
            return team;
        }
        #endregion

        public Pokemon SendOutFirstPokemon(Transform spawnPoint)
        {
            Pokemon pokemon = Instantiate(team.GetPokemonByIndex(0));

            return pokemon;
        }
    }
}
