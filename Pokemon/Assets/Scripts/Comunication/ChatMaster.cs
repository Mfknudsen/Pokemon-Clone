#region SDK

using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Mfknudsen.Comunication
{
    public class ChatMaster : MonoBehaviour
    {
        #region Values

        [Header("Object Reference:")] public static ChatMaster instance;
        [SerializeField] private Chat running;
        [SerializeField] private List<Chat> waitlist = new List<Chat>();
        private Coroutine chatCoroutine;

        [Header("Display:")] [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private KeyCode continueKey = 0;
        [SerializeField] private bool waitForInput = true;

        [Header("Chat Settings:")] [SerializeField]
        private int textPerSecond = 30;

        private int defaultTextSpeed;

        #endregion

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                defaultTextSpeed = textPerSecond;
            }
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (textField is null)
            {
                textField = TextField.instance;
                return;
            }

            if (!(running is null) && waitForInput)
            {
                if (running.GetNeedInput()) return;

                if (running.GetDone())
                {
                    running = null;
                    chatCoroutine = null;
                }
                else
                    chatCoroutine = StartCoroutine(running.PlayNext());

                waitForInput = false;
            }
            else if (running is null && waitlist.Count > 0)
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

        public bool GetIsClear()
        {
            if (running != null || waitlist.Count != 0) return false;

            textField.gameObject.SetActive(false);
            return true;
        }

        public float GetTextSpeed()
        {
            return 1 / textPerSecond;
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

        public void Add(Chat[] toAdd)
        {
            foreach (Chat c in toAdd)
                waitlist.Add(c.GetChat());
        }

        public void Add(Chat toAdd)
        {
            waitlist.Add(toAdd.GetChat());
        }

        [UsedImplicitly]
        public void OnNextChatChange(InputAction.CallbackContext value)
        {
            if (!value.performed || running == null) return;

            if (!running.GetNeedInput() || !waitForInput) return;

            if (running.GetDone())
            {
                running = null;
                chatCoroutine = null;
            }
            else
                chatCoroutine = StartCoroutine(running.PlayNext());

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
            running = waitlist[0];
            waitlist.RemoveAt(0);

            Play(running);
        }

        private void Play(Chat toPlay)
        {
            textField.gameObject.SetActive(true);

            running = toPlay;
            chatCoroutine = StartCoroutine(running.Play());
        }

        #endregion
    }
}