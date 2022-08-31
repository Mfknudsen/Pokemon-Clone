using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Pokémon;
using Runtime.Systems;
using Runtime.Trainer;

namespace Runtime.Battle.Systems.Static_Operations
{
    public class CaughtPokemon : IOperation
    {
        private readonly Pokemon target;
        private readonly Team toReceive;
        private bool done;

        public CaughtPokemon(Pokemon target, Team toReceive)
        {
            this.target = target;
            this.toReceive = toReceive;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots()
                .Where(spot => spot.GetBattleMember().GetTeam().PartOfTeam(target)))
            {
                Team team = spot.GetBattleMember().GetTeam();
                team.RemovePokemonFromTeam(target);

                break;
            }

            if (toReceive != null)
            {
                toReceive.AddNewPokemonToTeam(target);
            }

            done = true;
            yield break;
        }

        public void End()
        {
        }
    }
}