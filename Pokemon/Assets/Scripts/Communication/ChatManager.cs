#region SDK

using System.Collections.Generic;
using JetBrains.Annotations;
using Mfknudsen.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Communication
{
    public class ChatManager : MonoBehaviour, ISetup
    {
        #region Values

        [Header("Object Reference:")] public static ChatManager instance;
        [SerializeField] private Chat running;
        [SerializeField] private List<Chat> waitList = new List<Chat>();

        [Header("Display:")] [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private bool waitForInput = true;

        [Header("Chat Settings:")] [SerializeField]
        private int textPerSecond = 30;

        private int defaultTextSpeed;

        #endregion

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

        private void Update()
        {
            if (textField == null)
            {
                textField = TextField.instance;
                return;
            }

            if (running != null && waitForInput)
            {
                if (running.GetNeedInput()) return;

                if (running.GetDone())
                    running = null;
                else
                    StartCoroutine(running.PlayNext());

                waitForInput = false;
            }
            else if (running is null && waitList.Count > 0)
            {
                PlayNextInLine();
            }
        }

        #region Defaults

        public void DefaultTextSpeed()
        {
            textPerSecond = defaultTextSpeed;
        }

        #endregion

        #region Getters

        public int Priority()
        {
            return 1;
        }
        
        public bool GetIsClear()
        {
            return running == null && waitList.Count == 0;
        }

        public float GetTextSpeed()
        {
            float result = 1;
            result /= textPerSecond;
            return result;
        }

        #endregion

        #region Setters

        public void SetDisplayText(string text)
        {
            textField.text = text;
        }

        public void SetTextField(TextMeshProUGUI newTextField)
        {
            string currentText = textField.text;
            textField = newTextField;
            textField.text = currentText;
        }

        public void SetTextSpeed(int speed)
        {
            textPerSecond = speed;
        }

        #endregion

        #region In

        public void Setup()
        {
            defaultTextSpeed = textPerSecond;
        }
        

        public void Add(Chat[] toAdd)
        {
            foreach (Chat c in toAdd)
                waitList.Add(c.GetChat());
        }

        public void Add(Chat toAdd)
        {
            waitList.Add(toAdd.GetChat());
        }

        [UsedImplicitly]
        public void OnNextChatChange(InputAction.CallbackContext value)
        {
            if (!value.performed || running == null) return;

            if (!running.GetNeedInput() || !waitForInput) return;

            if (running.GetDone())
                running = null;
            else
                StartCoroutine(running.PlayNext());

            waitForInput = false;
        }

        #endregion

        #region Internal

        public void CheckRunningState()
        {
            waitForInput = true;
        }

        private void PlayNextInLine()
        {
            running = waitList[0];
            waitList.RemoveAt(0);

            Play(running);
        }

        private void Play(Chat toPlay)
        {
            textField.gameObject.SetActive(true);

            running = toPlay;
            StartCoroutine(running.Play());
        }

        #endregion
    }
}