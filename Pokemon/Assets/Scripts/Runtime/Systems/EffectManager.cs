#region Packages

using System.Collections.Generic;
using Runtime.Player;
using Runtime.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    [CreateAssetMenu(menuName = "Manager/Effect")]
    public class EffectManager : Manager
    {
        #region Values

        [SerializeField, Required]
        private PlayerManager playerManager;

        private readonly List<EffectBase> registeredEffects = new();

        #endregion

        #region Build In States

        private void OnEnable() => this.registeredEffects.Clear();

        #endregion

        #region In

        public override void UpdateManager()
        {
            Vector3 playerPos = this.playerManager.GetController().transform.position;

            foreach (EffectBase effect in this.registeredEffects)
            {
                if (Vector3.Distance(playerPos, effect.transform.position) > effect.getMaxDistance)
                {
                    effect.StartDisable();
                }
                else if (effect.getIsDisablingSelf)
                    effect.StopDisable();
            }
        }

        public void RegisterEffect(EffectBase effectBase)
        {
            if (!this.registeredEffects.Contains(effectBase))
                this.registeredEffects.Add(effectBase);
        }

        public void UnregisterEffect(EffectBase effectBase)
        {
            if (this.registeredEffects.Contains(effectBase))
                this.registeredEffects.Remove(effectBase);
        }

        #endregion
    }
}