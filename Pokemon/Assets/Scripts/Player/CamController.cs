using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class CamController : MonoBehaviour
    {
        #region Values
        [Header("Object Reference")]
        [SerializeField] private Transform holder = null;
        [SerializeField] private Transform camTransform = null;
        #endregion

        #region Build In States
        private void Start()
        {
            holder = transform;
            camTransform = holder.GetChild(1);
        }

        private void Update()
        {

        }
        #endregion

        #region Getters

        #endregion

        #region Setters
        #endregion

        #region In
        public void LoadSaveString(string input)
        {
            string[] values = input.Split(';');
            holder.localRotation = Quaternion.Euler(new Vector3(0, float.Parse(values[0]), 0));
            camTransform.localRotation = Quaternion.Euler(new Vector3(float.Parse(values[1]), 0, 0));
        }
        #endregion

        #region Out
        public string ToSaveString()
        {
            string result = "";

            result += holder.localRotation.eulerAngles.y + ";";
            result += camTransform.localRotation.eulerAngles.x + ";";

            return result;
        }
        #endregion
    }
}