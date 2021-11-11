#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using UnityEngine;

// ReSharper disable ParameterHidesMember

#endregion

namespace Mfknudsen.UI.Scene_Transitions.Transitions
{
    public abstract class Transition : ScriptableObject, IOperation
    {
        protected SceneTransitionUI transitionUI;
        protected bool cover;

        public virtual void Trigger(SceneTransitionUI transitionUI, bool cover)
        {
            this.transitionUI = transitionUI;
            this.cover = cover;
        }

        public abstract bool Done();
        public abstract IEnumerator Operation();
        public abstract void End();
    }
}