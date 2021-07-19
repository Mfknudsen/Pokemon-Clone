#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.UI;
using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Actions.Switch
{
    [CreateAssetMenu(fileName = "SwitchAction", menuName = "Action/Create new Switch Action")]
    public class SwitchAction : BattleAction
    {
        #region Values
        [Header("Switch Specific Values:")]
        [SerializeField] private Trainer.Team team;
        [SerializeField] private PokemonDisplaySlot displaySlot;
        [SerializeField] private Pokemon nextPokemon;
        [SerializeField] private Chat[] nextChat;
        [SerializeField] private Spot spot;
        #endregion

        #region Getters
        public Pokemon GetNextPokemon()
        {
            return nextPokemon;
        }
        #endregion

        #region Setters
        public void SetDisplay(PokemonDisplaySlot input)
        {
            displaySlot = input;
        }

        public void SetNextPokemon(Pokemon next)
        {
            nextPokemon = next;
            
            next.SetGettingSwitched(true);
        }

        public void SetTeam(Trainer.Team set)
        {
            team = set;
        }

        public void SetSpot(Spot set)
        {
            spot = set;
        }
        #endregion

        #region Overrides
        public override IEnumerator Activate()
        {
            return Operation();
        }

        protected override IEnumerator Operation()
        {
            done = false;
            List<Chat> toSend = new List<Chat>();

            //Start of match there will be no current pokemon
            if (currentPokemon != null)
            {
                //Switch team member places
                team.SwitchTeamPlaces(currentPokemon, nextPokemon);

                currentPokemon.DespawnPokemon();

                if (currentPokemon.GetSpawnedObject() != null)
                {
                    for (int i = 0; i < chatOnActivation.Length; i++)
                    {
                        Chat c = Instantiate(chatOnActivation[i]);
                        c.AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                        toSend.Add(c);
                    }
                    ChatMaster.instance.Add(toSend.ToArray());

                    GameObject obj = currentPokemon.GetSpawnedObject();

                    while (!ChatMaster.instance.GetIsClear() && obj.transform.localScale.magnitude > 0.01f)
                    {
                        obj.transform.localScale += -Vector3.one * Time.deltaTime;
                        yield return null;
                    }
                }
            }

            //Out text + Spawn new Pokemon
            toSend.Clear();
            for (int i = 0; i < nextChat.Length; i++)
            {
                Chat c = Instantiate(nextChat[i]);
                c.AddToOverride("<NEXT_POKEMON>", nextPokemon.GetName());
                toSend.Add(c);
            }
            ChatMaster.instance.Add(toSend.ToArray());

            BattleMaster.instance.SpawnPokemon(nextPokemon, spot);
            Transform inTrans = nextPokemon.GetSpawnedObject().transform;
            inTrans.localScale = Vector3.one * 0.1f;

            while (inTrans.localScale.y < 1)
            {
                inTrans.transform.localScale += Vector3.one * Time.deltaTime;
                if (inTrans.localScale.y > 1)
                    inTrans.localScale = Vector3.one;
                yield return null;
            }

            yield return new WaitForSeconds(1);

            nextPokemon.Setup();
            
            done = true;
        }
        #endregion
    }
}