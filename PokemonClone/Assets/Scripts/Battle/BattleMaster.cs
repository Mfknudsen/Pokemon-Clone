using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Trainer;
using BattleUI;

public enum MasterState { Setup, Starting, ChoosingMove, AI, Checking, Action, RoundDone, SelectNew }

public enum Weather { Rain, HarshSunlight, Hail }

public class BattleMaster : MonoBehaviour
{
    #region Values
    public static BattleMaster instance;
    public MasterState state = 0;
    public static Weather weather = 0;

    [Header("Members:")]
    public BattleMember player = null;
    public BattleMember enemy = null;

    [Header("Pokemons:")]
    public Pokemon playerPokemon = null;
    public Pokemon enemyPokemon = null;
    private BattleAction playerAction = null;
    private BattleAction enemyAction = null;

    [Header("Transforms")]
    public Transform playerTransform = null;
    public Transform enemyTransform = null;

    [Header("States:")]
    string splacerholder;
    [Header(" -- SetUp:")]
    int iplaceholder;
    [Header(" -- Action Selection:")]
    float fplaceholder;
    [Header(" -- Checking:")]
    [SerializeField] private List<BattleAction> instantActions = new List<BattleAction>(), fastActions = new List<BattleAction>(), normalActions = new List<BattleAction>(), slowActions = new List<BattleAction>();
    [Header(" -- Actions:")]
    [SerializeField] float secondsPerPokemonMove = 1;
    [SerializeField] PokemonMove currentPokeMove = null;
    [SerializeField] private int actionIndex = 0;
    Coroutine actionOperation = null;
    [Header(" -- Select New:")]
    Vector3 v3placeholder;

    [Header("UI:")]
    [SerializeField]
    public BattleDisplay display = null;
    public PokemonDisplay playerPokemonDisplay = null, enemyPokemonDisplay = null;
    [Header(" -- Turn Display:")]
    public GameObject turnDisplay = null;
    public TextMeshProUGUI[] buttonsText = new TextMeshProUGUI[4];
    [Header(" -- Selectio Menu:")]
    public GameObject selectionMenu = null;

    [Header("Chat:")]
    [SerializeField] private Chat SwitchChat = null;
    #endregion

    private void Start()
    {
        instance = this;

        if (player == null)
            player = Player.MasterPlayer.instance.gameObject.GetComponent<BattlePlayer>();

        state = MasterState.Setup;
    }

    private void Update()
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

    #region States
    private void Setup()
    {
        BattleEnemy enemyChat = enemy as BattleEnemy;

        ChatMaster.instance.Add(enemyChat.GetStartChat());

        state = MasterState.Starting;
    }

    private void Starting()
    {
        if (ChatMaster.instance.GetIsClear())
        {
            if (playerPokemon != null && enemyPokemon != null)
            {
                state = MasterState.ChoosingMove;
            }
            else if (playerPokemon == null && enemyPokemon == null)
            {
                if (!player.GetTeam().GetReady())
                    player.GetTeam().Setup();
                playerPokemon = player.GetTeam().GetPokemonByIndex(0);
                playerPokemonDisplay.SetNewPokemon(playerPokemon);
                playerPokemon.SpawnPokemon(playerTransform, true);

                if (!enemy.GetTeam().GetReady())
                    enemy.GetTeam().Setup();
                enemyPokemon = enemy.GetTeam().GetPokemonByIndex(0);
                enemyPokemonDisplay.SetNewPokemon(enemyPokemon);
                enemyPokemon.SpawnPokemon(enemyTransform, false);
            }
        }
    }

    private void ChoosingMove()
    {
        if (!turnDisplay.activeSelf)
        {
            turnDisplay.SetActive(true);
            DisplayForPokemon();
        }

        if (playerAction != null)
            state = MasterState.AI;
    }

    private void AI()
    {
        //
        // For TEST without AI
        if (enemyAction == null)
        {
            enemyAction = Instantiate(enemyPokemon.GetMoveByIndex(0));
            PokemonMove move = enemyAction as PokemonMove;
            move.SetCurrentPokemon(enemyPokemon);
            move.SetTargetPokemon(new Pokemon[] { playerPokemon });
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
        PokemonMove action = null;
        if (actionIndex >= 0 && actionIndex < normalActions.Count)
            action = normalActions[actionIndex] as PokemonMove;

        if (actionOperation == null && ChatMaster.instance.GetIsClear())
        {
            if (action != null)
            {
                bool check = true;

                if (action == playerAction && playerPokemon.GetCurrentHealth() == 0)
                    check = false;
                else if (action == enemyAction && enemyPokemon.GetCurrentHealth() == 0)
                    check = false;

                if (check)
                {
                    actionOperation = StartCoroutine(action.Activate());
                }
            }
            else
            {
                actionIndex++;
                actionOperation = null;

                if (actionIndex == normalActions.Count)
                {
                    actionIndex = 0;
                    normalActions.Clear();
                    enemyAction = null;
                    playerAction = null;

                    if (playerPokemon.GetCurrentHealth() == 0 || enemyPokemon.GetCurrentHealth() == 0)
                        state = MasterState.RoundDone;
                    else
                        state = MasterState.ChoosingMove;
                }
            }
        }
        else if (actionOperation != null)
        {
            if (action.IsDone() && ChatMaster.instance.GetIsClear())
            {
                actionIndex++;
                actionOperation = null;

                if (actionIndex == normalActions.Count)
                {
                    actionIndex = 0;
                    normalActions.Clear();
                    enemyAction = null;
                    playerAction = null;

                    if (playerPokemon.GetCurrentHealth() == 0 || enemyPokemon.GetCurrentHealth() == 0)
                        state = MasterState.RoundDone;
                    else
                        state = MasterState.ChoosingMove;
                }
            }
        }
    }

    private void RoundDone()
    {
        if (playerPokemon.GetCurrentHealth() == 0)
        {
            Debug.Log("You Lose!");
            playerPokemon = null;
        }
        else if (enemyPokemon.GetCurrentHealth() == 0)
        {
            Debug.Log("You Win!");
            enemyPokemon = null;
        }

        state = MasterState.SelectNew;
    }

    private void SelectNew()
    {
        if (WorldMaster.instance != null)
        {
            if (WorldMaster.instance.GetEmpty())
            {
                WorldMaster.instance.UnloadCurrentBattleScene();
                Debug.Log("Unloading");
            }
        }
    }
    #endregion

    #region Getters
    public float GetSecPerPokeMove()
    {
        return secondsPerPokemonMove;
    }
    #endregion

    #region In
    public void CheckOnAction()
    {
        PokemonMove m = normalActions[actionIndex] as PokemonMove;

        if (m.IsDone())
        {
            //Update();
        }
    }
    #endregion

    #region Out

    #endregion

    #region Internal
    public void StartBattle(BattleMember player, BattleMember[] enemies)
    {
        this.player = player;
        enemy = enemies[0];
    }

    private void EndBattle()
    {

    }

    private void DisplayForPokemon()
    {
        PokemonMove[] toDisplay = playerPokemon.GetMoves();
        for (int i = 0; i < 4; i++)
        {
            if (toDisplay[i] != null)
            {
                buttonsText[i].text = toDisplay[i].GetName();
            }
            else
                buttonsText[i].text = "";
        }
    }

    private void CheckActions()
    {
        if (enemyPokemon.GetSpeed() < playerPokemon.GetSpeed())
        {
            normalActions.Add(playerAction);
            normalActions.Add(enemyAction);
        }
        else
        {
            normalActions.Add(enemyAction);
            normalActions.Add(playerAction);
        }
    }

    public void SelectAction(int i)
    {
        if (i > 0 && i < 5)
        {
            playerAction = Instantiate(playerPokemon.GetMoveByIndex(i - 1));
            PokemonMove move = playerAction as PokemonMove;
            move.SetCurrentPokemon(playerPokemon);
            move.SetTargetPokemon(new Pokemon[] { enemyPokemon });
        }
        else if (i == 5)
        {
            selectionMenu.SetActive(true);
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
        Pokemon result = null;

        if (i > 0 && i < 6)
        {
            result = player.GetTeam().GetPokemonByIndex(i);

            if (result != null)
            {
                player.GetTeam().SwitchTeamPlaces(0, i);

                playerPokemon.DespawnPokemon();

                playerPokemon = player.GetTeam().GetPokemonByIndex(0);

                playerPokemon.SpawnPokemon(playerTransform, true);

                playerPokemonDisplay.SetNewPokemon(playerPokemon);

                selectionMenu.SetActive(false);

                ChatMaster.instance.Add(new Chat[] { SwitchChat.GetChat() });

                state = MasterState.AI;
            }
        }
    }
    #endregion
}