#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Spots;
using UnityEngine;

#endregion

namespace Runtime.Battle.UI.Information_Display
{
    public class DisplayManager : MonoBehaviour
    {
        #region Values

        [SerializeField] private BattleManager battleManager;
        [SerializeField] private List<PokemonDisplaySlot> allyDisplays, enemyDisplays;
        [SerializeField] private Transform allyOut, enemyOut;

        private readonly Vector3[] allyPositions = new Vector3[3];
        private SpotOversight spotOversight;

        private bool ready;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            int i = 2;
            foreach (PokemonDisplaySlot pokemonDisplay in this.allyDisplays)
            {
                this.allyPositions[i] = pokemonDisplay.transform.position;
                i--;

                pokemonDisplay.Setup();
                pokemonDisplay.transform.position = this.allyOut.position;
            }

            foreach (PokemonDisplaySlot pokemonDisplay in this.enemyDisplays)
            {
                pokemonDisplay.Setup();
                pokemonDisplay.transform.position = this.enemyOut.position;
            }

            this. battleManager.SetDisplayManager(this);
            this. spotOversight = battleManager.GetSpotOversight();

            this. ready = true;

            yield break;
        }

        private void Update()
        {
            if (!this.ready) return;

            Animate(this.allyDisplays, this.allyOut.position);
            Animate(this.enemyDisplays, this.enemyOut.position);
        }

        #endregion

        #region In

        public void UpdateSlots()
        {
            int allyIndex = 0, enemyIndex = 0, allyOffset = 0;
            allyOffset += this.spotOversight.GetSpots()
                .Where(spot => spot.GetIsAlly())
                .Count(spot => spot.GetActivePokemon() is not null);

            //From Count to Index Value
            allyOffset--;

            foreach (Spot spot in this.spotOversight.GetSpots())
            {
                if (spot.GetIsAlly())
                {
                    PokemonDisplaySlot slot = this.allyDisplays[allyIndex];

                    slot.SetPokemon(spot.GetActivePokemon());

                    if (allyOffset - allyIndex >= 0 && allyOffset - allyIndex < this.allyPositions.Length)
                        slot.SetOriginPosition(this.allyPositions[allyOffset - allyIndex]);

                    allyIndex++;
                }
                else
                {
                    PokemonDisplaySlot slot = this.enemyDisplays[enemyIndex];

                    slot.SetPokemon(spot.GetActivePokemon());

                    enemyIndex++;
                }
            }
        }

        #endregion

        #region Internal

        private static void Animate(List<PokemonDisplaySlot> list, Vector3 outPos)
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