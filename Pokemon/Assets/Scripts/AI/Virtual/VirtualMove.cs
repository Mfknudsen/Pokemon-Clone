#region Packages

using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Virtual
{
    public class VirtualMove
    {
        public int value;

        private readonly PokemonMove move;
        private readonly VirtualMove preValue;

        public VirtualMove(VirtualMove preValue, PokemonMove move, Pokemon user, Pokemon target)
        {
            this.preValue = preValue;
            this.move = move;

            if (move.GetCategory() == Category.Physical || move.GetCategory() == Category.Special)
            {
                int attack = move.GetCategory() == Category.Physical
                        ? user.GetStat(Stat.Attack)
                        : user.GetStat(Stat.SpAtk),
                    defence = move.GetCategory() == Category.Physical
                        ? user.GetStat(Stat.Defence)
                        : user.GetStat(Stat.SpDef);

                if (move.GetCategory() == Category.Physical)
                    value = BattleMathf.CalculateDamage(user.GetLevel(), attack, defence, move.GetPower(),
                        BattleMathf.CalculateModifiers(user, target, move, false, false));
            }
            else
            {
                
            }
        }
    }
}