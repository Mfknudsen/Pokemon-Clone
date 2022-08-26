#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI.Battle.Evaluator.Virtual;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using Mfknudsen.Weathers;

#endregion

// ReSharper disable SuspiciousTypeConversion.Global
namespace Mfknudsen.AI.Battle.Evaluator
{
    public static class VirtualMathf
    {
        public static bool MoveCanHit(BattleAction move, VirtualSpot user, VirtualSpot target)
        {
            if (move is PokemonMove pokemonMove)
            {
                HitType hitType = pokemonMove.GetHitType();
                if (hitType == HitType.All)
                    return true;

                if ((hitType == HitType.One || hitType == HitType.AllAdjacent))
                {
                    bool[] targets = pokemonMove.GetTargetable();

                    Pokemon actual = target.virtualPokemon.GetActualPokemon();
                    if (targets[0] && user.front == actual)
                        return true;
                }

                if (hitType == HitType.AllExceptUser && user != target)
                    return true;

                //TODO Add All
            }

            return false;
        }

        public static int CalculateVirtualDamage(PokemonMove move, Pokemon user, Pokemon target,
            VirtualBattle virtualBattle)
        {
            Category category = move.GetCategory();
            int attack = category == Category.Physical
                    ? user.GetStat(Stat.Attack)
                    : user.GetStat(Stat.SpAtk),
                defense = category == Category.Physical
                    ? user.GetStat(Stat.Defence)
                    : user.GetStat(Stat.SpDef);

            float result = ((2 * user.GetLevel()) / 5) + 2;
            result *= move.GetPower() * (attack / defense);
            result /= 50;
            result += 2;

            result = CalculateVirtualModifiers(user, target, move, virtualBattle)
                .Aggregate(result, (current, modifier) => current * modifier);

            return (int)result;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private static float[] CalculateVirtualModifiers(Pokemon user, Pokemon target, PokemonMove move,
            VirtualBattle virtualBattle)
        {
            List<float> result = new();

            Type[] defTypes = target.GetTypes();

            List<IBypassImmune> bypassImmune = new();
            List<IImmuneAttackType> immuneAttackType = new();
            List<IBurnStop> burnStop = new();
            List<IFinalModifier> finalModifier = new();

            foreach (Pokemon pokemon in virtualBattle.spotOversight.spots.Select(spot =>
                spot.virtualPokemon.GetFakePokemon()))
            {
                bypassImmune.AddRange(pokemon.GetAbilitiesOfType<IBypassImmune>());
                immuneAttackType.AddRange(pokemon.GetAbilitiesOfType<IImmuneAttackType>());
                burnStop.AddRange(pokemon.GetAbilitiesOfType<IBurnStop>());
                finalModifier.AddRange(pokemon.GetAbilitiesOfType<IFinalModifier>());
            }

            foreach (Weather virtualBattleWeather in virtualBattle.weathers.Where(virtualBattleWeather => virtualBattleWeather != null))
            {
                bypassImmune.Add(virtualBattleWeather as IBypassImmune);
                immuneAttackType.Add(virtualBattleWeather as IImmuneAttackType);
                burnStop.Add(virtualBattleWeather as IBurnStop);
                finalModifier.Add(virtualBattleWeather as IFinalModifier);
            }


            #region Immune

            if (target.GetTypes().Where(t => t != null).Any(t => t.GetNoEffect(move.GetMoveType().GetTypeName())) ||
                immuneAttackType.Any(i => i.MatchType(move.GetMoveType().GetTypeName())))
            {
                if (bypassImmune.Any(b => b.CanEffect(move.GetMoveType().GetTypeName(), defTypes[0].GetTypeName())))
                    return new[] { 0f };

                if (defTypes.Length > 1)
                {
                    if (bypassImmune.Any(b => b.CanEffect(move.GetMoveType().GetTypeName(), defTypes[1].GetTypeName())))
                        return new[] { 0f };
                }
            }

            #endregion

            #region Stab

            result.Add(user.IsSameType(move.GetMoveType().GetTypeName()) ? 1.5f : 1);

            #endregion

            #region Burn

            if (move.GetCategory() == Category.Physical &&
                user.GetConditionOversight().GetNonVolatileStatus() is BurnCondition &&
                !burnStop.Any(b => b.CanStopBurn(user)))
            {
                result.Add(0.5f);
            }

            #endregion

            #region Other

            result.AddRange(finalModifier.Select(f => f.Modify(move)));

            #endregion

            return result.ToArray();
        }
    }
}