#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Static_Operations;
using Runtime.Common;
using Runtime.Communication;
using Runtime.Pokémon;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Items.Pokeballs
{
    [CreateAssetMenu(menuName = "Item/Pokeballs/Standard Pokeball")]
    public class Pokeball : BattleItem, IThrowableItem
    {
        #region Values

        [SerializeField] private int catchStat;
        [SerializeField] private Chat noClickBreak, oneClickBreak, twoClickBreak, threeClickBreak, caught;

        private int clicks;

        #endregion

        #region Getters

        public int GetCatchStat()
        {
            return this.catchStat;
        }

        #endregion

        #region Setters

        #endregion

        #region In

        //IThrowableItem
        public void OnCollision(Collision collision)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.GetFirstComponentByRoot<WildPokemonUnit>() is not { } pokemonHit) return;

            Debug.Log("Hit Pokemon: " + pokemonHit.GetPokemon().name);

            this.operationManager.AddAsyncOperationsContainer(
                new OperationsContainer(new CatchPokemon(
                    pokemonHit.GetPokemon(),
                    this,
                    pokemonHit.PauseUnit)));
        }

        #endregion

        #region Out

        public override bool IsUsableTarget(Pokemon pokemon)
        {
            return BattleSystem.instance.GetSpotOversight().GetSpots().Any(spot =>
                spot.GetActivePokemon() == pokemon && spot.GetBattleMember().IsWild());
        }

        public override IEnumerator Operation()
        {
            this.done = false;

            #region Preparing Values

            int clickStage = this.clicks = BattleMathf.CalculateCatch(this.target, this);
            Chat selectedChat = this.clicks switch
            {
                0 => this.noClickBreak,
                1 => this.oneClickBreak,
                2 => this.twoClickBreak,
                3 => this.threeClickBreak,
                _ => this.caught
            };
            selectedChat = selectedChat.GetChat();
            selectedChat.AddToOverride("<TARGET_NAME>", this.target.GetName());

            List<Chat> chats = new();
            foreach (Chat chat in this.chatOnUse)
            {
                Chat c = chat.GetChat();
                c.AddToOverride("<USER_NAME>", this.userName);
                c.AddToOverride("<ITEM_NAME>", this.itemName);
                chats.Add(c);
            }

            #endregion

            #region Prepare Operations

            OperationsContainer container = new();

            ThrowPokeball throwPokeball = new(this.playerManager, this.operationManager, this.target, clickStage, selectedChat);
            container.Add(throwPokeball);

            ChatOperation chatOperation = new(chats.ToArray());
            container.Add(chatOperation);

            this.operationManager.AddOperationsContainer(container);

            #endregion

            this.done = true;

            yield break;
        }

        #endregion
    }
}