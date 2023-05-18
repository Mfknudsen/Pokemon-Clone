#region Libraries

using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Runtime.Pokémon;

#endregion

namespace Runtime.World.Overworld.Spawner
{
    public class SpawnLocation : MonoBehaviour
    {
        #region Values

        [SerializeReference, HideReferenceObjectPicker]
        private ISpawnType spawnType = new Area();

        [SerializeField] private Pokemon[] allowedToSpawn = new Pokemon[0];

#if UNITY_EDITOR
        [ValueDropdown("valueOptions")]
        public string selectedDropdownOption = "Area";

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly string[] valueOptions = { "Area", "Points" };
#pragma warning restore IDE0052 // Remove unread private members
#endif

        #endregion

        #region Build In States
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.spawnType is not Location && this.selectedDropdownOption.Equals("Points"))
                this.spawnType = new Location();

            if (this.spawnType is not Area && this.selectedDropdownOption.Equals("Area"))
                this.spawnType = new Area();

            this.spawnType.Validate(this);
        }

        private void OnDrawGizmos()
        {
            this.spawnType.Gizmo(this);
        }
#endif

        #endregion

        #region Getters

#if UNITY_EDITOR
        public bool IsAreaSpawnType => this.spawnType is Area;

        public Vector3[] GetAreaPoints => ((Area)this.spawnType).GetPoints;

        public Transform[] GetLocationPoints => ((Location)this.spawnType).GetTransforms;

#endif

        #endregion

        #region Setters

#if UNITY_EDITOR
        public void SetAreaPointPosition(int i, Vector3 pos) => ((Area)this.spawnType).SetPointByID(i, pos);
#endif

        #endregion

        #region In

#if UNITY_EDITOR

        public void CreateAreaPoint() => ((Area)this.spawnType).AddPoint(this.transform.position);

        public bool TryRemoveAreaPoint(int id) => ((Area)this.spawnType).TryRemovePoint(id);

        public bool TryCreateNewAreaTriangle(int[] ids) => ((Area)this.spawnType).TryCreateNewTriangle(ids);

        public bool TryRemoveAreaTriangle(int[] ids) => ((Area)this.spawnType).TryRemoveTriangle(ids);

        public bool TryCleanAreaPoints() => ((Area)this.spawnType).TryCleanPoints();

#endif
        #endregion

        #region Out

        public bool Allowed(Pokemon pokemon) => this.allowedToSpawn.Contains(pokemon);

        public SpawnTypeResult GetSpawnResult => this.spawnType.GetResult();

        #endregion
    }

    public struct SpawnTypeResult
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    #region Area Types

    internal interface ISpawnType
    {
#if UNITY_EDITOR
        public void Gizmo(SpawnLocation self);

        public void Validate(SpawnLocation self);
#endif

        public SpawnTypeResult GetResult();

    }

    [Serializable]
    internal struct Location : ISpawnType
    {
        #region Values

        [SerializeField] private Transform[] locations;

        #endregion

        public SpawnTypeResult GetResult()
        {
            Transform t = this.locations[UnityEngine.Random.Range(0, this.locations.Length)];

            return new SpawnTypeResult
            {
                Position = t.position,
                Rotation = t.rotation,
            };
        }

        #region Getters

#if UNITY_EDITOR
        public Transform[] GetTransforms => this.locations;
#endif

        #endregion

        #region Setters
        #endregion

        #region In

#if UNITY_EDITOR
        public void Gizmo(SpawnLocation self)
        {
        }

        public void Validate(SpawnLocation self) =>
            this.locations ??= new Transform[0];
#endif

        #endregion
    }

    [Serializable]
    internal struct Area : ISpawnType
    {
        #region Values
#if UNITY_EDITOR
        [SerializeField] private List<Vector3> storedPoints;
        [SerializeField] private List<Vector3Int> storedTriangles;
#endif

        [SerializeField, HideInInspector] private Mesh mesh;

        #endregion

        #region Getters

#if UNITY_EDITOR
        public Vector3[] GetPoints => this.storedPoints.ToArray();

        public Mesh GetMesh => this.mesh;
#endif

        #endregion

        #region Setters
#if UNITY_EDITOR
        public void SetPointByID(int i, Vector3 position)
        {
            this.storedPoints[i] = position;
            this.mesh.SetVertices(this.storedPoints);
        }
#endif
        #endregion

        #region In

#if UNITY_EDITOR
        public void Gizmo(SpawnLocation self)
        {
            if (this.mesh != null)
            {
                List<Vector3> normals = new();

                for (int i = 0; i < this.mesh.vertexCount; i++)
                    normals.Add(Vector3.up);

                this.mesh.normals = normals.ToArray();
                Gizmos.DrawWireMesh(this.mesh, self.transform.position, self.transform.rotation);
            }
        }

        public void Validate(SpawnLocation self)
        {
            this.storedPoints ??= new List<Vector3>(0);
            this.storedTriangles ??= new List<Vector3Int>(0);

            if (this.storedPoints.Count < 3)
            {
                for (int i = this.storedPoints.Count; i < 3; i++)
                    this.storedPoints.Add(self.transform.position);
            }

            if (this.storedTriangles.Count == 0)
                this.storedTriangles.Add(new Vector3Int(0, 1, 2));

            this.GenerateMesh();
        }

        public void AddPoint(Vector3 startPosition)
        {
            this.storedPoints.Add(startPosition);
        }

        public bool TryRemovePoint(int toDelete)
        {
            if (this.storedTriangles.Count == 1)
            {
                Vector3Int t = this.storedTriangles[0];
                if (t.x == toDelete || t.y == toDelete || t.z == toDelete)
                    return false;
            }

            if (this.storedPoints.Count < toDelete - 1)
                return false;

            this.storedPoints.RemoveAt(toDelete);

            List<Vector3Int> replaceList = new();
            for (int i = 0; i < this.storedTriangles.Count; i++)
            {
                Vector3Int v = this.storedTriangles[i];

                if (v.x == toDelete || v.y == toDelete || v.z == toDelete)
                    continue;
                Debug.Log("Adding: " + v.ToString());
                replaceList.Add(v);
            }

            this.storedTriangles = replaceList;
            Debug.Log(storedTriangles.Count);

            this.StoreTrianglesToMeshTriangles();

            return true;
        }

        public bool TryCreateNewTriangle(int[] ids)
        {
            Vector3Int triangle = new(ids[0], ids[1], ids[2]);

            if (this.storedTriangles.Contains(triangle))
                return false;

            this.storedTriangles.Add(triangle);

            this.StoreTrianglesToMeshTriangles();

            return true;
        }

        public bool TryRemoveTriangle(int[] ids)
        {
            if (this.storedTriangles.Count == 1)
                return false;

            foreach (Vector3Int v in this.storedTriangles)
            {
                if (!(v.x == ids[0] || v.x == ids[1] || v.x == ids[2]) ||
                    !(v.y == ids[0] || v.y == ids[1] || v.y == ids[2]) ||
                    !(v.z == ids[0] || v.z == ids[1] || v.z == ids[2]))
                    continue;

                this.storedTriangles.Remove(v);

                this.StoreTrianglesToMeshTriangles();

                return true;
            }

            return false;
        }

        public bool TryCleanPoints()
        {
            bool changeHappend = false;

            List<int> toRemove = new();
            for (int i = 0; i < this.storedPoints.Count; i++)
            {
                if (!this.storedTriangles.Any(v => v.x == i || v.y == i || v.z == i))
                {
                    toRemove.Add(i);
                    changeHappend = true;
                }
            }

            for (int i = toRemove.Count - 1; i >= 0; i--)
            {
                this.storedPoints.RemoveAt(toRemove[i]);
                this.RecalculateIndexs(toRemove[i]);
            }

            return changeHappend;
        }
#endif
        #endregion

        #region Out

        public SpawnTypeResult GetResult()
        {
            Vector3Int ids = this.storedTriangles[UnityEngine.Random.Range(0, this.storedTriangles.Count)];
            Vector3 dirA = this.storedPoints[ids.y] - this.storedPoints[ids.x],
                dirB = this.storedPoints[ids.z] - this.storedPoints[ids.x];

            float aRandom = UnityEngine.Random.Range(0f, 1f), bRandom = UnityEngine.Random.Range(0f, 1f);

            if (!(aRandom + bRandom <= 1))
            {
                aRandom = 1 - aRandom;
                bRandom = 1 - bRandom;
            }

            Vector3 newPoint = this.storedPoints[ids.x] +
                Vector3.Lerp(Vector3.zero, dirA, aRandom) +
                Vector3.Lerp(Vector3.zero, dirB, bRandom);

            return new()
            {
                Position = newPoint,
                Rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0))
            };
        }

        #endregion

        #region Internal

#if UNITY_EDITOR
        private void RecalculateIndexs(int removed)
        {
            for (int i = 0; i < this.storedTriangles.Count; i++)
            {
                Vector3Int v = this.storedTriangles[i];
                v = new Vector3Int(
                    v.x > removed ? v.x - 1 : v.x,
                    v.y > removed ? v.y - 1 : v.y,
                    v.z > removed ? v.z - 1 : v.z
                    );

                this.storedTriangles[i] = v;
            }

            this.StoreTrianglesToMeshTriangles();
        }

        private void GenerateMesh()
        {
            this.mesh = new()
            {
                vertices = this.storedPoints.ToArray(),
            };

            this.StoreTrianglesToMeshTriangles();
        }

        private void StoreTrianglesToMeshTriangles()
        {
            List<int> triangles = new();
            this.storedTriangles.ForEach(t =>
            {
                triangles.Add(t.x);
                triangles.Add(t.y);
                triangles.Add(t.z);
            });

            this.mesh.triangles = triangles.ToArray();
        }
#endif

        #endregion
    }

    #endregion
}