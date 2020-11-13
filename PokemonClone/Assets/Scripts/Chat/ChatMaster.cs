using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMaster : MonoBehaviour
{
    [Header("Object Reference:")]
    public static ChatMaster chatMaster;
    [SerializeField] private Chat running = null;
    [SerializeField] private List<Chat> waitlist = new List<Chat>();

    [Header("Chat Settings:")]
    [SerializeField] private float textPerSecond = 0;

    private void Start()
    {
        if (chatMaster == null)
        {
            chatMaster = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (running != null && waitlist.Count > 0)
        {

        }
    }

    #region Getters
    public float GetTextSpeed()
    {
        return 1 / textPerSecond;
    }
    #endregion

    public void Play(Chat toPlay)
    {
        running = toPlay;
        StartCoroutine(running.Play(GetTextSpeed()));
    }

    public void Add()
    {

    }
}
