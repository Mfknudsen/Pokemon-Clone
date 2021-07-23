#region SDK

using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Item;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Mfknudsen.Battle.UI.Selection
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
            spotOversight = BattleMaster.instance.GetSpotOversight();
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
            {
                selectionMenu.DisplaySelection(SelectorGoal.Target, battleAction);
            }
        }

        #region Side Bottuns

        public void SwitchButton()
        {
            SwitchAction action = BattleMaster.instance.InstantiateSwitchAction();

            action.SetCurrentPokemon(pokemon);

            selectionMenu.DisplaySelection(SelectorGoal.Switch, action);
        }

        public void ItemButton()
        {
            ItemAction action = BattleMaster.instance.InstantiateItemAction();

            action.SetCurrentPokemon(pokemon);

            selectionMenu.DisplaySelection(SelectorGoal.Item, action);
        }

        public void RunButton()
        {
            Debug.Log("Run Not Implementet");
        }

        #endregion

        #endregion
    }
}