#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace Trainer
{
    public class BattleMember : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        [SerializeField] private string memberName = "";

        [Header("Team:")]
        [SerializeField] private Team pokemonTeam = null;
        [SerializeField] private int teamNumber = 1; //0 is player and player teammates

        [Header("Move Selection:")]
        public bool selectMove = false;
        public bool hasSelectedMove = false;
        #endregion

        #region Getters
        public string GetName()
        {
            return memberName;
        }

        public Team GetTeam()
        {
            return pokemonTeam;
        }

        public int GetTeamNumber()
        {
            return teamNumber;
        }
        #endregion

        #region Setters
        public void SetTeamNumber(int set)
        {
            teamNumber = set;
        }
        #endregion
    }
}