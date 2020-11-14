using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;
using UnityEngine.SceneManagement;


public class WorldMaster : MonoBehaviour
{
    public static WorldMaster instance;
    public BattleMember player = null, enemy = null;
   [SerializeField] private Scene battleScene;

    private void Start()
    {
        if (instance == null)
        {
            player.GetTeam().Setup();
            enemy.GetTeam().Setup();

            instance = this;
        }
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        if (enemy != null)
            DontDestroyOnLoad(enemy.gameObject);
    }

    private void Update()
    {
        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(LoadSceneAsync());
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TestBattle");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        battleScene = SceneManager.GetSceneByName("TestBattle");

        BattleMaster.instance.StartBattle(player, new BattleMember[] { enemy });

        player = null;
        enemy = null;
    }
}
