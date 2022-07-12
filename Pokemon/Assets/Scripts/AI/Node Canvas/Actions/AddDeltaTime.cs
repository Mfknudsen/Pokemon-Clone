#region Packages

using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen
{
    public class AddDeltaTime : ActionTask
    {
        public BBParameter<float> counter;

        protected override void OnExecute()
        {
            counter.SetValue(counter.value + Time.deltaTime);
        }
    }
}
