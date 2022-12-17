using System.Collections;
using Runtime.Communication;
using Runtime.Pokémon;
using Runtime.Systems;
using UnityEngine;

namespace Runtime.Battle.Systems.Static_Operations
{
    public class MissHit : IOperation
    {
        private ChatManager chatManager;
        private readonly Chat missChat;
        private readonly Pokemon targetPokemon;
        private bool done;

        public MissHit(Pokemon targetPokemon)
        {
            this.targetPokemon = targetPokemon;

            this.missChat = Object.Instantiate(BattleMathf.GetMissChat());
        }

        public bool IsOperationDone => this.done;

        public IEnumerator Operation()
        {
            this.done = false;

            this.missChat.AddToOverride("<POKEMON_NAME>", this.targetPokemon.GetName());

            this.chatManager.Add(this.missChat);

            while (!this.chatManager.GetIsClear())
                yield return null;

            this.done = true;
        }

        public void OperationEnd()
        {
        }
    }
}