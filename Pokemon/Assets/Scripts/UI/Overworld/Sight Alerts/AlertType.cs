#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using UnityEngine;
// ReSharper disable ParameterHidesMember

#endregion

namespace Mfknudsen.UI.Overworld.Sight_Alerts
{
    public abstract class AlertType : ScriptableObject, IOperation
    {
        protected Transform uiParent;
        protected bool beginNew;

        public void Trigger(Transform uiParent, bool beginNew)
        {
            this.uiParent = uiParent;
            this.beginNew = beginNew;
        }

        public abstract bool Done();

        public abstract IEnumerator Operation();

        public abstract void End();
    }
}