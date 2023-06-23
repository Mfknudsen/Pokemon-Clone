#region Libraries

using Assets.Scripts.Runtime.World.Overworld;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#endregion

[CustomEditor(typeof(TileSubController))]
public sealed class TileSubControllerEditor : OdinEditor
{
    TileSubController tileSubController;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.tileSubController = (TileSubController)this.target;
    }

    private void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        Vector3 pos = Handles.PositionHandle(this.tileSubController.GetCleanUpPoint, Quaternion.identity);

        GUIStyle style = new()
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter
        };
        style.normal.textColor = Color.blue;
        Handles.Label(pos, "Navmesh Clean Up Point", style);

        if (!EditorGUI.EndChangeCheck())
            return;

        this.tileSubController.SetCleanUpPoint(pos);
        Undo.RecordObject(this.target, "Moved Navmesh Clean Up Point");
    }
}