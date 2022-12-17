#region Packages

using System.Linq;
using Cinemachine;
using Runtime._Debug;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Battle.Systems.States;
using Runtime.Battle.UI.Information_Display;
using Runtime.Battle.UI.Selection;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Player.Camera;
using Runtime.Pokémon;
using Runtime.Pokémon.Conditions;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using Runtime.Systems;
using Runtime.Systems.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = Runtime._Debug.Logger;

#endregion

namespace Runtime.Battle.Systems
{
    public class BattleSystem : MonoBehaviour
    {
        #region Values

        public static BattleSystem instance;

        [SerializeField, Required] private OperationManager operationManager;
        [SerializeField, Required] private ChatManager chatManager;
        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField, Required] private UIManager uiManager;

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

        [SerializeField, FoldoutGroup("Camera")]
        private CameraEvent cameraEvent;

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

        private void Start()
        {
            if (instance == null)
            {
                instance = this;

                this.operationManager.AddAsyncOperationsContainer(
                    new OperationsContainer(this.cameraEvent));

                BattleMathf.SetSuperEffective(this.superEffective);
                BattleMathf.SetNotEffective(this.notEffective);
                BattleMathf.SetNoEffect(this.noEffect);
                BattleMathf.SetBarelyEffective(this.barelyEffective);
                BattleMathf.SetExtremlyEffective(this.extremelyEffective);
                BattleMathf.SetMissChat(this.miss);


                this.SetState(new BeginState(this, this.operationManager, this.chatManager, this.uiManager,
                    this.playerManager));
            }

            Destroy(this.gameObject);
        }

        #endregion

        #region Getters

        public SelectionMenu GetSelectionMenu()
        {
            return this.selectionMenu;
        }

        public float GetSecPerPokeMove()
        {
            return this.secondsPerPokemonMove;
        }

        public BattleStarter GetStarter()
        {
            return this.starter;
        }

        public WeatherManager GetWeatherManager()
        {
            return this.weatherManager ??= new WeatherManager();
        }

        public SpotOversight GetSpotOversight()
        {
            return this.spotOversight ??= new SpotOversight();
        }

        public AbilityOversight GetAbilityOversight()
        {
            return this.abilityOversight;
        }

        public DisplayManager GetDisplayManager()
        {
            return this.displayManager;
        }

        public Battlefield GetBattlefield()
        {
            return this.battlefield;
        }

        public CinemachineVirtualCameraBase GetBattleCamera()
        {
            return this.battleCameraRig;
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

        public void StartBattle(BattleStarter battleStarter)
        {
            this.starter = battleStarter;
        }

        public void SpawnPokemon(Pokemon pokemon, Spot spot)
        {
            Logger.AddLog(this.name, "Spawning: " + pokemon.GetName());

            Transform sTransform = spot.GetTransform();
            GameObject obj = Instantiate(pokemon.GetVisuelPrefab(), sTransform, true);

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
            obj.GetComponent<PokemonPlaceholder>().CheckPlaceholder(pokemon);
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
            if (this.stateManager != null) this.StopCoroutine(this.stateManager);

            this.stateManager = this.StartCoroutine(set.Tick());
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