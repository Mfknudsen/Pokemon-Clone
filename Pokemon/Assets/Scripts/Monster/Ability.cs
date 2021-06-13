#region SDK

using Mfknudsen.Battle.Actions;
using UnityEngine;

#endregion

namespace Mfknudsen.Monster
{
    #region Enums
    public enum AbilityName { Test }
    #endregion

    [CreateAssetMenu(fileName = "Ability", menuName = "Pokemon/Create New Ablility", order = 2)]
    public class Ability : ScriptableObject
    {
        #region Values
        [Header("Ability Reference:")]
        [SerializeField] private AbilityName abilityName = 0;
        [SerializeField, TextArea] private string description = "";

        [Header("Move:")]
        [SerializeField] private bool effectsMovePriority = false;
        [SerializeField] private int effectCount = 0;
        #endregion

        #region Getters
        public bool GetEffectsMovePriority()
        {
            return effectsMovePriority;
        }
        #endregion

        #region Setters
        #endregion

        #region In
        public virtual int PriorityEffect(BattleAction action)
        {
            return 0;
        }
        #endregion
    }
}