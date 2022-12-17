#region Packages

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Systems.PersistantRunner
{
    public class PersistantRunner : MonoBehaviour
    {
        #region Values

        [SerializeField, AssetsOnly] private List<Manager> managers = new();

        private IFrameStart[] frameStarts;
        private IFrameUpdate[] frameUpdates;
        private IFrameLateUpdate[] frameLateUpdates;

        #endregion

        #region Build In States

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            this.frameStarts = this.managers
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameStart)))
                .Select(m => m as IFrameStart)
                .ToArray();
            this.frameUpdates = this.managers
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameUpdate)))
                .Select(m => m as IFrameUpdate)
                .ToArray();
            this.frameLateUpdates = this.managers
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameLateUpdate)))
                .Select(m => m as IFrameLateUpdate)
                .ToArray();

            foreach (IFrameStart frameStart in this.frameStarts)
                this.StartCoroutine(frameStart.FrameStart(this));
        }

        private void Update()
        {
            foreach (IFrameUpdate frameUpdate in this.frameUpdates)
                frameUpdate.FrameUpdate();
        }

        private void LateUpdate()
        {
            foreach (IFrameLateUpdate frameLateUpdate in this.frameLateUpdates)
                frameLateUpdate.FrameLateUpdate();
        }

        #endregion
    }
}