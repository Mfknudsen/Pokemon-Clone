#region Packages

using System;
using System.Collections.Generic;
using Mfknudsen.Files.Save_Types;

#endregion

namespace Mfknudsen.Files
{
    [Serializable]
    public class SaveInformation
    {
        //Player Character
        public int badgeCount;
        public string playerName = "";

        //NPCs
        public List<NPCSave> npcSaves = new();

        //Pokemons
        public List<PokemonSave> partyPokemons = new();
        public List<PokemonSave> boxPokemons = new();
    }
}