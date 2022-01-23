#region Packages

using System;
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
using Mfknudsen.Settings.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

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

        private State stateManager;

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
            displayManager = set;
        }

        public void SetSelectionMenu(SelectionMenu set)
        {
            selectionMenu = set;
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

                BattleMathf.SetSuperEffective(superEffective);
                BattleMathf.SetNotEffective(notEffective);
                BattleMathf.SetNoEffect(noEffect);
                BattleMathf.SetBarelyEffective(barelyEffective);
                BattleMathf.SetExtremlyEffective(extremelyEffective);
                BattleMathf.SetMissChat(miss);

                while (displayManager == null || selectionMenu == null)
                    yield return null;

                SetState(new BeginState(this));
            }
            else
                Destroy(gameObject);

            yield break;
        }

        public void StartBattle(BattleStarter battleStarter)
        {
            starter = battleStarter;
        }

        public void SpawnPokemon(Pokemon pokemon, Spot spot)
        {
            BattleLog.AddLog(name, "Spawning: " + pokemon.GetName());

            Transform trans = spot.GetTransform();
            GameObject obj = Instantiate(pokemon.GetPokemonPrefab(), trans, true);

            obj.transform.position = trans.position;
            obj.transform.rotation = trans.rotation;

            pokemon.SetSpawnedObject(obj);
            pokemon.SetInBattle(true);
            pokemon.SetGettingSwitched(false);
            pokemon.SetRevived(false);

            spot.SetActivePokemon(pokemon);
            spot.SetNeedNew(false);

            displayManager.UpdateSlots();

            //Check if spawned object is placeholder;
            PokemonPlaceholder.CheckPlaceholder(pokemon, obj);
        }

        // ReSharper disable once IdentifierTypo
        public void DespawnPokemon(Pokemon pokemon)
        {
            if (pokemon == null) return;

            foreach (Spot s in spotOversight.GetSpots()
                         // ReSharper disable once Unity.NoNullPropagation
                         .Select(s => new {s, p = s?.GetActivePokemon()})
                         .Where(t => t.p is { })
                         .Where(t => t.p == pokemon)
                         .Select(t => t.s))
            {
                pokemon.DespawnPokemon();

                s.SetActivePokemon(null);

                break;
            }

            displayManager.UpdateSlots();
        }

        public void SetupAbilityOversight()
        {
            abilityOversight = new AbilityOversight();

            abilityOversight.Setup();
        }

        public void SetState(State s)
        {
            stateManager = s;
            StartCoroutine(s.Tick());
        }

        public void EndBattle(bool playerVictory)
        {
            starter.EndBattle(playerVictory);
        }

        public void SetPokemonFainted(Pokemon pokemon)
        {
            FaintedCondition condition = faintCondition.GetCondition() as FaintedCondition;

            // ReSharper disable once PossibleNullReferenceException
            condition.SetAffectedPokemon(pokemon);

            pokemon.GetConditionOversight().TryApplyNonVolatileCondition(condition);
        }

        #endregion

        #region Out

        public Spot CreateSpot(Transform parent)
        {
            return Instantiate(spotPrefab, parent).GetComponent<Spot>();
        }

        public SwitchAction InstantiateSwitchAction()
        {
            return (SwitchAction) Instantiate(switchAction);
        }

        public ItemAction InstantiateItemAction()
        {
            return Instantiate(itemAction) as ItemAction;
        }

        #endregion
    }
}