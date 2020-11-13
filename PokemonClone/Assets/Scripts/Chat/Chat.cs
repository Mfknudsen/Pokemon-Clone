using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chat", menuName = "Chat/Create New Chat", order = 0)]
public class Chat : ScriptableObject
{
    [SerializeField] private string location = "";
    [SerializeField, TextArea] private string description = "";
    [SerializeField, TextArea] private string[] textList = new string[0];
    private string showText = "";
    private int index;
    private bool active = false, done = false;

    public bool GetDone()
    {
        return done;
    }

    public string GetShowText()
    {
        return showText;
    }

    public IEnumerator Play(float speed)
    {
        if (!active)
        {
            Debug.Log(location);
            index = 0;
            showText = "";
        }

        if (showText == textList[index])
        {
            if (index - 1 == textList.Length)
                done = true;
        }

        yield return new WaitForSeconds(speed);
    }
}
