﻿#region Packages

using System.Collections;
using System.Linq;
using Cinemachine;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.Systems.States;
using Mfknudsen.Battle.UI.Information_Display;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Communication;
using Mfknudsen.Player.Camera;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using Mfknudsen.Settings.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = Mfknudsen._Debug.Logger;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class BattleManager : Manager
    {
        #region Values

        public static BattleManager instance;

        [FoldoutGroup("Conditions")] [SerializeField]
        private Condition faintCondition;

        [FoldoutGroup("Actions")] [SerializeField]
        private BattleAction switchAction, itemAction;

        [FoldoutGroup("Actions")] [SerializeField]
        private float secondsPerPokemonMove = 1;

        [FoldoutGroup("Battlefield")] [SerializeField]
        private GameObject spotPrefab;

        [FoldoutGroup("Battlefield")] [SerializeField]
        private Battlefield battlefield;

        [FoldoutGroup("UI")] [SerializeField] private SelectionMenu selectionMenu;

        [FoldoutGroup("UI")] [SerializeField] private DisplayManager displayManager;

        [FoldoutGroup("Chat")] [SerializeField]
        private Chat superEffective;

        [FoldoutGroup("Chat")] [SerializeField]
        private Chat notEffective, noEffect, barelyEffective, extremelyEffective, miss;

        [FoldoutGroup("Camera")] [SerializeField]
        private CinemachineVirtualCameraBase battleCameraRig;

        private BattleStarter starter;

        private WeatherManager weatherManager;

        private SpotOversight spotOversight;

        private AbilityOversight abilityOversight;

        private Coroutine stateManager;

        #endregion

        #region Build In States

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        #endregion

        #region Getters

        public SelectionMenu GetSelectionMenu()
        {
            return selectionMenu;
        }

        public float GetSecPerPokeMove()
        {
            return secondsPerPokemonMove;
        }

        public BattleStarter GetStarter()
        {
            return starter;
        }

        public WeatherManager GetWeatherManager()
        {
            return weatherManager ??= new WeatherManager();
        }

        public SpotOversight GetSpotOversight()
        {
            return spotOversight ??= new SpotOversight();
        }

        public AbilityOversight GetAbilityOversight()
        {
            return abilityOversight;
        }

        public DisplayManager GetDisplayManager()
        {
            return displayManager;
        }

        public Battlefield GetBattlefield()
        {
            return battlefield;
        }

        public CinemachineVirtualCameraBase GetBattleCamera()
        {
            return battleCameraRig;
        }

        #endregion

        #region Setters

        public void SetDisplayManager(DisplayManager set)
        {
            this.displayManager = set;
        }

        public void SetSelectionMenu(SelectionMenu set)
        {
            this.selectionMenu = set;
        }

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;

                OperationManager.instance.AddAsyncOperationsContainer(
                    new OperationsContainer(CameraEvent.ReturnToDefaultBattle()));

                BattleMathf.SetSuperEffective(this.superEffective);
                BattleMathf.SetNotEffective(this.notEffective);
                BattleMathf.SetNoEffect(this.noEffect);
                BattleMathf.SetBarelyEffective(this.barelyEffective);
                BattleMathf.SetExtremlyEffective(this.extremelyEffective);
                BattleMathf.SetMissChat(this.miss);

                while (this.displayManager == null || this.selectionMenu == null)
                    yield return null;

                SetState(new BeginState(this));

                yield break;
            }

            Destroy(gameObject);
        }

        public void StartBattle(BattleStarter battleStarter)
        {
            this.starter = battleStarter;
        }

        public void SpawnPokemon(Pokemon pokemon, Spot spot)
        {
            Logger.AddLog(this.name, "Spawning: " + pokemon.GetName());

            Transform sTransform = spot.GetTransform();
            GameObject obj = Instantiate(pokemon.GetPokemonPrefab(), sTransform, true);

            obj.transform.position = sTransform.position;
            obj.transform.rotation = sTransform.rotation;

            pokemon.SetSpawnedObject(obj);
            pokemon.SetInBattle(true);
            pokemon.SetGettingSwitched(false);
            pokemon.SetRevived(false);

            spot.SetActivePokemon(pokemon);
            spot.SetNeedNew(false);

            this.displayManager.UpdateSlots();

            //Check if spawned object is placeholder;
            PokemonPlaceholder.CheckPlaceholder(pokemon, obj);
        }

        public void DespawnPokemon(Pokemon pokemon)
        {
            if (!pokemon) return;

            foreach (Spot s in this.spotOversight.GetSpots()
                         // ReSharper disable once Unity.NoNullPropagation
                         .Select(s => new { s, p = s?.GetActivePokemon() })
                         .Where(t => t.p == pokemon)
                         .Select(t => t.s))
            {
                pokemon.DespawnPokemon();

                s.SetActivePokemon(null);

                break;
            }

            this.displayManager.UpdateSlots();
        }

        public void SetupAbilityOversight()
        {
            this.abilityOversight = new AbilityOversight();

            this.abilityOversight.Setup();
        }

        public void SetState(State set)
        {
            if (this.stateManager != null)
                StopCoroutine(this.stateManager);

            this.stateManager = StartCoroutine(set.Tick());
        }

        public void EndBattle(bool playerVictory)
        {
            this.starter.EndBattle(playerVictory);
        }

        public void SetPokemonFainted(Pokemon pokemon)
        {
            FaintedCondition condition = Instantiate(this.faintCondition) as FaintedCondition;

            // ReSharper disable once PossibleNullReferenceException
            condition.SetAffectedPokemon(pokemon);

            pokemon.GetConditionOversight().TryApplyNonVolatileCondition(condition);
        }

        #endregion

        #region Out

        public Spot CreateSpot(Transform parent)
        {
            return Instantiate(this.spotPrefab, parent).GetComponent<Spot>();
        }

        public SwitchAction InstantiateSwitchAction()
        {
            return (SwitchAction)Instantiate(this.switchAction);
        }

        public ItemAction InstantiateItemAction()
        {
            return Instantiate(this.itemAction) as ItemAction;
        }

        public bool CheckTeamDefeated(bool isAlly)
        {
            return this.spotOversight.GetSpots()
                .Select(s =>
                    s.GetBattleMember())
                .Where(bm =>
                    bm.GetTeamAffiliation() == isAlly)
                .Any(bm =>
                    !bm.GetTeam().HasMorePokemon());
        }

        #endregion
    }
}