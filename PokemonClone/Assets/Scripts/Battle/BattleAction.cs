using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionPriority { Instant, Fast, Normal, Slow}

public class BattleAction : ScriptableObject
{
    [Header("Move Reference:")]
    [SerializeField] protected ActionPriority priority = ActionPriority.Normal;

    protected Pokemon currentPokemon = null;
    protected Pokemon[] targetPokemon = new Pokemon[0];

    protected bool moveActive = false;

    public virtual void Activate()
    {
        Debug.Log("Active");
    }
}
