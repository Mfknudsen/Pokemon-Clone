using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;

public class TestEnemy : BattleMember
{
    [SerializeField]
    private string[] startText;

    public string[] getStartText()
    {
        return startText;
    }
}
