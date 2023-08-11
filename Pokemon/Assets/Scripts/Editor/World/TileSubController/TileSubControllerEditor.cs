#region Libraries

using Sirenix.OdinInspector.Editor;
using UnityEditor;

#endregion

namespace Editor.World.TileSubController
{
    [CustomEditor(typeof(Runtime.World.Overworld.TileSubController))]
    public sealed class TileSubControllerEditor : OdinEditor
    {
        #region Build In States

        /*
        private void OnSceneGUI()
        {
            Runtime.World.Overworld.TileSubController ileSubController = (Runtime.World.Overworld.TileSubController)this.target;
        
            EditorGUI.BeginChangeCheck();

            Vector3 pos = Handles.PositionHandle(this.tileSubController.GetCleanUpPoint, Quaternion.identity);

            GUIStyle style = new()
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter,
                normal =
                {
                    textColor = Color.blue
                }
            };
            Handles.Label(pos, "Navmesh Clean Up Point", style);

            if (EditorGUI.EndChangeCheck())
            {
                this.tileSubController.SetCleanUpPoint(pos);
                Undo.RecordObject(this.target, "Moved Navmesh Clean Up Point");
            }

            if (this.tileSubController.GetNavmesh == null) return;

            Handles.color = Color.red;
            Vector3 scenePos = SceneView.GetAllSceneCameras()[0].transform.position,
                forward = SceneView.GetAllSceneCameras()[0].transform.forward;
            Vector3[] verts = this.tileSubController.GetNavmesh.Vertices();
            NavTriangle[] t = this.tileSubController.GetNavmesh.Triangles;
            for (int i = 0; i < t.Length; i++)
            {
                Vector3 v = t[i].Center(verts);
                if ((v - scenePos).sqrMagnitude > 4000)
                    continue;

                if (Vector3.Angle(forward, v - scenePos) > 75f)
                    continue;

                Handles.Label(v + Vector3.up, i.ToString(), style);

                List<int> corners = t[i].Neighbors;
                foreach (int c in corners.Where(c => t[c].ID >= i))
                    Handles.DrawLine(v, t[c].Center(verts), 2f);
            }

            int[] indices = this.tileSubController.GetNavmesh.Triangles.SelectMany(navTriangle => navTriangle.Vertices)
                .ToArray();
            for (int i = 0; i < indices.Length; i += 3)
            {
                if (verts[indices[i]].QuickSquareDistance(scenePos) > 2000 ||
                    Vector3.Angle(forward, verts[indices[i]] - scenePos) > 75f)
                    continue;

                if (verts[indices[i + 1]].QuickSquareDistance(scenePos) > 2000 ||
                    Vector3.Angle(forward, verts[indices[i + 1]] - scenePos) > 75f)
                    continue;

                if (verts[indices[i + 2]].QuickSquareDistance(scenePos) > 2000 ||
                    Vector3.Angle(forward, verts[indices[i + 2]] - scenePos) > 75f)
                    continue;

                Debug.DrawLine(verts[indices[i]] + Vector3.up, verts[indices[i + 1]] + Vector3.up, Color.green);
                Debug.DrawLine(verts[indices[i]] + Vector3.up, verts[indices[i + 2]] + Vector3.up, Color.green);
                Debug.DrawLine(verts[indices[i + 2]] + Vector3.up, verts[indices[i + 1]] + Vector3.up, Color.green);
            }
        }
        */

        #endregion
    }
}