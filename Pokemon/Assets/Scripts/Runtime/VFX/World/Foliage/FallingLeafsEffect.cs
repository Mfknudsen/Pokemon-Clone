#region Packages

using Runtime.Common;
using Runtime.ScriptableVariables.Objects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Runtime.VFX.World.Foliage
{
    public sealed class FallingLeafsEffect : WorldEffect
    {
        #region Values

        [SerializeField] private VisualEffect leafEffect;
        [SerializeField, Required] private TransformVariable playerTransform;
        [SerializeField] private float maxDistance;

        private Transform t;

        #endregion

        #region In

        public override void Play()
        {
            this.leafEffect.Play();

            this.isSwitching = false;
        }

        public override void Stop()
        {
            this.leafEffect.Stop();

            this.isSwitching = false;
        }

        public override void CheckRules()
        {
            if (this.disableRules || this.isSwitching) return;

            bool check = this.t.position.QuickDistanceLessThen(this.playerTransform.value.position, this.maxDistance);

            if (check == this.isActive) return;

            this.isActive = check;

            base.CheckRules();
        }

        #endregion

        #region Internal

        protected override void OnStart()
        {
            this.t = this.transform;
            
            this.leafEffect.Stop();
        }

        #endregion
    }
}