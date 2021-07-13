#region SDK

using Mfknudsen.Battle.Actions.Move;
using UnityEngine;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Pokémon
{
    [System.Serializable]
    [Node("Selection/Pokemon/Random Move", "Choose Random Move")]
    public class ChooseRandomMove : BaseNode
    {
        [InputType("User Spot", typeof(Spot))] public Spot userSpot;

        [OutputType("Selection", typeof(PokemonMove))]
        public PokemonMove selection;

        public override void Tick(BattleAI setup)
        {
            Pokemon user = userSpot.GetActivePokemon();
            PokemonMove[] moves = user.GetMoves();
            List<PokemonMove> options = new List<PokemonMove>();

            foreach (Spot spot in BattleMaster.instance.GetSpotOversight().GetSpots())
            {
                foreach (PokemonMove pokemonMove in moves)
                {
                    if (pokemonMove is null) continue;

                    if (!BattleMathf.CanHit(userSpot, spot, pokemonMove)) continue;

                    PokemonMove move = Object.Instantiate(pokemonMove);
                    move.SetCurrentPokemon(user);
                    move.SetTargets(spot.GetActivePokemon());
                    options.Add(move);
                }
            }

            int index = Random.Range(0, options.Count);
            
            selection = options[index];

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}