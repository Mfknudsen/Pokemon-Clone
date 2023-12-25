#region Libraries

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Lights
{
    public sealed class DayTimeLight : MonoBehaviour
    {
        #region Values

        [SerializeField, Required] private Light affectedLight;

        [SerializeField] private List<DayTimeLightSettings> settings = new List<DayTimeLightSettings>();

        #endregion

        #region Build In States

#if UNITY_EDITOR
        private void OnValidate()
        {
            while (this.settings.Count < 5)
                this.settings.Add(new DayTimeLightSettings());

            while (this.settings.Count > 5)
                this.settings.RemoveAt(5);
        }
#endif

        private void OnEnable() => DayNight.AddLight(this);

        private void OnDisable() => DayNight.RemoveLight(this);

        #endregion

        #region In

        public void Interpolate(DayTime from, DayTime towards)
        {
        }

        #endregion
    }
}