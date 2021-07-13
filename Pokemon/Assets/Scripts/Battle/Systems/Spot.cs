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

        [SerializeField] private bool needNew = true;
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
            #region DEBUG

            return;

            Vector3 pos = currentTransform.position;
            if (left != null)
                Debug.DrawRay(pos, left.GetTransform().position - pos, Color.white);
            if (right != null)
                Debug.DrawRay(pos, right.GetTransform().position - pos, Color.black);
            if (front != null)
                Debug.DrawRay(pos, front.GetTransform().position - pos, Color.red);
            if (strafeLeft != null)
                Debug.DrawRay(pos, strafeLeft.GetTransform().position - pos, Color.blue);
            if (strafeRight != null)
                Debug.DrawRay(pos, strafeRight.GetTransform().position - pos, Color.green);

            #endregion
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

        public Spot GetStrafeLeft()
        {
            return strafeLeft;
        }

        public Spot GetStrafeRight()
        {
            return strafeRight;
        }

        public List<Spot> GetAllAdjacentSpots()
        {
            List<Spot> result = new List<Spot>();

            if (!(front is null))
                result.Add(front);

            if (!(left is null))
                result.Add(left);

            if (!(right is null))
                result.Add(right);

            if (!(strafeLeft is null))
                result.Add(strafeLeft);

            if (!(strafeRight is null))
                result.Add(strafeRight);

            return result;
        }

        public List<Spot> GetAllAdjacentOneSideSpots(bool ally)
        {
            if (!ally)
            {
                return new List<Spot>
                {
                    GetFront(),
                    GetStrafeLeft(),
                    GetStrafeRight()
                };
            }

            return new List<Spot>
            {
                GetStrafeLeft(),
                GetStrafeRight()
            };
        }

        public bool GetNeedNew()
        {
            return needNew;
        }

        public Pokemon GetActivePokemon()
        {
            return activePokemon;
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
            GameObject obj = Instantiate(displayPrefab, origin, true);
            Vector2 v2 = origin.position;
            obj.transform.position = v2 + offset * i;
            obj.transform.localScale = Vector3.one;

            display = obj.GetComponent<PokemonDisplay>();
        }

        #endregion
    }

    public class SpotOversight
    {
        private readonly List<Spot> list;
        private int counts;

        public SpotOversight()
        {
            list = new List<Spot>();
            counts = 0;
        }

        public void SetSpot(Spot spot)
        {
            spot.SetID(counts);
            counts += 1;

            list.Add(spot);
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public List<Spot> GetSpots()
        {
            return list;
        }

        public void Reorganise(bool removeEmpty)
        {
            List<Spot> enemies = new List<Spot>(), allies = new List<Spot>();

            List<Spot> toRemove = new List<Spot>();

            foreach (Spot spot in list)
            {
                if (spot.GetActivePokemon() is null && removeEmpty)
                    toRemove.Add(spot);
                else
                {
                    if (spot.GetTeamNumber() == 0)
                        allies = InsertByID(allies, spot);
                    else
                        enemies = InsertByID(enemies, spot);
                }
            }

            if (removeEmpty)
            {
                foreach (Spot spot in toRemove)
                    list.Remove(spot);
            }

            #region Set Front, Left and Right

            for (int i = 0; i < allies.Count; i++)
            {
                Spot spot = allies[i];
                int count = i;

                while (spot.GetFront() is null && i >= 0)
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

                while (spot.GetFront() is null && i >= 0)
                {
                    spot.SetFront(allies[count]);
                    count -= 1;
                }

                if (i != 0)
                    spot.SetLeft(enemies[i - 1]);

                if (i != enemies.Count - 1)
                    spot.SetRight(enemies[i + 1]);
            }

            #endregion

            #region Set StrafeRight and StrafeLeft

            foreach (Spot spot in list)
            {
                // ReSharper disable once Unity.NoNullPropagation
                if (spot?.GetFront() is null) continue;

                Spot s = spot.GetFront();

                if (s.GetRight() != null)
                    spot.SetStrafeLeft(s.GetRight());
                
                if (s.GetLeft() != null)
                    spot.SetStrafeRight(s.GetLeft());
            }

            #endregion
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