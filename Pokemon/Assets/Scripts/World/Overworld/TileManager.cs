#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class TileManager : MonoBehaviour
{
    #region Values
    public Dictionary<string, GameObject> objectsInTile = new Dictionary<string, GameObject>();
    [SerializeField] private Transform[] spawnPoints = new Transform[0];
    [SerializeField] private Animation[] spawnAnimation = new Animation[0];
    #endregion

    private void OnValidate()
    {
        objectsInTile.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            objectsInTile.Add(obj.name, obj);
        }
    }

    public void SpawnAtPoint(int pointIndex)
    {
        
    }
}