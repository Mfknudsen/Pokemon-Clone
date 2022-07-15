#region Packages

using Mfknudsen.Common.CommonPath.Road;
using UnityEditor;
using UnityEngine;

#endregion

namespace Mfknudsen.Editor.Tools
{
    [CustomEditor(typeof(RoadCreator))]
    public class RoadEditor : UnityEditor.Editor
    {
        private RoadCreator creator;

        private void OnSceneGUI()
        {
            if (creator.autoUpdate && Event.current.type == EventType.Repaint) 
                creator.UpdateRoad();
        }

        private void OnEnable()
        {
            creator = (RoadCreator)target;
        }
    }
}