#region SDK

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mfknudsen.AI.Behavior_Tree.Scripts;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.Pokémon;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;

#endregion

namespace Mfknudsen.AI
{
    #region Enums

    public enum MacroState
    {
        Aggresor,
        Defensiv,
        Recorver,
        Support
    }

    public enum MicroState
    {
        Temp
    }

    #endregion

    [CreateAssetMenu(menuName = "AI/Battle AI", fileName = "New Battle AI")]
    public class BattleAI : ScriptableObject
    {
        #region Values

        private bool isInstance;

        [Header("AI Reference:")] public BehaviorSetup behaviorSetup;

        protected LocalMemories localMemories;

        [Header(" - Opponent Information:")] [SerializeField]
        protected bool canRememberOpponent;

        protected EnemiesMemories enemiesMemories;

        [Header(" - Ally Information:")] [SerializeField]
        protected bool canRememberAlly;

        protected AlliesMemories alliesMemories;

        #region Nodes

        private readonly List<BaseNode> nodeQueue = new List<BaseNode>();

        public void AddNodeToQueue(BaseNode node)
        {
            if (node == null)
                return;

            if (!nodeQueue.Contains(node))
                nodeQueue.Add(node);
        }

        public List<BaseNode> GetNodes()
        {
            return behaviorSetup.nodes;
        }

        public List<BaseNode> GetNodeQueue()
        {
            return nodeQueue;
        }

        #endregion

        #endregion

        #region Getters

        public BattleAI GetInstance()
        {
            if (isInstance) return this;

            BattleAI ai = CreateInstance(GetType()) as BattleAI;

            ai.SetIsInstance(true);

            return ai;
        }

        public bool GetRememberEnemies()
        {
            return canRememberOpponent;
        }

        public bool GetRememberAllies()
        {
            return canRememberAlly;
        }

        #endregion

        #region Setters

        public void SetIsInstance(bool set)
        {
            isInstance = set;
        }

        public void SetLocalMemories(LocalMemories local)
        {
            localMemories = local;
        }

        public void SetEnemiesMemories(EnemiesMemories enemies)
        {
            enemiesMemories = enemies;
        }

        public void SetAlliesMemories(AlliesMemories allies)
        {
            alliesMemories = allies;
        }

        #endregion

        #region In

        public void TickBrain()
        {
            nodeQueue.Clear();

            behaviorSetup.Setup();

            //Local
            foreach (BaseNode baseNode in GetNodes())
            {
                foreach (FieldInfo fieldInfo in baseNode.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    OutputType type = (OutputType) fieldInfo.GetCustomAttribute(typeof(OutputType));

                    if (type == null || type.type != typeof(LocalMemories))
                        continue;
                    
                    fieldInfo.SetValue(baseNode, localMemories);
                }
            }

            //Enemies

            //Allies

            behaviorSetup.Tick(this);
        }

        #endregion
    }
    public struct LocalMemories
    {
        public Pokemon currentPokemon;
    }

    public struct EnemiesMemories
    {
        public Dictionary<string, Team> teams;
    }

    public struct AlliesMemories
    {
        public Dictionary<string, Team> teams;
    }
}