#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using BattleUI;
#endregion

/// <summary>
/// Switch out text
/// Despawn active pokemon
/// Out text + Spawn new pokemon
/// End
/// </summary>

[CreateAssetMenu(fileName = "SwitchAction", menuName = "Create new Switch Action")]
public class SwitchAction : BattleAction
{
    #region Values
    [SerializeField] private int fieldSpot = -1;
    [SerializeField] private PokemonDisplay display = null;
    [SerializeField] private Pokemon nextPokemon = null;
    #endregion

    #region Getters
    public void SetDisplay(PokemonDisplay input)
    {
        display = input;
    }
    #endregion

    #region Setters
    public void SetNextPokemon(Pokemon next)
    {
        nextPokemon = next.GetPokemon();
    }
    #endregion

    #region Overrides
    public override BattleAction GetAction()
    {
        return this;
    }

    public override IEnumerator Activate()
    {
        return Operation();
    }

    protected override IEnumerator Operation()
    {
        //Switch out text
        if (!active)
        {
            Chat[] toSend = new Chat[chatOnActivation.Length];

            for (int i = 0; i < toSend.Length; i++)
            {
                Chat c = chatOnActivation[i].GetChat();
                c.AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                c.AddToOverride("<NEXT_NAME>", nextPokemon.GetName());
                toSend[i] = c;
            }
            ChatMaster.instance.Add(toSend);
        }

        //Debug.Log("First " + ChatMaster.instance.GetIsClear());

        while (!ChatMaster.instance.GetIsClear())
            yield return null;
        //Debug.Log("Second " + ChatMaster.instance.GetIsClear());

        //Despawn Current
        yield return new WaitForSeconds(1);
        //Out text + Spawn new Pokemon

        //End

        done = true;
    }
    #endregion
}
