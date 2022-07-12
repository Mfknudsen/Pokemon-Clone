#region Packages

using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.Sources
{
    public abstract class Source : MonoBehaviour
    {
        public abstract void Consume(Pokemon pokemon);
    }
}