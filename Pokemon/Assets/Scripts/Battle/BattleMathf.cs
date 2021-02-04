using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMathf : ScriptableObject
{
    #region Values
    [SerializeField] private static Chat superEffective = null, notEffective = null, noEffect = null, barelyEffective = null, extremlyEffective = null;
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
    public static void SetNoEffect(Chat input)
    {
        noEffect = input;
    }
    public static void SetBarelyEffective(Chat input)
    {
        barelyEffective = input;
    }
    public static void SetExtremlyEffective(Chat input)
    {
        extremlyEffective = input;
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
        TypeName attackType = attackMove.GetMoveType().GetTypeName();

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
            type += (toCheck[0].GetWeakness(attackType) - toCheck[0].GetResistance(attackType));

            if (toCheck[1] != null)
                type += (toCheck[1].GetWeakness(attackType) - toCheck[1].GetResistance(attackType));

            if (type == 0)
            {
                type = 0.5f;
                ChatMaster.instance.Add(notEffective.GetChat());
            }
            else if (type == -1)
            {
                type = 0.25f;
                ChatMaster.instance.Add(barelyEffective.GetChat());
            }
            else if (type == 2)
            {
                type = 1.5f;
                ChatMaster.instance.Add(superEffective.GetChat());
            }
            else if (type == 3)
            {
                type = 2;
                ChatMaster.instance.Add(extremlyEffective.GetChat());
            }
            else
                type = 1;
        }
        else if (noEffect != null)
            ChatMaster.instance.Add(noEffect.GetChat());

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

    public static int CalculateOtherStat(int baseStat, int IV, int EV, int level)
    {
        float result = (baseStat + IV) * 2;
        result += (EV / 4);
        result *= level;
        result /= 100;
        result += 5;

        return (int)Mathf.Floor(result);
    }

    public static int CalculateHPStat(int baseStat, int IV, int EV, int level)
    {
        float result = (baseStat + IV) * 2;
        result += (EV / 4);
        result *= level;
        result /= 100;
        result += level + 10;

        Debug.Log(baseStat + " becomes " + (int)Mathf.Floor(result));

        return (int)Mathf.Floor(result);
    }

    public static bool CalculateHit(float accurMove, float accurUser, float evaTarget, bool holdingBrightPowder)
    {
        float t = accurMove * (accurUser - evaTarget);
        if (holdingBrightPowder)
            t -= 20;
        t *= 2.55f;

        float r = Random.Range(0, 255);

        return (r <= t);
    }
    #endregion
}