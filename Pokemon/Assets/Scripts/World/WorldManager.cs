#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Player;
using Mfknudsen.Settings.Manager;
using Mfknudsen.UI;
using Mfknudsen.UI.Scene_Transitions;
using Mfknudsen.UI.Scene_Transitions.Transitions;
using Mfknudsen.World.Overworld.TileS;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable Unity.PreferAddressByIdToGraphicsParams

#endregion

namespace Mfknudsen.World
{
    public class WorldManager : Manager
    {
        #region Values

        public static WorldManager instance;

        private Coroutine currentOperation;

        [SerializeField] private float progressMeter;

        [SerializeField] private string currentLoadedBattleScene;

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

            PlayerManager.Instance.DisableOverworld();

            //Scene Loading
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            progressMeter = 0;

            while (!asyncLoad.isDone)
            {
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }

            SetupManager.Instance.Trigger();

            currentLoadedBattleScene = sceneName;

            PlayerManager.Instance.EnableOverworld();

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

            PlayerManager.Instance.EnableOverworld();
        }

        #endregion

        #region Load/Unload World Scenes

        private IEnumerator LoadWorldSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            progressMeter = 0;

            UIManager.Instance.ActivateLoadingUI(true);

            while (!asyncLoad.isDone)
            {
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }

            SetupManager.Instance.Trigger();

            UIManager.Instance.ActivateLoadingUI(false);

            currentOperation = null;
        }

        private IEnumerator UnloadWorldSceneAsync(string sceneName)
        {
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

        private void UnloadWorldInterfaces(string sceneName)
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