#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Systems;
using TMPro;
using UnityEngine;

#endregion

namespace Runtime.Communication
{
    [CreateAssetMenu(menuName = "Manager/Chat")]
    public class ChatManager : Manager
    {
        #region Values

        private ChatController controller;

        [Header("Object Reference:")] [SerializeField]
        private Chat running;

        [SerializeField] private List<Chat> waitList = new();

        [Header("Display:")] [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private bool waitForInput = true;

        [Header("Chat Settings:")] [SerializeField]
        private int textPerSecond = 30;

        #endregion

        #region Build In States

        public override IEnumerator StartManager()
        {
            this.controller = new GameObject("Chat Controller").AddComponent<ChatController>();

            InputManager.instance.nextChatInputEvent.AddListener(OnNextChatChange);
            
            yield break;
        }

        private void OnDisable() => InputManager.instance.nextChatInputEvent.RemoveListener(OnNextChatChange);

        #endregion

        #region Getters

        public bool GetIsClear()
        {
            return this.running is null && this.waitList.Count == 0;
        }

        public float GetTextSpeed()
        {
            float result = 1;
            result /= this.textPerSecond;
            return result;
        }

        #endregion

        #region Setters

        public void SetDisplayText(string text) => this.textField.text = text;

        public void SetTextField(TextMeshProUGUI newTextField)
        {
            string currentText = this.textField.text;
            this.textField = newTextField;
            this.textField.text = currentText;
        }

        public void SetTextSpeed(int speed)
        {
            this.textPerSecond = speed;
        }

        #endregion

        #region In

        public override void UpdateManager()
        {
            if (this.textField is null)
            {
                this.textField = TextField.instance;
                return;
            }

            if (this.running is not null && this.waitForInput)
            {
                if (this.running.GetNeedInput()) return;

                if (this.running.GetDone())
                    this.running = null;
                else
                    this.controller.StartCoroutine(this.running.PlayNext());

                this.waitForInput = false;
            }
            else if (this.running is null && this.waitList.Count > 0)
            {
                PlayNextInLine();
            }
        }

        public void Add(IEnumerable<Chat> toAdd)
        {
            foreach (Chat c in toAdd)
                this.waitList.Add(c.GetChat());
        }

        public void Add(Chat toAdd)
        {
            this.waitList.Add(toAdd.GetChat());
        }

        #endregion

        #region Internal

        private void OnNextChatChange()
        {
            if (this.running == null || !this.running.GetNeedInput() || !this.waitForInput) return;

            if (this.running.GetDone())
                this.running = null;
            else
                this.controller.StartCoroutine(this.running.PlayNext());

            this.waitForInput = false;
        }

        public void CheckRunningState()
        {
            this.waitForInput = true;
        }

        private void PlayNextInLine()
        {
            this.running = this.waitList[0];
            this.waitList.RemoveAt(0);

            Play(this.running);
        }

        private void Play(Chat toPlay)
        {
            this.textField.gameObject.SetActive(true);

            this.running = toPlay;
            this.controller.StartCoroutine(this.running.Play());
        }

        #endregion
    }
}