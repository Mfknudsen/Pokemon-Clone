using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMaster : MonoBehaviour
{
    [Header("Object Reference:")]
    public static ChatMaster instance;
    [SerializeField] private bool empty = true;
    [SerializeField] private Chat running = null;
    [SerializeField] private List<Chat> waitlist = new List<Chat>();
    Coroutine coroutine = null;
    public bool ACTIVE = false;

    [Header("Display:")]
    [SerializeField] private TextMeshProUGUI textField = null;
    [SerializeField] private KeyCode continueKey = 0;
    [SerializeField] private bool waitForInput = false;

    [Header("Chat Settings:")]
    [SerializeField] private float textPerSecond = 0;

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

        ACTIVE = (coroutine != null);

        if (waitForInput)
        {
            if (Input.GetKeyDown(continueKey))
            {
                if (running != null)
                {
                    if (running.GetDone())
                        running = null;
                    else
                        coroutine = StartCoroutine(running.PlayNext());
                }
                else if (waitlist.Count > 0)
                {
                    Play(waitlist[0]);
                    waitlist.RemoveAt(0);
                }

                waitForInput = false;
            }
        }

        if (running == null && waitlist.Count > 0)
        {
            running = waitlist[0];
            waitlist.RemoveAt(0);

            Play(running);
        }
    }

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
    #endregion

    public void Play(Chat toPlay)
    {
        textField.gameObject.SetActive(true);

        running = toPlay;
        running.CheckTextOverride();
        coroutine = StartCoroutine(running.Play());
    }

    public void Add(Chat[] toAdd)
    {
        if (running == null)
        {
            if (toAdd[0] != null)
                Play(Instantiate(toAdd[0]));

            if (toAdd.Length > 1)
            {
                for (int i = 1; i < toAdd.Length; i++)
                {
                    if (toAdd[i] != null)
                        waitlist.Add(Instantiate(toAdd[i]));
                }
            }
        }
        else
        {
            foreach (Chat c in toAdd)
                waitlist.Add(Instantiate(c));
        }
    }

    public void CheckRunningState()
    {
        waitForInput = true;
        coroutine = null;

        if (!running.HasMore())
        {
            running.SetDone(true);
        }
    }
}
