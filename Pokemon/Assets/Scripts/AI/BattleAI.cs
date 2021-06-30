#region SDK

using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    #region Enums

    public enum MacroState
    {
        Aggresor,
        Defensiv,
        Recorver,
        Support
    }

    public enum MicroState
    {
        Temp
    }

    #endregion

    public class BattleAI : ScriptableObject
    {
        #region Values

        private bool isInstance;
        
        [Header("AI Reference:")] public BehaviorController behaviorController;

        [Header("Information:")] [SerializeField]
        protected Pokemon currentPokemon;

        protected bool canRemember = false;

        [Header(" - Opponent Information:")] [SerializeField]
        protected bool canRememberOpponent = false;

        [SerializeField] protected List<Dictionary<string, Team>> opponentTeams = new List<Dictionary<string, Team>>();

        [Header(" - Ally Information:")] [SerializeField]
        protected bool canRememberAlly = false;

        [SerializeField] protected List<Dictionary<string, Team>> allyTeams = new List<Dictionary<string, Team>>();

        #endregion

        #region Getters

        public BattleAI GetInstance()
        {
            if (isInstance) return this;

            BattleAI ai = CreateInstance(GetType()) as BattleAI;

            ai.SetIsInstance(true);
            
            return ai;
        }
        
        #endregion

        #region Setters

        public void SetIsInstance(bool set)
        {
            isInstance = set;
        }
        #endregion

        #region In

        public void TickBrain()
        {
            
        }
        
        #endregion
    }

    public readonly struct Decision
    {
        private readonly BattleAction action;
        private readonly int targetSpotIndex;

        public Decision(BattleAction action, int targetSpotIndex)
        {
            this.action = action;
            this.targetSpotIndex = targetSpotIndex;
        }

        public BattleAction GetAction()
        {
            return action;
        }

        public int GetTargetSpotIndex()
        {
            return targetSpotIndex;
        }
    }
}