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

            this.pokemonSelection.Setup();
            this.itemSelection.Setup();
            this.targetSelection.Setup();
            this.turnSelection.Setup();
        }

        #endregion
        
        #region In

        public void DisplaySelection(SelectorGoal goal, object parse)
        {
            this.DisableDisplaySelection();

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (goal)
            {
                case SelectorGoal.Switch:
                    this.pokemonSelection.DisplaySelection((SwitchAction) parse);
                    break;

                case SelectorGoal.Item:
                    this.itemSelection.DisplaySelection((ItemAction) parse);
                    break;

                case SelectorGoal.Target:
                    this.targetSelection.DisplaySelection((BattleAction) parse);
                    break;

                case SelectorGoal.Turn:
                    this.turnSelection.DisplaySelection((Pokemon) parse);
                    break;
            }
        }

        public void DisableDisplaySelection()
        {
            this.pokemonSelection.DisableDisplaySelection();
            this.itemSelection.DisableDisplaySelection();
            this.targetSelection.DisableDisplaySelection();
            this.turnSelection.DisableDisplaySelection();
        }

        #endregion
    }
}