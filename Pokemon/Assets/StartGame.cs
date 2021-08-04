#region SDK

using System.Collections;
using Mfknudsen.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Mfknudsen
{
    public class StartGame : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Setup());
        }

        private IEnumerator Setup()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            
            while (!asyncOperation.isDone)
                yield return null;

            while (UIManager.instance == null)
                yield return null;
            
            UIManager.instance.SwitchUI(UISelection.Start);
        }
    }
}