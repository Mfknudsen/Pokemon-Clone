#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Settings.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Mfknudsen._Debug
{
    public class Logger : MonoBehaviour
    {
        #region Values

        public static Logger instance;
        [SerializeField] private bool active, show;
        [SerializeField] private TextMeshProUGUI textField;

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<string> textLog = new List<string>();
        [SerializeField] private Scrollbar scroller;

        #endregion

        private void Start()
        {
            if (instance != null) return;

            instance = this;
            textField.text = "";

            StartCoroutine(SetupInput());
            ShowHide();
        }

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject obj = transform.GetChild(i).gameObject;

                obj.SetActive(active);
            }
        }

        #region In

        // ReSharper disable Unity.PerformanceAnalysis
        public void AddNewLog(string script, string input)
        {
            string scriptText = script + "[" + System.DateTime.Now.ToLocalTime().ToString("HH:mm:ss") + "]: ";
            textLog.Add(scriptText + input);

            textField.text += scriptText + "\n" + input + "\n";

            Invoke(nameof(ScrollControl), 0.01f);
        }

        #region Statics

        public static void AddLog(string script, string input)
        {
            instance.AddNewLog(script, input);
        }

        #endregion

        #endregion

        #region Out

        private void ScrollControl()
        {
            scroller.value = 0;
        }

        #endregion

        #region Internal

        private void ShowHide()
        {
            foreach (Transform t in transform)
                t.gameObject.SetActive(show);
            
            show = !show;
        }

        private IEnumerator SetupInput()
        {
            yield return new WaitWhile(() => !InputManager.instance);
            
            InputManager.instance.showHideEvent.AddListener(ShowHide);
        }

        #endregion
    }
}