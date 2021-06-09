#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Monster;
using Battle;
using Mfknudsen.Chat;

#endregion

public abstract class BattleAction : ScriptableObject
{
    #region Values
    [Header("Object Reference:")]
    [SerializeField] protected bool active = false;
    [SerializeField] protected bool done = false;

    [Header("Move Reference:")]
    [SerializeField] protected Pokemon currentPokemon = null;
    [SerializeField] protected List<Pokemon> targetPokemon = new List<Pokemon>();
    [SerializeField] protected bool moveActive = false;

    [Header("Priority:")]
    [SerializeField] protected int priority = 0;
    [SerializeField] protected string[] priorityInteraction;

    [Header("Chat:")]
    [SerializeField] protected Chat[] chatOnActivation = new Chat[0];

    [Header("Instatiation:")]
    protected bool isInstantiated = false;
    #endregion

    #region Getters
    public virtual BattleAction GetAction()
    {
        return this;
    }

    public Pokemon GetCurrentPokemon()
    {
        return currentPokemon;
    }

    public bool GetDone()
    {
        return done;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public int GetPriority()
    {
        return priority;
    }
    #endregion

    #region Setter
    public void SetCurrentPokemon(Pokemon pokemon)
    {
        currentPokemon = pokemon;
    }

    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }

    public void SetPriority(int set)
    {
        priority = set;
    }
    #endregion

    #region Out
    public virtual IEnumerator Activate()
    {
        Debug.Log("Activate need Override!");
        return Operation();
    }

    #endregion

    #region Internal
    protected void SetupChats()
    {
        for (int i = 0; i < chatOnActivation.Length; i++)
            chatOnActivation[i] = Instantiate(chatOnActivation[i]);
    }

    protected virtual Chat[] TransferInformationToChat()
    {
        Debug.Log("Transfering");
        return new Chat[0];
    }

    protected virtual Spot[] SetupTargets(int targetIndex)
    {
        Debug.Log("Setup Targets Need Over!");
        return new Spot[0];
    }
    #endregion

    #region IEnumerator
    protected virtual IEnumerator Operation()
    {
        Debug.Log("Operation need Override!");
        yield return null;
    }
    #endregion
}