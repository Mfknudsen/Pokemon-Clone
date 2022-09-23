#region SDK

using System.Collections.Generic;
using Runtime.Trainer;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.Spots
{
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
            this.list = new List<Spot>();
            this.counts = 0;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public List<Spot> GetSpots()
        {
            return this.list;
        }

        public bool GetToDefaultTargeting()
        {
            return this.defaultTargets;
        }

        #endregion

        #region Setters

        public void SetSpot(Spot spot)
        {
            if(spot == null)
                return;
            
            spot.SetID(this.counts);
            this.counts += 1;

            this.list.Add(spot);
        }

        #endregion

        #region In

        public void SetupSpots(BattleMember[] members, Transform parent)
        {
            foreach (BattleMember battleMember in members)
            {
                Team team = battleMember.GetTeam();
                for (int i = 0; i < battleMember.GetSpotsToOwn(); i++)
                {
                    if (!team.CanSendMorePokemon())
                    {
                        battleMember.ForceHasAllSpots();
                        break;
                    }

                    Spot spot = BattleManager.instance.CreateSpot(parent);
                    spot.SetBattleMember(battleMember);
                    SetSpot(spot);
                    battleMember.SetOwnedSpot(spot);
                }
            }
        }

        public void Reorganise(bool removeEmpty)
        {
            List<Spot> enemies = new(), 
                allies = new(),
                toRemove = new();

            foreach (Spot spot in this.list)
            {
                if (spot.GetActivePokemon() is null && removeEmpty)
                    toRemove.Add(spot);
                else
                {
                    if (spot.GetIsAlly())
                        allies.Add(spot);
                    else
                        enemies.Add(spot);
                }
            }

            if (removeEmpty)
            {
                foreach (Spot spot in toRemove)
                {
                    this.list.Remove(spot);

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
                if (i > 0)
                    allies[i].SetLeft(allies[i - 1]);
                if (i < allies.Count - 1)
                    allies[i].SetRight(allies[i + 1]);

                allies[i].SetRelations(i + 1, allies.Count, enemies);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetRelations(i + 1, enemies.Count, allies);
            }

            #endregion

            this.defaultTargets = this.list.Count == 2;
        }

        #endregion
    }
}