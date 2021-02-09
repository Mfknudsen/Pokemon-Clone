#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Trainer;
#endregion

#region Enums
public enum MacroState { Aggresor, Defensiv, Recorver, Support }
public enum MicroState { Temp}
#endregion

namespace AI
{
    public class BattleAI : ScriptableObject
    {
        #region Values
        [Header("AI Reference:")]
        [SerializeField] private MacroState macroState = 0;
        [SerializeField] private MicroState microState = 0;

        [Header("Information:")]
        [SerializeField] protected bool canRemember = false;
        [Header(" - Opponent Information:")]
        [SerializeField] protected bool canRememberOpponent = false;
        [SerializeField] protected List<Dictionary<string, Team>> opponentTeams = new List<Dictionary<string, Team>>();
        [Header(" - Ally Information:")]
        [SerializeField] protected bool canRememberAlly = false;
        [SerializeField] protected List<Dictionary<string, Team>> allyTeams = new List<Dictionary<string, Team>>();
        #endregion

        #region Getters
        #endregion

        #region Setters
        #endregion

        #region In
        #endregion

        #region Out
        #endregion

        #region Internal
        #endregion
    }
}