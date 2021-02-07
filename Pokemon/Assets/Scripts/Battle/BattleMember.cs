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
        [SerializeField] bool isPlayer = false;

        [Header("Inventory:")]
        [SerializeField] private Inventory inventory = null;

        [Header("Team:")]
        [SerializeField] private Team pokemonTeam = null;
        [SerializeField] private int teamNumber = 1; //0 is player and player teammates

        [Header("Spots")]
        [SerializeField] private int spotsToOwn = 1;
        [SerializeField] bool hasAllSpots = false;
        [SerializeField] private List<Spot> owndSpots = new List<Spot>();

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

        public List<Spot> GetOwnedSpots()
        {
            return owndSpots;
        }

        public bool OwnSpot(Spot spot)
        {

            return owndSpots.Contains(spot);
        }

        public bool IsPlayer()
        {
            return isPlayer;
        }

        public bool HasAllSpots()
        {
            return hasAllSpots;
        }

        public Inventory GetInventory()
        {
            return inventory;
        }
        #endregion

        #region Setters
        public void SetTeamNumber(int set)
        {
            teamNumber = set;
        }

        public void SetOwndSpot(Spot set)
        {
            if (set != null)
            {
                if (!owndSpots.Contains(set))
                    owndSpots.Add(set);
            }

            hasAllSpots = (owndSpots.Count == spotsToOwn);
        }
        #endregion
    }
}