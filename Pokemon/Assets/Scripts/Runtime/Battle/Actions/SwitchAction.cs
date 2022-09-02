﻿#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.AI.Battle.Evaluator;
using Runtime.AI.Battle.Evaluator.Virtual;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Pokémon;
using UnityEngine;

#endregion

namespace Runtime.Battle.Actions
{
    [CreateAssetMenu(fileName = "SwitchAction", menuName = "Action/Create new Switch Action")]
    public sealed class SwitchAction : BattleAction
    {
        #region Values

        [Header("Switch Specific Values:")] [SerializeField]
        private Trainer.Team team;

        [SerializeField] private Pokemon nextPokemon;
        [SerializeField] private Chat[] nextChat;
        [SerializeField] private Spot spot;

        #endregion

        #region Getters

        public Pokemon GetNextPokemon()
        {
            return nextPokemon;
        }

        public Spot GetSpot()
        {
            return spot;
        }

        #endregion

        #region Setters

        public void SetNextPokemon(Pokemon next)
        {
            if (next == null)
                return;

            nextPokemon = next;

            next.SetGettingSwitched(true);
        }

        public void SetTeam(Trainer.Team set)
        {
            team = set;
        }

        public void SetSpot(Spot set)
        {
            if (set == null)
                return;

            spot = set;
        }

        #endregion

        #region In

        public override IEnumerator Operation()
        {
            done = false;
            List<Chat> toSend = new();

            //Start of match there will be no current pokemon
            if (currentPokemon != null)
            {
                //Switch team member places
                team.SwitchTeamPlaces(currentPokemon, nextPokemon);
                
                if (currentPokemon.GetSpawnedObject() != null)
                {
                    foreach (Chat t in chatOnActivation)
                    {
                        Chat c = Instantiate(t);
                        c.AddToOverride("<POKEMON_NAME>", currentPokemon.GetName());
                        toSend.Add(c);
                    }

                    ChatManager.instance.Add(toSend.ToArray());

                    GameObject obj = currentPokemon.GetSpawnedObject();

                    while (!ChatManager.instance.GetIsClear() && obj.transform.localScale.magnitude > 0.01f)
                    {
                        obj.transform.localScale += -Vector3.one * Time.deltaTime;
                        yield return null;
                    }
                    
                    currentPokemon.DespawnPokemon();
                }
            }

            nextPokemon.Setup();

            //Out text + Spawn new Pokemon
            toSend.Clear();
            foreach (Chat t in nextChat)
            {
                Chat c = Instantiate(t);
                c.AddToOverride("<NEXT_POKEMON>", nextPokemon.GetName());
                toSend.Add(c);
            }

            ChatManager.instance.Add(toSend.ToArray());

            BattleManager.instance.SpawnPokemon(nextPokemon, spot);
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

            done = true;
        }

        #endregion

        #region Out

        public override float Evaluate(Pokemon user, Pokemon target, VirtualBattle virtualBattle,
            PersonalitySetting personalitySetting)
        {
            Debug.LogError("Evaluate Switch");

            return 0;
        }

        #endregion
    }
}