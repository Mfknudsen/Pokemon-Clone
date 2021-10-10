using System.Collections;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.Battle.Systems.Static_Operations
{
    public class MissHit : IOperation
    {
        private readonly Chat missChat;
        private readonly Pokemon targetPokemon;
        private bool done;

        public MissHit(Pokemon targetPokemon)
        {
            this.targetPokemon = targetPokemon;

            missChat = Object.Instantiate(BattleMathf.GetMissChat());
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            missChat.AddToOverride("<POKEMON_NAME>", targetPokemon.GetName());

            ChatManager chatManager = ChatManager.instance;
            chatManager.Add(missChat);

            while (!chatManager.GetIsClear())
                yield return null;

            done = true;
        }

        public void End()
        {
        }
    }
}