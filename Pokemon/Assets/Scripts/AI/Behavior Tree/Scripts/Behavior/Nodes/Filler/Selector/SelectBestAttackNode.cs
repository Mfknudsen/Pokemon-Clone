#region SDK

using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Selector
{
    [System.Serializable]
    [Node("Select/Pokémon/Best Attack", "Select Best Attack")]
    public class SelectBestAttackNode : BaseNode
    {
        [InputType("User", typeof(Spot))] public Spot userSpot;

        [InputType("Targets", typeof(Spot[]))] public Spot[] targets;

        [OutputType("Best Move", typeof(PokemonMove))]
        public PokemonMove bestMove;

        public override void Tick(BattleAI ai)
        {
            float highestDamage = 0;
            Pokemon user = userSpot.GetActivePokemon();

            foreach (Spot target in targets)
            {
                Pokemon pokemon = target.GetActivePokemon();

                foreach (PokemonMove pokemonMove in user.GetMoves())
                {
                    if (!BattleMathf.CanHit(userSpot, target, pokemonMove)) continue;

                    Category category = pokemonMove.GetCategory();

                    float attackPower = user.GetStat(category == Category.Physical ? Stat.Attack : Stat.SpAtk);

                    float defencePower = user.GetStat(category == Category.Physical ? Stat.Defence : Stat.SpDef);

                    float damage = BattleMathf.CalculateDamage(user.GetLevel(), attackPower, defencePower,
                        pokemonMove.GetPower(),
                        BattleMathf.CalculateModifiers(user, pokemon, pokemonMove,
                            BattleMathf.MultiTargets(target, userSpot, pokemonMove)));

                    if (damage <= highestDamage) continue;

                    highestDamage = damage;

                    bestMove = pokemonMove;
                    bestMove.SetCurrentPokemon(user);
                    bestMove.SetTargets(pokemon);
                }
            }

            ContinueTransitions(ai);
        }

        protected override void Resets()
        {
        }
    }
}