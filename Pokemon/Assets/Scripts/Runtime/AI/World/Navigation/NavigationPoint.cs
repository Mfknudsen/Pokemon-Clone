#region Libraries

using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI.World.Navigation
{
    public abstract class NavigationPoint : MonoBehaviour
    {
        #region Values

        [SerializeField]
        private NavigationPointEntry entryAlpha, entryBeta;

        #endregion

        #region Build In States

        protected virtual void OnValidate()
        {
            Vector3 pos = this.transform.position;
            this.entryAlpha = new NavigationPointEntry(pos + this.transform.forward, 1, this);
            this.entryBeta = new NavigationPointEntry(pos - this.transform.forward, 1, this);
        }

        protected virtual void Start()
        {
            this.entryAlpha.SetActive(this.gameObject.activeInHierarchy);
            this.entryBeta.SetActive(this.gameObject.activeInHierarchy);
        }

        protected virtual void OnEnable()
        {
            this.entryAlpha.SetActive(this.gameObject.activeInHierarchy);
            this.entryBeta.SetActive(this.gameObject.activeInHierarchy);
        }

        protected virtual void OnDisable()
        {
            this.entryAlpha.SetActive(this.gameObject.activeInHierarchy);
            this.entryBeta.SetActive(this.gameObject.activeInHierarchy);
        }

        #endregion

        #region Getters

        public NavigationPointEntry[] GetEntryPoints() => new NavigationPointEntry[] { this.entryAlpha, this.entryBeta };

        #endregion
    }

    [SerializeField]
    public struct NavigationPointEntry
    {
        #region Values

        private Vector3 position;
        [MinValue(0)]
        private float reachRange;
        private NavigationPoint point;
        private bool active;

        #endregion

        #region Build In States

        public NavigationPointEntry(Vector3 position, float reachRange, NavigationPoint point)
        {
            this.position = position;
            this.reachRange = reachRange;
            this.point = point;
            this.active = true;
        }

        #endregion

        #region Getters

        public readonly Vector3 Position => this.position;

        public readonly float ReachRange => this.reachRange;

        public readonly bool Active => this.active;

        public readonly NavigationPoint NavigationPoint => this.point;

        #endregion

        #region Setters

        internal void SetActive(bool set) => this.active = set;

        #endregion
    }
}