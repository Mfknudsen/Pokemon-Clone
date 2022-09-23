#region Packages

using System.Collections;
using System.Linq;
using Runtime._Debug;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class ActionState : State
    {
        private readonly SpotOversight spotOversight;

        public ActionState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager,
            UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager,
            uiManager, playerManager)
        {
            this.spotOversight = battleManager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (Pokemon pokemon in this.spotOversight.GetSpots()
                         .Select(s =>
                             s.GetActivePokemon())
                         .Where(p =>
                             !p &&
                             !p.GetBattleAction() &&
                             p.GetConditionOversight().GetNonVolatileStatus() is not FaintedCondition))
            {
                #region Start Action

                BattleAction action = pokemon.GetBattleAction();

                Logger.instance.AddNewLog(action.name, "Starting Action: " + action.name.Replace("(Clone)", ""));

                OperationsContainer container = new(action);
                this.operationManager.AddOperationsContainer(container);

                while (!this.operationManager.GetDone() || !this.chatManager.GetIsClear())
                    yield return null;

                #endregion

                #region Check Any Fainted

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Pokemon checkPokemon in this.spotOversight.GetSpots()
                             .Select(s =>
                                 s.GetActivePokemon())
                             .Where(p =>
                                 !p &&
                                 p.GetCurrentHealth() == 0))
                {
                    this.battleManager.SetPokemonFainted(checkPokemon);

                    if (checkPokemon.GetConditionOversight()
                            .GetNonVolatileStatus() is FaintedCondition faintedCondition)
                    {
                        this.operationManager.AddOperationsContainer(new OperationsContainer(faintedCondition));

                        yield return null;

                        while (!faintedCondition.IsOperationDone())
                            yield return null;
                    }

                    while (!this.operationManager.GetDone())
                        yield return null;
                }

                #endregion
            }

            if (this.battleManager.CheckTeamDefeated(true) ||
                this.battleManager.CheckTeamDefeated(false))
            {
                this.battleManager.SetState(new RoundDoneState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));

                yield break;
            }

            this.battleManager.SetState(new AfterConditionState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
        }
    }
}