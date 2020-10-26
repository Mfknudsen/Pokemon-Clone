using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trainer
{
    public class Team : MonoBehaviour
    {
        public GameObject[] teamMembers = new GameObject[6];

        public void SwitchTeamPlaces(int from, int to)
        {
            GameObject toStore = teamMembers[to];
            teamMembers[to] = teamMembers[from];
            teamMembers[from] = toStore;
        }
    }
}