#region Libraries

using Assets.Scripts.Runtime.World.Overworld;
using Cysharp.Threading.Tasks;
using Runtime.AI.World.Navigation;
using Runtime.Common;
using Runtime.Editor;
using Runtime.World.Overworld;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
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
        #region Build In States

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            propertyInfos.AddDelegate("Bake Navigation Mesh", () => this.BakeNavmesh(this.ValueEntry.Values[0]), new FoldoutGroupAttribute("Navigation"));
            propertyInfos.AddDelegate("Bake Lightning", () => this.BakeLighting(this.ValueEntry.Values[0]));
        }

        #endregion

        #region Internal

        private async void BakeNavmesh(TileSubController tileSubController)
        {
            if (BakedEditorManager.IsBakeRunning || tileSubController == null)
                return;

            string editorProgressParTitel = "Baking Navigation Mesh for " + tileSubController.name;

            BakedEditorManager.SetRunning(true);

            List<string> neighborsToLoad = new();

            foreach (string path in UnityEngine.Object.FindObjectsOfType<ConnectionPoint>().Select(cp => cp.ScenePath).ToArray())
            {
                if (!neighborsToLoad.Contains(path))
                    neighborsToLoad.Add(path);
            }

            try
            {
                ///Load neighboring scenes that the navigation mesh should cover
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Loading Neightbors", .25f);
                List<UniTask<Scene>> loadSceneTasks = new();

                for (int i = 0; i < neighborsToLoad.Count; i++)
                {
                    loadSceneTasks.Add(this.AsyncLoadScene(neighborsToLoad[i]));
                    EditorUtility.DisplayProgressBar(editorProgressParTitel, "Loading Neightbors", .25f + (.5f / neighborsToLoad.Count * i));
                }

                Scene[] loadedScenes = await UniTask.WhenAll(loadSceneTasks);
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Loading Neightbors", 1f);

                ///Custruct Navigation Mesh and setup custom navmesh logic
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Calculation Navigation Mesh Triangulation", 0f);
                NavMeshSurface surface = tileSubController.GetComponent<NavMeshSurface>();
                surface.BuildNavMesh();
                NavMeshTriangulation navmesh = NavMesh.CalculateTriangulation();
                surface.RemoveData();

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Getting Navigation Points", .5f);
                NavigationPoint[] navigationPoints = tileSubController.gameObject.GetAllComponentsByRoot<NavigationPoint>();
                Dictionary<int, NavigationPointEntry[]> pointsByTriangleIndex = new();


                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Creating first iteration NavTriangles", 0f);
                List<NavTriangle> triangles = new();
                Dictionary<int, List<int>> trianglesByVertexID = new();
                for (int i = 0; i < navmesh.indices.Length; i += 3)
                {
                    int a = navmesh.indices[i], b = navmesh.indices[i + 1], c = navmesh.indices[i + 2];
                    NavTriangle triangle = new(a, b, c);

                    triangles.Add(triangle);

                    int tID = triangles.Count - 1;

                    if (trianglesByVertexID.TryGetValue(a, out List<int> listA))
                        listA.Add(tID);
                    else
                        trianglesByVertexID.Add(a, new List<int>() { tID });

                    if (trianglesByVertexID.TryGetValue(b, out List<int> listB))
                        listB.Add(tID);
                    else
                        trianglesByVertexID.Add(b, new List<int>() { tID });

                    if (trianglesByVertexID.TryGetValue(c, out List<int> listC))
                        listC.Add(tID);
                    else
                        trianglesByVertexID.Add(c, new List<int>() { tID });
                }

                this.SetupNeighbors(triangles.ToArray(), editorProgressParTitel);

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Checking NavTriangle neighbor connections", .5f);
                Vector3[] vertices = navmesh.vertices;
                Vector3 cleanPoint = tileSubController.GetCleanUpPoint;
                int closestTriangleToCleanUpPoint = 0, closestVert = 0;
                float closestDistance = cleanPoint.QuickSquareDistance(vertices[closestVert]);
                for (int i = 1; i < vertices.Length; i++)
                {
                    float d = cleanPoint.QuickSquareDistance(vertices[i]);
                    if (d >= closestDistance)
                        continue;

                    closestDistance = d;
                    closestVert = i;
                }

                closestTriangleToCleanUpPoint = triangles.IndexOf(triangles.Where(t => t.Vertices.Contains(closestVert)).ToArray().RandomFrom());

                List<int> connected = new(), toCheck = new() { closestTriangleToCleanUpPoint };

                while (toCheck.Count > 0)
                {
                    NavTriangle navTriangle = triangles[toCheck[0]];
                    toCheck.RemoveAt(0);
                    connected.Add(triangles.IndexOf(navTriangle));

                    foreach (int n in navTriangle.Neighbors)
                    {
                        if (!toCheck.Contains(n) && !connected.Contains(n))
                            toCheck.Add(n);
                    }

                    EditorUtility.DisplayProgressBar(editorProgressParTitel, "Checking NavTriangle neighbor connections", .5f + (.5f / triangles.Count * connected.Count));
                }

                Debug.Log(connected.Count + "  /  " + triangles.Count);
                List<Vector3> fixedVerties = new();
                List<NavTriangle> fixedTriangles = new();

                int count = 0;
                foreach (NavigationPointEntry entry in navigationPoints.SelectMany(p => p.GetEntryPoints()))
                {
                    count++;
                    Vector3 pointPosition = entry.Position;
                    int closestIndex = 0;
                    float curDistance = 1000f;
                    for (int i = 1; i < vertices.Count(); i++)
                    {
                        if (pointPosition.QuickSquareDistance(vertices[i]) > curDistance)
                            continue;

                        closestIndex = i;
                        curDistance = pointPosition.QuickSquareDistance(vertices[i]);
                    }

                    if (pointsByTriangleIndex.TryGetValue(closestIndex, out NavigationPointEntry[] value))
                        value.Append(entry);
                    else
                        pointsByTriangleIndex.Add(closestIndex, new NavigationPointEntry[] { entry });

                    EditorUtility.DisplayProgressBar(editorProgressParTitel, "Indexing Navigation Points", .5f + (.5f / navigationPoints.Count() * count));
                    await UniTask.NextFrame();
                }
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Indexing Navigation Points", 1f);

                CalculatedNavMesh calculatedNavMesh = ScriptableObject.CreateInstance<CalculatedNavMesh>();
                calculatedNavMesh.SetValues(navmesh.vertices, connected.Select(i => triangles[i]).ToArray(), navmesh.areas, navigationPoints, pointsByTriangleIndex);
                tileSubController.SetCalculatedNavMesh(calculatedNavMesh);

                tileSubController.gameObject.GetFirstComponentByRoot<NavMeshVisualizor>()?.Create();

                ///Unload the neighboring scenes
                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Unloading Neightbors", 0f);
                List<UniTask> unloadTasks = new();

                for (int i = 0; i < loadedScenes.Length; i++)
                    unloadTasks.Add(this.AsyncUnloadScene(loadedScenes[i]));

                await UniTask.WhenAll(unloadTasks);

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Unloading Neightbors", 1f);
            }
            catch (Exception e)
            {
                Debug.LogError("Baking Navigation Mesh Failed");
                Debug.LogError(e);
            }

            EditorUtility.ClearProgressBar();

            BakedEditorManager.SetRunning(false);
        }

        private async void BakeLighting(TileSubController tileSubController)
        {
            if (BakedEditorManager.IsBakeRunning)
                return;

            BakedEditorManager.SetRunning(true);

            try
            {

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

        private void SetupNeighbors(NavTriangle[] triangles, string editorProgressParTitel)
        {
            EditorUtility.DisplayProgressBar(editorProgressParTitel, "Setting up NavTriangles neighbors", .25f);
            for (int i = 0; i < triangles.Length; i++)
            {
                NavTriangle t = triangles[i];
                List<int> neighbors = new();

                int a = t.Vertices[0], b = t.Vertices[1], c = t.Vertices[2];

                for (int j = 0; j < triangles.Length; j++)
                {
                    if (i == j)
                        continue;

                    NavTriangle t2 = triangles[j];

                    int sharedVertexID = 0;

                    if (t2.Vertices.Contains(a))
                        sharedVertexID++;

                    if (t2.Vertices.Contains(b))
                        sharedVertexID++;

                    if (t2.Vertices.Contains(c))
                        sharedVertexID++;

                    if (sharedVertexID >= 2)
                        neighbors.Add(j);
                }

                t.SetNeighborIDs(neighbors.ToArray());

                EditorUtility.DisplayProgressBar(editorProgressParTitel, "Setting up NavTriangles neighbors", .25f + (25f / triangles.Length * i));
            }
        }

        #endregion
    }
}