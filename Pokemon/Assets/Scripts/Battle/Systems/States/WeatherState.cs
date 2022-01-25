#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems.Interfaces;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class WeatherState : State
    {
        public WeatherState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            OperationManager operationManager = OperationManager.instance;
            foreach (IOnTurnEnd onTurnEnd in manager.GetWeatherManager().GetWeatherWithInterface<IOnTurnEnd>()
                .Where(i => i is IOperation))
            {
                OperationsContainer container = new OperationsContainer();
                container.Add((IOperation)onTurnEnd);
                operationManager.AddOperationsContainer(container);
            }

            manager.SetState(new RoundDoneState(manager));
            yield break;
        }
    }
}