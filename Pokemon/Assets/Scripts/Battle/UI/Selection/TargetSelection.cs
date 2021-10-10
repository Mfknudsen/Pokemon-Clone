#region SDK

using System.Collections.Generic;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using UnityEngine;
using UnityEngine.UI;

#endregion

// ReSharper disable PossibleNullReferenceException
// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
{
    public class TargetSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private GameObject background;
        [SerializeField] private TargetSlot[] enemies, allies;
        private SpotOversight oversight;
        private int playerTeamNumber;
        private BattleAction action;

        #endregion

        #region In

        public void Setup()
        {
            oversight = BattleManager.instance.GetSpotOversight();
            playerTeamNumber = PlayerManager.instance.GetBattleMember().GetTeamNumber();
        }

        public void DisplaySelection(BattleAction action)
        {
            this.action = action;

            background.SetActive(true);

            Spot currentSpot = null;

            List<Spot> eSpots = new List<Spot>(), aSpots = new List<Spot>();

            foreach (Spot spot in oversight.GetSpots())
            {
                if (spot.GetActivePokemon() == action.GetCurrentPokemon())
                    currentSpot = spot;

                if (spot.GetTeamNumber() == playerTeamNumber)
                    aSpots.Add(spot);
                else
                    eSpots.Add(spot);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                if (i < eSpots.Count)
                    enemies[i].SetPokemon(this, eSpots[i]);
                else
                    enemies[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < allies.Length; i++)
            {
                if (i < aSpots.Count)
                    allies[i].SetPokemon(this, aSpots[i]);
                else
                    allies[i].gameObject.SetActive(false);
            }

            if (action is PokemonMove pokemonMove)
            {
                // ReSharper disable once IdentifierTypo
                bool[] targetable = pokemonMove.GetTargetable();
                bool selfTarget = targetable[5], allyClose = targetable[3], allyLong = targetable[4];
                bool enemyFront = targetable[0], enemyStrafe = targetable[1], enemyLong = targetable[2];

                foreach (TargetSlot targetSlot in enemies)
                {
                    Button button = targetSlot.gameObject.GetComponent<Button>();
                    Spot targetSpot = targetSlot.GetSpot();

                    if (targetSpot == currentSpot.GetFront())
                        button.enabled = enemyFront;
                    else if (targetSpot == currentSpot.GetStrafeLeft() || targetSpot == currentSpot.GetStrafeRight())
                        button.enabled = enemyStrafe;
                    else
                        button.enabled = enemyLong;
                }

                foreach (TargetSlot targetSlot in allies)
                {
                    Button button = targetSlot.gameObject.GetComponent<Button>();
                    Spot targetSpot = targetSlot.GetSpot();

                    if (targetSpot == currentSpot)
                        button.enabled = selfTarget;
                    else if (targetSpot == currentSpot.GetLeft() || targetSpot == currentSpot.GetRight())
                        button.enabled = allyClose;
                    else
                        button.enabled = allyLong;
                }
            }
        }

        public void DisableDisplaySelection()
        {
            background.SetActive(false);
        }

        public void ReceiveSpot(Spot spot)
        {
            Pokemon pokemon = action.GetCurrentPokemon();

            if (pokemon is null) return;

            action.SetTargets(spot.GetActivePokemon());

            pokemon.SetBattleAction(action);
        }

        #endregion
    }
}