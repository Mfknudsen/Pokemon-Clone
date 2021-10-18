#region Packages

using System.Collections.Generic;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Virtual
{
    public class VirtualSpotOversight
    {
        public List<VirtualSpot> spots;

        public VirtualSpotOversight()
        {
            spots = new List<VirtualSpot>();
        }
    }

    public class VirtualSpot
    {
        public readonly VirtualPokemon virtualPokemon;

        public VirtualSpot(Pokemon pokemon)
        {
            virtualPokemon = new VirtualPokemon(pokemon);
        }

        public VirtualSpot(VirtualPokemon virtualPokemon)
        {
            this.virtualPokemon = new VirtualPokemon(virtualPokemon.GetFakePokemon());
        }
    }
}