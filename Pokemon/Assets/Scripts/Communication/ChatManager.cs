#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Settings.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Communication
{
    public class ChatManager : Manager
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

        #region Build In States

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

        #endregion

        #region Getters

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

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            defaultTextSpeed = textPerSecond;

            InputManager.instance.nextChatInputEvent.AddListener(OnNextChatChange);

            yield break;
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

        public void OnNextChatChange()
        {
            if (running == null || !running.GetNeedInput() || !waitForInput) return;

            if (running.GetDone())
                running = null;
            else
                StartCoroutine(running.PlayNext());

            waitForInput = false;
        }


        #region Defaults

        public void DefaultTextSpeed()
        {
            textPerSecond = defaultTextSpeed;
        }

        #endregion

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