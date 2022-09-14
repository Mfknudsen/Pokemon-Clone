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
            return catchStat;
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

            OperationManager.instance.AddAsyncOperationsContainer(
                new OperationsContainer(new CatchPokemon(
                    pokemonHit.GetPokemon(),
                    this,
                    pokemonHit.DisableUnit)));
        }

        #endregion

        #region Out

        public override bool IsUsableTarget(Pokemon pokemon)
        {
            return BattleManager.instance.GetSpotOversight().GetSpots().Any(spot =>
                spot.GetActivePokemon() == pokemon && spot.GetBattleMember().IsWild());
        }

        public override IEnumerator Operation()
        {
            done = false;

            #region Preparing Values

            int clickStage = clicks = BattleMathf.CalculateCatch(target, this);
            Chat selectedChat = clicks switch
            {
                0 => noClickBreak,
                1 => oneClickBreak,
                2 => twoClickBreak,
                3 => threeClickBreak,
                _ => caught
            };
            selectedChat = selectedChat.GetChat();
            selectedChat.AddToOverride("<TARGET_NAME>", target.GetName());

            List<Chat> chats = new();
            foreach (Chat chat in chatOnUse)
            {
                Chat c = chat.GetChat();
                c.AddToOverride("<USER_NAME>", userName);
                c.AddToOverride("<ITEM_NAME>", itemName);
                chats.Add(c);
            }

            #endregion

            #region Prepare Operations

            OperationManager operationManager = OperationManager.instance;
            OperationsContainer container = new();

            ThrowPokeball throwPokeball = new(target, clickStage, selectedChat);
            container.Add(throwPokeball);

            ChatOperation chatOperation = new(chats.ToArray());
            container.Add(chatOperation);

            operationManager.AddOperationsContainer(container);

            #endregion

            done = true;

            yield break;
        }

        #endregion
    }
}