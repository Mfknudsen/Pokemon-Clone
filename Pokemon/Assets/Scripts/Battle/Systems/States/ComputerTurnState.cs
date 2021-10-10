#region SDK

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Pokémon;
using UnityEngine;

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

                BattleAI ai = battleMember.GetBattleAI();

                LocalMemories local = ai.GetLocalMemories();
                local.currentPokemon = pokemon;
                local.currentSpot = spot;
                local.switchInNew = false;

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

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            manager.SetState(new ActionState(manager));
        }
    }
}