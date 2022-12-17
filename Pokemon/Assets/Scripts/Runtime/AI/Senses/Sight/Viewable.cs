#region Packages

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.AI.Senses.Sight
{
    public sealed class Viewable : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private ViewableRegistry registry;

        [SerializeField, Min(0)] private float checkRadius;

        [SerializeField] private Transform[] viewableTransforms;

        [SerializeField] private List<ViewableTag> tags;

        #endregion

        #region Build In States

        private void OnEnable() =>
            this.registry.Register(this);

        private void OnDisable() =>
            this.registry.Unregister(this);

        #endregion

        #region Getters

        public float GetCheckRadius => this.checkRadius;

        public Transform[] GetViewableTransforms => this.viewableTransforms;

        #endregion

        #region Out

        public bool ContainsTag(ViewableTag compareTag) => this.tags.Any(t => t == compareTag);

        #endregion
    }
}