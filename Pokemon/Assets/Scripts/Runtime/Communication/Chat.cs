#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Player;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Communication
{
    [CreateAssetMenu(fileName = "Chat", menuName = "Chat/Create new Standard Chat", order = 0)]
    public class Chat : ScriptableObject
    {
        #region Values

        [SerializeField, Required] protected ChatManager chatManager;
        [SerializeField, Required] private PlayerManager playerManager;

        [Header("Object Reference:")] [SerializeField]
        protected bool isInstantiated;

        [SerializeField] protected string location;
        [SerializeField, TextArea] protected string description;
        [SerializeField, TextArea] protected string[] textList;
        [SerializeField] protected float[] timeToNext;

        [SerializeField] protected bool hideAfter;

        [Header("Continuation:")] [SerializeField]
        protected bool needInput = true;

        [Header("Overriding:")] [SerializeField]
        protected List<string> replaceString = new();

        [SerializeField] protected List<string> addString = new();

        [Header("Text Animation:")] [SerializeField]
        protected string showText;

        [SerializeField] protected int index;
        [SerializeField] protected bool active, waiting, done;
        [SerializeField] protected int nextCharacter;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            this.textList ??= Array.Empty<string>();
            this.timeToNext ??= Array.Empty<float>();

            if (this.textList.Length > this.timeToNext.Length)
            {
                float[] arr = new float[this.textList.Length];

                for (int i = 0; i < this.timeToNext.Length; i++)
                    arr[i] = this.timeToNext[i];

                this.timeToNext = arr;
            }
            else if (this.textList.Length < this.timeToNext.Length)
            {
                float[] arr = new float[this.textList.Length];

                for (int i = 0; i < this.textList.Length; i++)
                    arr[i] = this.timeToNext[i];

                this.timeToNext = arr;
            }
        }

        #endregion

        #region Getters

        public Chat GetChatInstantiated()
        {
            Chat result = this;

            if (this.isInstantiated) return result;

            result = Instantiate(result);
            result.SetIsInstantiated();

            return result;
        }

        public bool GetDone() =>
            this.done;

        public bool GetNeedInput() =>
            this.needInput;

        public int GetListCount =>
            this.textList.Length;

        public string GetTextByIndex(int i) =>
            this.textList[i];

        public float GetTimeToNextByIndex(int i) =>
            this.timeToNext[i];

        public bool GetHideAfter() => 
            this.hideAfter;

        #endregion

        #region Setters

        private void SetIsInstantiated() =>
            this.isInstantiated = true;

#if UNITY_EDITOR
        public void SetTextByIndex(int i, string set) => this.textList[i] = set;

        public void SetTimeToNextByIndex(int i, float set) => this.timeToNext[i] = set;
#endif

        #endregion

        #region In

#if UNITY_EDITOR
        public void CreateNew()
        {
            int i = this.textList.Length + 1;
            string[] sArr = new string[i];
            float[] fArr = new float[i];

            for (int j = 0; j < this.textList.Length; j++)
            {
                sArr[j] = this.textList[j];
                fArr[j] = this.timeToNext[j];
            }

            this.textList = sArr;
            this.timeToNext = fArr;
        }

        public void DeleteByIndex(int i)
        {
            int length = this.textList.Length - 1;
            string[] sArr = new string[length];
            float[] fArr = new float[length];

            int offset = 0;

            for (int j = 0; j < this.textList.Length; j++)
            {
                if (j == i)
                {
                    offset = -1;
                    continue;
                }

                sArr[j + offset] = this.textList[j];
                fArr[j + offset] = this.timeToNext[j];
            }

            this.textList = sArr;
            this.timeToNext = fArr;
        }
#endif

        public void AddToOverride(string replace, string add)
        {
            this.replaceString.Add(replace);
            this.addString.Add(add);
        }

        #endregion

        #region Out

        public IEnumerator Play()
        {
            this.Setup();

            while (!this.done && !this.waiting && this.index < this.textList.Length)
            {
                string tempText = "", fromList = this.textList[this.index];
                float relativeSpeed = this.chatManager.GetTextSpeed();

                if (this.nextCharacter + 1 < fromList.Length)
                    tempText = "" + fromList[this.nextCharacter] + fromList[this.nextCharacter + 1];

                if (tempText == "\n")
                {
                    this.showText += "\n";
                    this.nextCharacter += 2;
                    relativeSpeed *= 1.5f;
                }
                else if (this.nextCharacter < fromList.Length)
                {
                    this.showText += fromList[this.nextCharacter];
                    this.nextCharacter++;
                }

                if (this.showText.Length != 0)
                    this.chatManager.SetDisplayText(this.showText);

                if (this.showText.Length == fromList.Length)
                {
                    yield return new WaitForSeconds(this.timeToNext[this.index]);

                    if (this.index < this.textList.Length - 1)
                        this.waiting = true;
                    else
                        this.OnFinalDone();

                    this.chatManager.CheckRunningState();
                }

                yield return new WaitForSeconds(relativeSpeed);
            }

            while (this.waiting)
                yield return null;
        }

        public IEnumerator PlayNext()
        {
            this.IncreaseIndex();

            this.done = false;

            return this.Play();
        }

        #endregion

        #region Internal

        protected virtual void Setup()
        {
            if (this.active) return;

            this.index = 0;
            this.CheckTextOverride();
            this.active = true;
            this.chatManager.ShowTextField(true);
        }

        protected virtual void OnFinalDone() =>
            this.done = true;

        private void IncreaseIndex()
        {
            this.index++;
            this.showText = "";
            this.nextCharacter = 0;
            this.waiting = false;
        }

        private void CheckTextOverride()
        {
            this.AddPronounsToOverride();

            for (int i = 0; i < this.textList.Length; i++)
            {
                for (int j = 0; j < this.replaceString.Count; j++)
                {
                    if (j < this.replaceString.Count && j < this.addString.Count)
                        this.textList[i] = this.textList[i].Replace(this.replaceString[j], this.addString[j]);
                }
            }
        }

        private void AddPronounsToOverride()
        {
            string[] pronouns = this.playerManager.GetPronouns();

            this.AddToOverride("P_ONE", pronouns[0]);
            this.AddToOverride("P_TWO", pronouns[1]);
        }

        #endregion
    }
}