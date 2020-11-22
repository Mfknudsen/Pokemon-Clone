using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionPriority { Instant, Fast, Normal, Slow }

public class BattleAction : ScriptableObject
{
    #region Values
    [Header("Move Reference:")]
    [SerializeField] protected ActionPriority priority = ActionPriority.Normal;
    [SerializeField] protected Pokemon currentPokemon = null;
    [SerializeField] protected Pokemon[] targetPokemon = new Pokemon[0];
    [SerializeField] protected bool moveActive = false;

    [Header("Chat:")]
    [SerializeField] protected Chat[] chatOnActivation = new Chat[0];
    #endregion

    #region Out
    protected void SendChatsToMaster()
    {
        ChatMaster.instance.Add(chatOnActivation);
    }
    #endregion

    #region In
    public virtual void Activate()
    {
        Debug.Log("Active");
    }
    #endregion

    #region Internal
    protected void SetupChats()
    {
        for (int i = 0; i < chatOnActivation.Length; i++)
            chatOnActivation[i] = Instantiate(chatOnActivation[i]);
    }

    protected virtual void TransferInformationToChat()
    {
        Debug.Log("Transfering");
    }
    #endregion
}