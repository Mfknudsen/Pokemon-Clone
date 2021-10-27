#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ComputerTurnState : State
    {
        public ComputerTurnState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            SpotOversight spotOversight = manager.GetSpotOversight();

            foreach (Spot spot in spotOversight.GetSpots().Where(spot =>
                spot.GetActivePokemon() != null &&
                !spot.GetBattleMember().IsPlayer()))
            {
                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = spot.GetActivePokemon();

                #region Send Information

                #endregion

                battleMember.ActivateAIBrain(pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            manager.SetState(new ActionState(manager));
        }
    }
}