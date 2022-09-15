#region Packages

using System.Collections.Generic;
using Runtime.Player;
using Runtime.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI
{
    public sealed class UnitManager : Manager
    {
        #region Values

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
            mediumDelay = mediumDelay > 0 ? mediumDelay : 1;
            farDelay = farDelay > 0 ? farDelay : 1;
        }

        public override void UpdateManager()
        {
            close.ForEach(c => c.TriggerBehaviourUpdate());

            if (count % farDelay == 0)
                far.ForEach(c => c.TriggerBehaviourUpdate());

            if (count % mediumDelay == 0)
            {
                medium.ForEach(c => c.TriggerBehaviourUpdate());

                if (count % farDelay == 0)
                    count = 0;
            }
        }

        #endregion

        #region In

        public void AddController(NpcController add)
        {
            if (add == null || controllers.Contains(add))
                return;

            controllers.Add(add);

            Transform cTrans = add.transform;
            cTrans.root.parent = this.holder.transform;

            Vector3 cPos = cTrans.position,
                pPos = playerManager.GetHolderObject() != null
                    ? playerManager.GetAgent().transform.position
                    : Vector3.zero;

            float distance = Vector3.Distance(cPos, pPos);

            switch (distance)
            {
                case < 50:
                    close.Add(add);
                    break;
                case < 100:
                    medium.Add(add);
                    break;
                default:
                    far.Add(add);
                    break;
            }
        }

        public void RemoveController(NpcController remove)
        {
            if (remove == null || !controllers.Contains(remove))
                return;

            controllers.Remove(remove);
        }

        #endregion
    }
}