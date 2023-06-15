#region Libraries

using Assets.Scripts.Runtime.World.Overworld;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#endregion

[CustomEditor(typeof(TileSubController))]
public sealed class TileSubControllerEditor : OdinEditor
{
    private void OnSceneGUI()
    {
        TileSubController tileSubController = (TileSubController)this.target;

        EditorGUI.BeginChangeCheck();

        Vector3 pos = Handles.PositionHandle(tileSubController.GetCleanUpPoint, Quaternion.identity);

        GUIStyle style = new();
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperCenter;
        Handles.Label(pos, "Navmesh Clean Up Point", style);

        if (!EditorGUI.EndChangeCheck())
            return;

        tileSubController.SetCleanUpPoint(pos);
        Undo.RecordObject(this.target, "Moved Navmesh Clean Up Point");
    }
}
