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

        [SerializeField, PropertyOrder(-2), Required]
        private Light affectedLight;

        [SerializeField,
         ListDrawerSettings(ListElementLabelName = "label", HideRemoveButton = true, HideAddButton = true,
             DraggableItems = false)]
        private List<DayTimeLightSettings> settings = new List<DayTimeLightSettings>();

#if UNITY_EDITOR
        [ShowInInspector, HorizontalGroup("Save"), ValueDropdown(nameof(SaveAsOptions)), HideLabel]
        private DayTime saveAsLabel;

        private DayTime[] SaveAsOptions = new DayTime[]
        {
            DayTime.Midnight, DayTime.Morning, DayTime.Evening, DayTime.Afternoon, DayTime.Night
        };
#endif

        #endregion

        #region Build In States

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = this.settings.Count; i < Enum.GetValues(typeof(DayTime)).Length; i++)
            {
                this.settings.Add(new DayTimeLightSettings((DayTime)i));
                this.settings[^1].SetValues(this.affectedLight);
            }
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

        #region Internal

#if UNITY_EDITOR
        [HorizontalGroup("Save"), PropertyOrder(-1), Button]
        private void SaveSetting() =>
            this.settings[(int)this.saveAsLabel].SetValues(this.affectedLight);
#endif

        #endregion
    }
}