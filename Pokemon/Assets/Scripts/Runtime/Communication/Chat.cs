#region Packages

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

        [SerializeField, Required] private ChatManager chatManager;
        [SerializeField, Required] private PlayerManager playerManager;

        [Header("Object Reference:")] [SerializeField]
        protected bool isInstantiated;

        [SerializeField] protected string location;
        [SerializeField, TextArea] protected string description;
        [SerializeField, TextArea] protected string[] textList;

        [Header("Continuation:")] [SerializeField]
        protected bool needInput = true;

        [SerializeField] protected float timeUntilNext = 0.75f;

        [Header("Overriding:")] [SerializeField]
        protected List<string> replaceString = new();

        [SerializeField] protected List<string> addString = new();

        [Header("Text Animation:")] [SerializeField]
        protected string showText;

        [SerializeField] protected int index;
        [SerializeField] protected bool active, waiting, done;
        [SerializeField] protected int nextCharacter;

        #endregion

        #region Getters

        public Chat GetChat()
        {
            Chat result = this;

            if (this.isInstantiated) return result;

            result = Instantiate(result);
            result.SetIsInstantiated();

            return result;
        }

        public bool GetDone()
        {
            return this.done;
        }

        public bool GetNeedInput()
        {
            return this.needInput;
        }

        #endregion

        #region Setters

        private void SetIsInstantiated()
        {
            this.isInstantiated = true;
        }

        #endregion

        #region In

        private void IncreaseIndex()
        {
            this.index++;
            this.showText = "";
            this.nextCharacter = 0;
            this.waiting = false;
        }

        public void AddToOverride(string replace, string add)
        {
            this.replaceString.Add(replace);
            this.addString.Add(add);
        }

        #endregion

        #region Out

        public IEnumerator Play()
        {
            if (!this.active)
            {
                this.index = 0;
                CheckTextOverride();
                this.active = true;
            }

            while (!this.done && !this.waiting && (this.index < this.textList.Length))
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

                if (this.showText.Length != 0) this.chatManager.SetDisplayText(this.showText);

                if (this.showText.Length == fromList.Length)
                {
                    if (!this.needInput)
                        yield return new WaitForSeconds(this.timeUntilNext);

                    if (this.index < (this.textList.Length - 1))
                        this.waiting = true;
                    else
                        this.done = true;

                    this.chatManager.CheckRunningState();
                }

                yield return new WaitForSeconds(relativeSpeed);
            }

            while (this.waiting)
                yield return null;
        }

        public IEnumerator PlayNext()
        {
            IncreaseIndex();

            this.done = false;

            return Play();
        }

        #endregion

        #region Internal

        protected virtual void CheckTextOverride()
        {
            AddPronounsToOverride();

            for (int i = 0; i < this.textList.Length; i++)
            {
                for (int j = 0; j < this.replaceString.Count; j++)
                {
                    if (j < this.replaceString.Count && j < this.addString.Count) this.textList[i] = this.textList[i].Replace(this.replaceString[j], this.addString[j]);
                }
            }
        }

        private void AddPronounsToOverride()
        {
            string[] pronouns = this.playerManager.GetPronouns();

            AddToOverride("P_ONE", pronouns[0]);
            AddToOverride("P_TWO", pronouns[1]);
        }

        #endregion
    }
}