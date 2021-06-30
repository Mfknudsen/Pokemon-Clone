#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Items;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using Mfknudsen.Trainer;
using TMPro;
using UnityEngine;

namespace Mfknudsen.Battle.UI
{

    #endregion

    #region Enums
    public enum SelectorGoal { Switch, Item }
    #endregion

    public class SelectionMenu : MonoBehaviour
    {
        #region Values
        [SerializeField] private TextMeshProUGUI[] fields = new TextMeshProUGUI[6];
        [SerializeField] private Team team = null;
        [SerializeField] private SelectorGoal goal = 0;
        [SerializeField] private Item item = null;
        #endregion

        #region Setters
        public void SetItem(Item set)
        {
            item = set;
        }
        #endregion

        #region In
        public void SetFieldNames(Team tSet, SelectorGoal gSet)
        {
            team = tSet;
            goal = gSet;

            for (int i = 0; i < 6; i++)
            {
                Pokemon pokemon = team.GetPokemonByIndex(i);

                if (pokemon != null)
                    fields[i].text = pokemon.GetName() + " (" + pokemon.GetLevel() + ")";
                else
                    fields[i].text = "Empty";
            }
        }

        public void TrySend(int i)
        {
            Pokemon p = team.GetPokemonByIndex(i);
            if (p == null)
                return;
            if (goal == SelectorGoal.Switch)
            {
                if (p.GetInBattle() || p.GetGettingSwitched())
                    return;
                Condition c = p.GetConditionOversight().GetNonVolatileStatus();
                if (c != null)
                {
                    if (c.GetConditionName() == NonVolatile.Fainted.ToString())
                        return;
                }

                BattleMaster.instance.SelectNewPokemon(i);
            }
            else if (goal == SelectorGoal.Item)
            {
                if (item.IsUsableTarget(p))
                    BattleMaster.instance.ParseTargetToItemSelector(p);
            }
        }
        #endregion
    }
}