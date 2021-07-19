#region SDK

using System.Collections.Generic;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

// ReSharper disable ConvertIfStatementToSwitchStatement
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

        [SerializeField] private int id;

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

        public void SetRelations(int selfIndex, int allyCount, List<Spot> opponentSpots)
        {
            if (opponentSpots.Count == 1)
                SetFront(opponentSpots[0]);
            else if (opponentSpots.Count == 2)
            {
                if (selfIndex > 1)
                {
                    SetFront(opponentSpots[0]);
                    SetStrafeLeft(opponentSpots[1]);
                }
                else
                {
                    SetFront(opponentSpots[1]);
                    SetStrafeRight(opponentSpots[0]);
                }
            }
            else
            {
                if (allyCount == 3)
                {
                    if (selfIndex == 1)
                    {
                        SetFront(opponentSpots[2]);
                        SetStrafeRight(opponentSpots[1]);
                    }
                    else if (selfIndex == 2)
                    {
                        SetFront(opponentSpots[1]);
                        SetStrafeLeft(opponentSpots[2]);
                        SetStrafeRight(opponentSpots[0]);
                    }
                    else
                    {
                        SetFront(opponentSpots[0]);
                        SetStrafeLeft(opponentSpots[1]);
                    }
                }
                else if (allyCount == 2)
                {
                    if (selfIndex == 1)
                    {
                        SetFront(opponentSpots[2]);
                        SetStrafeRight(opponentSpots[1]);
                    }
                    else
                    {
                        SetFront(opponentSpots[0]);
                        SetStrafeLeft(opponentSpots[1]);
                    }
                }
                else
                {
                    SetFront(opponentSpots[1]);
                    SetStrafeLeft(opponentSpots[2]);
                    SetStrafeRight(opponentSpots[0]);
                }
            }
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        #endregion
    }

    public class SpotOversight
    {
        #region Values

        private readonly List<Spot> list;
        private int counts;
        private bool defaultTargets;

        #endregion

        #region Getters

        public SpotOversight()
        {
            list = new List<Spot>();
            counts = 0;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public List<Spot> GetSpots()
        {
            return list;
        }

        public bool GetToDefaultTargeting()
        {
            return defaultTargets;
        }

        #endregion

        #region Setters

        public void SetSpot(Spot spot)
        {
            spot.SetID(counts);
            counts += 1;

            list.Add(spot);
        }

        #endregion

        #region In

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
                        allies.Add(spot);
                    else
                        enemies.Add(spot);
                }
            }

            if (removeEmpty)
            {
                foreach (Spot spot in toRemove)
                {
                    list.Remove(spot);

                    if (allies.Contains(spot))
                        allies.Remove(spot);
                    else if (enemies.Contains(spot))
                        enemies.Remove(spot);

                    spot.DestroySelf();
                }
            }

            #region Set Relations

            for (int i = 0; i < allies.Count; i++)
            {
                if(i > 0)
                    allies[i].SetLeft(allies[i - 1]);
                if(i < allies.Count - 1)
                    allies[i].SetRight(allies[i + 1]);
                
                allies[i].SetRelations(i + 1, allies.Count, enemies);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetRelations(i + 1, enemies.Count, allies);
            }

            #endregion

            defaultTargets = list.Count == 2;
        }

        #endregion
    }
}