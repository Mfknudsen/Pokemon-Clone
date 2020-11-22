using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMathf : MonoBehaviour
{
    #region Values
    [SerializeField] private static Chat superEffective = null, notEffective = null;
    #endregion

    #region Setter
    public static void SetSuperEffective(Chat input)
    {
        superEffective = input;
    }
    public static void SetNotEffective(Chat input)
    {
        notEffective = input;
    }
    #endregion

    #region Calculations
    public static bool CalculateCatch()
    {
        return false;
    }

    public static bool CalculateWildEncounter()
    {
        return false;
    }

    public static int CalculateDamage(int level, float attack, float defense, float power, float[] modifiers)
    {
        float result = ((2 * level) / 5) + 2;
        result *= power * (attack / defense);
        result /= 50;
        result += 2;
        foreach (float modifier in modifiers)
            result *= modifier;

        return (int)result;
    }

    public static float[] CalculateModifiers(Pokemon attacker, Pokemon target, PokemonMove attackMove, bool multiTarget)
    {
        TypeName attackType = attackMove.GetType().GetTypeName();

        List<float> result = new List<float>();
        #region Targets
        float targets = 1;
        if (multiTarget)
            targets = 0.75f;
        result.Add(targets);
        #endregion
        #region Weather
        float weather = 1;
        if (BattleMaster.weather == Weather.Rain)
        {
            if (attackType == TypeName.Fire)
                weather = 0.5f;
            else if (attackType == TypeName.Water)
                weather = 1.5f;
        }
        else if (BattleMaster.weather == Weather.HarshSunlight)
        {
            if (attackType == TypeName.Fire)
                weather = 1.5f;
            else if (attackType == TypeName.Water)
                weather = 0.5f;
        }
        result.Add(weather);
        #endregion
        #region Badge
        float badge = 1;

        result.Add(badge);
        #endregion
        #region Critical
        float critical = 1;

        if (CalculateCriticalRool())
            critical = 2;

        result.Add(critical);
        #endregion
        #region Random
        float random = Random.Range(0.75f, 1.0f);
        result.Add(random);
        #endregion
        #region Stab
        float stab = 1;
        if (attacker.IsSameType(attackType))
            stab = 1.5f;
        result.Add(stab);
        #endregion
        #region Type
        float type = 1;

        Type[] toCheck = target.GetTypes();
        if (toCheck[0].GetNoEffect(attackType))
            type = 0;
        else if (toCheck[1] != null)
        {
            if (toCheck[1].GetNoEffect(attackType))
                type = 0;
        }

        if (type != 0)
        {
            type += (0.5f * toCheck[0].GetWeakness(attackType)) - (0.5f * toCheck[0].GetResistance(attackType));

            if (toCheck[1] != null)
                type += (0.5f * toCheck[1].GetWeakness(attackType)) - (0.5f * toCheck[1].GetResistance(attackType));

            if (type == 1.5f)
                ChatMaster.instance.Add(superEffective.GetChat());
            else if (type == 0.5f)
                ChatMaster.instance.Add(notEffective.GetChat());
        }

        result.Add(type);
        #endregion
        #region Burn
        float burn = 1;
        if (attackMove.GetCategory() == Category.Physical)
        {
            if (attacker.GetConditionOversight() != null)
            {
                if (attacker.GetConditionOversight().GetNonVolatileStatus() != null)
                {
                    if (attacker.GetConditionOversight().GetNonVolatileStatus().GetConditionName() == "Burn")
                        burn = 0.5f;
                }
            }
        }
        result.Add(burn);
        #endregion
        #region Other

        #endregion
        return result.ToArray();
    }

    public static bool CalculateCriticalRool()
    {
        float result = 0;

        return false;
    }

    public static int CalculateConfusionDamage(int level, float attack, float defense)
    {
        float power = 40;
        float result = ((2 * level) / 5) + 2;
        result *= power * (attack / defense);
        result /= 50;
        result += 2;
        result *= Random.Range(0.75f, 1.0f);

        return (int)result;
    }

    #endregion
}