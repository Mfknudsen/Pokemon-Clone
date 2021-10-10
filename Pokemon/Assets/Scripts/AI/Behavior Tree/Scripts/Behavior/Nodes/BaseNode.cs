#region Packages

using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes
{
    [System.Serializable]
    public abstract class BaseNode
    {
        public int id;
        public bool ready, inCall, resetOnEnd;

        public List<Transition> transitions;
        public Dictionary<string, bool> checkState;

        public abstract void Tick(BattleAI ai);

        protected abstract void Resets();

        protected void ContinueTransitions(BattleAI setup)
        {
            ready = false;

            if (transitions == null) return;

            foreach (Transition t in transitions)
                t.Tick(setup);

            if (!resetOnEnd) return;

            Resets();
            
            checkState.Clear();
        }

        public void AddTransition(Transition transition)
        {
            transitions ??= new List<Transition>();

            transitions.Add(transition);
        }

        public void AddCheckState(string key, bool value)
        {
            checkState ??= new Dictionary<string, bool>();

            if (!checkState.ContainsKey(key))
                checkState.Add(key, value);
        }

        protected bool CheckNodeReady(BaseNode n)
        {
            if (!n.ready && n.inCall) return false;

            FieldInfo[] infos = n.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (FieldInfo f in infos)
            {
                InputType type = (InputType) f.GetCustomAttribute(typeof(InputType));

                if (type == null)
                    continue;
                
                if (!n.checkState.ContainsKey(f.Name) || !n.checkState[f.Name]) return false;
            }

            return true;
        }
    }
}