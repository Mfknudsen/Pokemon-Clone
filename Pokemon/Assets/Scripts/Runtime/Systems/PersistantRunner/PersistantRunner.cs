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

        private List<IFrameStart> frameStarts;
        private List<IFrameUpdate> frameUpdates;
        private List<IFrameLateUpdate> frameLateUpdates;

        #endregion

        #region Build In States

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            this.frameStarts = this.managers
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameStart)))
                .Select(m => m as IFrameStart)
                .ToList();
            this.frameUpdates = this.managers
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameUpdate)))
                .Select(m => m as IFrameUpdate)
                .ToList();
            this.frameLateUpdates = this.managers
                .Where(m => m is not null && m.GetType().GetInterfaces().Contains(typeof(IFrameLateUpdate)))
                .Select(m => m as IFrameLateUpdate)
                .ToList();

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

        #region In

#if UNITY_EDITOR
        public void StartManagers()
        {
            foreach (IFrameStart frameStart in this.frameStarts)
                this.StartCoroutine(frameStart.FrameStart(this));
        }

        public void AddManager(Manager toAdd)
        {
            if (toAdd is IFrameStart start)
                this.frameStarts.Add(start);

            if (toAdd is IFrameUpdate update)
                this.frameUpdates.Add(update);

            if (toAdd is IFrameLateUpdate late)
                this.frameLateUpdates.Add(late);
        }
#endif

        #endregion
    }
}