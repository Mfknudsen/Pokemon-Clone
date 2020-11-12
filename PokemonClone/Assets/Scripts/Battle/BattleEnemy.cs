using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;

public class BattleEnemy : BattleMember
{
    [SerializeField] private string startText = "";

    public string getStartText()
    {
        return startText;
    }
}
