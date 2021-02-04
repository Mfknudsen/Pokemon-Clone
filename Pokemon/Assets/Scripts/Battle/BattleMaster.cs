#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//Custom
using Trainer;
using BattleUI;
#endregion

#region Enums
public enum MasterState { Setup, Starting, ChoosingMove, AI, Checking, Action, RoundDone, SelectNew }

public enum Weather { Rain, HarshSunlight, Hail }
#endregion

public class BattleMaster : MonoBehaviour
{
    #region Values
    public static BattleMaster instance;
    public MasterState state = 0;
    public static Weather weather = 0;
    [SerializeField] private Condition faintCondtion = null;

    [Header("Members:")]
    [SerializeField] private BattleMember[] members = new BattleMember[2];
    [SerializeField] private Pokemon[] activePokemons = new Pokemon[2];

    [Header("Battlefield:")]
    [SerializeField] private Spot[] spots;

    /////
    /////
    [Header(" -- Actions:")]
    [SerializeField] private SwitchAction switchAction = null;
    [SerializeField] private int actionIndex = 0;
    [SerializeField] private List<BattleAction> actionList = new List<BattleAction>();
    [SerializeField] private float secondsPerPokemonMove = 1;
    Coroutine actionOperation = null;
    [SerializeField] BattleAction action = null;
    [SerializeField] private int actionSwitchState = 1;
    /////
    /////

    [Header(" -- Select New:")]
    Vector3 v3placeholder;

    [Header("Conditions:")]
    [SerializeField] private ConditionOversight checking = null;
    private Coroutine conditionChecker = null;
    private Coroutine conditionOperation = null;

    [Header("UI:")]
    [SerializeField] private BattleDisplay display = null;
    [SerializeField] private PokemonDisplay[] pokemonDisplay = new PokemonDisplay[2];
    [Header(" -- Turn Display:")]
    [SerializeField] private GameObject turnDisplay = null;
    [SerializeField] private TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];
    [Header(" -- Selectio Menu:")]
    [SerializeField] private GameObject selectionMenu = null;
    [SerializeField] private SelectionMenu menu = null;
    [Header(" -- Effective")]
    [SerializeField] private Chat superEffective = null;
    [SerializeField] private Chat notEffective = null, noEffect = null, barelyEffective = null, extremlyEffective = null;


    [Header("Chat:")]
    [SerializeField] private Chat SwitchChat = null;

    private void OnValidate()
    {
        actionSwitchState = 1;
    }
    #endregion

    #region Build In States
    private void Start()
    {
        instance = this;

        BattleMathf.SetSuperEffective(superEffective);
        BattleMathf.SetNotEffective(notEffective);
        BattleMathf.SetNoEffect(noEffect);
        BattleMathf.SetBarelyEffective(barelyEffective);
        BattleMathf.SetExtremlyEffective(extremlyEffective);

        for (int i = 0; i < spots.Length; i++)
        {
            spots[i].SetTransform();
            spots[i].SetSpotNumber(i);
        }

        state = MasterState.Setup;
    }

    private void Update()
    {
        if (members[0] == null)
            members[0] = Player.MasterPlayer.instance.gameObject.GetComponent<BattlePlayer>();

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
    #endregion

    #region States
    private void Setup()
    {
        ChatMaster.instance.Add((members[1] as BattleEnemy).GetStartChat());

        state = MasterState.Starting;
    }

    private void Starting()
    {
        if (ChatMaster.instance.GetIsClear())
        {
            for (int i = 0; i < members.Length; i++)
            {
                BattleMember member = members[i];

                if (!member.GetTeam().GetReady())
                    member.GetTeam().Setup();

                activePokemons[i] = member.GetTeam().GetPokemonByIndex(0);
                pokemonDisplay[i].SetNewPokemon(activePokemons[i]);
                SpawnPokemon(activePokemons[i], i);
            }

            state = MasterState.ChoosingMove;
        }
    }

    private void ChoosingMove()
    {
        if (!turnDisplay.activeSelf)
        {
            BattleLog.instance.AddNewLog(name, "Player Choose Action");

            turnDisplay.SetActive(true);
            DisplayMovesForPokemon();
        }

        bool ready = true;
        foreach (Pokemon pokemon in activePokemons)
        {
            if (members[0].GetTeam().PartOfTeam(pokemon) && pokemon.GetBattleAction() == null)
                ready = false;
        }

        if (ready)
            state = MasterState.AI;
    }

    private void AI()
    {
        BattleLog.instance.AddNewLog(name, "AI Deciding");
        //
        // For TEST without AI
        foreach (Pokemon pokemon in activePokemons)
        {
            if (!members[0].GetTeam().PartOfTeam(pokemon))
            {
                pokemon.SetBattleAction(pokemon.GetMoveByIndex(Random.Range(0, 3)));

                if (pokemon.GetBattleAction() != null)
                {
                    (pokemon.GetBattleAction() as PokemonMove).SetCurrentPokemon(pokemon);
                    (pokemon.GetBattleAction() as PokemonMove).SetTargetIndex(0);
                }
            }
        }

        state = MasterState.Checking;
    }

    private void Checking()
    {
        turnDisplay.SetActive(false);

        CheckActions();

        state = MasterState.Action;
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
                {
                    Debug.LogError("No action to perform!");
                    actionSwitchState = 7;
                }
                break;
            #endregion
            #region Case 2
            case 2:
                //Check Pokemon Can Attack
                if ((action as PokemonMove) != null)
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
                else if ((action as SwitchAction) != null)
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
                    foreach (Pokemon p in activePokemons)
                        CheckFaintedAndDecide(p);

                    actionSwitchState = 5;
                }
                break;
            #endregion
            #region Case 5
            case 5:
                //Moving on
                if (actionIndex < actionList.Count)
                {
                    actionIndex++;

                    if (actionIndex == actionList.Count || (activePokemons[0].GetCurrentHealth() == 0 || activePokemons[1].GetCurrentHealth() == 0))
                    {
                        foreach (Pokemon member in activePokemons)
                            member.GetConditionOversight().Reset();

                        actionSwitchState = 6;
                    }
                    else
                        actionSwitchState = 1;
                }
                break;
            #endregion
            #region Case 6
            case 6:
                //Conditions
                if (conditionChecker == null && checking == null)
                {
                    foreach (Pokemon pokemon in activePokemons)
                    {
                        if (!pokemon.GetConditionOversight().GetDone())
                        {
                            checking = pokemon.GetConditionOversight();
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

                        foreach (Pokemon pokemon in activePokemons)
                        {
                            if (!pokemon.GetConditionOversight().GetDone())
                            {
                                checking = pokemon.GetConditionOversight();
                                conditionChecker = StartCoroutine(checking.CheckConditionEndTurn());
                            }
                        }

                        if (checking == null)
                        {
                            actionSwitchState = 7;

                            foreach (Pokemon pokemon in activePokemons)
                            {
                                pokemon.GetConditionOversight().Reset();
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

                foreach (Pokemon p in activePokemons)
                    p.SetBattleAction(null);

                //Clear
                actionList.Clear();

                //Check if there is enough Pokemon to continue the battle
                bool allOpposite = true;
                int checkNumber = -1;
                foreach (BattleMember member in members)
                {
                    if (member.GetTeam().HasMorePokemon())
                    {
                        if (checkNumber == -1)
                            checkNumber = member.GetTeamNumber();
                        else
                        {
                            if (checkNumber != member.GetTeamNumber())
                            {
                                allOpposite = false;
                                break;
                            }
                        }
                    }
                }
                Debug.Log(allOpposite);
                if (allOpposite)
                    state = MasterState.RoundDone;
                else
                    state = MasterState.ChoosingMove;
                break;
                #endregion
        }
    }

    private void RoundDone()
    {
        if (members[0].GetTeam().HasMorePokemon())
            Debug.Log("You Win!");
        else
            Debug.Log("You Lose!");
    }

    private void SelectNew()
    {
        if (activePokemons[1] != null)
        {
            if (activePokemons[1].GetCurrentHealth() == 0)
            {
                Debug.Log("Despawning Enemy Pokemon");
                activePokemons[1].DespawnPokemon();
            }
        }

        if (WorldMaster.instance != null)
        {
            if (WorldMaster.instance.GetEmpty())
            {
                //WorldMaster.instance.UnloadCurrentBattleScene();
                //Debug.Log("Unloading");
            }
        }
    }
    #endregion

    #region Getters
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
        return spots;
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
    public void SetNewActivePokemonByIndex(int index)
    {

    }

    public void SpawnPokemon(Pokemon pokemon, int spotIndex)
    {
        BattleLog.instance.AddNewLog(name, "Spawning: " + pokemon.GetName());
        if (spotIndex < 0 || spotIndex >= spots.Length)
            return;
        GameObject obj = Instantiate(pokemon.GetPokemonPrefab());
        Transform trans = spots[spotIndex].GetTransform();

        activePokemons[spotIndex] = pokemon;

        pokemon.SetSpawnedObject(obj);

        obj.transform.position = trans.position;
        obj.transform.rotation = trans.rotation;
        obj.transform.parent = trans;

        spots[spotIndex].SetActivePokemon(pokemon);

        //Check if spawned object is placeholder;
        PokemonPlaceholder.CheckPlaceholder(pokemon, obj);
    }

    public void RemoveAction(BattleAction action)
    {
        if (actionList.Contains(action))
            actionList.Remove(action);
    }
    #endregion

    #region Internal
    public void StartBattle(BattleMember player, BattleMember[] enemies)
    {
        members[0] = player;

        for (int i = 0; i < enemies.Length; i++)
            members[i + 1] = enemies[i];

        string s = "Starting Battle Between:";
        foreach (BattleMember member in members)
            s += "\n    - " + member.GetName();
        BattleLog.instance.AddNewLog(name, s);
    }

    private void EndBattle()
    {

    }

    private void DisplayMovesForPokemon()
    {
        PokemonMove[] toDisplay = activePokemons[0].GetMoves();

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

    private void CheckActions()
    {
        BattleLog.instance.AddNewLog(name, "Checking Action Priority");

        //Setup
        List<BattleAction> tempList = new List<BattleAction>();
        foreach (Pokemon pokemon in activePokemons)
            tempList.Add(pokemon.GetBattleAction());

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
                    if (one.GetPriority() == two.GetPriority() && one.GetCurrentPokemon().GetStat(Stat.Speed) > two.GetCurrentPokemon().GetStat(Stat.Speed))
                    {
                        end = false;

                        actionList[i - 1] = one;
                        actionList[i] = two;
                    }
                }
            }
        }
    }

    public void SelectAction(int i)
    {
        if (i > 0 && i < 5)
        {
            PokemonMove move = activePokemons[0].GetMoveByIndex(i - 1);
            if (move != null)
            {
                move.SetCurrentPokemon(activePokemons[0]);
                move.SetTargetIndex(1);
                move.GetCurrentPokemon().SetBattleAction(move);
            }
        }
        else if (i == 5)
        {
            Debug.Log("Bag Not Implemented!");
            return;
            //selectionMenu.SetActive(true);
        }
        else if (i == 6)
        {
            Debug.Log("Bag Not Implemented!");
            return;
        }
        else
        {
            Debug.Log("Run Not Implemented!");
            return;
        }
    }

    public void SelectNewPokemon(int i)
    {
        if (i > 0 && i < 6)
        {
            Pokemon result = members[0].GetTeam().GetPokemonByIndex(i);

            if (result != null)
            {
                SwitchAction action = Instantiate(switchAction);
                action.SetCurrentPokemon(members[0].GetTeam().GetPokemonByIndex(0));

                for (int j = 0; j < activePokemons.Length; j++)
                {
                    if (activePokemons[j] == action.GetCurrentPokemon())
                    {
                        action.SetFieldSpot(j);
                        break;
                    }
                }
                action.SetNextPokemon(result);
                action.SetTeam(members[0].GetTeam());

                //pokemonActions[0] = action;

                selectionMenu.SetActive(false);
            }
        }
    }

    private int CheckFaintedAndDecide(Pokemon pokemon, int? direct = 0, int? indirect = 0)
    {
        if (pokemon.GetCurrentHealth() == 0)
        {
            Debug.Log("Fainted");

            Condition condition = faintCondtion.GetCondition();
            condition.SetAffectedPokemon(pokemon);
            pokemon.GetConditionOversight().TryApplyNonVolatileCondition(condition);

            return indirect.Value;
        }

        return direct.Value;
    }
    #endregion
}