#region Libraries

using Assets.Scripts.Runtime.World.Overworld;
using Runtime.AI.Navigation;
using Runtime.Common;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace Editor.Systems.World
{
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

            if (EditorGUI.EndChangeCheck())
            {
                this.tileSubController.SetCleanUpPoint(pos);
                Undo.RecordObject(this.target, "Moved Navmesh Clean Up Point");
            }

            if (this.tileSubController.GetNavmesh != null)
            {
                Handles.color = Color.red;
                Vector3 scenePos = SceneView.GetAllSceneCameras()[0].transform.position, forward = SceneView.GetAllSceneCameras()[0].transform.forward;
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
                    for (int j = 0; j < corners.Count; j++)
                    {
                        if (t[corners[j]].ID < i)
                            continue;

                        Handles.DrawLine(v, t[corners[j]].Center(verts), 2f);
                    }

                }
                int[] inds = this.tileSubController.GetNavmesh.Triangles.SelectMany(t => t.Vertices).ToArray();
                for (int i = 0; i < inds.Length; i += 3)
                {
                    if (verts[inds[i]].QuickSquareDistance(scenePos) > 2000 ||
                        Vector3.Angle(forward, verts[inds[i]] - scenePos) > 75f)
                        continue;

                    if (verts[inds[i + 1]].QuickSquareDistance(scenePos) > 2000 ||
                        Vector3.Angle(forward, verts[inds[i + 1]] - scenePos) > 75f)
                        continue;

                    if (verts[inds[i + 2]].QuickSquareDistance(scenePos) > 2000 ||
                        Vector3.Angle(forward, verts[inds[i + 2]] - scenePos) > 75f)
                        continue;

                    Debug.DrawLine(verts[inds[i]] + Vector3.up, verts[inds[i + 1]] + Vector3.up, Color.green);
                    Debug.DrawLine(verts[inds[i]] + Vector3.up, verts[inds[i + 2]] + Vector3.up, Color.green);
                    Debug.DrawLine(verts[inds[i + 2]] + Vector3.up, verts[inds[i + 1]] + Vector3.up, Color.green);
                }
            }

        }
    }
}