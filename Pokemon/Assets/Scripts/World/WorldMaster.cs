﻿#region SDK

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Mfknudsen.World
{
    public class WorldMaster : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        public static WorldMaster instance;
        private Coroutine currentOperation;

        [Header("Loading:")]
        [SerializeField] private float progressMeter = 0;

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
        public float GetLoadMeter()
        {
            return progressMeter;
        }

        public bool GetEmpty()
        {
            return (currentOperation == null);
        }
        #endregion

        #region In
        public void SpawnWorld()
        {

        }

        public void LoadScene(string sceneName)
        {
            currentOperation = StartCoroutine(LoadBattleSceneAsync(sceneName, false));
        }

        public void LoadBattleScene(string sceneName)
        {
            currentLoadedBattleScene = sceneName;
            currentOperation = StartCoroutine(LoadBattleSceneAsync(sceneName, true));
        }

        public void UnloadCurrentBattleScene()
        {
            currentOperation = StartCoroutine(UnloadBattleSceneAsync(currentLoadedBattleScene));
        }
        #endregion

        #region IEnumerator
        #region Load/Unload Battle Scenes
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
                progressMeter = asyncLoad.progress + 0.1f;
                yield return null;
            }

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

        #region Load/Unload World Scenes
        private IEnumerator SpawnWorldAsync()
        {
            yield return null;
        }

        private IEnumerator LoadWorldSceneAsync()
        {
            yield return null;
        }

        private IEnumerator DeloadWorldSceneAsync()
        {
            yield return null;
        }
        #endregion
        #endregion
    }
}