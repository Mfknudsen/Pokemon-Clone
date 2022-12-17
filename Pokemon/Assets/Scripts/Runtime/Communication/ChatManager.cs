#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Systems;
using Runtime.Systems.PersistantRunner;
using TMPro;
using UnityEngine;

#endregion

namespace Runtime.Communication
{
    [CreateAssetMenu(menuName = "Manager/Chat")]
    public class ChatManager : Manager, IFrameStart
    {
        #region Values
        
        [Header("Object Reference:")] [SerializeField]
        private Chat running;

        [SerializeField] private List<Chat> waitList = new();

        [Header("Display:")] [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private bool waitForInput = true;

        [Header("Chat Settings:")] [SerializeField]
        private int textPerSecond = 30;

        private PersistantRunner persistantRunner;
        
        #endregion

        #region Build In States

        public IEnumerator FrameStart(PersistantRunner persistantRunner)
        {
            this.persistantRunner = persistantRunner;
            InputManager.instance.nextChatInputEvent.AddListener(this.OnNextChatChange);

            this.ready = true;
            yield break;
        }

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

        public void FrameUpdate()
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
                    this.persistantRunner.StartCoroutine(this.running.PlayNext());

                this.waitForInput = false;
            }
            else if (this.running is null && this.waitList.Count > 0)
            {
                this.PlayNextInLine();
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
                this.persistantRunner.StartCoroutine(this.running.PlayNext());

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

            this.Play(this.running);
        }

        private void Play(Chat toPlay)
        {
            this.textField.gameObject.SetActive(true);

            this.running = toPlay;
            this.persistantRunner.StartCoroutine(this.running.Play());
        }

        #endregion
    }
}