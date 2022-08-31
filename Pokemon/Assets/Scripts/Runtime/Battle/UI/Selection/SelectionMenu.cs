#region Packages

using System.Collections;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Pokémon;
using UnityEngine;

#endregion

namespace Runtime.Battle.UI.Selection
{
    #region Enums

    public enum SelectorGoal
    {
        Switch,
        Item,
        Target,
        Turn
    }

    #endregion

    public class SelectionMenu : MonoBehaviour
    {
        #region Values

        [SerializeField] private PokemonSelection pokemonSelection;
        [SerializeField] private ItemSelection itemSelection;
        [SerializeField] private TargetSelection targetSelection;
        [SerializeField] private TurnSelection turnSelection;

        #endregion

        #region Build In States

        private IEnumerator Start()
        {
            yield return new WaitWhile(() => BattleManager.instance == null);
            
            BattleManager.instance.SetSelectionMenu(this);
            
            pokemonSelection.Setup();
            itemSelection.Setup();
            targetSelection.Setup();
            turnSelection.Setup();
        }

        #endregion
        
        #region In

        public void DisplaySelection(SelectorGoal goal, object parse)
        {
            DisableDisplaySelection();

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (goal)
            {
                case SelectorGoal.Switch:
                    pokemonSelection.DisplaySelection((SwitchAction) parse);
                    break;

                case SelectorGoal.Item:
                    itemSelection.DisplaySelection((ItemAction) parse);
                    break;

                case SelectorGoal.Target:
                    targetSelection.DisplaySelection((BattleAction) parse);
                    break;

                case SelectorGoal.Turn:
                    turnSelection.DisplaySelection((Pokemon) parse);
                    break;
            }
        }

        public void DisableDisplaySelection()
        {
            pokemonSelection.DisableDisplaySelection();
            itemSelection.DisableDisplaySelection();
            targetSelection.DisableDisplaySelection();
            turnSelection.DisableDisplaySelection();
        }

        #endregion
    }
}