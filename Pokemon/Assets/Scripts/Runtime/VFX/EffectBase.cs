#region Packages

using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.VFX
{
    public abstract class EffectBase : MonoBehaviour
    {
        #region Values
        
        [SerializeField, Required] private EffectManager effectManager;

        #endregion

        #region Build In States

        private void OnEnable()
        {
            this.effectManager.RegisterStaticEffect(this);
            
            this.OnStart();
        }

        private void OnDisable() => this.effectManager.UnregisterStaticEffect(this);

        #endregion

        #region In

        public abstract void Play();

        public abstract void Stop();
        
        #endregion

        #region Internal

        protected virtual void OnStart(){}

        #endregion
    }
}