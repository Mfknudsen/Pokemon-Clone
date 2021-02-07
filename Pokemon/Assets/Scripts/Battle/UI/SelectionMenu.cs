#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Custom
using Trainer;
#endregion 

public class SelectionMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] fields = new TextMeshProUGUI[6];
    [SerializeField] private Team team = null;

    public void SetFieldNames(Team set)
    {
        team = set;

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
}
