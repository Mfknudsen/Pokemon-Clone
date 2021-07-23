#region SDK

using System.Collections.Generic;
using UnityEngine;
using Mfknudsen.AI;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Items;
using Mfknudsen.Trainer;

#endregion

namespace Mfknudsen.Battle.Systems
{
    [RequireComponent(typeof(Team), typeof(Inventory))]
    public class BattleMember : MonoBehaviour
    {
        #region Values

        [Header("Object Reference:")] [SerializeField]
        private string memberName = "";

        [SerializeField] private bool isPlayer = false;

        [Header("AI")] [SerializeField] private BattleAI ai = null;

        [Header("Inventory:")] [SerializeField]
        private Inventory inventory = null;

        [Header("Team:")] [SerializeField] private Team pokemonTeam = null;
        [SerializeField] private int teamNumber = 1; //0 is player and player teammates

        [Header("Spots")] [SerializeField] private int spotsToOwn = 1;
        [SerializeField] bool hasAllSpots = false;
        [SerializeField] private List<Spot> owndSpots = new List<Spot>();

        [Header("Move Selection:")] [SerializeField]
        private bool selectMove = false;

        [SerializeField] private bool hasSelectedMove = false;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            if (pokemonTeam == null)
                pokemonTeam = GetComponent<Team>();
            if (inventory == null)
                inventory = GetComponent<Inventory>();
        }

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

        public int GetSpotsToOwn()
        {
            return spotsToOwn;
        }

        public BattleAI GetBattleAI()
        {
            return ai;
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

        #region In

        public void ForceHasAllSpots()
        {
            hasAllSpots = true;
        }

        public void ActivateAIBrain()
        {
            ai.TickBrain();
        }

        #endregion
    }
}