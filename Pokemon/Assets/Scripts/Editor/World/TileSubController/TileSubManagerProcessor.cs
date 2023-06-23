#region Libraries

using Assets.Scripts.Runtime.World.Overworld;
using Cysharp.Threading.Tasks;
using Runtime.AI.Navigation;
using Runtime.Common;
using Runtime.Editor;
using Runtime.World.Overworld;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

#endregion

namespace Editor.Systems.World
{
    public sealed class TileSubControllerProcessor : OdinPropertyProcessor<TileSubController>
    {
        #region Values

        private float overlapCheckDistance = .3f;

        #endregion

        #region Build In States

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            propertyInfos.AddValue("Vertex Overlap Check Distance",
                (ref TileSubController c) => this.overlapCheckDistance,
                (ref TileSubController c, float d) => this.overlapCheckDistance = d,
                new FoldoutGroupAttribute("Navigation"),
                new MinValueAttribute(.001f));

            propertyInfos.AddDelegate("Bake Navigation Mesh", () => this.BakeNavmesh(this.ValueEntry.Values[0]), new FoldoutGroupAttribute("Navigation"));
            propertyInfos.AddDelegate("Bake Lightning", () => this.BakeLighting(this.ValueEntry.Values[0]), new FoldoutGroupAttribute("Lightning"));
        }

        #endregion

        #region Internal

        private async void BakeNavmesh(TileSubController tileSubController)
        {
            if (BakedEditorManager.IsBakeRunning || tileSubController == null)
                return;

            string editorProgressParTitel = "Baking Navigation: " + tileSubController.gameObject.scene.name;

            BakedEditorManager.SetRunning(true);

            List<string> neighborsToLoad = new();

            foreach (string path in UnityEngine.Object.FindObjectsOfType<ConnectionPoint>().Select(cp => cp.ScenePath).ToArray())
            {
                if (!neighborsToLoad.Contains(path))
                    neighborsToLoad.Add(path);
            }

            Scene[] loadedScenes = Array.Empty<Scene>();
            try
            {
                #region Load neighbor scenes and build navmesh triangulation

                ///Load neighboring scenes that the navigation mesh should cover
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Loading Neightbors", .25f);
                List<UniTask<Scene>> loadSceneTasks = new();

                for (int i = 0; i < neighborsToLoad.Count; i++)
                {
                    loadSceneTasks.Add(this.AsyncLoadScene(neighborsToLoad[i]));
                    EditorUtility.DisplayProgressBar(editorProgressParTitel, "Loading Neightbors", .25f + (.5f / neighborsToLoad.Count * i));
                }

                loadedScenes = await UniTask.WhenAll(loadSceneTasks);
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Loading Neightbors", 1f);

                ///Custruct Navigation Mesh and setup custom navmesh logic
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Calculation Navigation Mesh Triangulation", 0f);

                NavMeshTriangulation navmesh = this.BuildNavMeshTriangulation(tileSubController);

                #endregion

                #region Check Vertices and Indices for overlap

                List<Vector3> verts = navmesh.vertices.ToList();
                List<int> inds = navmesh.indices.ToList();
                List<int> areas = navmesh.areas.ToList();
                Dictionary<Vector2Int, List<int>> vertsByPos = new();

                const float groupdSize = 5f;

                for (int i = 0; i < verts.Count; i++)
                {
                    Vector3 v = verts[i];
                    Vector2Int id = new(Mathf.FloorToInt(v.x / groupdSize), Mathf.FloorToInt(v.z / groupdSize));

                    if (vertsByPos.TryGetValue(id, out List<int> outList))
                        outList.Add(i);
                    else
                        vertsByPos.Add(id, new() { i });
                }

                this.CheckOverlap(verts, inds, vertsByPos, groupdSize, editorProgressParTitel);

                #endregion

                #region Create first iteration of NavTriangles

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Creating first iteration NavTriangles", 0f);
                List<NavTriangle> triangles = new();
                Dictionary<int, List<int>> trianglesByVertexID = new();

                this.SetupNavTriangles(verts, inds, areas, triangles, trianglesByVertexID, editorProgressParTitel);

                triangles = this.SetupNeighbors(triangles, trianglesByVertexID, editorProgressParTitel, "First");

                NavigationPoint[] navigationPoints = tileSubController.GetNavigationPoints();

                Dictionary<int, List<NavigationPointEntry>> entries = this.SetupEntryPointsForTriangles(triangles.ToArray(), trianglesByVertexID, verts.ToArray());

                #endregion

                #region Check neighbor connections

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Checking NavTriangle neighbor connections", .5f);
                Vector3 cleanPoint = tileSubController.GetCleanUpPoint;
                int closestVert = 0;
                float closestDistance = cleanPoint.QuickSquareDistance(verts[closestVert]);

                for (int i = 1; i < verts.Count; i++)
                {
                    float d = cleanPoint.QuickSquareDistance(verts[i]);
                    if (d >= closestDistance)
                        continue;

                    if (trianglesByVertexID.TryGetValue(i, out List<int> value) && !value.Any(t => triangles[t].Neighbors.Count > 0))
                        continue;

                    closestDistance = d;
                    closestVert = i;
                }

                List<int> connected = new(), toCheck = new();
                toCheck.AddRange(trianglesByVertexID[closestVert]);

                while (toCheck.Count > 0)
                {
                    int index = toCheck[0];
                    NavTriangle navTriangle = triangles[index];
                    toCheck.RemoveAt(0);
                    connected.Add(index);

                    //Debug.Log(navTriangle.Neighbors.Length);
                    foreach (int n in navTriangle.Neighbors)
                    {
                        if (!toCheck.Contains(n) && !connected.Contains(n))
                            toCheck.Add(n);
                    }

                    EditorUtility.DisplayProgressBar(editorProgressParTitel, "Checking NavTriangle neighbor connections", .5f + (.5f / triangles.Count * connected.Count));
                }

                #endregion

                #region Fill holes and final iteration of NavTriangles

                List<Vector3> fixedVerties = new();
                List<int> fixedIndices = new(), fixedAreas = new();
                Dictionary<int, List<int>> fixedTrianglesByVertexID = new();

                foreach (NavTriangle t in connected.Select(i => triangles[i]))
                {
                    int aID = t.Vertices[0], bID = t.Vertices[1], cID = t.Vertices[2];
                    Vector3 a = verts[aID], b = verts[bID], c = verts[cID];

                    if (!fixedVerties.Contains(a))
                        fixedVerties.Add(a);

                    if (!fixedVerties.Contains(b))
                        fixedVerties.Add(b);

                    if (!fixedVerties.Contains(c))
                        fixedVerties.Add(c);

                    fixedIndices.Add(fixedVerties.IndexOf(a));
                    fixedIndices.Add(fixedVerties.IndexOf(b));
                    fixedIndices.Add(fixedVerties.IndexOf(c));

                    fixedAreas.Add(t.Area);

                    Vector2Int id = new(Mathf.FloorToInt(a.x / groupdSize), Mathf.FloorToInt(a.z / groupdSize));
                    if (vertsByPos.TryGetValue(id, out List<int> outListA))
                        outListA.Add(fixedVerties.IndexOf(a));
                    else
                        vertsByPos.Add(id, new() { fixedVerties.IndexOf(a) });

                    id = new(Mathf.FloorToInt(b.x / groupdSize), Mathf.FloorToInt(b.z / groupdSize));
                    if (vertsByPos.TryGetValue(id, out List<int> outListB))
                        outListB.Add(fixedVerties.IndexOf(b));
                    else
                        vertsByPos.Add(id, new() { fixedVerties.IndexOf(b) });

                    id = new(Mathf.FloorToInt(c.x / groupdSize), Mathf.FloorToInt(c.z / groupdSize));
                    if (vertsByPos.TryGetValue(id, out List<int> outListC))
                        outListC.Add(fixedVerties.IndexOf(c));
                    else
                        vertsByPos.Add(id, new() { fixedVerties.IndexOf(c) });
                }

                this.FillHoles(fixedVerties, fixedAreas, fixedIndices, vertsByPos, editorProgressParTitel);

                List<NavTriangle> fixedTriangles = new();

                this.SetupNavTriangles(fixedVerties, fixedIndices, fixedAreas, fixedTriangles, fixedTrianglesByVertexID, editorProgressParTitel);

                fixedTriangles = this.SetupNeighbors(fixedTriangles, fixedTrianglesByVertexID, editorProgressParTitel, "Final");

                for (int i = 0; i < fixedTriangles.Count; i++)
                {
                    //fixedTriangles[i].SetBorderWidth(fixedVerties, fixedTriangles);
                    EditorUtility.DisplayCancelableProgressBar(editorProgressParTitel, "Setting border width", 1f / fixedTriangles.Count * (i + 1));
                }

                Dictionary<int, List<NavigationPointEntry>> fixedEntryPoints = this.SetupEntryPointsForTriangles(fixedTriangles.ToArray(), fixedTrianglesByVertexID, fixedVerties.ToArray());

                #endregion

                #region Create the storage to contain the final result

                Scene s = tileSubController.gameObject.scene;
                string assetPath = s.path.Replace(".unity", "/"), assetName = s.name + " NavMesh Calculations.asset";

                try
                {
                    AssetDatabase.LoadAssetAtPath<CalculatedNavMesh>(assetPath + assetName).SetValues(fixedVerties.ToArray(), fixedTriangles.ToArray(), fixedAreas.ToArray(), fixedEntryPoints);
                }
                catch
                {
                    CalculatedNavMesh calculatedNavMesh = ScriptableObject.CreateInstance<CalculatedNavMesh>();
                    calculatedNavMesh.name = s.name + " NavMesh Calculations";
                    calculatedNavMesh.SetValues(fixedVerties.ToArray(), fixedTriangles.ToArray(), fixedAreas.ToArray(), fixedEntryPoints);
                    tileSubController.SetCalculatedNavMesh(calculatedNavMesh);

                    AssetDatabase.CreateAsset(calculatedNavMesh, assetPath + assetName);
                }

                AssetDatabase.SaveAssets();

                tileSubController.gameObject.GetFirstComponentByRoot<NavMeshVisualizor>()?.Create();

                #endregion
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Cancel"))
                    Debug.Log("Baking navmesh was canceled");
                else
                {
                    Debug.LogError("Baking Navigation Mesh Failed");
                    Debug.LogError(e);
                }
            }

            #region UnLoad neighbor scenes

            if (loadedScenes.Length > 0)
            {
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Unloading Neightbors", 0f);
                List<UniTask> unloadTasks = new();

                for (int i = 0; i < loadedScenes.Length; i++)
                    unloadTasks.Add(this.AsyncUnloadScene(loadedScenes[i]));

                await UniTask.WhenAll(unloadTasks);

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Unloading Neightbors", 1f);
            }

            #endregion

            EditorUtility.ClearProgressBar();

            EditorSceneManager.SaveScene(tileSubController.gameObject.scene);

            BakedEditorManager.SetRunning(false);
        }

        private async void BakeLighting(TileSubController tileSubController)
        {
            if (BakedEditorManager.IsBakeRunning)
                return;

            BakedEditorManager.SetRunning(true);

            try
            {
                await UniTask.NextFrame();
            }
            catch (Exception e)
            {
                Debug.LogError("Baking Lighting Failed");
                Debug.LogError(e);
            }

            EditorUtility.ClearProgressBar();

            BakedEditorManager.SetRunning(false);
        }

        private async UniTask<Scene> AsyncLoadScene(string path)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            await UniTask.WaitWhile(() => !scene.isLoaded);

            return scene;
        }

        private async UniTask AsyncUnloadScene(Scene scene)
        {
            EditorSceneManager.CloseScene(scene, true);

            await UniTask.WaitWhile(() => scene.isLoaded);
        }

        private NavMeshTriangulation BuildNavMeshTriangulation(TileSubController tileSubController)
        {
            NavMeshSurface surface = tileSubController.GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
            NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
            surface.RemoveData();
            surface.navMeshData = null;
            return navmesh;
        }

        private void SetupNavTriangles(List<Vector3> verts, List<int> indices, List<int> areas, List<NavTriangle> triangles, Dictionary<int, List<int>> trianglesByVertexID, string editorProgressParTitel)
        {
            for (int i = 0; i < indices.Count; i += 3)
            {
                int a = indices[i], b = indices[i + 1], c = indices[i + 2];
                NavTriangle triangle = new(i / 3, a, b, c, areas[i / 3], verts[a], verts[b], verts[c]);

                triangles.Add(triangle);

                int tID = triangles.Count - 1;

                if (trianglesByVertexID.TryGetValue(a, out List<int> list))
                {
                    if (!list.Contains(tID))
                        list.Add(tID);
                }
                else
                    trianglesByVertexID.Add(a, new List<int>() { tID });

                if (trianglesByVertexID.TryGetValue(b, out list))
                {
                    if (!list.Contains(tID))
                        list.Add(tID);
                }
                else
                    trianglesByVertexID.Add(b, new List<int>() { tID });

                if (trianglesByVertexID.TryGetValue(c, out list))
                {
                    if (!list.Contains(tID))
                        list.Add(tID);
                }
                else
                    trianglesByVertexID.Add(c, new List<int>() { tID });

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitel,
                    $"Creating NavTriangles: {i / 3f} / {indices.Count / 3f}",
                    1f / (indices.Count / 3f) * (i / 3f)))
                    throw new Exception("Cancel");
            }
        }

        private List<NavTriangle> SetupNeighbors(List<NavTriangle> triangles, Dictionary<int, List<int>> trianglesByVert, string editorProgressParTitel, string iteration)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                List<int> neighbors = new();
                List<int> possibleNeighbors = new();
                possibleNeighbors.AddRange(trianglesByVert[triangles[i].Vertices[0]]);
                possibleNeighbors.AddRange(trianglesByVert[triangles[i].Vertices[1]]);
                possibleNeighbors.AddRange(trianglesByVert[triangles[i].Vertices[2]]);

                possibleNeighbors = possibleNeighbors.Where(t => t != i).ToList();

                for (int j = 0; j < possibleNeighbors.Count; j++)
                {
                    if (triangles[i].Vertices.SharedBetween(triangles[possibleNeighbors[j]].Vertices).Length == 2)
                        neighbors.Add(possibleNeighbors[j]);

                    if (triangles.Count == 3)
                        break;
                }

                triangles[i].SetNeighborIDs(neighbors.ToArray());

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitel,
                    $"Setting up NavTriangles neighbors. {iteration} iteration: {i} / {triangles.Count}",
                    1f / triangles.Count * i))
                    throw new Exception("Cancel");
            }

            return triangles;
        }

        private Dictionary<int, List<NavigationPointEntry>> SetupEntryPointsForTriangles(NavTriangle[] navTriangles, Dictionary<int, List<int>> trianglesByVertexID, Vector3[] verts)
        {
            NavigationPointEntry[] entries = GameObject.FindObjectsOfType<NavigationPoint>().SelectMany(p => p.GetEntryPoints()).ToArray();
            Dictionary<int, List<NavigationPointEntry>> result = new();

            for (int e = 0; e < entries.Length; e++)
            {
                Vector3 pos = entries[e].Position;
                int closestVertIndex = 0;
                float dist = Vector3.Distance(verts[0], pos);

                for (int i = 1; i < verts.Length; i++)
                {
                    float d = Vector3.Distance(verts[i], pos);
                    if (d > dist)
                        continue;

                    dist = d;
                    closestVertIndex = i;
                }

                int[] tIDs = trianglesByVertexID[closestVertIndex].ToArray();

                foreach (int i in tIDs)
                {
                    Vector3[] tPos = navTriangles[i].Vertices.Select(v => verts[v]).ToArray();
                    if (!ExtMathf.PointWithinTriangle2D(pos, tPos[0], tPos[1], tPos[2]))
                        continue;

                    navTriangles[i].SetNavPointIDs(navTriangles[i].NavPoints.Append(e).ToArray());

                    if (result.TryGetValue(i, out List<NavigationPointEntry> r))
                        r.Add(entries[e]);
                    else
                        result.Add(i, new List<NavigationPointEntry>() { entries[e] });

                    break;
                }
            }

            return result;
        }

        private void CheckOverlap(List<Vector3> verts, List<int> inds, Dictionary<Vector2Int, List<int>> vertsByPos, float groupSize, string editorProgressParTitel)
        {
            float devided = 20f;
            Dictionary<int, List<int>> removed = new();
            for (int original = 0; original < verts.Count; original++)
            {
                if (removed.TryGetValue(Mathf.FloorToInt(original / devided), out List<int> removedList))
                {
                    if (removedList.Contains(original))
                        continue;
                }

                Vector2Int id = new(Mathf.FloorToInt(verts[original].x / groupSize), Mathf.FloorToInt(verts[original].z / groupSize));
                List<int> toCheck = new();
                if (vertsByPos.TryGetValue(id, out List<int> l1))
                    toCheck.AddRange(l1);
                if (vertsByPos.TryGetValue(id + new Vector2Int(-1, -1), out List<int> l2))
                    toCheck.AddRange(l2);
                if (vertsByPos.TryGetValue(id + new Vector2Int(-1, 0), out List<int> l3))
                    toCheck.AddRange(l3);
                if (vertsByPos.TryGetValue(id + new Vector2Int(-1, 1), out List<int> l4))
                    toCheck.AddRange(l4);
                if (vertsByPos.TryGetValue(id + new Vector2Int(0, -1), out List<int> l5))
                    toCheck.AddRange(l5);
                if (vertsByPos.TryGetValue(id + new Vector2Int(0, 1), out List<int> l6))
                    toCheck.AddRange(l6);
                if (vertsByPos.TryGetValue(id + new Vector2Int(1, -1), out List<int> l7))
                    toCheck.AddRange(l7);
                if (vertsByPos.TryGetValue(id + new Vector2Int(1, 0), out List<int> l8))
                    toCheck.AddRange(l8);
                if (vertsByPos.TryGetValue(id + new Vector2Int(1, 1), out List<int> l9))
                    toCheck.AddRange(l9);

                toCheck = toCheck.Where(x => x != original).ToList();

                for (int o = 0; o < toCheck.Count; o++)
                {
                    int other = toCheck[o];
                    if (removed.TryGetValue(Mathf.FloorToInt(other / devided), out removedList))
                    {
                        if (removedList.Contains(other))
                            continue;
                    }

                    if (Vector3.Distance(verts[original], verts[other]) > this.overlapCheckDistance)
                        continue;

                    if (removed.TryGetValue(Mathf.FloorToInt(other / devided), out removedList))
                        removedList.Add(other);
                    else
                        removed.Add(Mathf.FloorToInt(other / devided), new() { other });

                    for (int indsIndex = 0; indsIndex < inds.Count; indsIndex++)
                    {
                        if (inds[indsIndex] == other)
                            inds[indsIndex] = original;
                    }
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitel, $"Checking vertex overlap: {original} / {verts.Count}", 1f / verts.Count * original))
                    throw new Exception("Cancel");
            }

            List<int> toRemove = removed.Values.SelectMany(l => l).OrderBy(x => -x).ToList();
            for (int i = 0; i < toRemove.Count; i++)
            {
                int index = toRemove[i];
                vertsByPos[new(Mathf.FloorToInt(verts[index].x / groupSize), Mathf.FloorToInt(verts[index].z / groupSize))].Remove(index);
                verts.RemoveAt(index);

                for (int j = 0; j < inds.Count; j++)
                {
                    if (inds[j] >= index)
                        inds[j] = inds[j] - 1;
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitel, $"Removing overlaping vertices: {i} / {toRemove.Count}", 1f / toRemove.Count * i))
                    throw new Exception("Cancel");
            }

            for (int i = inds.Count - 1; i >= 0; i--)
            {
                if (i % 3 != 0)
                    continue;

                if (inds[i] == inds[i + 1] || inds[i] == inds[i + 2] || inds[i + 1] == inds[i + 2] ||
                    inds[i] >= verts.Count || inds[i + 1] >= verts.Count || inds[i + 2] >= verts.Count)
                {
                    inds.RemoveAt(i);
                    inds.RemoveAt(i);
                    inds.RemoveAt(i);
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitel, $"Correcting indicies indexes: {inds.Count - i} / {verts.Count}", 1f / verts.Count * (inds.Count - i)))
                    throw new Exception("Cancel");
            }
        }

        private void FillHoles(List<Vector3> verts, List<int> areas, List<int> inds, Dictionary<Vector2Int, List<int>> vertsByPos, string editorProgreesParTitel)
        {
            Dictionary<int, List<int>> connectionsByIndex = new();
            Dictionary<int, List<int>> indsByIndex = new();
            for (int i = 0; i < verts.Count; i++)
            {
                connectionsByIndex.Add(i, new());
                indsByIndex.Add(i, new());
            }

            for (int i = 0; i < inds.Count; i += 3)
            {
                if (!connectionsByIndex[inds[i]].Contains(inds[i + 1]))
                    connectionsByIndex[inds[i]].Add(inds[i + 1]);
                if (!connectionsByIndex[inds[i]].Contains(inds[i + 2]))
                    connectionsByIndex[inds[i]].Add(inds[i + 2]);

                if (!connectionsByIndex[inds[i + 1]].Contains(inds[i]))
                    connectionsByIndex[inds[i + 1]].Add(inds[i]);
                if (!connectionsByIndex[inds[i + 1]].Contains(inds[i + 2]))
                    connectionsByIndex[inds[i + 1]].Add(inds[i + 2]);

                if (!connectionsByIndex[inds[i + 2]].Contains(inds[i]))
                    connectionsByIndex[inds[i + 2]].Add(inds[i]);
                if (!connectionsByIndex[inds[i + 2]].Contains(inds[i + 1]))
                    connectionsByIndex[inds[i + 2]].Add(inds[i + 1]);

                int[] arr = { inds[i], inds[i + 1], inds[i + 2] };
                indsByIndex[inds[i]].AddRange(arr);
                indsByIndex[inds[i + 1]].AddRange(arr);
                indsByIndex[inds[i + 2]].AddRange(arr);

                if (EditorUtility.DisplayCancelableProgressBar(editorProgreesParTitel, $"Collecting vertex connections and indicies: {i} / {inds.Count}", 1f / inds.Count * i))
                    throw new Exception("Cancel");
            }

            for (int i = 0; i < verts.Count; i++)
            {
                Vector2 p = verts[i].XZ();

                for (int j = 0; j < inds.Count; j += 3)
                {
                    if (inds[j] == i || inds[j + 1] == i || inds[j + 2] == i)
                        continue;

                    Vector2 a = verts[inds[j]].XZ(), b = verts[inds[j + 1]].XZ(), c = verts[inds[j + 2]].XZ();

                    if (!ExtMathf.PointWithinTriangle2D(p, a, b, c))
                        continue;

                    Vector2 close1 = ExtMathf.ClosetPointOnLine(p, a, b),
                    close2 = ExtMathf.ClosetPointOnLine(p, a, b),
                    close3 = ExtMathf.ClosetPointOnLine(p, a, b);

                    Vector2 close;
                    if (Vector2.Distance(close1, p) < Vector2.Distance(close2, p) && Vector2.Distance(close1, p) < Vector2.Distance(close3, p))
                        close = close1;
                    else if (Vector2.Distance(close2, p) < Vector2.Distance(close3, p))
                        close = close2;
                    else
                        close = close3;

                    Vector2 offset = close - p;

                    verts[i] = verts[i] + (offset.normalized * (offset.magnitude + .01f)).ToV3(0);
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgreesParTitel, $"Checking point overlap with triangles: {i} / {verts.Count}", 1f / verts.Count * i))
                    throw new Exception("Cancel");
            }

            for (int original = 0; original < verts.Count; original++)
            {
                List<int> originalConnections = connectionsByIndex[original];

                for (int otherIndex = 0; otherIndex < originalConnections.Count; otherIndex++)
                {
                    int other = originalConnections[otherIndex];

                    if (other <= original)
                        continue;

                    for (int finalIndex = otherIndex + 1; finalIndex < originalConnections.Count; finalIndex++)
                    {
                        int final = originalConnections[finalIndex];

                        if (final <= original || !connectionsByIndex[final].Contains(other))
                            continue;

                        bool denied = false;

                        Vector2 a = verts[original].XZ(), b = verts[other].XZ(), c = verts[final].XZ();
                        Vector2 center = Vector2.Lerp(Vector2.Lerp(a, b, .5f), c, .5f);

                        float minX = Mathf.Min(Mathf.Min(a.x, b.x), c.x),
                            minY = Mathf.Min(Mathf.Min(a.y, b.y), c.y),
                            maxX = Mathf.Max(Mathf.Max(a.x, b.x), c.x),
                            maxY = Mathf.Max(Mathf.Max(a.y, b.y), c.y);

                        for (int x = 0; x < inds.Count; x += 3)
                        {
                            List<int> checkArr = new() { inds[x], inds[x + 1], inds[x + 2] };
                            if (checkArr.Contains(original) && checkArr.Contains(other) && checkArr.Contains(final))
                            {
                                //The triangle already exists
                                denied = true;
                                break;
                            }

                            Vector2 aP = verts[checkArr[0]].XZ(),
                                bP = verts[checkArr[1]].XZ(),
                                cP = verts[checkArr[2]].XZ();

                            //Boundings
                            if (maxX < Mathf.Min(Mathf.Min(aP.x, bP.x), cP.x) ||
                                maxY < Mathf.Min(Mathf.Min(aP.y, bP.y), cP.y) ||
                                minX > Mathf.Max(Mathf.Max(aP.x, bP.x), cP.x) ||
                                minY > Mathf.Max(Mathf.Max(aP.y, bP.y), cP.y))
                                continue;

                            //One of the new triangle points is within an already existing triangle
                            if (ExtMathf.PointWithinTriangle2D(center, aP, bP, cP) ||
                                ExtMathf.PointWithinTriangle2D(a, aP, bP, cP) ||
                                ExtMathf.PointWithinTriangle2D(b, aP, bP, cP) ||
                                ExtMathf.PointWithinTriangle2D(c, aP, bP, cP))
                            {
                                denied = true;
                                break;
                            }

                            if (ExtMathf.TriangleIntersect2D(a, b, c, aP, bP, cP))
                            {
                                denied = true;
                                break;
                            }
                        }

                        if (denied)
                            continue;

                        areas.Add(0);
                        inds.Add(original);
                        inds.Add(other);
                        inds.Add(final);
                    }
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgreesParTitel, $"Finding and filling holes: {original} / {verts.Count}", 1f / verts.Count * original))
                    throw new Exception("Cancel");
            }
        }

        #endregion
    }
}