#region Packages

using System.Collections.Generic;
using Runtime.Pokémon;
using UnityEngine;

#endregion

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Runtime.Battle.Systems.Spots
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
            List<Spot> result = new();

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

        public bool GetIsAlly()
        {
            return battleMember.GetTeamAffiliation();
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
            if (opponentSpots.Count == 0)
                return;

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

        #region Out

        public List<Spot> GetAllOneSide()
        {
            bool continueCheck = true;
            List<Spot> result = new() { this };

            while (continueCheck)
            {
                continueCheck = false;

                foreach (Spot s in result)
                {
                    if (!result.Contains(s.GetLeft()))
                    {
                        result.Add(s.GetLeft());
                        continueCheck = true;
                    }

                    if (result.Contains(s.GetRight())) continue;

                    result.Add(s.GetRight());
                    continueCheck = true;
                }
            }

            return result;
        }

        #endregion
    }
}