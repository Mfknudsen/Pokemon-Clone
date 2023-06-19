#region Libraries

using Editor.Common;
using Runtime.AI.Navigation;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#endregion

namespace Editor.AI
{
    [CustomEditor(typeof(UnitNavigationAgent))]
    public sealed class UnitNavigationAgentEditor : OdinEditor
    {
        #region Values

        UnitNavigationAgent agent;

        #endregion

        #region Build In States

        protected override void OnEnable()
        {
            base.OnEnable();

            this.agent = (UnitNavigationAgent)this.target;
        }

        private void OnSceneGUI()
        {
            if (this.agent == null)
                return;

            UnitAgentSettings settings = this.agent.Settings;

            Draw.DrawCylinder(this.agent.gameObject.transform.position, settings.Height, settings.Radius, Color.yellow);
        }

        #endregion
    }
}