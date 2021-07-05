#region SDK

using System.Collections.Generic;
using Mfknudsen.Battle.UI;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class Spot : MonoBehaviour
    {
        #region Values

        [SerializeField] private bool active = true, needNew = true;
        [SerializeField] private Pokemon activePokemon;
        [SerializeField] private Spot left, right, front, strafeLeft, strafeRight;
        [SerializeField] private Transform currentTransform;
        [SerializeField] private BattleMember battleMember;

        [Header("UI:")] [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private PokemonDisplay display;
        [SerializeField] private GameObject displayPrefab;

        [SerializeField] private int id; 

        #endregion

        private void Update()
        {
            //DEBUG
            Vector3 pos = currentTransform.position;
            if (left != null)
                Debug.DrawRay(pos, pos - left.GetTransform().position);
            if (right != null)
                Debug.DrawRay(pos, pos - right.GetTransform().position);
            if (front != null)
                Debug.DrawRay(pos, pos - front.GetTransform().position);
            if (strafeLeft != null)
                Debug.DrawRay(pos, pos - strafeLeft.GetTransform().position);
            if (strafeRight != null)
                Debug.DrawRay(pos, pos - strafeRight.GetTransform().position);
            //
        }

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

        public bool GetNeedNew()
        {
            return needNew;
        }

        public Pokemon GetActivePokemon()
        {
            return activePokemon;
        }

        public bool GetActive()
        {
            return active;
        }

        public Transform GetTransform()
        {
            return currentTransform;
        }

        public BattleMember GetBattleMember()
        {
            return battleMember;
        }

        public int GetTeamNumber()
        {
            return battleMember.GetTeamNumber();
        }

        public int GetID()
        {
            return id;
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

        public void SetStrafeLeft(Spot set)
        {
            strafeLeft = set;
        }

        public void SetStrafeRight(Spot set)
        {
            strafeRight = set;
        }

        public void SetNeedNew(bool set)
        {
            needNew = set;
        }

        public void SetActivePokemon(Pokemon set)
        {
            activePokemon = set;

            return;

            //FIX!

            if (set != null)
                display.SetNewPokemon(set);
        }

        public void SetActive(bool set)
        {
            active = set;
        }

        public void SetTransform()
        {
            currentTransform = transform;
        }

        public void SetBattleMember(BattleMember member)
        {
            battleMember = member;
        }

        public void SetID(int set)
        {
            id = set;
        }

        #endregion

        #region In

        public void Setup(Transform origin, int i)
        {
            GameObject obj = Instantiate(displayPrefab);
            Vector2 v2 = origin.position;
            obj.transform.position = v2 + offset * i;
            obj.transform.parent = origin;
            obj.transform.localScale = Vector3.one;

            display = obj.GetComponent<PokemonDisplay>();
        }

        #endregion
    }

    public struct SpotOversight
    {
        private List<Spot> list;
        private int counts;

        public void SetSpot(Spot spot)
        {
            if (list == null)
            {
                list = new List<Spot>();
                counts = 0;
            }

            spot.SetID(counts);
            counts += 1;
            
            list.Add(spot);
        }

        public List<Spot> GetSpots()
        {
            return list;
        }

        public void Reorganise()
        {
            List<Spot> enemies = new List<Spot>(), allies = new List<Spot>();

            foreach (Spot spot in list)
            {
                if (spot.GetTeamNumber() == 0)
                    allies = InsertByID(allies, spot);
                else
                    enemies = InsertByID(enemies, spot);
            }

            for (int i = 0; i < allies.Count; i++)
            {
                Spot spot = allies[i];
                int count = i;
                
                while (spot.GetFront() == null && i >= 0)
                {
                    spot.SetFront(enemies[count]);
                    count -= 1;
                }

                if (i != 0)
                    spot.SetLeft(allies[i - 1]);

                if (i != allies.Count - 1)
                    spot.SetRight(allies[i + 1]);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                Spot spot = enemies[i];
                int count = i;

                while (spot.GetFront() == null && i >= 0)
                {
                    spot.SetFront(allies[count]);
                    count -= 1;
                }

                if (i != 0)
                    spot.SetLeft(enemies[i - 1]);

                if (i != enemies.Count - 1)
                    spot.SetRight(enemies[i + 1]);
            }

            foreach (Spot spot in list)
            {
                Spot s = spot.GetFront();

                spot.SetStrafeLeft(s.GetRight());
                spot.SetStrafeRight(s.GetLeft());
            }
        }

        private List<Spot> InsertByID(List<Spot> toInsert, Spot spot)
        {
            for (int i = 0; i < toInsert.Count; i++)
            {
                if (toInsert[i].GetID() <= spot.GetID()) continue;
                
                toInsert.Insert(i, spot);
                return toInsert;
            }
            
            toInsert.Add(spot);
            
            return toInsert;
        }
    }
}