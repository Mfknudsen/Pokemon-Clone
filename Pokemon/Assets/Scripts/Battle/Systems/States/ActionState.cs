#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ActionState : State
    {
        // ReSharper disable once IdentifierTypo
        private readonly SpotOversight spotOversight;
        private readonly OperationManager operationManager;

        // ReSharper disable once IdentifierTypo
        public ActionState(BattleManager manager) : base(manager)
        {
            operationManager = OperationManager.Instance;
            spotOversight = manager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (Pokemon pokemon in from spot in spotOversight.GetSpots()
                select spot.GetActivePokemon()
                into pokemon
                where pokemon != null
                where pokemon.GetBattleAction() != null &&
                      !(pokemon.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition)
                select pokemon)
            {
                #region Start Action

                BattleAction action = pokemon.GetBattleAction();

                BattleLog.instance.AddNewLog(action.name, "Starting Action: " + action.name.Replace("(Clone)", ""));

                OperationsContainer container = new OperationsContainer();
                container.Add(action);
                operationManager.AddOperationsContainer(container);

                while (!operationManager.GetDone() || !ChatManager.instance.GetIsClear())
                    yield return null;

                #endregion

                #region Check Any Fainted

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Pokemon checkFainted in from spot in spotOversight.GetSpots()
                    select spot.GetActivePokemon()
                    into checkFainted
                    where checkFainted != null
                    where checkFainted.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition
                    select checkFainted)
                {
                    manager.SetPokemonFainted(checkFainted);

                    while (!operationManager.GetDone())
                        yield return null;
                }

                #endregion
            }

            manager.SetState(new AfterConditionState(manager));
        }
    }
}