#region SDK

using System;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.Player
{
    [RequireComponent(typeof(BattleMember), typeof(Team), typeof(Inventory))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;

        #region Values

        public BattleStarter closestBattleStarter;
        
        [Header("Object Reference:")] [SerializeField]
        private Team team;

        [SerializeField] private Controller controller;

        [SerializeField] private BattleMember battleMember;

        [SerializeField] private Interactions interactions;
        
        [Header("Character Sheet:")] [SerializeField]
        private int bagdeCount = 0;

        [SerializeField] private string[] pronouns = new string[2] {"They", "Them"}; //Inspired by Temtem

        #endregion

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        #region Getters

        public string[] GetPronouns()
        {
            return pronouns;
        }

        public Team GetTeam()
        {
            return team;
        }

        public BattleMember GetBattleMember()
        {
            return battleMember;
        }

        public Interactions GetInteractions()
        {
            return interactions;
        }

        #endregion

        #region Setters

        public void SetPronous(string one, string two)
        {
            if (one != "")
                pronouns[0] = one;
            else if (two != "")
                pronouns[1] = two;
        }

        #endregion
    }
}