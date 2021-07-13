#region SDK

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using UnityEngine;
using Random = UnityEngine.Random;
using Type = Mfknudsen.Pokémon.Type;

#endregion

// ReSharper disable once ParameterTypeCanBeEnumerable.Global
// ReSharper disable once MemberCanBePrivate.Global

namespace Mfknudsen.Battle.Systems
{
    public class BattleMathf : ScriptableObject
    {
        #region Values

        private static Chat _superEffective,
            _notEffective,
            _noEffect,
            _barelyEffective,
            _extremlyEffective;

        #endregion

        #region Setter

        public static void SetSuperEffective(Chat input)
        {
            _superEffective = input;
        }

        public static void SetNotEffective(Chat input)
        {
            _notEffective = input;
        }

        public static void SetNoEffect(Chat input)
        {
            _noEffect = input;
        }

        public static void SetBarelyEffective(Chat input)
        {
            _barelyEffective = input;
        }

        public static void SetExtremlyEffective(Chat input)
        {
            _extremlyEffective = input;
        }

        #endregion

        #region Out

        public static bool CanHit(Spot user, Spot target, PokemonMove pokemonMove)
        {
            if (user is null || target is null || pokemonMove is null)
                return false;

            HitType type = pokemonMove.GetHitType();

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if ((type == HitType.AllAdjacent || type == HitType.AllAdjacentOneSide || type == HitType.One) &&
                user.GetAllAdjacentSpots().Contains(target))
                return true;
            
            if ((type == HitType.AllExceptUser || type == HitType.AllOneSideExceptUser) && target == user)
                return false;

            if (type == HitType.AllExceptUser || type == HitType.AllOneSideExceptUser || type == HitType.All ||
                type == HitType.AllOneSide)
                return true;

            return false;
        }

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

            return (int) result;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static float[] CalculateModifiers(Pokemon attacker, Pokemon target, PokemonMove attackMove,
            bool multiTarget)
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
            if (BattleMaster.instance.GetWeather() == Weather.Rain)
            {
                weather = attackType switch
                {
                    TypeName.Fire => 0.5f,
                    TypeName.Water => 1.5f,
                    _ => weather
                };
            }
            else if (BattleMaster.instance.GetWeather() == Weather.HarshSunlight)
            {
                weather = attackType switch
                {
                    TypeName.Fire => 1.5f,
                    TypeName.Water => 0.5f,
                    _ => weather
                };
            }

            result.Add(weather);

            #endregion

            #region Badge

            // ReSharper disable once ConvertToConstant.Local
            float badge = 1;

            result.Add(badge);

            #endregion

            #region Critical

            float critical = 1;

            if (CalculateCriticalRoll())
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
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (toCheck[1] != null && type != 0)
            {
                if (toCheck[1].GetNoEffect(attackType))
                    type = 0;
            }

            if (type != 0)
            {
                type += toCheck[0].GetWeakness(attackType) - toCheck[0].GetResistance(attackType);

                // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
                if (toCheck[1] != null)
                    type += (toCheck[1].GetWeakness(attackType) - toCheck[1].GetResistance(attackType));

                switch (type)
                {
                    case 0:
                        type = 0.5f;
                        ChatMaster.instance.Add(_notEffective.GetChat());
                        break;
                    case -1:
                        type = 0.25f;
                        ChatMaster.instance.Add(_barelyEffective.GetChat());
                        break;
                    case 2:
                        type = 1.5f;
                        ChatMaster.instance.Add(_superEffective.GetChat());
                        break;
                    case 3:
                        type = 2;
                        ChatMaster.instance.Add(_extremlyEffective.GetChat());
                        break;
                    default:
                        type = 1;
                        break;
                }
            }
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            else if (_noEffect != null)
                ChatMaster.instance.Add(_noEffect.GetChat());

            result.Add(type);

            #endregion

            #region Burn

            float burn = 1;
            if (attackMove.GetCategory() == Category.Physical)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
                if (attacker.GetConditionOversight() != null)
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
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

        public static bool CalculateCriticalRoll()
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

            return (int) result;
        }

        public static int CalculateOtherStat(int baseStat, int iv, int ev, int level)
        {
            float result = (baseStat + iv) * 2;
            result += (float) ev / 4;
            result *= level;
            result /= 100;
            result += 5;

            return (int) Mathf.Floor(result);
        }

        // ReSharper disable once InconsistentNaming
        public static int CalculateHPStat(int baseStat, int iv, int ev, int level)
        {
            float result = (baseStat + iv) * 2;
            result += (float) ev / 4;
            result *= level;
            result /= 100;
            result += level + 10;

            Debug.Log(baseStat + " becomes " + (int) Mathf.Floor(result));

            return (int) Mathf.Floor(result);
        }

        public static bool CalculateHit(float accuracyMove, float accuracyUser, float evaTarget,
            bool holdingBrightPowder)
        {
            float t = accuracyMove * (accuracyUser - evaTarget);
            if (holdingBrightPowder)
                t -= 20;
            t *= 2.55f;

            float r = Random.Range(0, 255);

            return (r <= t);
        }

        public static bool MultiTargets(Spot target, Spot user, PokemonMove move)
        {
            HitType type = move.GetHitType();
            List<Spot> totals = new List<Spot>();

            switch (type)
            {
                case HitType.One:
                    return false;

                case HitType.AllAdjacent:
                    totals = user.GetAllAdjacentSpots();

                    return totals.Count > 1;

                case HitType.AllAdjacentOneSide:
                    totals.Add(target);
                    totals.Add(target.GetLeft());
                    totals.Add(target.GetRight());

                    List<Spot> checks = user.GetAllAdjacentOneSideSpots(target.GetBattleMember().GetTeamNumber() ==
                                                                        user.GetBattleMember().GetTeamNumber());

                    foreach (Spot total in totals.Where(total => total != target)
                        .Where(total => !checks.Contains(total)))
                        totals.Remove(total);

                    return totals.Count > 1;

                case HitType.AllOneSideExceptUser:
                    totals = GetAllOneSide(target);

                    if (totals.Contains(user))
                        totals.Remove(user);

                    return totals.Count > 1;

                case HitType.AllOneSide:
                    totals = GetAllOneSide(target);

                    return totals.Count > 1;

                case HitType.AllExceptUser:
                    totals = BattleMaster.instance.GetSpotOversight().GetSpots();

                    if (totals.Contains(user))
                        totals.Remove(user);

                    return totals.Count > 1;

                case HitType.All:
                    totals = BattleMaster.instance.GetSpotOversight().GetSpots();

                    return totals.Count > 1;

                default:
                    return false;
            }
        }

        #endregion

        #endregion

        #region Internal

        private static List<Spot> GetAllOneSide(Spot spot)
        {
            bool continueCheck = true;
            List<Spot> result = new List<Spot> {spot};

            while (continueCheck)
            {
                continueCheck = false;

                foreach (Spot s in result)
                {
                    if (!result.Contains(s.GetLeft()))
                    {
                        result.Add(s.GetLeft());
                        continueCheck = true;
                    }

                    if (result.Contains(s.GetRight())) continue;

                    result.Add(s.GetRight());
                    continueCheck = true;
                }
            }

            return result;
        }

        #endregion
    }
}