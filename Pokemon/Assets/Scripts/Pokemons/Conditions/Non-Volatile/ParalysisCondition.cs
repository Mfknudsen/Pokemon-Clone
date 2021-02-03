using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Paralysis", order = 1)]
public class ParalysisCondition : Condition
{
    [SerializeField] private NonVolatile conditionName = NonVolatile.Poison;
    [SerializeField] private Chat onEffectChat = null;

    public override string GetConditionName()
    {
        return conditionName.ToString();
    }
}
