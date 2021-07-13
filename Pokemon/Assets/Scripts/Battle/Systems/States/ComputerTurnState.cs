using System.Collections;
using System.Collections.Generic;
using Mfknudsen.AI;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Pokémon;
using UnityEngine;

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

                LocalMemories local = new LocalMemories
                {
                    currentPokemon = pokemon,
                    currentSpot = spot
                };

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

                while (pokemon.GetBattleAction() == null)
                    yield return 0;
            }

            // ReSharper disable once IdentifierTypo
            List<Pokemon> pokemonsWithAction = new List<Pokemon>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot.GetActivePokemon().GetBattleAction() is null) continue;

                pokemonsWithAction.Add(spot.GetActivePokemon());
            }

            master.SetState(new ActionState(master, pokemonsWithAction));
        }
    }
}