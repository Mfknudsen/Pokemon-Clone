#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI.Virtual;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    public class Evaluator
    {
        private readonly Pokemon pokemon;
        private readonly List<BattleAction> actions;

        private readonly EvaluatorSetting setting;
        private string preMoveName;
        private float preValue;

        public bool UsedForPokemon(Pokemon comparor)
        {
            return pokemon == comparor;
        }

        public Evaluator(Pokemon pokemon, EvaluatorSetting setting)
        {
            this.pokemon = pokemon;
            this.setting = setting;

            preMoveName = "";
            preValue = 0;
            actions = new List<BattleAction>();

            actions.AddRange(pokemon.GetMoves().Where(a => a != null));

            if (setting.canSwitchOut)
                actions.Add(BattleManager.instance.InstantiateSwitchAction());
            if (setting.canUseItems)
                actions.Add(BattleManager.instance.InstantiateItemAction());
        }

        public void EvaluateForPokemon()
        {
            VirtualBattle virtualBattle = new VirtualBattle();

            List<VirtualMove> virtualMoves = new List<VirtualMove>();
            VirtualSpotOversight spotOversight = virtualBattle.spotOversight;
            VirtualSpot userSpot = spotOversight.GetPokemonSpot(pokemon);
            VirtualPokemon user = userSpot.virtualPokemon;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (BattleAction battleAction in actions.Where(m => m != null))
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (VirtualSpot spot in spotOversight.spots.Where(s =>
                    VirtualMathf.MoveCanHit(battleAction, userSpot, s)))
                {
                    BattleAction rootAction = SetupRootAction(battleAction, user.GetActualPokemon(),
                        spot.virtualPokemon.GetActualPokemon());

                    virtualMoves.Add(new VirtualMove(
                        rootAction,
                        0,
                        battleAction,
                        pokemon,
                        spot.virtualPokemon.GetFakePokemon(),
                        virtualBattle,
                        setting.personalitySetting)
                    );
                }
            }

            setting.personalitySetting.Tick();
            Evaluate(user, setting.depth, virtualMoves.ToArray());
        }

        private void Evaluate(VirtualPokemon user, int depth, VirtualMove[] toCheck)
        {
            if (depth == 0)
            {
                VirtualMove highest = toCheck[0];
                for (int i = 1; i < toCheck.Length; i++)
                {
                    if (toCheck[i].value <= highest.value)
                        continue;

                    highest = toCheck[i];
                }

                BattleAction rootAction = highest.rootAction;

                if (rootAction.name == preMoveName)
                {
                    preValue += setting.continuesIncrease;

                    if (Random.Range(20, 100) < preValue)
                    {
                        for (int i = 1; i < toCheck.Length; i++)
                        {
                            if (toCheck[i].value <= highest.value ||
                                toCheck[i].rootAction.name == preMoveName)
                                continue;

                            highest = toCheck[i];
                        }
                    }
                }
                else
                {
                    preMoveName = rootAction.name;
                    preValue = 0;
                }

                user.GetActualPokemon().SetBattleAction(highest.rootAction);
                return;
            }

            List<VirtualMove> nextMoves = new List<VirtualMove>();
            foreach (VirtualMove virtualMove in toCheck)
            {
                if (virtualMove.rootAction is SwitchAction)
                    continue;

                foreach (VirtualSpot spot in virtualMove.virtualBattle.spotOversight.spots)
                {
                    foreach (BattleAction battleAction in actions)
                    {
                        nextMoves.Add(new VirtualMove(
                            virtualMove.rootAction,
                            virtualMove.value,
                            battleAction,
                            user.GetFakePokemon(),
                            spot.virtualPokemon.GetFakePokemon(),
                            virtualMove.virtualBattle,
                            setting.personalitySetting
                        ));
                    }
                }
            }

            // ReSharper disable once TailRecursiveCall
            Evaluate(user, depth - 1, nextMoves.ToArray());
        }

        private static BattleAction SetupRootAction(BattleAction battleAction, Pokemon user, Pokemon target)
        {
            BattleAction result = Object.Instantiate(battleAction);
            result.SetCurrentPokemon(user);
            result.SetTargets(target);

            return result;
        }
    }
}