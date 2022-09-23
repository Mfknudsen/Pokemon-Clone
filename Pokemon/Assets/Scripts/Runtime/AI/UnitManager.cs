#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Player;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI
{
    [CreateAssetMenu(menuName = "Manager/Unit")]
    public sealed class UnitManager : Manager
    {
        #region Values

        private Transform parentTransform;

        [SerializeField, Required] private PlayerManager playerManager;
        [SerializeField] private int mediumDelay = 1, farDelay = 1;

        private readonly List<NpcController> controllers = new(),
            close = new(),
            medium = new(),
            far = new();

        private int count;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            this.mediumDelay = this.mediumDelay > 0 ? this.mediumDelay : 1;
            this.farDelay = this.farDelay > 0 ? this.farDelay : 1;
        }

        public override IEnumerator StartManager()
        {
            this.parentTransform = new GameObject("Units").transform;

            yield break;
        }

        public override void UpdateManager()
        {
            this.close.ForEach(c => c.TriggerBehaviourUpdate());

            if (this.count % this.farDelay == 0) this.far.ForEach(c => c.TriggerBehaviourUpdate());

            if (this.count % this.mediumDelay == 0)
            {
                this.medium.ForEach(c => c.TriggerBehaviourUpdate());

                if (this.count % this.farDelay == 0) this.count = 0;
            }
        }

        #endregion

        #region In

        public void AddController(NpcController add)
        {
            if (add == null || this.controllers.Contains(add))
                return;

            this.controllers.Add(add);

            Transform cTrans = add.transform;
            cTrans.root.parent = this.parentTransform.transform;

            Vector3 cPos = cTrans.position,
                pPos = this.playerManager.GetAgent() != null
                    ? this.playerManager.GetAgent().transform.position
                    : Vector3.zero;

            float distance = Vector3.Distance(cPos, pPos);

            switch (distance)
            {
                case < 50:
                    this.close.Add(add);
                    break;
                case < 100:
                    this.medium.Add(add);
                    break;
                default:
                    this.far.Add(add);
                    break;
            }
        }

        public void RemoveController(NpcController remove)
        {
            if (remove == null || !this.controllers.Contains(remove))
                return;

            this.controllers.Remove(remove);
        }

        #endregion
    }
}