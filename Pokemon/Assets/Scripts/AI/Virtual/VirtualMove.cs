#region Packages

using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Virtual
{
    public class VirtualMove
    {
        public readonly int value;

        private readonly BattleAction move;
        private readonly VirtualMove preValue;
        public readonly VirtualBattle virtualBattle;

        public VirtualMove(VirtualMove preValue, BattleAction move, Pokemon user, Pokemon target,
            VirtualBattle virtualBattle)
        {
            this.preValue = preValue;
            this.move = move;
            this.virtualBattle = new VirtualBattle(virtualBattle);

            if (preValue != null)
                value = preValue.value;

            if (move is PokemonMove pokemonMove)
            {
                if (pokemonMove.GetCategory() == Category.Status)
                {
                }
                else
                {
                    value += VirtualMathf.CalculateVirtualDamage(pokemonMove, user, target, virtualBattle);
                }
            }
        }
    }
}