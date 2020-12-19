using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    #region Values
    [Header("Object Reference")]
    [SerializeField] private Transform toFollow = null;
    [SerializeField] private Transform camTransform = null;
    #endregion
}
