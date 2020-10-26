using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enums
public enum PokemonState { EnterBattle, Idle, Attacking }
#endregion

public class Pokemon : MonoBehaviour, IPokemon
{
    #region Values:
    [Header("Object Reference:")]
    public PokemonState state;

    [Header(" - Stats:")]
    [SerializeField] protected float health;
    [SerializeField] protected float defence, specialDefence;
    [SerializeField] protected float attack, specialAttack;
    #endregion

    public void UpdatePokemon()
    {
        switch (state)
        {
            case PokemonState.Idle:
                break;

            case PokemonState.Attacking:
                break;
        }
    }
}
