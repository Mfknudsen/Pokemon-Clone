using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;

public class BattleEnemy : BattleMember
{
    #region Values
    [SerializeField] private Chat[] startChat = new Chat[1];
    [SerializeField] private Chat[] endChat = new Chat[2];
    #endregion

    #region Getters
    public Chat[] GetStartChat()
    {
        for (int i = 0; i < startChat.Length; i++)
            startChat[i] = startChat[i].GetChat();

        return startChat;
    }

    public Chat GetEndChat(bool lost)
    {
        int i = 0;
        if (!lost)
            i = 1;

        if (endChat[i].GetIsInstantiated())
            return endChat[i];
        else
            return Instantiate(endChat[i]);

    }
    #endregion
}