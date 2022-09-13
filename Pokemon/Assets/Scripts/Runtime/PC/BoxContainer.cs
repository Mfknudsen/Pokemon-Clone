#region SDK

using System.Collections.Generic;
using Runtime.Pokémon;
using TMPro;
using UnityEngine;

//Custom

#endregion

namespace Runtime.PC
{
    public class BoxContainer : MonoBehaviour
    {
        #region Values
        [SerializeField] string boxName = "";
        [SerializeField] Pokemon[] list = new Pokemon[0];

        //GUI
        [SerializeField]
        private TextMeshProUGUI numberGUI;
        private List<BoxSpot> spots = new();

        private void OnValidate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                BoxSpot spot = transform.GetChild(i).GetComponent<BoxSpot>();

                if (spot != null)
                    spots.Add(spot);
            }
        }
        #endregion

        public void Setup(int number, Pokemon[] list)
        {
            numberGUI.text = boxName;
            for (int i = 0; i < spots.Count; i++)
            {
                spots[i].SetPokemon(list[i]);
            }
        }
    }
}
