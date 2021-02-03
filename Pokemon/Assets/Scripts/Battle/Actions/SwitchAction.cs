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
    [Header("Switch Specific Values:")]
    [SerializeField] private Trainer.Team team = null;
    [SerializeField] private int fieldSpot = -1;
    [SerializeField] private PokemonDisplay display = null;
    [SerializeField] private Pokemon nextPokemon = null;
    [SerializeField] private Chat[] nextChat = new Chat[0];
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

    public void SetFieldSpot(int set)
    {
        fieldSpot = set;
    }

    public void SetTeam(Trainer.Team set)
    {
        team = set;
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
        Debug.Log("Starting Switch Action\n" + currentPokemon.GetName()+"\n"+nextPokemon.GetName());

        done = false;
        Chat[] toSend = new Chat[0];

        //Switch team member places
        team.SwitchTeamPlaces(currentPokemon, nextPokemon);

        //Start of match there will be no current pokemon
        if (currentPokemon != null)
        {
            //Switch out text
            toSend = new Chat[chatOnActivation.Length];

            for (int i = 0; i < toSend.Length; i++)
            {
                Chat c = chatOnActivation[i].GetChat();
                c.AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                toSend[i] = c;
            }
            ChatMaster.instance.Add(toSend);

            GameObject obj = currentPokemon.GetSpawnedObject();

            while (!ChatMaster.instance.GetIsClear() && obj != null)
            {
                if (obj.transform.localScale.y > 0.01f)
                    obj.transform.localScale += -Vector3.one * Time.deltaTime;

                yield return null;
            }

            //Despawn current
            currentPokemon.DespawnPokemon();
        }

        //Out text + Spawn new Pokemon
        toSend = new Chat[nextChat.Length];
        for (int i = 0; i < toSend.Length; i++)
        {
            Chat c = chatOnActivation[i].GetChat();
            c.AddToOverride("<NEXT_NAME>", nextPokemon.GetName());
            toSend[i] = c;
        }
        BattleMaster.instance.SpawnPokemon(nextPokemon, fieldSpot);

        yield return new WaitForSeconds(1);
        //End

        done = true;
    }
    #endregion
}
