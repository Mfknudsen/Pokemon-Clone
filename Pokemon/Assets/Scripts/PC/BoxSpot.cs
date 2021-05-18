#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Custom
using Monster;
#endregion

public class BoxSpot : MonoBehaviour
{
    private Pokemon onSpot = null;
    private Image visual = null;

    public void SetPokemon(Pokemon set)
    {
        onSpot = set;

        visual.sprite = null;
    }
}