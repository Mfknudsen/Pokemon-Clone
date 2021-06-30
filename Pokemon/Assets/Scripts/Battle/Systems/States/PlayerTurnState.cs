using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using Mfknudsen.Player;

namespace Mfknudsen.Battle.Systems.States
{
    public class PlayerTurnState : State
    {
        public PlayerTurnState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            List<Pokemon> playerPokemon = new List<Pokemon>();
            Team team = MasterPlayer.instance.GetTeam();
            SpotOversight spotOversight = master.GetSpotOversight();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Spot spot in spotOversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();
                
                if (pokemon == null) continue;

                if (team.PartOfTeam(pokemon))
                    playerPokemon.Add(pokemon);
            }
            
            while (playerPokemon.Count > 0)
            {
                Pokemon pokemon = playerPokemon[0];

                master.DisplayMoves(pokemon);
                
                while (pokemon.GetBattleAction() == null)
                    yield return 0;

                playerPokemon.RemoveAt(0);
            }

            master.DisableMovesDisplay();
            
            master.SetState(new ComputerTurnState(master));
        }
    }
}