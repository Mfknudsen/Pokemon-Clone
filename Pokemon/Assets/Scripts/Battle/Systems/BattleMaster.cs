#region SDK

using System;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.AI;
using Mfknudsen._Debug;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Actions.Item;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems.States;
using Mfknudsen.Battle.UI;
using Mfknudsen.Comunication;
using Mfknudsen.Items;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random; //Custom

#endregion

namespace Mfknudsen.Battle.Systems
{
    #region Enums

    public enum MasterState
    {
        Setup,
        Starting,
        ChoosingMove,
        AI,
        Checking,
        Action,
        RoundDone,
        SelectNew
    }

    public enum Weather
    {
        None,
        Rain,
        HarshSunlight,
        Hail
    }

    #endregion

    public class BattleMaster : MonoBehaviour
    {
        #region Values

        [Header("Object Reference:")] [HideInInspector]
        public static BattleMaster instance;

        [SerializeField] private bool active = false;
        [SerializeField] private BattleStarter starter = null;
        [SerializeField] private MasterState state = 0;
        [SerializeField] private Weather weather = 0;
        [SerializeField] private Condition faintCondition = null;
        [SerializeField] private BattleAction switchAction = null, itemAction = null;

        [Header("Members:")] [SerializeField] private List<BattleMember> members = new List<BattleMember>();

        [Header("Battlefield:")] [SerializeField]
        private GameObject spotPrefab = null;

        [SerializeField] private int closeCount = 0, farCount = 0;
        [SerializeField] private Transform closeSpotTransform = null, farSpotTransform = null;
        [SerializeField] private Transform closeUITransform = null, farUITransform = null;
        [SerializeField] private List<Spot> spots = new List<Spot>();
        private SpotOversight spotOversight;

        private State stateManage;
        [Header("Start:")] [SerializeField] private List<BattleAction> startAction = new List<BattleAction>();

        [Header(" -- Actions:")] [SerializeField]
        private int actionIndex = 0;

        [SerializeField] private List<BattleAction> actionList = new List<BattleAction>();
        [SerializeField] private float secondsPerPokemonMove = 1;
        Coroutine actionOperation = null;
        [SerializeField] BattleAction action = null;
        [SerializeField] private int actionSwitchState = 1;

        [Header(" -- Fainted:")] [SerializeField]
        private bool isFainted = false;

        [SerializeField] private List<Pokemon> faintedPokemon = new List<Pokemon>();

        [Header(" -- Select New:")] [SerializeField]
        private bool chooseNew = false;

        [SerializeField] private Spot needNew = null;

        [Header("Conditions:")] [SerializeField]
        private ConditionOversight checking = null;

        private Coroutine conditionChecker = null;
        private Coroutine conditionOperation = null;

        [Header("UI:")] [SerializeField] private BattleDisplay display = null;

        [Header(" -- Turn Display:")] [SerializeField]
        private GameObject turnDisplay = null;

        [SerializeField] private TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];

        [Header(" -- Pokemon Selection Menu:")] [SerializeField]
        private GameObject pokemonSelectionMenu = null;

        [SerializeField] private SelectionMenu pokemonMenu = null;

        [Header(" -- Item Selection Menu:")] [SerializeField]
        private GameObject itemSelectionMenu = null;

        [SerializeField] private ItemSelection itemMenu = null;

        [Header(" -- Effective")] [SerializeField]
        private Chat superEffective = null;

        [SerializeField]
        private Chat notEffective = null, noEffect = null, barelyEffective = null, extremlyEffective = null;

        [Header("Chat:")] [SerializeField] private Chat SwitchChat = null;

        [Header("Targeting:")] [SerializeField]
        private bool waitForTarget = false;

        [SerializeField] private Pokemon user = null;
        [SerializeField] private BattleAction actionInWait = null;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            actionSwitchState = 1;
        }

        private void Start()
        {
            instance = this;

            BattleMathf.SetSuperEffective(superEffective);
            BattleMathf.SetNotEffective(notEffective);
            BattleMathf.SetNoEffect(noEffect);
            BattleMathf.SetBarelyEffective(barelyEffective);
            BattleMathf.SetExtremlyEffective(extremlyEffective);
        }

        private void Update()
        {
            /*
            if (!active)
                return;

            if (!isFainted)
            {
                switch (state)
                {
                    case MasterState.Setup:
                        Setup();
                        break;

                    case MasterState.Starting:
                        Starting();
                        break;

                    case MasterState.ChoosingMove:
                        ChoosingMove();
                        break;

                    case MasterState.AI:
                        AI();
                        break;

                    case MasterState.Checking:
                        Checking();
                        break;

                    case MasterState.Action:
                        Action();
                        break;

                    case MasterState.RoundDone:
                        RoundDone();
                        break;

                    case MasterState.SelectNew:
                        SelectNew();
                        break;
                }
            }
            else if (isFainted)
                DoFaintedPokemon();
                */
        }

        #endregion

        #region States

        private void AI()
        {
            //
            // For TEST without AI
            bool nextState = true;
            foreach (BattleMember m in members)
            {
                if (m == null)
                    continue;
                if (m.IsPlayer())
                    continue;

                foreach (Spot s in spots)
                {
                    if (s == null)
                        continue;
                    Pokemon p = s.GetActivePokemon();
                    if (p == null)
                        continue;
                    if (!m.GetTeam().PartOfTeam(p))
                        continue;

                    if (p.GetBattleAction() == null)
                    {
                        nextState = false;

                        PokemonMove action = null;
                        if (p.GetMoves().Length > 1)
                            action = p.GetMoveByIndex(Random.Range(0, p.GetMoves().Length - 1));
                        else
                            action = p.GetMoveByIndex(0);

                        if (action != null)
                        {
                            action.SetCurrentPokemon(p);
                            action.SetTargetIndex(0);
                            p.SetBattleAction(action);
                        }
                    }
                }
            }

            if (nextState)
                SwitchState(MasterState.Checking);
        }

        private void Checking()
        {
            turnDisplay.SetActive(false);

            CheckActions();

            SwitchState(MasterState.Action);
        }

        private void Action()
        {
            switch (actionSwitchState)
            {
                #region Case 1

                case 1:
                    //Setup
                    if (actionIndex < actionList.Count)
                    {
                        action = actionList[actionIndex];
                        actionSwitchState = 2;

                        checking = null;
                        actionOperation = null;

                        BattleLog.instance.AddNewLog(name, "Starting Action: " + action.name.Replace("(Clone)", ""));
                    }
                    else
                        actionSwitchState = 7;

                    break;

                #endregion

                #region Case 2

                case 2:
                    //Check Pokemon Can Attack
                    if (action is PokemonMove)
                    {
                        if (checking == null)
                        {
                            checking = action.GetCurrentPokemon().GetConditionOversight();
                            conditionChecker = StartCoroutine(checking.CheckConditionBeforeMove());
                        }
                        else if (checking != null)
                        {
                            if (ChatMaster.instance.GetIsClear() && checking.GetDone())
                            {
                                if (checking.GetIsStunned())
                                    actionSwitchState = 5;
                                else
                                    actionSwitchState = CheckFaintedAndDecide(action.GetCurrentPokemon(), 3, 5);

                                checking = null;
                                conditionChecker = null;
                            }
                        }
                    }
                    else
                        actionSwitchState = 3;

                    break;

                #endregion

                #region Case 3

                case 3:
                    //Activation
                    if (actionOperation == null)
                        actionOperation = StartCoroutine(action.Activate());

                    if (actionOperation == null)
                    {
                        Debug.Log("No operation in action");
                        actionSwitchState = 7;
                    }
                    else
                        actionSwitchState = 4;

                    break;

                #endregion

                #region Case 4

                case 4:
                    //Checking
                    if (action.GetDone() && ChatMaster.instance.GetIsClear())
                    {
                        foreach (Spot s in spots)
                        {
                            if (s == null)
                                continue;
                            Pokemon p = s.GetActivePokemon();
                            if (p == null)
                                continue;

                            CheckFaintedAndDecide(p);
                        }

                        actionSwitchState = 5;
                    }

                    break;

                #endregion

                #region Case 5

                case 5:
                    //Moving on
                    actionIndex++;

                    if (actionIndex == actionList.Count)
                    {
                        foreach (Spot s in spots)
                        {
                            if (s == null)
                                continue;
                            Pokemon p = s.GetActivePokemon();
                            if (p == null)
                                continue;

                            p.GetConditionOversight().Reset();
                        }

                        actionSwitchState = 6;
                    }
                    else
                        actionSwitchState = 1;

                    break;

                #endregion

                #region Case 6

                case 6:
                    //Conditions
                    if (conditionChecker == null && checking == null)
                    {
                        foreach (Spot s in spots)
                        {
                            if (s == null)
                                continue;
                            Pokemon p = s.GetActivePokemon();
                            if (p == null)
                                continue;

                            if (!p.GetConditionOversight().GetDone())
                            {
                                checking = p.GetConditionOversight();
                                conditionChecker = StartCoroutine(checking.CheckConditionEndTurn());
                            }
                        }
                    }
                    else if (checking != null)
                    {
                        if (checking.GetDone())
                        {
                            checking = null;
                            conditionChecker = null;

                            foreach (Spot s in spots)
                            {
                                if (s == null)
                                    continue;
                                Pokemon p = s.GetActivePokemon();
                                if (p == null)
                                    continue;

                                if (!p.GetConditionOversight().GetDone())
                                {
                                    checking = p.GetConditionOversight();
                                    conditionChecker = StartCoroutine(checking.CheckConditionEndTurn());
                                }
                            }

                            if (checking == null)
                            {
                                actionSwitchState = 7;

                                foreach (Spot s in spots)
                                {
                                    if (s == null)
                                        continue;
                                    Pokemon p = s.GetActivePokemon();
                                    if (p == null)
                                        continue;

                                    p.GetConditionOversight().Reset();
                                }
                            }
                        }
                    }

                    break;

                #endregion

                #region Case 7

                case 7:
                    actionIndex = 0;
                    actionSwitchState = 1;
                    action = null;
                    actionOperation = null;

                    foreach (Spot s in spots)
                    {
                        if (s == null)
                            continue;
                        Pokemon p = s.GetActivePokemon();
                        if (p == null)
                            continue;
                        p.SetBattleAction(null);
                    }

                    //Clear
                    foreach (BattleAction a in actionList)
                    {
                        if (a.GetIsInstantiated())
                            Destroy(a);
                    }

                    actionList.Clear();

                    //Check if there is enough Pokemon to continue the battle
                    bool noMore = true;
                    int checkNumber = -1;
                    foreach (BattleMember m in members)
                    {
                        if (m == null)
                            continue;

                        if (m.GetTeam().HasMorePokemon())
                        {
                            if (checkNumber == -1)
                                checkNumber = m.GetTeamNumber();
                            else
                            {
                                if (checkNumber != m.GetTeamNumber())
                                {
                                    noMore = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (noMore)
                        SwitchState(MasterState.RoundDone);
                    else if (chooseNew)
                        SwitchState(MasterState.SelectNew);
                    else
                        SwitchState(MasterState.ChoosingMove);
                    break;

                #endregion
            }
        }

        private void RoundDone()
        {
            bool victory = false;

            foreach (BattleMember m in members)
            {
                if (m == null)
                    continue;
                if (m.GetTeamNumber() == 1)
                    continue;

                if (m.GetTeam().HasMorePokemon())
                    victory = true;
            }

            starter.EndBattle(victory);
        }

        private void SelectNew()
        {
            if (needNew == null)
            {
                foreach (Spot s in spots)
                {
                    if (s == null)
                        continue;

                    if (s.GetActive())
                        continue;

                    if (!s.GetNeedNew())
                        continue;

                    foreach (BattleMember m in members)
                    {
                        if (m == null)
                            continue;

                        if (m.OwnSpot(s) && m.GetTeam().CanSendMorePokemon())
                        {
                            s.SetActive(true);

                            if (m.IsPlayer())
                            {
                                BattleLog.instance.AddNewLog(name, "Waiting For Player Pokemon Selection");
                                pokemonSelectionMenu.SetActive(true);
                                pokemonMenu.SetFieldNames(m.GetTeam(), SelectorGoal.Switch);
                            }
                            else
                            {
                                pokemonSelectionMenu.SetActive(false);
                            }

                            needNew = s;
                            break;
                        }
                    }

                    if (needNew != null)
                        break;
                }

                if (needNew == null)
                {
                    pokemonSelectionMenu.SetActive(false);
                    chooseNew = false;
                    SwitchState(MasterState.Starting);
                }
            }
        }

        private void DoFaintedPokemon()
        {
            foreach (Pokemon p in faintedPokemon)
            {
                if (checking == null)
                {
                    chooseNew = true;
                    checking = p.GetConditionOversight();
                    conditionChecker = StartCoroutine(p.GetConditionOversight().CheckFaintedCondition());
                }
            }

            if (conditionChecker != null)
            {
                if (checking.GetDone())
                {
                    Pokemon p = checking.GetNonVolatileStatus().GetAffectedPokemon();

                    if (p.GetBattleAction() != null)
                        actionList.Remove(p.GetBattleAction());

                    faintedPokemon.Remove(p);
                    checking = null;
                    conditionChecker = null;
                }
            }

            if (faintedPokemon.Count == 0)
                isFainted = false;
        }

        #endregion

        #region Getters

        public bool GetActive()
        {
            return active;
        }

        public float GetSecPerPokeMove()
        {
            return secondsPerPokemonMove;
        }

        public Coroutine GetConditionOperation()
        {
            return conditionOperation;
        }

        public Spot[] GetSpots()
        {
            return spots.ToArray();
        }

        public Weather GetWeather()
        {
            return weather;
        }

        public BattleStarter GetStarter()
        {
            return starter;
        }

        public BattleMember[] GetMembers()
        {
            return members.ToArray();
        }

        public List<Pokemon> GetFaintedPokemon()
        {
            return faintedPokemon;
        }

        public SpotOversight GetSpotOversight()
        {
            return spotOversight;
        }

        #endregion

        #region Setters

        public void SetConditionOperation(IEnumerator toStart)
        {
            if (toStart != null)
                conditionOperation = StartCoroutine(toStart);
            else
                conditionOperation = null;
        }

        #endregion

        #region In

        public void StartBattle(BattleStarter bs, BattleMember[] players)
        {
            starter = bs;
            members.Clear();

            string s = "Starting Battle Between:";
            foreach (BattleMember member in players)
            {
                if (!members.Contains(member))
                {
                    members.Add(member);
                    s += "\n    - " + member.GetName();
                }
            }

            BattleLog.instance.AddNewLog(name, s);

            active = true;

            SetState(new BeginState(this));
        }

        public void SelectAction(int i)
        {
            Debug.Log(i);
            if (i > 0 && i < 5)
            {
                BattleMember battleMember = MasterPlayer.instance.GetBattleMember();

                foreach (Spot spot in spotOversight.GetSpots())
                {
                    Pokemon pokemon = spot.GetActivePokemon();

                    if (!battleMember.GetTeam().PartOfTeam(pokemon))
                        continue;

                    PokemonMove move = pokemon.GetMoveByIndex(i - 1);

                    if (move == null)
                        return;

                    move.SetCurrentPokemon(pokemon);

                    pokemon.SetBattleAction(move);
                }
            }
            else if (i == 5)
            {
                pokemonSelectionMenu.SetActive(true);

                foreach (BattleMember m in members)
                {
                    if (m.IsPlayer())
                    {
                        pokemonMenu.SetFieldNames(m.GetTeam(), SelectorGoal.Switch);
                        break;
                    }
                }
            }
            else if (i == 6)
            {
                itemSelectionMenu.SetActive(true);
                foreach (BattleMember m in members)
                {
                    if (m == null)
                        continue;
                    if (!m.IsPlayer())
                        continue;

                    itemMenu.Setup(m.GetInventory().GetAllItems());
                    break;
                }
            }
            else
            {
                Debug.Log("Run Not Implemented!");
            }
        }

        public void SelectNewPokemon(int i)
        {
            if (i < 0 && i > 5)
                return;

            if (chooseNew)
            {
                Pokemon p = needNew.GetAllowedTeam().GetPokemonByIndex(i);
                if (p == null)
                    return;

                SwitchAction a = Instantiate(switchAction) as SwitchAction;

                a.SetSpot(needNew);
                a.SetTeam(needNew.GetAllowedTeam());
                a.SetCurrentPokemon(needNew.GetActivePokemon());
                a.SetNextPokemon(needNew.GetAllowedTeam().GetPokemonByIndex(i));

                foreach (BattleMember m in members)
                {
                    if (m == null)
                        continue;

                    if (m.OwnSpot(needNew))
                        startAction.Add(a);
                }


                needNew = null;
            }
            else
            {
                SwitchAction a = Instantiate(switchAction) as SwitchAction;
                foreach (Spot s in spots)
                {
                    if (s == null)
                        continue;
                    Pokemon p = s.GetActivePokemon();
                    if (p == null)
                        continue;

                    if (p.GetBattleAction() == null)
                    {
                        a.SetCurrentPokemon(p);
                        a.SetSpot(s);
                        a.SetTeam(s.GetAllowedTeam());
                        a.SetNextPokemon(s.GetAllowedTeam().GetPokemonByIndex(i));

                        foreach (BattleMember m in members)
                        {
                            if (m == null)
                                continue;

                            if (m.IsPlayer())
                            {
                                p.SetBattleAction(a);
                                pokemonSelectionMenu.SetActive(false);
                            }
                        }

                        break;
                    }
                }
            }
        }

        public void SelectItem(Item item, Pokemon target)
        {
            foreach (Spot s in spots)
            {
                if (s == null)
                    continue;
                Pokemon p = s.GetActivePokemon();
                if (p == null)
                    continue;
                if (p.GetBattleAction() != null)
                    continue;

                ItemAction action = itemAction.GetAction() as ItemAction;
                action.SetToUse(item);
                action.SetCurrentPokemon(target);

                //Delegate to secondary selection if more then one target
                foreach (BattleMember m in members)
                {
                    if (!m.GetInventory().IsItemInBag(item))
                        continue;
                    action.SetBattleMember(m);
                }

                actionInWait = action;
                user = p;
                waitForTarget = true;

                p.SetBattleAction(action);

                pokemonSelectionMenu.SetActive(false);
                itemSelectionMenu.SetActive(false);

                break;
            }
        }

        public void SpawnPokemon(Pokemon pokemon, Spot spot)
        {
            BattleLog.instance.AddNewLog(name, "Spawning: " + pokemon.GetName());

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

            //Check if spawned object is placeholder;
            PokemonPlaceholder.CheckPlaceholder(pokemon, obj);
        }

        public void DespawnPokemon(Pokemon pokemon)
        {
            foreach (Spot s in spots)
            {
                if (s == null)
                    continue;
                Pokemon p = s.GetActivePokemon();
                if (p == null)
                    continue;

                if (p == pokemon)
                {
                    pokemon.DespawnPokemon();
                    s.SetActive(false);
                    s.SetNeedNew(true);
                }
            }
        }

        public void RemoveAction(BattleAction action)
        {
            if (actionList.Contains(action))
                actionList.Remove(action);
        }

        public void LateTarget(Spot target)
        {
            if (waitForTarget)
            {
                actionInWait.SetCurrentPokemon(target.GetActivePokemon());
                user.SetBattleAction(actionInWait);

                user = null;
                actionInWait = null;
                waitForTarget = false;

                pokemonSelectionMenu.SetActive(false);
                itemSelectionMenu.SetActive(false);
            }
        }

        public void StartRemoteCoroutine(IEnumerator operation)
        {
            StartCoroutine(operation);
        }

        public void ShowPokemonSelector(SelectorGoal goal, Item item = null)
        {
            pokemonSelectionMenu.SetActive(true);

            foreach (BattleMember m in members)
            {
                if (m == null)
                    continue;
                if (!m.IsPlayer())
                    continue;
                pokemonMenu.SetFieldNames(m.GetTeam(), goal);
            }

            pokemonMenu.SetItem(item);
        }

        public void ParseTargetToItemSelector(Pokemon p)
        {
            itemMenu.ReceiveTarget(p);
        }

        public void GetComputerAction(Decision decision)
        {
        }

        public SpotOversight SetupSpotOversight(int x)
        {
            spotOversight = new SpotOversight(x);

            return spotOversight;
        }

        public void SetState(State state)
        {
            stateManage = state;
            StartCoroutine(state.Tick());
        }

        public void DisplayMoves(Pokemon pokemon)
        {
            if (!turnDisplay.activeSelf)
                turnDisplay.SetActive(true);

            PokemonMove[] toDisplay = pokemon.GetMoves();
            for (int i = 0; i < 4; i++)
            {
                Image img = buttonsText[i].transform.parent.GetComponent<Image>();

                img.color = Color.white;

                if (toDisplay[i] != null)
                {
                    buttonsText[i].text = toDisplay[i].GetName();
                    img.color = toDisplay[i].GetMoveType().GetTypeColor();
                }
                else
                    buttonsText[i].text = "";
            }
        }

        public void DisableMovesDisplay()
        {
            turnDisplay.SetActive(faintCondition);
        }

        #endregion

        #region Out

        public GameObject CreateSpot()
        {
            return Instantiate(spotPrefab);
        }

        public SwitchAction InstantiateSwitchAction()
        {
            return (SwitchAction) Instantiate(switchAction);
        }

        #endregion

        #region Internal

        private void DisplayMovesForPokemon()
        {
            foreach (Spot s in spots)
            {
                if (s == null)
                    continue;
                Pokemon p = s.GetActivePokemon();
                if (p == null)
                    continue;
                if (p.GetBattleAction() != null)
                    continue;

                foreach (BattleMember m in members)
                {
                    if (!m.IsPlayer())
                        continue;

                    PokemonMove[] toDisplay = p.GetMoves();

                    for (int i = 0; i < 4; i++)
                    {
                        Image img = buttonsText[i].transform.parent.GetComponent<Image>();

                        img.color = Color.white;

                        if (toDisplay[i] != null)
                        {
                            buttonsText[i].text = toDisplay[i].GetName();
                            img.color = toDisplay[i].GetMoveType().GetTypeColor();
                        }
                        else
                            buttonsText[i].text = "";
                    }

                    return;
                }
            }
        }

        private void CheckActions()
        {
            BattleLog.instance.AddNewLog(name, "Checking Action Priority");

            //Setup
            List<BattleAction> tempList = new List<BattleAction>();
            foreach (Spot s in spots)
            {
                if (s == null)
                    continue;
                Pokemon p = s.GetActivePokemon();
                if (p == null)
                    continue;

                tempList.Add(p.GetBattleAction());
            }

            //Ability Effect on Priority
            foreach (BattleAction action in tempList)
            {
                Ability ability = action.GetCurrentPokemon().GetAbility();

                if (ability != null)
                {
                    if (ability.GetEffectsMovePriority())
                    {
                        action.SetPriority(action.GetPriority() + ability.PriorityEffect(action));
                    }
                }
            }

            //Setup Action by Priority
            foreach (BattleAction action in tempList)
            {
                if (actionList.Count == 0)
                    actionList.Add(action);

                for (int i = 0; i < actionList.Count; i++)
                {
                    if (!actionList.Contains(action))
                    {
                        if (actionList[i].GetPriority() < action.GetPriority())
                            actionList.Insert(i, action);
                        else
                            actionList.Add(action);
                    }
                }
            }

            //Speed Effect on Priority
            if (actionList.Count > 1)
            {
                bool end = false;
                while (!end)
                {
                    end = true;

                    for (int i = 1; i < actionList.Count; i++)
                    {
                        BattleAction one = actionList[i], two = actionList[i - 1];
                        if (one.GetPriority() == two.GetPriority() && one.GetCurrentPokemon().GetStat(Stat.Speed) >
                            two.GetCurrentPokemon().GetStat(Stat.Speed))
                        {
                            end = false;

                            actionList[i - 1] = one;
                            actionList[i] = two;
                        }
                    }
                }
            }
        }

        private int CheckFaintedAndDecide(Pokemon pokemon, int? direct = 0, int? indirect = 0)
        {
            if (pokemon.GetCurrentHealth() == 0)
            {
                Condition condition = faintCondition.GetCondition();
                condition.SetAffectedPokemon(pokemon);
                pokemon.GetConditionOversight().TryApplyNonVolatileCondition(condition);

                faintedPokemon.Add(pokemon);
                isFainted = true;

                return indirect.Value;
            }

            return direct.Value;
        }

        private void SwitchState(MasterState s)
        {
            state = s;
            BattleLog.instance.AddNewLog(name, "Switching to: " + state.ToString());
        }

        #endregion
    }
}