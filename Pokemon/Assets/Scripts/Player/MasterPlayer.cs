using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;

namespace Player
{
    public class MasterPlayer : MonoBehaviour
    {
        [Header("Object Reference:")]
        public static MasterPlayer instance = null;
        public Team team;
        public Controller controller;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}
