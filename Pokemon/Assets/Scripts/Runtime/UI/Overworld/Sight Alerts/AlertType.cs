#region Packages

using System.Collections;
using Runtime.Systems.Operation;
using UnityEngine;

// ReSharper disable ParameterHidesMember

#endregion

namespace Runtime.UI.Overworld.Sight_Alerts
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

        public abstract bool IsOperationDone();

        public abstract IEnumerator Operation();

        public abstract void OperationEnd();
    }
}