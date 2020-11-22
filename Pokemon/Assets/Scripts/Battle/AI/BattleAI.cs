using System.Collections;
using System.Collections.Generic;
using Trainer;
using UnityEngine;

public class BattleAI : MonoBehaviour
{
    protected Team team = null;
    protected Pokemon[] currentEnemyPokemon = new Pokemon[0];

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
}
