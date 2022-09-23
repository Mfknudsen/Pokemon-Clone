#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class ComputerTurnState : State
    {
        public ComputerTurnState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager, uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            SpotOversight spotOversight = this.battleManager.GetSpotOversight();

            foreach (Spot spot in spotOversight.GetSpots().Where(spot =>
                spot.GetActivePokemon() != null &&
                !spot.GetBattleMember().IsPlayer()))
            {
                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = spot.GetActivePokemon();

                battleMember.ActivateAIBrain(pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            this.battleManager.SetState(new ActionState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
        }
    }
}