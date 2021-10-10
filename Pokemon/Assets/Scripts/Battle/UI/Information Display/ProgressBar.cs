using UnityEngine;
using UnityEngine.UI;

namespace Mfknudsen.Battle.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public Image bar;
        public float curBar, maxBar;

        public void SetBarMax(float max)
        {
            maxBar = max;
            curBar = maxBar;

            SetCurrentBar(curBar);
        }

        public void SetCurrentBar(float input)
        {
            curBar = Mathf.Clamp(input, 0, Mathf.Infinity);
            
            if(maxBar == 0) return;
            
            float procent = (100 / maxBar) * curBar / 100;
            
            if (procent < 0.25)
                bar.color = Color.red;
            else if (procent < 0.7)
                bar.color = Color.yellow;
            else
                bar.color = Color.green;

            procent = Mathf.Clamp(procent, 0.0f, 1.0f);

            
            bar.transform.localScale = new Vector3(procent, 1, 1);
        }
    }
}
