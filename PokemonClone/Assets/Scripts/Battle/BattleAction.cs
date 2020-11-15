using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionPriority { Instant, Fast, Normal, Slow }

public class BattleAction : ScriptableObject
{
    [Header("Move Reference:")]
    [SerializeField] protected ActionPriority priority = ActionPriority.Normal;
    [SerializeField] protected Pokemon currentPokemon = null;
    [SerializeField] protected Pokemon[] targetPokemon = new Pokemon[0];
    [SerializeField] protected bool moveActive = false;

    [Header("Chat:")]
    [SerializeField] protected Chat[] chatOnActivation = new Chat[0];

    protected void SetupChats()
    {
        for (int i = 0; i < chatOnActivation.Length; i++)
            chatOnActivation[i] = Instantiate(chatOnActivation[i]);
    }

    protected void SendChatsToMaster()
    {
        ChatMaster.instance.Add(chatOnActivation);
    }

    public virtual void Activate()
    {
        Debug.Log("Active");
    }
}
