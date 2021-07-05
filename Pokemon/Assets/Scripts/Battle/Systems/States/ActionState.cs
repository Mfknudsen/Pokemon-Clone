using System.Collections;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen._Debug;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

namespace Mfknudsen.Battle.Systems.States
{
    public class ActionState : State
    {
        public ActionState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            bool faintedCheck = false;
            Debug.Log("Start");

            SpotOversight spotOversight = master.GetSpotOversight();

            foreach (Spot spot in spotOversight.GetSpots())
            {
                if (spot == null || spot.GetActivePokemon() == null ||
                    spot.GetActivePokemon().GetBattleAction() == null)
                    continue;

                Pokemon pokemon = spot.GetActivePokemon();

                BattleAction action = pokemon.GetBattleAction();

                BattleLog.instance.AddNewLog(action.name, "Starting Action: " + action.name.Replace("(Clone)", ""));

                master.StartCoroutine(action.Activate());

                while (action.GetDone() && !ChatMaster.instance.GetIsClear())
                    yield return 0;

                pokemon.SetBattleAction(null);

                foreach (Spot sCheck in spotOversight.GetSpots())
                {
                    if (sCheck == null || sCheck.GetActivePokemon() == null ||
                        sCheck.GetActivePokemon().GetConditionOversight().GetNonVolatileStatus().GetType() ==
                        typeof(FaintedCondition))
                        continue;

                    faintedCheck = true;
                    break;
                }

                if (faintedCheck)
                    break;
            }

            Debug.Log("Done");

            if (faintedCheck)
                master.SetState(new DoFaintedState(master, this));
            else
                master.SetState(new RoundDoneState(master));
        }
    }
}