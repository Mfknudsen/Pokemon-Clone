﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    #region Values

    #endregion

    #region In
    #endregion

    #region Out
    public void LoadScene(string sceneName)
    {
        WorldMaster.instance.LoadScene(sceneName);
    }
    #endregion
}