#region SDK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mfknudsen.Comunication
{
    [CreateAssetMenu(fileName = "Chat", menuName = "Chat/Create new Standard Chat", order = 0)]
    public class Chat : ScriptableObject
    {
        #region Values
        [Header("Object Reference:")]
        [SerializeField] protected bool isInstantiated = false;
        [SerializeField] protected string location = "";
        [SerializeField, TextArea] protected string description = "";
        [SerializeField, TextArea] protected string[] textList = new string[0];

        [Header("Continuation:")]
        [SerializeField] protected bool needInput = true;
        [SerializeField] protected float timeUntilNext = 0;

        [Header("Overriding:")]
        [SerializeField] protected List<string> replaceString = new List<string>();
        [SerializeField] protected List<string> addString = new List<string>();

        [Header("Text Animation:")]
        [SerializeField] protected string showText = "";
        [SerializeField] protected int index;
        [SerializeField] protected bool active = false, waiting = false, done = false;
        [SerializeField] protected int nextCharacter = 0;
        #endregion

        #region Getters
        public Chat GetChat()
        {
            Chat result = this;

            if (!result.GetIsInstantiated())
            {
                result = Instantiate(result);
                result.SetIsInstantiated();
            }

            return result;
        }

        public bool GetIsInstantiated()
        {
            return isInstantiated;
        }

        public bool GetDone()
        {
            return done;
        }

        public bool GetHasMore()
        {
            return (index < textList.Length - 1);
        }

        public bool GetNeedInput()
        {
            return needInput;
        }
        #endregion

        #region Setters
        public void SetIsInstantiated()
        {
            isInstantiated = true;
        }

        public void SetDone(bool set)
        {
            done = set;

            showText = "";
            index = 0;
            active = false;
            nextCharacter = 0;
        }

        public void SetWaiting(bool set)
        {
            waiting = set;
        }
        #endregion

        #region In
        protected void IncreaseIndex()
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
                float relativeSpeed = ChatMaster.instance.GetTextSpeed();

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
                    ChatMaster.instance.SetDisplayText(showText);
                
                if (showText.Length == fromList.Length)
                {
                    if (!needInput)
                        yield return new WaitForSeconds(timeUntilNext);

                    if (index < (textList.Length - 1))
                        waiting = true;
                    else
                        done = true;

                    ChatMaster.instance.CheckRunningState();
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
            string[] pronouns = Player.MasterPlayer.instance.GetPronouns();

            AddToOverride("P_ONE", pronouns[0]);
            AddToOverride("P_TWO", pronouns[1]);
        }
        #endregion
    }
}