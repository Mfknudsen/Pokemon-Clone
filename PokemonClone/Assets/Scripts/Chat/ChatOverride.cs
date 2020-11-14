using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatOverride : ScriptableObject
{
    [SerializeField] List<string> replaceOldTexts = new List<string>(), replaceNewTexts = new List<string>();

    public string[] OverrideText(string[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < replaceOldTexts.Count; j++)
            {
                input[i] = input[i].Replace(replaceOldTexts[j], replaceNewTexts[j]);
            }
        }

        return input;
    }
}
