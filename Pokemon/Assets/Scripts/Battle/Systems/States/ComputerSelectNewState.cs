#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.AI;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ComputerSelectNewState : State
    {
        private readonly List<SwitchAction> switchActions;

        public ComputerSelectNewState(BattleMaster master, List<SwitchAction> switchActions) : base(master)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            BattleMember playerBattleMember = PlayerManager.instance.GetBattleMember();

            foreach (Spot spot in master.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (!(spot.GetActivePokemon() is null) ||
                      battleMember == playerBattleMember ||
                      !battleMember.GetTeam().CanSendMorePokemon()) continue;

                #region Send Information

                BattleAI ai = battleMember.GetBattleAI();

                LocalMemories local = ai.GetLocalMemories();

                local.currentPokemon = null;
                local.currentSpot = spot;
                local.switchInNew = true;

                ai.SetLocalMemories(local);

                if (ai.GetRememberEnemies())
                {
                    EnemiesMemories enemies = new EnemiesMemories()
                    {
                    };

                    ai.SetEnemiesMemories(enemies);
                }

                if (ai.GetRememberAllies())
                {
                    AlliesMemories allies = new AlliesMemories()
                    {
                    };

                    ai.SetAlliesMemories(allies);
                }

                #endregion

                battleMember.ActivateAIBrain();
            }

            master.SetState(new SwitchNewInState(master, switchActions));

            yield break;
        }
    }
}