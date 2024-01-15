#region Libraries

using System;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Lights
{
    [Serializable]
    public struct DayTimeLightSettings
    {
        #region Values

        [SerializeField] private Color lightColor;
        [SerializeField] private Vector3 rotation;

#if UNITY_EDITOR
        [UsedImplicitly] [SerializeField, HideInInspector]
        private DayTime label;
#endif

        #endregion

        #region Build In States

#if UNITY_EDITOR
        public DayTimeLightSettings(DayTime label)
        {
            this.label = label;
            this.lightColor = Color.white;
            this.rotation = Vector3.zero;
        }
#endif

        #endregion

        #region Getters

        public Color GetLightColor() => this.lightColor;

        public Quaternion GetRotation() => Quaternion.Euler(this.rotation);

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetValues(Light light)
        {
        }
#endif

        #endregion
    }
}