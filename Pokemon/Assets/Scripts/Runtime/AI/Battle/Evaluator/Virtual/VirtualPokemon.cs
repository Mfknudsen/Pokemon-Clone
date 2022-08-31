using Runtime.Pokémon;
using Object = UnityEngine.Object;
using Type = System.Type;

namespace Runtime.AI.Battle.Evaluator.Virtual
{
    public class VirtualPokemon
    {
        private readonly Pokemon pokemon;
        private readonly Pokemon fakePokemon;
        private bool isKnown;
        public bool isAlly;


        public VirtualPokemon(Pokemon pokemon)
        {
            this.pokemon = pokemon;
            fakePokemon = Object.Instantiate(pokemon);
            fakePokemon.ResetForAIMemory();
            isKnown = false;
        }

        #region Getters

        public Pokemon GetFakePokemon()
        {
            return fakePokemon;
        }

        public Pokemon GetActualPokemon()
        {
            return pokemon;
        }

        public bool GetKnown()
        {
            return isKnown;
        }

        #endregion

        #region In

        public void OnEnterCheck(Pokemon toCheck)
        {
            if (pokemon != toCheck)
                return;

            isKnown = true;
        }

        public void OnAbilityTrigger(Pokemon toCheck, Ability ability)
        {
            if (pokemon != toCheck)
                return;

            try
            {
                Ability[] abilities = toCheck.GetAbilities();
                Type type = ability.GetType();
                if (abilities[0].GetType() == type)
                    fakePokemon.SetFirstAbility(ability);
                else if (abilities[1].GetType() == type)
                    fakePokemon.SetSecondAbility(ability);
                else if (abilities[2].GetType() == type)
                    fakePokemon.SetHiddenAbility(ability);
            }
            catch
            {
                //Ignore
            }
        }

        #endregion
    }
}