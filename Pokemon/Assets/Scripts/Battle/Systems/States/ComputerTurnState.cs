#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.AI;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class ComputerTurnState : State
    {
        public ComputerTurnState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            SpotOversight spotOversight = master.GetSpotOversight();

            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot is null)
                    continue;

                BattleMember battleMember = spot.GetBattleMember();
                Pokemon pokemon = spot.GetActivePokemon();

                if (battleMember is null || pokemon is null || battleMember.IsPlayer())
                    continue;

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
            
            master.SetState(new ActionState(master));
        }
    }
}