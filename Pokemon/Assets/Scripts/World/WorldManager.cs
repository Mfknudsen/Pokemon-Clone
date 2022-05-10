#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Files;
using Mfknudsen.Player;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI;
using Mfknudsen.UI.Scene_Transitions;
using Mfknudsen.UI.Scene_Transitions.Transitions;
using Mfknudsen.World.Overworld.Tiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Mfknudsen._Debug.Logger;

#endregion

namespace Mfknudsen.World
{
    public class WorldManager : Manager
    {
        #region Values

        public static WorldManager instance;

        private Coroutine currentOperation;

        [SerializeField] private float progressMeter;

        [SerializeField] private string currentLoadedBattleScene, currentLoadedWorldScene;

        private Transition transition;

        private readonly List<Coroutine> activeLoading = new(), activeUnloading = new();

        private StoryTriggers storyTriggers;
        private const string fileName = "StoryTriggers";

        #endregion

        #region Getters

        public float GetLoadMeter()
        {
            return progressMeter;
        }

        public bool GetEmpty()
        {
            return (currentOperation == null);
        }

        public bool GetIsLoading()
        {
            return activeLoading.Count == 0;
        }

        public bool GetActiveUnloading()
        {
            return activeUnloading.Count == 0;
        }

        public string GetCurrentLoadedWorldScene()
        {
            return currentLoadedWorldScene;
        }

        #endregion

        #region Setters

        public void SetTransition(Transition set)
        {
            transition = set;

            transition.SetTransitionParent(SceneTransitionUI.instance);
        }

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            storyTriggers = FileManager.LoadData<StoryTriggers>(fileName);

            yield break;
        }

        public void LoadSceneAsync(string sceneName)
        {
            activeLoading.Add(StartCoroutine(LoadWorldSceneAsync(sceneName)));
        }

        public void UnloadSceneAsync(string sceneName)
        {
            activeUnloading.Add(StartCoroutine(UnloadWorldSceneAsync(sceneName)));
        }

        public void LoadBattleScene(string sceneName)
        {
            activeLoading.Add(StartCoroutine(LoadBattleSceneAsync(sceneName)));
        }

        public void UnloadCurrentBattleScene()
        {
            activeUnloading.Add(StartCoroutine(UnloadBattleSceneAsync(currentLoadedBattleScene)));
        }

        #endregion

        #region Internal

        #region Load/Unload Battle Scenes

        private IEnumerator LoadBattleSceneAsync(string sceneName)
        {
            Logger.AddLog(ToString(), "Loading Battle Scene Async: \n" + sceneName);

            //Start Transition
            yield return StartTransition();

            PlayerManager.instance.DisableOverworld();

            //Scene Loading
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            progressMeter = 0;

            if (asyncLoad == null)
            {
                Debug.LogWarning("No scene loaded");
                yield break;
            }

            while (!asyncLoad.isDone)
            {
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }
            
            SetupManager.instance.Trigger();

            currentLoadedBattleScene = sceneName;

            PlayerManager.instance.EnableOverworld();

            //End Transition
            yield return EndTransition();
        }

        private IEnumerator UnloadBattleSceneAsync(string sceneName)
        {
            Logger.AddLog(ToString(), "Unloading Battle Scene Async: \n" + sceneName);

            //Start Transition
            yield return StartTransition();

            //Scene Unloading
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

            progressMeter = 0;

            while (!asyncUnload.isDone)
            {
                progressMeter = (int)(asyncUnload.progress + 0.1f) * 100;
                yield return null;
            }

            currentOperation = null;

            //End Transition
            yield return EndTransition();

            PlayerManager.instance.EnableOverworld();
        }

        #endregion

        #region Load/Unload World Scenes

        private IEnumerator LoadWorldSceneAsync(string sceneName)
        {
            Logger.AddLog(ToString(), "Loading World Scene Async: \n" + sceneName);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            progressMeter = 0;

            UIManager.instance.ActivateLoadingUI(true);

            while (!asyncLoad.isDone)
            {
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }

            SetupManager.instance.Trigger();

            UIManager.instance.ActivateLoadingUI(false);

            currentOperation = null;
        }

        private IEnumerator UnloadWorldSceneAsync(string sceneName)
        {
            Logger.AddLog(ToString(), "Unloading World Scene Async: \n" + sceneName);

            UnloadWorldInterfaces(sceneName);

            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
                yield return null;

            currentOperation = null;
        }

        #endregion

        #region Transitions

        private IEnumerator StartTransition()
        {
            yield return transition.Trigger(true);
        }

        private IEnumerator EndTransition()
        {
            yield return transition.Trigger(false);
        }

        #endregion

        private static void UnloadWorldInterfaces(string sceneName)
        {
            List<GameObject> toUnload = new List<GameObject>();

            TileSubManager subManager = TileManager.instance.GetSubManagerByName(sceneName);
            if (subManager != null)
                toUnload.Add(subManager.gameObject);

            while (toUnload.Count > 0)
            {
                GameObject obj = toUnload[0];

                foreach (IUnload i in obj.GetComponents<IUnload>())
                    i.Unload();

                for (int i = 0; i < obj.transform.childCount; i++)
                    toUnload.Add(obj.transform.GetChild(i).gameObject);

                toUnload.RemoveAt(0);
            }
        }

        #endregion
    }
}