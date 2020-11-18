﻿#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

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
    [SerializeField] protected int time;

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

    public IEnumerator Play()
    {
        if (!done)
        {
            if (!active)
            {
                index = 0;
                CheckTextOverride();
                done = false;
                active = true;
            }

            while (!done && !waiting && (index < textList.Length))
            {
                string tempText = "";
                string fromList = textList[index];
                float relativSpeed = ChatMaster.instance.GetTextSpeed();

                if (nextCharacter + 1 < fromList.Length)
                    tempText = "" + fromList[nextCharacter] + fromList[nextCharacter + 1];

                if (tempText == "\n")
                {
                    showText += "\n";
                    nextCharacter++;
                    relativSpeed *= 1.5f;
                }
                else if (nextCharacter < fromList.Length)
                {
                    showText += fromList[nextCharacter];
                }

                if (showText.Length != 0)
                    ChatMaster.instance.SetDisplayText(showText);

                if (showText.Length < fromList.Length)
                {
                    nextCharacter++;
                }
                else if (showText.Length == fromList.Length)
                {
                    if (!needInput)
                        yield return new WaitForSeconds(0.5f);
                 
                    waiting = true;
                    ChatMaster.instance.CheckRunningState();
                }
                yield return new WaitForSeconds(relativSpeed);
            }
            while (waiting)
                yield return 0;
        }
    }

    public IEnumerator PlayNext()
    {
        IncreaseIndex();

        return Play();
    }
    #endregion

    #region Internal
    protected virtual void CheckTextOverride()
    {
        for (int i = 0; i < textList.Length; i++)
        {
            for (int j = 0; j < replaceString.Count; j++)
                textList[i] = textList[i].Replace(replaceString[j], addString[j]);
        }
    }
    #endregion
}