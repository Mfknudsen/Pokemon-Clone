using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trainer;

namespace Player
{
    public class MasterPlayer : MonoBehaviour
    {
        [Header("Object Reference:")]
        public Team team;
        public Controller controller;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
