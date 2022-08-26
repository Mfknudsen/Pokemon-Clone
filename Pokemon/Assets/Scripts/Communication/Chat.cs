#region Packages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mfknudsen.Communication
{
    [CreateAssetMenu(fileName = "Chat", menuName = "Chat/Create new Standard Chat", order = 0)]
    public class Chat : ScriptableObject
    {
        #region Values

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

            if (isInstantiated) return result;

            result = Instantiate(result);
            result.SetIsInstantiated();

            return result;
        }

        public bool GetDone()
        {
            return done;
        }

        public bool GetNeedInput()
        {
            return needInput;
        }

        #endregion

        #region Setters

        private void SetIsInstantiated()
        {
            isInstantiated = true;
        }

        #endregion

        #region In

        private void IncreaseIndex()
        {
            index++;
            showText = "";
            nextCharacter = 0;
            waiting = false;
        }

        public void AddToOverride(string replace, string add)
        {
            replaceString.Add(replace);
            addString.Add(add);
        }

        #endregion

        #region Out

        public IEnumerator Play()
        {
            if (!active)
            {
                index = 0;
                CheckTextOverride();
                active = true;
            }

            while (!done && !waiting && (index < textList.Length))
            {
                string tempText = "", fromList = textList[index];
                float relativeSpeed = ChatManager.instance.GetTextSpeed();

                if (nextCharacter + 1 < fromList.Length)
                    tempText = "" + fromList[nextCharacter] + fromList[nextCharacter + 1];

                if (tempText == "\n")
                {
                    showText += "\n";
                    nextCharacter += 2;
                    relativeSpeed *= 1.5f;
                }
                else if (nextCharacter < fromList.Length)
                {
                    showText += fromList[nextCharacter];
                    nextCharacter++;
                }

                if (showText.Length != 0)
                    ChatManager.instance.SetDisplayText(showText);

                if (showText.Length == fromList.Length)
                {
                    if (!needInput)
                        yield return new WaitForSeconds(timeUntilNext);

                    if (index < (textList.Length - 1))
                        waiting = true;
                    else
                        done = true;

                    ChatManager.instance.CheckRunningState();
                }

                yield return new WaitForSeconds(relativeSpeed);
            }

            while (waiting)
                yield return null;
        }

        public IEnumerator PlayNext()
        {
            IncreaseIndex();

            done = false;

            return Play();
        }

        #endregion

        #region Internal

        protected virtual void CheckTextOverride()
        {
            AddPronounsToOverride();

            for (int i = 0; i < textList.Length; i++)
            {
                for (int j = 0; j < replaceString.Count; j++)
                {
                    if (j < replaceString.Count && j < addString.Count)
                        textList[i] = textList[i].Replace(replaceString[j], addString[j]);
                }
            }
        }

        private void AddPronounsToOverride()
        {
            string[] pronouns = Player.PlayerManager.instance.GetPronouns();

            AddToOverride("P_ONE", pronouns[0]);
            AddToOverride("P_TWO", pronouns[1]);
        }

        #endregion
    }
}