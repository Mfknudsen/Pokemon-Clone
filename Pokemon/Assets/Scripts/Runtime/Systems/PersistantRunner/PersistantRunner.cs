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

        [SerializeField, AssetsOnly] private List<Manager> managersToUpdate = new();

        #endregion

        #region Build In States

        private void OnValidate()
        {
            this.managersToUpdate = this.managersToUpdate
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameUpdate)))
                .ToList();
        }

        private void Start()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (IFrameUpdate frameUpdate in this.managersToUpdate.Select(m => m as IFrameUpdate))
                frameUpdate?.FrameStart();
        }

        private void Update()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (IFrameUpdate frameUpdate in this.managersToUpdate.Select(m => m as IFrameUpdate))
                frameUpdate?.FrameUpdate();
        }

        private void LateUpdate()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (IFrameUpdate frameUpdate in this.managersToUpdate.Select(m => m as IFrameUpdate))
                frameUpdate?.FrameLateUpdate();
        }

        #endregion
    }
}