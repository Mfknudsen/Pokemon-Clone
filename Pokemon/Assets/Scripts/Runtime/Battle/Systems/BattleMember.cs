#region Packages

using System.Collections.Generic;
using System.Linq;
using Runtime.AI;
using Runtime.AI.Battle.Evaluator;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Items;
using Runtime.Pokémon;
using Runtime.Settings;
using Runtime.Trainer;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems
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
        private List<EvaluatorSetting> evaluatorSettings = new();

        [BoxGroup(" /Personality")] [SerializeField]
        private bool useDefaultPersonalitySetting = true;

        [BoxGroup(" /Personality")] [HideIf("useDefaultPersonalitySetting")] [SerializeField]
        private PersonalitySetting personalitySetting;

        [FoldoutGroup(" /On Defeat")] [SerializeField]
        private Chat onDefeatedChats;

        private Inventory inventory;
        private Team pokemonTeam;

        private bool hasAllSpots;
        private readonly List<Spot> ownedSpots = new();

        private readonly List<Evaluator> evaluators = new();

        #endregion

        #region Build In States

        private void OnValidate()
        {
            if (this.pokemonTeam == null) this.pokemonTeam = GetComponent<Team>();
            if (this.inventory == null) this.inventory = GetComponent<Inventory>();
        }

        #endregion

        #region Getters

        public string GetName()
        {
            return this.memberName;
        }

        public bool IsWild()
        {
            return this.isWild;
        }

        public Team GetTeam()
        {
            return this.pokemonTeam;
        }

        public bool GetTeamAffiliation()
        {
            return this.isAlly;
        }

        public List<Spot> GetOwnedSpots()
        {
            return this.ownedSpots;
        }

        public bool OwnSpot(Spot spot)
        {
            return this.ownedSpots.Contains(spot);
        }

        public bool IsPlayer()
        {
            return this.isPlayer;
        }

        public bool HasAllSpots()
        {
            return this.hasAllSpots;
        }

        public Inventory GetInventory()
        {
            return this.inventory;
        }

        public int GetSpotsToOwn()
        {
            return this.spotsToOwn;
        }

        public Chat GetOnDefeatedChats()
        {
            return this.onDefeatedChats;
        }

        #endregion

        #region Setters

        public void SetTeamNumber(bool set)
        {
            this.isAlly = set;
        }

        public void SetOwnedSpot(Spot set)
        {
            if (set != null)
            {
                if (!this.ownedSpots.Contains(set)) this.ownedSpots.Add(set);
            }

            this.hasAllSpots = (this.ownedSpots.Count == this.spotsToOwn);
        }

        #endregion

        #region In

        public void Setup()
        {
            if (this.isPlayer)
                return;

            for (int i = 0; i < 6; i++)
            {
                Pokemon pokemon = this.pokemonTeam.GetPokemonByIndex(i);
                if (pokemon == null)
                    break;
                EvaluatorSetting evaluatorSetting;
                // ReSharper disable once LocalVariableHidesMember
                PersonalitySetting personalitySetting = this.useDefaultPersonalitySetting
                    ? new PersonalitySetting()
                    : this.personalitySetting;

                try
                {
                    evaluatorSetting = this.evaluatorSettings[i];
                }
                catch
                {
                    evaluatorSetting = GameplaySetting.GetDefaultEvaluatorSetting(Setting.Difficultly);
                }

                evaluatorSetting.SetPersonalitySetting(personalitySetting);

                Evaluator evaluator = new(pokemon, evaluatorSetting);
                this.evaluators.Add(evaluator);
            }
        }

        public void ForceHasAllSpots()
        {
            this.hasAllSpots = true;
        }

        public void ActivateAIBrain(Pokemon toTick)
        {
            Evaluator evaluator = this.evaluators.FirstOrDefault(e => e.UsedForPokemon(toTick));

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