#region SDK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using AI.BehaviourTreeEditor.EditorNodes;
using AI.BehaviorTree;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;
using UnityEditor;

#endregion

namespace AI.BehaviourTreeEditor
{
    [CreateAssetMenu(menuName = "Behavior Tree/Graph")]
    public class BehaviorGraph : ScriptableObject
    {
        #region Values

        //Custom
        public bool reset;

        public BehaviourSetup behaviour;

        //Base
        [SerializeField] public List<BaseNodeSetting> windows = new List<BaseNodeSetting>();
        [SerializeField] public int idCount;
        List<int> indexToDelete = new List<int>();

        private void OnValidate()
        {
            if (reset)
            {
                windows.Clear();
                idCount = 0;

                reset = false;
            }
        }

        #endregion

        #region Checkers

        public BaseNodeSetting GetNodeWithIndex(int index)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].id == index)
                    return windows[i];
            }

            return null;
        }

        public void DeleteWindowsThatNeedTo()
        {
            for (int i = 0; i < indexToDelete.Count; i++)
            {
                BaseNodeSetting b = GetNodeWithIndex(indexToDelete[i]);
                if (b != null)
                    windows.Remove(b);
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
            if (!indexToDelete.Contains(index))
            {
                indexToDelete.Add(index);
                if (behaviour != null)
                    behaviour.RemoveNode(index);
            }
        }

        public bool IsStateDuplicate(BaseNodeSetting b)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].id == b.id)
                    continue;
            }

            return false;
        }

        public bool IsTransitionDuplicate(BaseNodeSetting b)
        {
            BaseNodeSetting enter = GetNodeWithIndex(b.enterNode);
            if (enter == null)
            {
                Debug.Log("false");
                return false;
            }

            return false;
        }

        #endregion
    }
}