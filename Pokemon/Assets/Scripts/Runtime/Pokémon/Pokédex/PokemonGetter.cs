#region Libraries

using Runtime.Pokémon;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#endregion

[Serializable]
public sealed class PokemonGetter
{
    #region Values

#if UNITY_EDITOR
    [SerializeField, ValueDropdown(nameof(PokemonOptions), FlattenTreeView = true), OnValueChanged(nameof(OnPokemonOptionChanged))] private string pokemonName = "Grottle";

    private IEnumerable<string> PokemonOptions() =>
        new DirectoryInfo(Application.dataPath + "/ScriptableObjects/Pokedex/").GetFiles().Select(f => f.Name).Where(s => !s.Contains(".meta"));

    private void OnPokemonOptionChanged()
    {
        Debug.Log(this.pokemonName.Contains(".asset"));
        if (this.pokemonName.Contains(".asset"))
        {
            this.path = "Assets/ScriptableObjects/Pokedex/" + this.pokemonName;
            Debug.Log(this.path);

            this.pokemonName = this.pokemonName.Replace(".asset", "");
        }
    }
#endif

    [SerializeField, HideInInspector] private string path = "Assets/ScriptableObjects/Pokedex/Grottle.asset";

    #endregion

    #region Out 

    public Pokemon Get => Pokedex.Get(this.path);

    #endregion
}