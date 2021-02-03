#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public abstract class BattleAction : ScriptableObject
{
    #region Values
    [Header("Object Reference:")]
    [SerializeField] protected bool active = false;
    [SerializeField] protected bool done = false;

    [Header("Move Reference:")]
    [SerializeField] protected Pokemon currentPokemon = null;
    [SerializeField] protected Pokemon[] targetPokemon = new Pokemon[0];
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
    public Pokemon GetCurrentPokemon()
    {
        return currentPokemon;
    }

    public bool GetDone()
    {
        return done;
    }

    public virtual BattleAction GetAction()
    {
        Debug.Log("Get Action needs override!");
        return null;
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
    protected void SendChatsToMaster(Chat[] toSend)
    {
        ChatMaster.instance.Add(toSend);
    }

    public virtual IEnumerator Activate()
    {
        Debug.Log("Activate need Override!");
        return Operation();
    }

    #endregion

    #region In
    public bool CheckAction(BattleAction action)
    {


        return false;
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
    #endregion

    #region IEnumerator
    protected virtual IEnumerator Operation()
    {
        Debug.Log("Operation need Override!");
        yield return null;
    }
    #endregion
}