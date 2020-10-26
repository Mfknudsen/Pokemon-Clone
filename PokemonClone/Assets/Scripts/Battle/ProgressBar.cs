using System.Collections;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float curBar, minBar, maxBar;

    private void Update()
    {
        
    }

    public void SetCurrentBar(float input)
    {
        curBar = input;
    }
}
