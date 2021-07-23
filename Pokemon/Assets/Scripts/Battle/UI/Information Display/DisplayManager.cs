#region SDK

using System;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Information_Display
{
    public class DisplayManager : MonoBehaviour
    {
        [SerializeField] private List<PokemonDisplaySlot> allyDisplays, enemyDisplays;
        [SerializeField] private Transform allyOut, enemyOut;

        private readonly Vector3[] allyPositions = new Vector3[3];
        private bool ready;
        private int playerTeamNumber;
        private SpotOversight spotOversight;

        private void Start()
        {
            int i = 2;
            foreach (PokemonDisplaySlot pokemonDisplay in allyDisplays)
            {
                allyPositions[i] = pokemonDisplay.transform.position;
                i--;

                pokemonDisplay.Setup();
                pokemonDisplay.transform.position = allyOut.position;
            }

            foreach (PokemonDisplaySlot pokemonDisplay in enemyDisplays)
            {
                pokemonDisplay.Setup();
                pokemonDisplay.transform.position = enemyOut.position;
            }
        }

        public void Setup()
        {
            spotOversight = BattleMaster.instance.GetSpotOversight();
            playerTeamNumber = MasterPlayer.instance.GetBattleMember().GetTeamNumber();

            ready = true;
        }

        private void Update()
        {
            if (!ready) return;

            Animate(allyDisplays, allyOut.position);

            Animate(enemyDisplays, enemyOut.position);
        }

        private void Animate(List<PokemonDisplaySlot> list, Vector3 outPos)
        {
            foreach (PokemonDisplaySlot pokemonDisplay in list)
            {
                if (!pokemonDisplay.GetActive())
                {
                    pokemonDisplay.transform.position =
                        Vector3.Lerp(pokemonDisplay.transform.position, outPos, 1.5f * Time.deltaTime);
                }
                else
                {
                    pokemonDisplay.transform.position = Vector3.Lerp(pokemonDisplay.transform.position,
                        pokemonDisplay.GetOriginPosition(), 1.5f * Time.deltaTime);

                    if (Vector3.Distance(pokemonDisplay.transform.position, pokemonDisplay.GetOriginPosition()) < 0.1f)
                        pokemonDisplay.transform.position = pokemonDisplay.GetOriginPosition();
                }
            }
        }

        public void UpdateSlots()
        {
            int allyIndex = 0, enemyIndex = 0, allyOffset = 0;

            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot.GetTeamNumber() == playerTeamNumber && !(spot.GetActivePokemon() is null))
                    allyOffset++;
            }

            //From Count to Index Value
            allyOffset--;

            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot.GetTeamNumber() == playerTeamNumber)
                {
                    PokemonDisplaySlot slot = allyDisplays[allyIndex];

                    slot.SetPokemon(spot.GetActivePokemon());

                    if (allyOffset - allyIndex >= 0 && allyOffset - allyIndex < allyPositions.Length)
                        slot.SetOriginPosition(allyPositions[allyOffset - allyIndex]);

                    allyIndex++;
                }
                else
                {
                    PokemonDisplaySlot slot = enemyDisplays[enemyIndex];

                    slot.SetPokemon(spot.GetActivePokemon());

                    enemyIndex++;
                }
            }
        }
    }
}