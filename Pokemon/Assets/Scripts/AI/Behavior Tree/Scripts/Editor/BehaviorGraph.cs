#region SDK

using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor
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
            foreach (BaseNodeSetting t in windows)
            {
                if (t.id == index)
                    return t;
            }

            return null;
        }

        public void DeleteWindowsThatNeedTo()
        {
            foreach (int t in indexToDelete)
            {
                BaseNodeSetting b = GetNodeWithIndex(t);
                if (b != null)
                    windows.Remove(b);
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
            if (indexToDelete.Contains(index)) return;

            if (behaviour == null) return;

            indexToDelete.Add(index);

            if (!(windows[index].baseNode is Transition))
            {
                foreach (Transition n in windows[index].baseNode.transitions)
                    DeleteNode(n.id);
            }

            behaviour.RemoveNode(index);
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