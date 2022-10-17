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

        [SerializeField, AssetsOnly] private List<Manager> managersToStart = new();
        [SerializeField, AssetsOnly] private List<Manager> managersToUpdate = new();
        [SerializeField, AssetsOnly] private List<Manager> managersToLateUpdate = new();

        private IFrameStart[] frameStarts;
        private IFrameUpdate[] frameUpdates;
        private IFrameLateUpdate[] frameLateUpdates;

        #endregion

        #region Build In States

        private void OnValidate()
        {
            this.managersToStart = this.managersToStart
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameStart)))
                .ToList();
            this.managersToUpdate = this.managersToUpdate
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameUpdate)))
                .ToList();
            this.managersToLateUpdate = this.managersToLateUpdate
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameLateUpdate)))
                .ToList();
        }

        private void Start()
        {
            this.frameStarts = this.managersToStart.Select(m => m as IFrameStart).ToArray();
            this.frameUpdates = this.managersToUpdate.Select(m => m as IFrameUpdate).ToArray();
            this.frameLateUpdates = this.managersToLateUpdate.Select(m => m as IFrameLateUpdate).ToArray();

            foreach (IFrameStart frameStart in this.frameStarts)
                frameStart.FrameStart();
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