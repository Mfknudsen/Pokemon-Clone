#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems;
using UnityEngine;

// ReSharper disable ParameterHidesMember

#endregion

namespace Mfknudsen.UI.Scene_Transitions.Transitions
{
    public abstract class Transition : ScriptableObject, IOperation
    {
        #region Values

        private SceneTransitionUI transitionUI;
        protected bool toCover;
        protected bool done;

        #endregion

        #region In

        public virtual void Trigger(SceneTransitionUI transitionUI, bool toCover)
        {
            this.transitionUI = transitionUI;
            this.toCover = toCover;
            done = false;
        }

        #region IOperation

        public bool Done()
        {
            return done;
        }

        public abstract IEnumerator Operation();

        public virtual void End()
        {
        }

        #endregion

        #endregion

        #region Internal

        public static float GetTimeOfClipByName(Animator anim, string name)
        {
            try
            {
                return anim.runtimeAnimatorController.animationClips
                    .First(clip => clip.name == name).length;
            }
            catch
            {
                Debug.LogError("Animator Doesnt Contain Clip Of Name: " + name);
                return 0;
            }
        }

        protected GameObject SpawnGameObject(GameObject gameObject)
        {
            return Instantiate(gameObject, transitionUI.transform);
        }

        #endregion
    }
}