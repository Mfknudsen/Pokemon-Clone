#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI.Virtual;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI
{
    public static class VirtualMathf
    {
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

            result = CalculateVirtualModifiers(virtualBattle)
                .Aggregate(result, (current, modifier) => current * modifier);

            return (int) result;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private static float[] CalculateVirtualModifiers(VirtualBattle virtualBattle)
        {
            List<IBypassImmune> bypassImmunes = new List<IBypassImmune>(),
                immuneAttackType;
            foreach (Pokemon pokemon in virtualBattle.spotOversight.spots.Select(spot => spot.virtualPokemon.GetFakePokemon()))
            {
                bypassImmunes.AddRange(pokemon.GetAbilitiesOfType<IBypassImmune>());
            }

            return new[] {1f};
        }
    }
}