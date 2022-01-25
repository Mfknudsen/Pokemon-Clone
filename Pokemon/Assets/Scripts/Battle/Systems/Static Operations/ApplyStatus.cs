using System.Collections;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;

namespace Mfknudsen.Battle.Systems.Static_Operations
{
    public class ApplyStatus : IOperation
    {
        private bool done;
        private readonly Chat statusChat;
        private readonly Pokemon targetPokemon;
        private readonly Condition statusCondition;

        public ApplyStatus(Chat statusChat, Pokemon targetPokemon, Condition statusCondition)
        {
            this.statusChat = statusChat.GetChat();
            this.targetPokemon = targetPokemon;
            this.statusCondition = statusCondition;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            statusChat.AddToOverride("<TARGET_NAME>", targetPokemon.GetName());

            ChatManager chatManager = ChatManager.instance;
            chatManager.Add(statusChat);

            if (statusCondition is NonVolatileCondition nonVolatile)
                targetPokemon.GetConditionOversight()
                    .TryApplyNonVolatileCondition(nonVolatile);
            else
                targetPokemon.GetConditionOversight().ApplyVolatileCondition((VolatileCondition)statusCondition);


            while (!chatManager.GetIsClear())
                yield return null;

            done = true;
        }

        public void End()
        {
        }
    }
}