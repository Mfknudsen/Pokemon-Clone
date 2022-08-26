#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Player;
using Mfknudsen.Settings.Managers;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    public class NpcManager : Manager
    {
        #region Values

        public static NpcManager instance;

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

        private void Update()
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

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            yield break;
        }

        public void AddController(NpcController add)
        {
            if (add == null || controllers.Contains(add))
                return;

            controllers.Add(add);

            Transform cTrans = add.transform;
            cTrans.root.parent = transform;

            Vector3 cPos = cTrans.position,
                pPos = PlayerManager.instance != null
                    ? PlayerManager.instance.GetAgent().transform.position
                    : Vector3.zero;

            float distance = Vector3.Distance(cPos, pPos);

            if (distance < 50)
                close.Add(add);
            else if (distance < 100)
                medium.Add(add);
            else
                far.Add(add);
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