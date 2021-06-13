#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Monster;
using Mfknudsen.World.Overworld;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.PC
{
    [RequireComponent(typeof(SphereCollider))]
    public class Box : MonoBehaviour, InteractableInterface
    {
        #region Values
        [SerializeField] private GameObject uiPrefab = null, uiInstance = null;
        [SerializeField] private Dictionary<int, Pokemon> inBox = new Dictionary<int, Pokemon>();

        private void OnValidate()
        {
            for (int i = 0; i < (6 * 10 * 5); i++)
            {
                if (!inBox.ContainsKey(i))
                    inBox.Add(i, null);
            }
        }
        #endregion

        #region In
        public void ShowNextBox()
        {

        }

        public void ShowPreviousBox()
        {

        }

        public void InteractNow()
        {
            StartCoroutine(StartConsole());
        }
        #endregion

        #region IEnumerator
        private IEnumerator StartConsole()
        {
            Debug.Log("Starting Console");

            if(uiInstance == null)
            {
                uiInstance = Instantiate(uiPrefab);
                uiInstance.transform.parent = GameObject.FindGameObjectWithTag("UI Canvas").transform;
            }

            yield return 0;

            Debug.Log("Console is Started");

            uiInstance.SetActive(true);
        }
        #endregion
    }
}