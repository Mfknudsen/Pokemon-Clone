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
            return this.left;
        }

        public Spot GetRight()
        {
            return this.right;
        }

        public Spot GetFront()
        {
            return this.front;
        }

        public Spot GetStrafeLeft()
        {
            return this.strafeLeft;
        }

        public Spot GetStrafeRight()
        {
            return this.strafeRight;
        }

        public List<Spot> GetAllAdjacentSpots()
        {
            List<Spot> result = new();

            if (!(this.front is null))
                result.Add(this.front);

            if (!(this.left is null))
                result.Add(this.left);

            if (!(this.right is null))
                result.Add(this.right);

            if (!(this.strafeLeft is null))
                result.Add(this.strafeLeft);

            if (!(this.strafeRight is null))
                result.Add(this.strafeRight);

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
            return this.needNew;
        }

        public Pokemon GetActivePokemon()
        {
            return this.activePokemon;
        }

        public Transform GetTransform()
        {
            return this.currentTransform;
        }

        public BattleMember GetBattleMember()
        {
            return this.battleMember;
        }

        public bool GetIsAlly()
        {
            return this.battleMember.GetTeamAffiliation();
        }

        public int GetID()
        {
            return this.id;
        }

        #endregion

        #region Setters

        public void SetLeft(Spot set)
        {
            this.left = set;
        }

        public void SetRight(Spot set)
        {
            this.right = set;
        }

        public void SetFront(Spot set)
        {
            this.front = set;
        }

        public void SetStrafeLeft(Spot set)
        {
            this.strafeLeft = set;
        }

        public void SetStrafeRight(Spot set)
        {
            this.strafeRight = set;
        }

        public void SetNeedNew(bool set)
        {
            this.needNew = set;
        }

        public void SetActivePokemon(Pokemon set)
        {
            this.activePokemon = set;
        }

        public void SetTransform()
        {
            this.currentTransform = transform;
        }

        public void SetBattleMember(BattleMember member)
        {
            this.battleMember = member;
        }

        public void SetID(int set)
        {
            this.id = set;
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