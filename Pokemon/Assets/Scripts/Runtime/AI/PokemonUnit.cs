#region Libraries

using Runtime.Pokémon;
using UnityEngine;
using Logger = Runtime.Testing.Logger;

#endregion

namespace Runtime.AI
{
    #region Enums

    public enum PokemonState
    {
        Wild,
        Batte
    }

    #endregion

    public sealed class PokemonUnit : UnitBase
    {
        #region Values

        private Pokemon pokemonInformation;

        private PokemonState currentState;

        private BattleBehaviours battleBehaviours;

        #endregion

        #region Build In States

        private void Start()
        {

        }

        #endregion

        #region Getters

        public Pokemon GetPokemonInformation() => this.pokemonInformation;

        public BattleBehaviours GetBattleBehaviours => this.battleBehaviours;

        internal bool IsBattleState => this.currentState.Equals(PokemonState.Batte);

        #endregion

        #region In

        public override void UpdateUnit()
        {
            if (this.currentState.Equals(PokemonState.Wild))
                base.UpdateUnit();
        }

        public override void InteractTrigger()
        {
        }

        public void SetState(PokemonState state)
        {
            if (this.currentState.Equals(state))
                return;

            this.currentState = state;

            Logger.AddLog(this, "Pokemon Unit Switching state to: " + state.ToString());
        }

        #endregion
    }

    public readonly struct BattleBehaviours
    {
        #region Values

        private readonly PokemonUnit instance;

        #endregion

        #region Build In States

        public BattleBehaviours(PokemonUnit instance) => this.instance = instance;

        #endregion

        #region In 

        public bool MoveToPoint(Vector3 position, Quaternion rotation)
        {
            if (!this.instance.IsBattleState)
                return false;

            this.instance.GetAgent().SetDestination(position);


            return true;
        }

        #endregion
    }
}