#region SDK

using System.Collections.Generic;
using TMPro;
using UnityEngine;

#endregion

namespace Mfknudsen.Chat
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
        private float textPerSecond = 30;

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

        private void Update()
        {
            if (textField == null)
                textField = TextField.instance;

            if (running != null && waitForInput)
            {
                if ((Input.GetKeyDown(continueKey) || Input.GetKeyDown(KeyCode.Mouse0)) || !running.GetNeedInput())
                {
                    if (running != null)
                    {
                        if (running.GetDone())
                        {
                            running = null;
                            chatCoroutine = null;
                        }
                        else
                            chatCoroutine = StartCoroutine(running.PlayNext());
                    }

                    waitForInput = false;
                }
            }
            else if (running == null && waitlist.Count > 0)
            {
                PlayNextInLine();
            }
        }

        #region Defaults

        public void DefaultTextSpeed()
        {
            textPerSecond = 20;
        }

        #endregion

        #region Getters

        public bool GetIsClear()
        {
            if (running == null && waitlist.Count == 0)
            {
                textField.gameObject.SetActive(false);
                return true;
            }

            return false;
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

        public void SetTextSpeed(float speed)
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