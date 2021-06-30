#region SDK

using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor
{
    [CreateAssetMenu(menuName = "Behavior Tree/Graph")]
    public class BehaviorGraph : ScriptableObject
    {
        #region Values

        //Custom
        public bool reset;

        [FormerlySerializedAs("behaviour")] public BehaviorSetup behavior;

        //Base
        [SerializeField] public List<BaseNodeSetting> windows = new List<BaseNodeSetting>();
        [SerializeField] public int idCount;
        private readonly List<int> indexToDelete = new List<int>();

        private void OnValidate()
        {
            if (!reset) return;

            windows.Clear();
            idCount = 0;

            reset = false;
        }

        #endregion

        #region Checkers

        public BaseNodeSetting GetNodeWithIndex(int index)
        {
            foreach (BaseNodeSetting t in windows)
            {
                if (t.id == index)
                    return t;
            }

            return null;
        }

        public void DeleteWindowsThatNeedTo()
        {
            foreach (int i in indexToDelete)
            {
                Debug.Log(i);
                BaseNodeSetting b = GetNodeWithIndex(i);

                if (b == null) continue;

                behavior.nodes.Remove(b.baseNode);
                windows.Remove(b);
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
            Debug.Log("Delete");
            if (indexToDelete.Contains(index) || behavior is null) return;

            indexToDelete.Add(index);

            BaseNodeSetting b = GetNodeWithIndex(index);

            if (b.baseNode is Transition) return;
            foreach (Transition n in b.baseNode.transitions)
                DeleteNode(n.id);
            
            DeleteWindowsThatNeedTo();
        }

        #endregion
    }
}