using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;
using UnityEngine.SceneManagement;


public class WorldMaster : MonoBehaviour
{
    #region Values
    [Header("Object Reference:")]
    public static WorldMaster instance;
    public BattleMember player = null, enemy = null;
    private Coroutine currentOperation;

    [Header("Loading:")]
    [SerializeField] private int progressMeter = 0;

    [Header("Battle Scene:")]
    [SerializeField] private string currentLoadedBattleScene = "";
    #endregion

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    #region Getters
    public int GetLoadMeter()
    {
        return progressMeter;
    }

    public bool GetEmpty()
    {
        return (currentOperation == null);
    }
    #endregion

    #region In
    public void LoadTestBattleScene()
    {
        currentOperation = StartCoroutine(LoadBattleSceneAsync("TestBattle", false));
    }

    public void UnloadCurrentBattleScene()
    {
        currentOperation = StartCoroutine(UnloadBattleSceneAsync(currentLoadedBattleScene));
    }
    #endregion

    #region Load/Unload Battle Scene
    private IEnumerator LoadBattleSceneAsync(string sceneName, bool additive)
    {
        AsyncOperation asyncLoad;

        if (additive)
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        else
            asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        progressMeter = 0;

        while (!asyncLoad.isDone)
        {
            progressMeter = (int)(asyncLoad.progress + 0.1f) * 100;
            yield return null;
        }

        BattleMaster.instance.StartBattle(player, new BattleMember[] { enemy });

        currentOperation = null;
    }

    private IEnumerator UnloadBattleSceneAsync(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        progressMeter = 0;

        while (!asyncUnload.isDone)
        {
            progressMeter = (int)(asyncUnload.progress + 0.1f) * 100;
            yield return null;
        }

        currentOperation = null;
    }
    #endregion
}
