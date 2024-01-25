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
        private WorldTimeZone saveAsLabel;

        private WorldTimeZone[] SaveAsOptions = new WorldTimeZone[]
        {
            WorldTimeZone.Midnight, WorldTimeZone.Morning, WorldTimeZone.Evening, WorldTimeZone.Afternoon, WorldTimeZone.Night
        };
#endif

        #endregion

        #region Build In States

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = this.settings.Count; i < Enum.GetValues(typeof(WorldTimeZone)).Length; i++)
            {
                this.settings.Add(new DayTimeLightSettings((WorldTimeZone)i));
                this.settings[^1].SetValues(this.affectedLight);
            }
        }
#endif

        private void OnEnable() => WorldTime.AddLight(this);

        private void OnDisable() => WorldTime.RemoveLight(this);

        #endregion

        #region In

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="towards"></param>
        /// <param name="interpolateTime">Between 0 - 1</param>
        public void Interpolate(WorldTimeZone from, WorldTimeZone towards, float interpolateTime)
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