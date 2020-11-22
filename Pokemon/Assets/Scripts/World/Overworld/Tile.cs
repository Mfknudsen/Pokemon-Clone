using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2 placement = Vector2.zero;
    private bool empty = true;

    public void SetPlacement(Vector2 placement)
    {
        this.placement = placement;
    }

    public Vector2 GetPlacement()
    {
        return placement;
    }

    public bool IsEmpty()
    {
        return empty;
    }
}
