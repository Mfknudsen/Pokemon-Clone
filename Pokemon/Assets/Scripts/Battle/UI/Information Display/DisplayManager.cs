#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;
using Mfknudsen.Settings.Manager;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Information_Display
{
    public class DisplayManager : Manager
    {
        #region Values

        [SerializeField] private List<PokemonDisplaySlot> allyDisplays, enemyDisplays;
        [SerializeField] private Transform allyOut, enemyOut;

        private readonly Vector3[] allyPositions = new Vector3[3];
        private bool ready;
        private bool isAlly;
        private SpotOversight spotOversight;

        #endregion

        #region Build In States

        private void Update()
        {
            if (!ready) return;

            Animate(allyDisplays, allyOut.position);
            Animate(enemyDisplays, enemyOut.position);
        }

        #endregion

        #region In

        public override void Setup()
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

            spotOversight = BattleManager.instance.GetSpotOversight();
            isAlly = PlayerManager.instance.GetBattleMember().GetTeamAffiliation();

            ready = true;
        }

        public void UpdateSlots()
        {
            int allyIndex = 0, enemyIndex = 0, allyOffset = 0;

            allyOffset += spotOversight.GetSpots()
                .Count(spot => spot.GetIsAlly() == isAlly && !(spot.GetActivePokemon() is null));

            //From Count to Index Value
            allyOffset--;

            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot.GetIsAlly() == isAlly)
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

        #endregion

        #region Internal

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

        #endregion
    }
}