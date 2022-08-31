#region Packages

using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Spots;
using Runtime.Pokémon;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Battle.UI.Selection
{
    public class TurnSelection : MonoBehaviour
    {
        #region Values

        [SerializeField] private SelectionMenu selectionMenu;
        [SerializeField] private GameObject background;
        [SerializeField] private TurnMoveSlot[] moveSlots;

        private SpotOversight spotOversight;
        private Pokemon pokemon;

        #endregion

        #region In

        public void Setup()
        {
            spotOversight = BattleManager.instance.GetSpotOversight();
        }

        public void DisplaySelection(Pokemon pokemon)
        {
            this.pokemon = pokemon;

            background.SetActive(true);

            for (int i = 0; i < moveSlots.Length; i++)
            {
                PokemonMove move = pokemon.GetMoveByIndex(i);

                moveSlots[i].SetMove(move);
            }
        }

        public void DisableDisplaySelection()
        {
            background.SetActive(false);
        }

        public void ReceiveAction(BattleAction battleAction)
        {
            battleAction.SetCurrentPokemon(pokemon);

            if (spotOversight.GetToDefaultTargeting())
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (Spot spot in spotOversight.GetSpots())
                {
                    bool enemy = spot.GetActivePokemon() != battleAction.GetCurrentPokemon();

                    if (!battleAction.GetDefaultTargetEnemy() && enemy ||
                        battleAction.GetDefaultTargetEnemy() && !enemy) continue;

                    battleAction.SetTargets(spot.GetActivePokemon());

                    pokemon.SetBattleAction(battleAction);

                    break;
                }
            }
            else
                selectionMenu.DisplaySelection(SelectorGoal.Target, battleAction);
        }

        #region Side Bottuns

        public void SwitchButton()
        {
            SwitchAction action = BattleManager.instance.InstantiateSwitchAction();

            action.SetCurrentPokemon(pokemon);

            selectionMenu.DisplaySelection(SelectorGoal.Switch, action);
        }

        public void ItemButton()
        {
            BattleManager manager = BattleManager.instance;
            ItemAction action = manager.InstantiateItemAction();
            foreach (Spot spot in manager.GetSpotOversight().GetSpots()
                .Where(spot => spot.GetActivePokemon() == pokemon))
            {
                action.SetBattleMember(spot.GetBattleMember());
                break;
            }

            action.SetCurrentPokemon(pokemon);

            selectionMenu.DisplaySelection(SelectorGoal.Item, action);
        }

        public void RunButton()
        {
            Debug.Log("Run Not Implemented");
        }

        #endregion

        #endregion
    }
}