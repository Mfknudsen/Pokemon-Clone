#region SDK

using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using TMPro;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Selection
{
    public class TargetSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private TargetSlot[] enemies, allies;
        private SpotOversight oversight;
        private int playerTeamNumber;

        #endregion

        #region In

        public void Setup()
        {
            oversight = BattleMaster.instance.GetSpotOversight();
            playerTeamNumber = MasterPlayer.instance.GetBattleMember().GetTeamNumber();
        }

        public void DisplaySelection()
        {
            List<Spot> eSpots = new List<Spot>(), aSpots = new List<Spot>();

            foreach (Spot spot in oversight.GetSpots())
            {
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

            float aY = allies[0].transform.position.y, eY = enemies[0].transform.position.y;
            
            switch (aSpots.Count)
            {
                case 1:
                    enemies[0].transform.position = new Vector3(800, aY, 0);
                    break;

                case 2:
                    enemies[0].transform.position = new Vector3(525, aY, 0);
                    enemies[1].transform.position = new Vector3(1075, aY, 0);
                    break;

                case 3:
                    enemies[0].transform.position = new Vector3(250, aY, 0);
                    enemies[1].transform.position = new Vector3(800, aY, 0);
                    enemies[2].transform.position = new Vector3(1350, aY, 0);
                    break;
            }

            switch (eSpots.Count)
            {
                case 1:
                    enemies[0].transform.position = new Vector3(800, eY, 0);
                    break;

                case 2:
                    enemies[0].transform.position = new Vector3(525, eY, 0);
                    enemies[1].transform.position = new Vector3(1075, eY, 0);
                    break;

                case 3:
                    enemies[0].transform.position = new Vector3(250, eY, 0);
                    enemies[1].transform.position = new Vector3(800, eY, 0);
                    enemies[2].transform.position = new Vector3(1350, eY, 0);
                    break;
            }
        }

        public void DisableDisplaySelection()
        {
        }

        public void ReceiveSpot(Spot spot)
        {
        }

        #endregion

        #region Out

        #endregion
    }
}