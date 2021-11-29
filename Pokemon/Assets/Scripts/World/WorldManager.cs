#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Player;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI.Scene_Transitions;
using Mfknudsen.UI.Scene_Transitions.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable Unity.PreferAddressByIdToGraphicsParams

#endregion

namespace Mfknudsen.World
{
    public class WorldManager : Manager
    {
        #region Values

        [Header("Object Reference:")] public static WorldManager instance;
        private Coroutine currentOperation;

        [Header("Loading:")] [SerializeField] private float progressMeter;

        [Header("Battle Scene:")] [SerializeField]
        private string currentLoadedBattleScene = "";

        private Transition transition;
        private readonly List<Coroutine> activeLoading = new List<Coroutine>(), activeUnloading = new List<Coroutine>();

        #endregion

        #region Build In States

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

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

        public bool GetActiveLoading()
        {
            return activeLoading.Count == 0;
        }

        public bool GetActiveUnloading()
        {
            return activeUnloading.Count == 0;
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

        public override void Setup()
        {
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
            //Start Transition
            yield return StartTransition();

            PlayerManager.instance.DisableOverworld();

            //Scene Loading
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            progressMeter = 0;

            while (!asyncLoad.isDone)
            {
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }

            SetupManager.instance.Trigger();
            
            currentLoadedBattleScene = sceneName;

            currentOperation = null;

            PlayerManager.instance.EnableOverworld();

            //End Transition
            yield return EndTransition();
        }

        private IEnumerator UnloadBattleSceneAsync(string sceneName)
        {
            //Start Transition
            yield return StartTransition();

            //Scene Unloading
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

            progressMeter = 0;

            while (!asyncUnload.isDone)
            {
                progressMeter = (int) (asyncUnload.progress + 0.1f) * 100;
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
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            progressMeter = 0;

            while (!asyncLoad.isDone)
            {
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }

            currentOperation = null;
        }

        private IEnumerator UnloadWorldSceneAsync(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
                yield return null;
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

        #endregion
    }
}