#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI;
using UnityEngine;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Communication;
using Mfknudsen.Items;
using Mfknudsen.NPC;
using Mfknudsen.Pokémon;
using Mfknudsen.Settings;
using Mfknudsen.Trainer;
using Sirenix.OdinInspector;

#endregion

namespace Mfknudsen.Battle.Systems
{
    [RequireComponent(typeof(NpcTeam), typeof(Inventory))]
    public class BattleMember : SerializedMonoBehaviour
    {
        #region Values

        [SerializeField] [BoxGroup(" ")] private string memberName;

        [SerializeField] [BoxGroup(" ")] private bool isAlly; //0 is player and player teammates

        [SerializeField] [BoxGroup(" ")] private int spotsToOwn = 1;

        [BoxGroup(" /Affiliation")]
        [HorizontalGroup(" /Affiliation/Team Affiliation")]
        [HideIf("isWild")]
        [LabelWidth(60)]
        [SerializeField]
        private bool isPlayer;

        [HorizontalGroup(" /Affiliation/Team Affiliation")] [HideIf("isPlayer")] [LabelWidth(60)] [SerializeField]
        private bool isWild;

        [BoxGroup(" ")] [TableList] [SerializeField]
        private List<EvaluatorSetting> evaluatorSettings = new List<EvaluatorSetting>();

        [BoxGroup(" /Personality")] [SerializeField]
        private bool useDefaultPersonalitySetting = true;

        [BoxGroup(" /Personality")] [HideIf("useDefaultPersonalitySetting")] [SerializeField]
        private PersonalitySetting personalitySetting;

        [FoldoutGroup(" /On Defeat")] [SerializeField]
        private Chat onDefeatedChats;

        private Inventory inventory;
        private Team pokemonTeam;

        private bool hasAllSpots;
        private readonly List<Spot> ownedSpots = new List<Spot>();

        private readonly List<Evaluator> evaluators = new List<Evaluator>();

        #endregion

        #region Build In States

        private void OnValidate()
        {
            if (pokemonTeam == null)
                pokemonTeam = GetComponent<Team>();
            if (inventory == null)
                inventory = GetComponent<Inventory>();
        }

        #endregion

        #region Getters

        public string GetName()
        {
            return memberName;
        }

        public bool IsWild()
        {
            return isWild;
        }

        public Team GetTeam()
        {
            return pokemonTeam;
        }

        public bool GetTeamAffiliation()
        {
            return isAlly;
        }

        public List<Spot> GetOwnedSpots()
        {
            return ownedSpots;
        }

        public bool OwnSpot(Spot spot)
        {
            return ownedSpots.Contains(spot);
        }

        public bool IsPlayer()
        {
            return isPlayer;
        }

        public bool HasAllSpots()
        {
            return hasAllSpots;
        }

        public Inventory GetInventory()
        {
            return inventory;
        }

        public int GetSpotsToOwn()
        {
            return spotsToOwn;
        }

        public Chat GetOnDefeatedChats()
        {
            return onDefeatedChats;
        }

        #endregion

        #region Setters

        public void SetTeamNumber(bool set)
        {
            isAlly = set;
        }

        public void SetOwnedSpot(Spot set)
        {
            if (set != null)
            {
                if (!ownedSpots.Contains(set))
                    ownedSpots.Add(set);
            }

            hasAllSpots = (ownedSpots.Count == spotsToOwn);
        }

        #endregion

        #region In

        public void Setup()
        {
            if (isPlayer)
                return;

            for (int i = 0; i < 6; i++)
            {
                Pokemon pokemon = pokemonTeam.GetPokemonByIndex(i);
                if (pokemon == null)
                    break;
                EvaluatorSetting evaluatorSetting;
                // ReSharper disable once LocalVariableHidesMember
                PersonalitySetting personalitySetting = useDefaultPersonalitySetting
                    ? new PersonalitySetting()
                    : this.personalitySetting;

                try
                {
                    evaluatorSetting = evaluatorSettings[i];
                }
                catch
                {
                    evaluatorSetting = GameplaySetting.GetDefaultEvaluatorSetting(Setting.Difficultly);
                }

                evaluatorSetting.SetPersonalitySetting(personalitySetting);

                Evaluator evaluator = new Evaluator(pokemon, evaluatorSetting);
                evaluators.Add(evaluator);
            }
        }

        public void ForceHasAllSpots()
        {
            hasAllSpots = true;
        }

        public void ActivateAIBrain(Pokemon toTick)
        {
            Evaluator evaluator = evaluators.FirstOrDefault(e => e.UsedForPokemon(toTick));

            if (evaluator == null)
            {
                Debug.LogWarning("Failed to Create Evaluation");
                evaluator = new Evaluator(toTick, GameplaySetting.GetDefaultEvaluatorSetting(Setting.Difficultly));
            }

            evaluator.EvaluateForPokemon();
        }

        #endregion
    }
}