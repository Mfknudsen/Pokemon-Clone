#region Libraries

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Editor.Systems;
using Runtime.AI.Navigation;
using Runtime.Common;
using Runtime.Editor;
using Runtime.World.Overworld;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

#endregion

namespace Editor.World.TileSubController
{
    public sealed class TileSubControllerProcessor : OdinPropertyProcessor<Runtime.World.Overworld.TileSubController>
    {
        #region Values

        private static float overlapCheckDistance = .3f;

        #endregion

        #region Build In States

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            propertyInfos.AddValue("Vertex Overlap Check Distance",
                (ref Runtime.World.Overworld.TileSubController _) => overlapCheckDistance,
                (ref Runtime.World.Overworld.TileSubController _, float d) => overlapCheckDistance = d,
                new FoldoutGroupAttribute("Navigation"),
                new MinValueAttribute(.001f),
                new LabelWidthAttribute(180f));

            propertyInfos.AddDelegate("Bake Navigation Mesh", () => this.BakeNavmesh(this.ValueEntry.Values[0]),
                new FoldoutGroupAttribute("Navigation"));
            propertyInfos.AddDelegate("Bake Lightning", () => this.BakeLighting(this.ValueEntry.Values[0]),
                new FoldoutGroupAttribute("Lightning"));
        }

        #endregion

        #region Internal

        #region Custom Navmesh Baking

        /// <summary>
        /// Bake a custom Navmesh for use with the custom Navmesh agents.
        /// </summary>
        /// <param name="tileSubController">The current TileSubController</param>
        private async void BakeNavmesh(Runtime.World.Overworld.TileSubController tileSubController)
        {
            if (BakedEditorManager.IsBakeRunning || tileSubController == null)
                return;

            string editorProgressParTitle = "Baking Navigation: " + tileSubController.gameObject.scene.name;

            BakedEditorManager.SetRunning(true);

            List<string> neighborsToLoad = new();

            foreach (string path in Object.FindObjectsOfType<ConnectionPoint>().Select(cp => cp.ScenePath)
                         .ToArray())
            {
                if (!neighborsToLoad.Contains(path))
                    neighborsToLoad.Add(path);
            }

            Scene[] loadedScenes = Array.Empty<Scene>();
            try
            {
                #region Load neighbor scenes and build navmesh triangulation

                //Load neighboring scenes that the navigation mesh should cover
                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Loading Neighbors", .25f);
                List<UniTask<Scene>> loadSceneTasks = new();

                for (int i = 0; i < neighborsToLoad.Count; i++)
                {
                    loadSceneTasks.Add(this.AsyncLoadScene(neighborsToLoad[i]));
                    EditorUtility.DisplayProgressBar(editorProgressParTitle, "Loading Neighbors",
                        .25f + (.5f / neighborsToLoad.Count * i));
                }

                loadedScenes = await UniTask.WhenAll(loadSceneTasks);
                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Loading Neighbors", 1f);

                //Construct Navigation Mesh and setup custom navmesh logic
                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Calculation Navigation Mesh Triangulation",
                    0f);

                NavMeshTriangulation navmesh = this.BuildNavMeshTriangulation(tileSubController);

                #endregion

                #region Check Vertices and Indices for overlap

                List<Vector3> verts = navmesh.vertices.ToList();
                List<int> indices = navmesh.indices.ToList();
                List<int> areas = navmesh.areas.ToList();
                Dictionary<Vector2Int, List<int>> vertsByPos = new();

                const float groupSize = 5f;

                for (int i = 0; i < verts.Count; i++)
                {
                    Vector3 v = verts[i];
                    Vector2Int id = new(Mathf.FloorToInt(v.x / groupSize), Mathf.FloorToInt(v.z / groupSize));

                    if (vertsByPos.TryGetValue(id, out List<int> outList))
                        outList.Add(i);
                    else
                        vertsByPos.Add(id, new List<int> { i });
                }

                this.CheckOverlap(verts, indices, vertsByPos, groupSize, editorProgressParTitle);

                #endregion

                #region Create first iteration of NavTriangles

                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Creating first iteration NavTriangles", 0f);
                List<NavTriangle> triangles = new();
                Dictionary<int, List<int>> trianglesByVertexID = new();

                this.SetupNavTriangles(verts, indices, areas, triangles, trianglesByVertexID, editorProgressParTitle);

                triangles = this.SetupNeighbors(triangles, trianglesByVertexID, editorProgressParTitle, "First");

                #endregion

                #region Check neighbor connections

                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Checking NavTriangle neighbor connections",
                    .5f);
                Vector3 cleanPoint = tileSubController.GetCleanUpPoint;
                int closestVert = 0;
                float closestDistance = cleanPoint.QuickSquareDistance(verts[closestVert]);

                for (int i = 1; i < verts.Count; i++)
                {
                    float d = cleanPoint.QuickSquareDistance(verts[i]);
                    if (d >= closestDistance)
                        continue;

                    if (trianglesByVertexID.TryGetValue(i, out List<int> value) &&
                        !value.Any(t => triangles[t].Neighbors.Count > 0))
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
                    foreach (int n in navTriangle.Neighbors)
                    {
                        if (!toCheck.Contains(n) && !connected.Contains(n))
                            toCheck.Add(n);
                    }

                    EditorUtility.DisplayProgressBar(editorProgressParTitle,
                        "Checking NavTriangle neighbor connections", .5f + (.5f / triangles.Count * connected.Count));
                }

                #endregion

                #region Fill holes and final iteration of NavTriangles

                List<Vector3> fixedVertices = new();
                List<int> fixedIndices = new(), fixedAreas = new();
                Dictionary<int, List<int>> fixedTrianglesByVertexID = new();

                foreach (NavTriangle t in connected.Select(i => triangles[i]))
                {
                    int aID = t.Vertices[0], bID = t.Vertices[1], cID = t.Vertices[2];
                    Vector3 a = verts[aID], b = verts[bID], c = verts[cID];

                    if (!fixedVertices.Contains(a))
                        fixedVertices.Add(a);

                    if (!fixedVertices.Contains(b))
                        fixedVertices.Add(b);

                    if (!fixedVertices.Contains(c))
                        fixedVertices.Add(c);

                    fixedIndices.Add(fixedVertices.IndexOf(a));
                    fixedIndices.Add(fixedVertices.IndexOf(b));
                    fixedIndices.Add(fixedVertices.IndexOf(c));

                    fixedAreas.Add(t.Area);

                    Vector2Int id = new(Mathf.FloorToInt(a.x / groupSize), Mathf.FloorToInt(a.z / groupSize));
                    if (vertsByPos.TryGetValue(id, out List<int> outListA))
                    {
                        if (outListA.Contains(fixedVertices.IndexOf(a)))
                            outListA.Add(fixedVertices.IndexOf(a));
                    }
                    else
                        vertsByPos.Add(id, new List<int> { fixedVertices.IndexOf(a) });

                    id = new Vector2Int(Mathf.FloorToInt(b.x / groupSize), Mathf.FloorToInt(b.z / groupSize));
                    if (vertsByPos.TryGetValue(id, out List<int> outListB))
                    {
                        if (outListB.Contains(fixedVertices.IndexOf(b)))
                            outListB.Add(fixedVertices.IndexOf(b));
                    }
                    else
                        vertsByPos.Add(id, new List<int> { fixedVertices.IndexOf(b) });

                    id = new Vector2Int(Mathf.FloorToInt(c.x / groupSize), Mathf.FloorToInt(c.z / groupSize));
                    if (vertsByPos.TryGetValue(id, out List<int> outListC))
                    {
                        if (outListC.Contains(fixedVertices.IndexOf(c)))
                            outListC.Add(fixedVertices.IndexOf(c));
                    }
                    else
                        vertsByPos.Add(id, new List<int> { fixedVertices.IndexOf(c) });
                }

                this.FillHoles(fixedVertices, fixedAreas, fixedIndices, editorProgressParTitle);

                List<NavTriangle> fixedTriangles = new();

                this.SetupNavTriangles(fixedVertices, fixedIndices, fixedAreas, fixedTriangles,
                    fixedTrianglesByVertexID, editorProgressParTitle);

                fixedTriangles = this.SetupNeighbors(fixedTriangles, fixedTrianglesByVertexID, editorProgressParTitle,
                    "Final");

                for (int i = 0; i < fixedTriangles.Count; i++)
                {
                    fixedTriangles[i].SetBorderWidth(fixedVertices, fixedTriangles);
                    EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle, "Setting border width",
                        1f / fixedTriangles.Count * (i + 1));
                }

                Dictionary<int, List<NavigationPointEntry>> fixedEntryPoints =
                    this.SetupEntryPointsForTriangles(fixedTriangles.ToArray(), fixedTrianglesByVertexID,
                        fixedVertices.ToArray());

                #endregion

                #region Create the storage to contain the final result

                Scene s = tileSubController.gameObject.scene;
                string assetPath = s.path.Replace(".unity", "/"), assetName = s.name + " NavMesh Calculations.asset";

                try
                {
                    CalculatedNavMesh calculatedNavMesh =
                        AssetDatabase.LoadAssetAtPath<CalculatedNavMesh>(assetPath + assetName);
                    calculatedNavMesh.SetValues(fixedVertices.ToArray(), fixedTriangles.ToArray(), fixedAreas.ToArray(),
                        fixedEntryPoints);
                    EditorUtility.SetDirty(calculatedNavMesh);
                }
                catch
                {
                    CalculatedNavMesh calculatedNavMesh = ScriptableObject.CreateInstance<CalculatedNavMesh>();
                    calculatedNavMesh.name = s.name + " NavMesh Calculations";
                    calculatedNavMesh.SetValues(fixedVertices.ToArray(), fixedTriangles.ToArray(), fixedAreas.ToArray(),
                        fixedEntryPoints);
                    tileSubController.SetCalculatedNavMesh(calculatedNavMesh);

                    EditorUtility.SetDirty(calculatedNavMesh);
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
                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Unloading Neighbors", 0f);
                List<UniTask> unloadTasks = new();

                for (int i = 0; i < loadedScenes.Length; i++)
                    unloadTasks.Add(this.AsyncUnloadScene(loadedScenes[i]));

                await UniTask.WhenAll(unloadTasks);

                EditorUtility.DisplayProgressBar(editorProgressParTitle, "Unloading Neighbors", 1f);
            }

            #endregion

            EditorUtility.ClearProgressBar();

            EditorSceneManager.SaveScene(tileSubController.gameObject.scene);

            BakedEditorManager.SetRunning(false);
        }

        /// <summary>
        /// Bake the lighting using Bakery asset
        /// </summary>
        /// <param name="tileSubController">The current TileSubController</param>
        private async void BakeLighting(Runtime.World.Overworld.TileSubController tileSubController)
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

        /// <summary>
        /// Loads the scene at the path async
        /// </summary>
        /// <param name="path">File path for the scene</param>
        /// <returns></returns>
        private async UniTask<Scene> AsyncLoadScene(string path)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

            await UniTask.WaitWhile(() => !scene.isLoaded);

            return scene;
        }

        /// <summary>
        /// Unloads a currently loaded scene async
        /// </summary>
        /// <param name="scene">Name of the loaded scene to unload</param>
        private async UniTask AsyncUnloadScene(Scene scene)
        {
            EditorSceneManager.CloseScene(scene, true);

            await UniTask.WaitWhile(() => scene.isLoaded);
        }

        /// <summary>
        /// Builds a navmesh from the TileSubController and returns the navmesh triangulation from the build navmesh.
        /// Then remove the build navmesh as it is no longer needed.
        /// </summary>
        /// <param name="tileSubController">To build the navmesh from</param>
        /// <returns>Navmesh triangulation containing vertices, indices and areas in arrays</returns>
        private NavMeshTriangulation BuildNavMeshTriangulation(
            Runtime.World.Overworld.TileSubController tileSubController)
        {
            NavMeshSurface surface = tileSubController.GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
            NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
            surface.RemoveData();
            surface.navMeshData = null;
            return navmesh;
        }

        /// <summary>
        /// Creates new NavTriangles from the indices 
        /// </summary>
        /// <param name="verts">3D vertices</param>
        /// <param name="indices">Each pair of threes indicate one triangle</param>
        /// <param name="areas">Area values for each triangle</param>
        /// <param name="triangles">List for the triangles to be added to</param>
        /// <param name="trianglesByVertexID">Each triangle will be assigned to each relevant vertex for optimization later</param>
        /// <param name="editorProgressParTitle">Editor loading bar title</param>
        /// <exception cref="Exception">Throws "Cancel" if the user cancels the progress</exception>
        private void SetupNavTriangles(List<Vector3> verts, List<int> indices, List<int> areas,
            List<NavTriangle> triangles, Dictionary<int, List<int>> trianglesByVertexID, string editorProgressParTitle)
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

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Creating NavTriangles: {i / 3f} / {indices.Count / 3f}",
                        1f / (indices.Count / 3f) * (i / 3f)))
                    throw new Exception("Cancel");
            }
        }

        /// <summary>
        /// Setup the connected neighbors for each NavTriangle
        /// </summary>
        /// <param name="triangles">The triangles to check</param>
        /// <param name="trianglesByVertexID">A list of triangle ids based on a vertex id</param>
        /// <param name="editorProgressParTitle">Editor loading bar title</param>
        /// <param name="iteration">This will run multiple times during baking. Will help the user know which step</param>
        /// <returns>The current triangle list now with neighbors set</returns>
        /// <exception cref="Exception">Throws "Cancel" if the user cancels the progress</exception>
        private List<NavTriangle> SetupNeighbors(List<NavTriangle> triangles,
            Dictionary<int, List<int>> trianglesByVertexID, string editorProgressParTitle, string iteration)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                List<int> neighbors = new();
                List<int> possibleNeighbors = new();
                possibleNeighbors.AddRange(trianglesByVertexID[triangles[i].Vertices[0]]);
                possibleNeighbors.AddRange(trianglesByVertexID[triangles[i].Vertices[1]]);
                possibleNeighbors.AddRange(trianglesByVertexID[triangles[i].Vertices[2]]);

                possibleNeighbors = possibleNeighbors.Where(t => t != i).ToList();

                for (int j = 0; j < possibleNeighbors.Count; j++)
                {
                    if (triangles[i].Vertices.SharedBetween(triangles[possibleNeighbors[j]].Vertices).Length == 2)
                        neighbors.Add(possibleNeighbors[j]);

                    if (triangles.Count == 3)
                        break;
                }

                triangles[i].SetNeighborIDs(neighbors.ToArray());

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Setting up NavTriangles neighbors. {iteration} iteration: {i} / {triangles.Count}",
                        1f / triangles.Count * i))
                    throw new Exception("Cancel");
            }

            return triangles;
        }

        /// <summary>
        /// Setup what entry points each triangle contains
        /// </summary>
        /// <param name="navTriangles">Triangles to check if entry point is within</param>
        /// <param name="trianglesByVertexID">A list of triangle ids based on a vertex id</param>
        /// <param name="verts">3D vertices</param>
        /// <returns>List of entry point ids based on triangle id</returns>
        private Dictionary<int, List<NavigationPointEntry>> SetupEntryPointsForTriangles(NavTriangle[] navTriangles,
            Dictionary<int, List<int>> trianglesByVertexID, Vector3[] verts)
        {
            NavigationPointEntry[] entries = Object.FindObjectsOfType<NavigationPoint>()
                .SelectMany(p => p.GetEntryPoints()).ToArray();
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

        /// <summary>
        /// Check each vertex and if its close to another then remove the other and replace any indices containing it with the current one.
        /// If the other vertex is in a triangle with the current then remove said triangle.
        /// </summary>
        /// <param name="verts">3D vertices</param>
        /// <param name="indices">Each pair of threes indicate one triangle</param>
        /// <param name="vertsByPos">List of vertex ids based on the divided floor int of x and z value</param>
        /// <param name="groupSize">Division value for creating the vertex groupings</param>
        /// <param name="editorProgressParTitle">Editor loading bar title</param>
        /// <exception cref="Exception">Throws "Cancel" if the user cancels the progress</exception>
        private void CheckOverlap(List<Vector3> verts, List<int> indices, Dictionary<Vector2Int, List<int>> vertsByPos,
            float groupSize, string editorProgressParTitle)
        {
            float divided = 20f;
            Dictionary<int, List<int>> removed = new();
            for (int original = 0; original < verts.Count; original++)
            {
                if (removed.TryGetValue(Mathf.FloorToInt(original / divided), out List<int> removedList))
                {
                    if (removedList.Contains(original))
                        continue;
                }

                Vector2Int id = new(Mathf.FloorToInt(verts[original].x / groupSize),
                    Mathf.FloorToInt(verts[original].z / groupSize));
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
                    if (removed.TryGetValue(Mathf.FloorToInt(other / divided), out removedList))
                    {
                        if (removedList.Contains(other))
                            continue;
                    }

                    if (Vector3.Distance(verts[original], verts[other]) > overlapCheckDistance)
                        continue;

                    if (removed.TryGetValue(Mathf.FloorToInt(other / divided), out removedList))
                        removedList.Add(other);
                    else
                        removed.Add(Mathf.FloorToInt(other / divided), new List<int> { other });

                    for (int indicesIndex = 0; indicesIndex < indices.Count; indicesIndex++)
                    {
                        if (indices[indicesIndex] == other)
                            indices[indicesIndex] = original;
                    }
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Checking vertex overlap: {original} / {verts.Count}", 1f / verts.Count * original))
                    throw new Exception("Cancel");
            }

            List<int> toRemove = removed.Values.SelectMany(l => l).OrderBy(x => -x).ToList();
            for (int i = 0; i < toRemove.Count; i++)
            {
                int index = toRemove[i];
                vertsByPos[
                    new Vector2Int(Mathf.FloorToInt(verts[index].x / groupSize),
                        Mathf.FloorToInt(verts[index].z / groupSize))].Remove(index);
                verts.RemoveAt(index);

                for (int j = 0; j < indices.Count; j++)
                {
                    if (indices[j] >= index)
                        indices[j] = indices[j] - 1;
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Removing overlapping vertices: {i} / {toRemove.Count}", 1f / toRemove.Count * i))
                    throw new Exception("Cancel");
            }

            for (int i = indices.Count - 1; i >= 0; i--)
            {
                if (i % 3 != 0)
                    continue;

                if (indices[i] == indices[i + 1] || indices[i] == indices[i + 2] || indices[i + 1] == indices[i + 2] ||
                    indices[i] >= verts.Count || indices[i + 1] >= verts.Count || indices[i + 2] >= verts.Count)
                {
                    indices.RemoveAt(i);
                    indices.RemoveAt(i);
                    indices.RemoveAt(i);
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Correcting indices indexes: {indices.Count - i} / {verts.Count}",
                        1f / verts.Count * (indices.Count - i)))
                    throw new Exception("Cancel");
            }
        }

        /// <summary>
        /// Fill any holes that might have appeared by checking overlap.
        /// If any three vertexes are directly connected to the two others then add a triangle between them.
        /// </summary>
        /// <param name="verts">3D vertices</param>
        /// <param name="areas">When a new triangle is created then add an area value as well</param>
        /// <param name="indices">Each pair of threes indicate one triangle</param>
        /// <param name="editorProgressParTitle">Editor loading bar title</param>
        /// <exception cref="Exception">Throws "Cancel" if the user cancels the progress</exception>
        private void FillHoles(List<Vector3> verts, List<int> areas, List<int> indices,
            string editorProgressParTitle)
        {
            Dictionary<int, List<int>> connectionsByIndex = new();
            Dictionary<int, List<int>> indicesByIndex = new();
            for (int i = 0; i < verts.Count; i++)
            {
                connectionsByIndex.Add(i, new List<int>());
                indicesByIndex.Add(i, new List<int>());
            }

            for (int i = 0; i < indices.Count; i += 3)
            {
                if (!connectionsByIndex[indices[i]].Contains(indices[i + 1]))
                    connectionsByIndex[indices[i]].Add(indices[i + 1]);
                if (!connectionsByIndex[indices[i]].Contains(indices[i + 2]))
                    connectionsByIndex[indices[i]].Add(indices[i + 2]);

                if (!connectionsByIndex[indices[i + 1]].Contains(indices[i]))
                    connectionsByIndex[indices[i + 1]].Add(indices[i]);
                if (!connectionsByIndex[indices[i + 1]].Contains(indices[i + 2]))
                    connectionsByIndex[indices[i + 1]].Add(indices[i + 2]);

                if (!connectionsByIndex[indices[i + 2]].Contains(indices[i]))
                    connectionsByIndex[indices[i + 2]].Add(indices[i]);
                if (!connectionsByIndex[indices[i + 2]].Contains(indices[i + 1]))
                    connectionsByIndex[indices[i + 2]].Add(indices[i + 1]);

                int[] arr = { indices[i], indices[i + 1], indices[i + 2] };
                indicesByIndex[indices[i]].AddRange(arr);
                indicesByIndex[indices[i + 1]].AddRange(arr);
                indicesByIndex[indices[i + 2]].AddRange(arr);

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Collecting vertex connections and indices: {i} / {indices.Count}", 1f / indices.Count * i))
                    throw new Exception("Cancel");
            }

            for (int i = 0; i < verts.Count; i++)
            {
                Vector2 p = verts[i].XZ();

                for (int j = 0; j < indices.Count; j += 3)
                {
                    if (indices[j] == i || indices[j + 1] == i || indices[j + 2] == i)
                        continue;

                    Vector2 a = verts[indices[j]].XZ(), b = verts[indices[j + 1]].XZ(), c = verts[indices[j + 2]].XZ();

                    if (!ExtMathf.PointWithinTriangle2D(p, a, b, c))
                        continue;

                    Vector2 close1 = ExtMathf.ClosetPointOnLine(p, a, b),
                        close2 = ExtMathf.ClosetPointOnLine(p, a, b),
                        close3 = ExtMathf.ClosetPointOnLine(p, a, b);

                    Vector2 close;
                    if (Vector2.Distance(close1, p) < Vector2.Distance(close2, p) &&
                        Vector2.Distance(close1, p) < Vector2.Distance(close3, p))
                        close = close1;
                    else if (Vector2.Distance(close2, p) < Vector2.Distance(close3, p))
                        close = close2;
                    else
                        close = close3;

                    Vector2 offset = close - p;

                    verts[i] = verts[i] + (offset.normalized * (offset.magnitude + .01f)).ToV3(0);
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Checking point overlap with triangles: {i} / {verts.Count}", 1f / verts.Count * i))
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

                        for (int x = 0; x < indices.Count; x += 3)
                        {
                            List<int> checkArr = new() { indices[x], indices[x + 1], indices[x + 2] };
                            if (checkArr.Contains(original) && checkArr.Contains(other) && checkArr.Contains(final))
                            {
                                //The triangle already exists
                                denied = true;
                                break;
                            }

                            Vector2 aP = verts[checkArr[0]].XZ(),
                                bP = verts[checkArr[1]].XZ(),
                                cP = verts[checkArr[2]].XZ();

                            //Bounding
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
                        indices.Add(original);
                        indices.Add(other);
                        indices.Add(final);
                    }
                }

                if (EditorUtility.DisplayCancelableProgressBar(editorProgressParTitle,
                        $"Finding and filling holes: {original} / {verts.Count}", 1f / verts.Count * original))
                    throw new Exception("Cancel");
            }
        }

        #endregion

        #endregion
    }
}