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

    public void SetFieldNames(Team team)
    {
        for (int i = 0; i < 6; i++)
        {
            fields[i].text = "";
            Pokemon pokemon = team.GetPokemonByIndex(i);
            if (pokemon != null)
                fields[i].text = pokemon.GetName();  
        }
    }
}
