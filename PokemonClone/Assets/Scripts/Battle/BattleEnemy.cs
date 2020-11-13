using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;

public class BattleEnemy : BattleMember
{
    [SerializeField] private Chat startChat = null;
    [SerializeField] private Chat[] endChat = new Chat[2];

    public Chat GetStartChat()
    {
        return startChat;
    }

    public Chat GetEndChat(bool lost)
    {
        if (lost)
            return endChat[0];
        else
            return endChat[1];
    }
}
