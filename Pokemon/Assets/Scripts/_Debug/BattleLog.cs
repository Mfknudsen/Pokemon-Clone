﻿#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
#endregion

public class BattleLog : MonoBehaviour
{
    #region Values
    public static BattleLog instance = null;
    [SerializeField] private bool active = false;
    [SerializeField] private TextMeshProUGUI textField = null;
    private List<string> textLog = new List<string>();
    [SerializeField] private Scrollbar scroller = null;
    #endregion

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            textField.text = "";
        }
        else
            Destroy(gameObject);
    }

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;

            obj.SetActive(active);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            active = (active == false);

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject obj = transform.GetChild(i).gameObject;

                obj.SetActive(active);
            }
        }
    }

    #region In
    public void AddNewLog(string script, string input)
    {
        textLog.Add(script + "[" + System.DateTime.Now.ToLocalTime().ToString("HH:mm:ss") + "]: " + input);

        textField.text += script + "[" + System.DateTime.Now.ToLocalTime().ToString("HH:mm:ss") + "]: \n   " + input + "\n";

        Invoke("ScrollControl", 0.01f);
    }
    #endregion

    #region Out
    private void ScrollControl()
    {
        scroller.value = 0;
    }
    #endregion
}