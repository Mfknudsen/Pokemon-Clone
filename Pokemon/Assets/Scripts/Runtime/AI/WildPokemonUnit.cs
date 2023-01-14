#region Packages

using Runtime.Pokémon;
using Sirenix.OdinInspector;

#endregion

namespace Runtime.AI
{
    public class WildPokemonUnit : UnitBase
    {
        #region Values

        [ShowInInspector, ReadOnly] private Pokemon pokemon;

        #endregion

        #region Getters

        public Pokemon GetPokemon() => this.pokemon;

        #endregion

        #region In

        public override void InteractTrigger()
        {
        }

        public void Setup(Pokemon pokemon)
        {
            this.pokemon = pokemon;
        }

        #endregion
    }
}