#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Poison", order = 1)]
public class PoisonCondition : Condition
{
    #region Values
    [SerializeField] private NonVolatile conditionName = 0;
    [SerializeField] private bool badlyPoison = false;
    [SerializeField] private float damage = 0;
    [SerializeField] private float n = 0, increaseN = 1;
    [SerializeField] private Chat onEffectChat = null;
    private void OnValidate()
    {
        n = increaseN;
    }
    #endregion
    #region Getters
    public override string GetConditionName()
    {
        return conditionName.ToString();
    }
    public override Condition GetCondition()
    {
        Condition result = this;

        if (!result.GetIsInstantiated())
        {
            result = Instantiate(this);
            result.SetIsInstantiated(true);
        }

        return result;
    }

    public float GetDamage()
    {
        float result = damage * n;
        if (badlyPoison)
            n += increaseN;
        return result;
    }
    #endregion
    #region Setters
    public void SetDamage(int maxHP)
    {
        damage = maxHP / 16;
    }
    public void SetBadlyPoison(bool set)
    {
        badlyPoison = set;
    }
    #endregion
    #region In
    public override IEnumerator ActivateCondition()
    {
        if (damage == 0)
            SetDamage(affectedPokemon.GetHealth());
        active = true;

        float divide = 200;
        float reletivSpeed = BattleMaster.instance.GetSecPerPokeMove() / divide;
        float relativeDamage = damage / divide;
        float appliedDamage = 0;

        while (appliedDamage < damage)
        {
            appliedDamage += relativeDamage;

            affectedPokemon.RecieveDamage(relativeDamage);

            yield return new WaitForSeconds(reletivSpeed);
        }

        done = true;
    }
    #endregion
}
