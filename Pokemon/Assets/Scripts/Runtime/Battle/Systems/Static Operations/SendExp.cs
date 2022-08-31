using System.Collections;
using Runtime.Pokémon;
using Runtime.Systems;
using UnityEngine;

namespace Runtime.Battle.Systems.Static_Operations
{
    public class SendExp : IOperation
    {
        private bool done;
        private readonly float totalTime;
        private readonly int points;
        private readonly Pokemon pokemon;

        public SendExp(Pokemon pokemon, int points, float totalTime)
        {
            this.pokemon = pokemon;
            this.points = points;
            this.totalTime = totalTime;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;

            // ReSharper disable once InconsistentNaming
            float splitEXP = points / 200, applied = 0;

            while (applied < points)
            {
                if (applied + splitEXP > points)
                    splitEXP = points - applied;

                pokemon.ReceiveExp((int)splitEXP);
                applied += splitEXP;

                yield return new WaitForSeconds(totalTime / 200);
            }

            done = true;
        }

        public void End()
        {
        }
    }
}