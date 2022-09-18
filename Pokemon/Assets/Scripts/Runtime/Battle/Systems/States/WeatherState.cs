#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class WeatherState : State
    {
        public WeatherState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager, uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            foreach (IOnTurnEnd onTurnEnd in this.battleManager.GetWeatherManager().GetWeatherWithInterface<IOnTurnEnd>()
                .Where(i => i is IOperation))
            {
                OperationsContainer container = new();
                container.Add((IOperation)onTurnEnd);
                operationManager.AddOperationsContainer(container);
            }

            this.battleManager.SetState(new RoundDoneState(this.battleManager, operationManager, chatManager, uiManager, playerManager));
            yield break;
        }
    }
}