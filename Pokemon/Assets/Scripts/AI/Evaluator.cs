#region Packages

using System.Linq;
using Mfknudsen.AI.Virtual;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;

#endregion

namespace Mfknudsen.AI
{
    public class Evaluator
    {
        private readonly int depth;
        private readonly BattleAction[] actions;
        private VirtualPokemon user;

        public Evaluator(int depth, BattleAction[] actions)
        {
            this.depth = depth;
            this.actions = actions;
        }

        public void TickForPokemon(Pokemon pokemon)
        {
            user = new VirtualPokemon(pokemon);

            VirtualBattle virtualBattle = new VirtualBattle();

            Evaluate(depth,
                (from move in actions
                    from spot in virtualBattle.spotOversight.spots
                    select new VirtualMove(null, move, user.GetFakePokemon(), spot.virtualPokemon.GetFakePokemon(),
                        virtualBattle))
                .ToArray()
            );
        }

        // ReSharper disable once ParameterHidesMember
        // ReSharper disable once SuggestBaseTypeForParameter
        private void Evaluate(int depth, VirtualMove[] toCheck)
        {
            if (depth == 0)
            {
                VirtualMove highest = toCheck[0];
                for (int i = 1; i < toCheck.Length; i++)
                {
                    if (toCheck[i].value <= highest.value)
                        continue;
                }

                return;
            }

            Evaluate(depth - 1,
                (from virtualMove in toCheck
                    from spot in virtualMove.virtualBattle.spotOversight.spots
                    from battleAction in actions
                    select new VirtualMove(virtualMove, battleAction, user.GetFakePokemon(),
                        spot.virtualPokemon.GetFakePokemon(), virtualMove.virtualBattle)).ToArray());
        }
    }
}