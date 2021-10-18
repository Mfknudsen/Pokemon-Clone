#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI.Virtual;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;

#endregion

namespace Mfknudsen.AI
{
    public class Evaluator
    {
        private int depth;
        private VirtualTeam ownTeam;
        private List<VirtualTeam> allies, enemies;
        private List<VirtualMove> moves;

        public Evaluator(int depth, Team team, int teamNumber)
        {
            this.depth = depth;
            ownTeam = new VirtualTeam(team);

            allies = new List<VirtualTeam>();
            enemies = new List<VirtualTeam>();

            List<Team> a = new List<Team>(), e = new List<Team>();

            foreach (BattleMember battleMember in BattleManager.instance.GetSpotOversight().GetSpots()
                .Select(spot => spot.GetBattleMember()))
            {
                Team t = battleMember.GetTeam();

                if (battleMember.GetTeamNumber() == teamNumber && !a.Contains(t))
                {
                    a.Add(t);
                    allies.Add(new VirtualTeam(t));
                }
                else if (!e.Contains(t))
                {
                    e.Add(t);
                    enemies.Add(new VirtualTeam(t));
                }
            }
        }

        public void TickForPokemon(Pokemon pokemon)
        {
            foreach (PokemonMove move in pokemon.GetMoves())
            {
                moves.Add(new VirtualMove(null, move, pokemon, ));
            }

            Evaluate(depth);
        }

        private void Evaluate(int depth, List<VirtualMove> toCheck)
        {
            if (this.depth == 0)
            {
                return;
            }

            foreach (VirtualMove virtualMove in toCheck)
            {
                
            }
        }
    }
}