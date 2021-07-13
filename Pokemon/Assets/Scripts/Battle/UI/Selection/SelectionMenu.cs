#region SDK

using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using Mfknudsen.Trainer;
using TMPro;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.UI.Selection
{
    #region Enums

    public enum SelectorGoal
    {
        Switch,
        Item,
        Target
    }

    #endregion

    public class SelectionMenu : MonoBehaviour
    {
        #region Values

        [SerializeField] private PokemonSelection pokemonSelection;
        [SerializeField] private ItemSelection itemSelection;
        [SerializeField] private TargetSelection targetSelection;

        #endregion

        #region In

        public void Setup()
        {
            pokemonSelection.Setup();
            itemSelection.Setup();
            targetSelection.Setup();
        }
        
        public void DisplaySelection(SelectorGoal goal)
        {
            DisableDisplaySelection();

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (goal)
            {
                case SelectorGoal.Switch:
                    pokemonSelection.DisplaySelection();
                    break;
                
                case SelectorGoal.Item:
                    itemSelection.DisplaySelection();
                    break;
                
                case SelectorGoal.Target:
                    targetSelection.DisplaySelection();
                    break;
            }
        }

        public void DisableDisplaySelection()
        {
            pokemonSelection.DisableDisplaySelection();
            itemSelection.DisableDisplaySelection();
            targetSelection.DisableDisplaySelection();
        }

        #endregion
    }
}