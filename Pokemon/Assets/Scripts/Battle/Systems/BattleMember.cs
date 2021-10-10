#region Packages

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
        private string memberName;

        [SerializeField] private bool isPlayer, isWild;

        [Header("AI")] [SerializeField] private BattleAI ai;

        [Header("Inventory:")] [SerializeField]
        private Inventory inventory;

        [Header("Team:")] [SerializeField] private Team pokemonTeam;
        [SerializeField] private int teamNumber = 1; //0 is player and player teammates

        [Header("Spots")] [SerializeField] private int spotsToOwn = 1;
        [SerializeField] bool hasAllSpots;
        [SerializeField] private List<Spot> ownedSpots = new List<Spot>();

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

        public bool IsWild()
        {
            return isWild;
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
            return ownedSpots;
        }

        public bool OwnSpot(Spot spot)
        {
            return ownedSpots.Contains(spot);
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

        public void SetOwnedSpot(Spot set)
        {
            if (set != null)
            {
                if (!ownedSpots.Contains(set))
                    ownedSpots.Add(set);
            }

            hasAllSpots = (ownedSpots.Count == spotsToOwn);
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