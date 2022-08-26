#region Packages

using System.Collections.Generic;
using Mfknudsen.Files;
using Mfknudsen.Pokémon;
using Mfknudsen.World.Overworld.Interactions;
using UnityEngine;

#endregion

namespace Mfknudsen.PC
{
    [RequireComponent(typeof(SphereCollider))]
    [AddComponentMenu("Overworld/Interactions")]
    public class Box : MonoBehaviour, IInteractable
    {
        #region Values

        private static List<Pokemon> pokemonsInBox = new();
        private string fileName = "PokemonBox";

        #endregion

        #region In

        #region Load/Save

        public void Load()
        {
            pokemonsInBox.Clear();
            pokemonsInBox.AddRange(FileManager.LoadData<Pokemon[]>(fileName));
        }

        public void Save()
        {
            FileManager.SaveData(fileName, pokemonsInBox);
        }

        #endregion

        public void ShowNextBox()
        {
        }

        public void ShowPreviousBox()
        {
        }

        #endregion

        public void Trigger()
        {
            Debug.Log("Trigger Box");
            //UIManager.instance.SwitchUI(UISelection.Box);
        }
    }
}