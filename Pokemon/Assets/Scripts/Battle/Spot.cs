#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

public class Spot : MonoBehaviour
{
    #region Values
    [SerializeField] private bool active = true;
    [SerializeField] private Pokemon activePokemon = null;
    [SerializeField] private int spotNumber = -1;
    [SerializeField] private Spot left = null, right = null, front = null;
    [SerializeField] private Transform currentTransform = null;
    [SerializeField] private Trainer.Team teamAllowed = null;
    #endregion

    #region Getters
    public Spot GetLeft()
    {
        return left;
    }
    public Spot GetRight()
    {
        return right;
    }
    public Spot GetFront()
    {
        return front;
    }

    public Pokemon GetActivePokemon()
    {
        return activePokemon;
    }

    public int GetSpotNumber()
    {
        return spotNumber;
    }

    public bool GetActive()
    {
        return active;
    }

    public Transform GetTransform()
    {
        return currentTransform;
    }
    #endregion

    #region Setters
    public void SetLeft(Spot set)
    {
        left = set;
    }
    public void SetRight(Spot set)
    {
        right = set;
    }
    public void SetFront(Spot set)
    {
        front = set;
    }

    public void SetActivePokemon(Pokemon set)
    {
        activePokemon = set;
    }

    public void SetSpotNumber(int set)
    {
        spotNumber = set;
    }

    public void SetActive(bool set)
    {
        active = set;
    }

    public void SetTransform()
    {
        currentTransform = transform;
    }
    #endregion
}