using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Category { Physical, Special, Status, Extents }
public enum Contest { None, Tough, Cute, Clever, Beutiful, Cool }

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new Move", order = 1)]
public class PokemonMove : BattleAction
{
    [SerializeField] protected int index = 0;
    [SerializeField] protected string moveName = "";
    [SerializeField] protected Type type = null;
    [SerializeField] protected Category category = 0;
    [SerializeField] protected Contest contest = 0;
    [SerializeField] protected int maxPP = 0;
    protected int currentPP = 0;
    [SerializeField] protected int power = 0, accuracy = 0;

    public override void Activate()
    {
        if (targetPokemon.Length > 0 && currentPokemon != null) {
            if (category == Category.Physical || category == Category.Special)
            {
                float attack = currentPokemon.GetSpecialAttack();

                if (category == Category.Physical)
                    attack = currentPokemon.GetAttack();

                foreach (Pokemon p in targetPokemon)
                {
                    float defence = p.GetSpecialDefence();

                    if (category == Category.Physical)
                        defence = p.GetDefence();

                    int damage = BattleMathf.CalculateDamage(
                        currentPokemon.GetLevel(),
                        attack,
                        defence,
                        power,
                        BattleMathf.CalculateModifiers(
                            currentPokemon, p, type.GetTypeName(), (targetPokemon.Length == 1)));

                    p.RecieveDamage(damage);
                }
            }
        }
    }

    #region Setters
    public void SetCurrentPokemon(Pokemon pokemon)
    {
        currentPokemon = pokemon;
    }

    public void SetTargetPokemon(Pokemon[] targets)
    {
        targetPokemon = targets;
    }
    #endregion

    #region Getters

    public string GetName()
    {
        return moveName;
    }
    #endregion
}
