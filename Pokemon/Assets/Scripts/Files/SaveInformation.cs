using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class SaveInformation
{
    //Player Character
    public int badgeCount = 0;
    public string playerName = "";

    //NPCs
    public List<NPCSave> NPCSaves = new List<NPCSave>();

    //Pokemons
    public List<PokemonSave> partyPokemons = new List<PokemonSave>();
    public List<PokemonSave> boxPokemons = new List<PokemonSave>();
}