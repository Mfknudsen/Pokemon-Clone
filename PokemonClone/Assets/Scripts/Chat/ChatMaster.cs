﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMaster : MonoBehaviour
{
    #region Values
    [Header("Object Reference:")]
    public static ChatMaster instance;
    [SerializeField] private bool empty = true;
    [SerializeField] private Chat running = null;
    [SerializeField] private List<Chat> waitlist = new List<Chat>();
    Coroutine coroutine = null;

    [Header("Display:")]
    [SerializeField] private TextMeshProUGUI textField = null;
    [SerializeField] private KeyCode continueKey = 0;
    [SerializeField] private bool waitForInput = false;

    [Header("Chat Settings:")]
    [SerializeField] private float textPerSecond = 0;
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

        if (waitForInput)
        {
            if (Input.GetKeyDown(continueKey) || !running.GetNeedInput())
            {
                if (running != null)
                {
                    if (running.GetDone())
                        running = null;
                    else
                        coroutine = StartCoroutine(running.PlayNext());
                }

                waitForInput = false;
            }
        }

        if (running == null && waitlist.Count > 0)
            PlayNextInLine();
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
    #endregion

    #region Internal
    public void CheckRunningState()
    {
        waitForInput = true;
        coroutine = null;

        if (!running.GetHasMore())
        {
            running.SetDone(true);
        }
    }

    private void PlayNextInLine()
    {
        running = waitlist[0];
        waitlist.RemoveAt(0);

        Play(running);
    }

    public void Play(Chat toPlay)
    {
        textField.gameObject.SetActive(true);

        running = toPlay;
        coroutine = StartCoroutine(running.Play());
    }
    #endregion
}