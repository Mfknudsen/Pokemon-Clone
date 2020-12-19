using System.Collections;
using System.Collections.Generic;
using Trainer;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    [Header("Object Reference:")]
    [SerializeField] protected Team team = null;
    [SerializeField] protected Pokemon[] currentEnemyPokemon = new Pokemon[0];

    [SerializeField] protected bool chooseActive = false;


    #region Getters
    #endregion

    #region Setters
    public void SetChooseActive(bool set)
    {
        chooseActive = set;
    }
    #endregion

    #region In
    #endregion

    #region Out
    #endregion

    #region Internal
    #region Decide
    public virtual void Decide()
    {

    }

    public virtual bool DecideChangeOut()
    {
        return false;
    }

    public virtual bool DecideUseMove()
    {
        return false;
    }
    #endregion
    #endregion
}
