using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image bar = null;
    public float curBar = 0, maxBar= 0;

    public void SetBarMax(float max)
    {
        maxBar = max;
        curBar = maxBar;

        SetCurrentBar(curBar);
    }

    public void SetCurrentBar(float input)
    {
        curBar = Mathf.Clamp(input, 0, Mathf.Infinity);

        float procent = (100 / maxBar) * curBar / 100;

        if (procent < 0.25)
            bar.color = Color.red;
        else if (procent < 0.7)
            bar.color = Color.yellow;
        else
            bar.color = Color.green;

        bar.transform.localScale = new Vector3(procent, 1, 1);
    }
}
