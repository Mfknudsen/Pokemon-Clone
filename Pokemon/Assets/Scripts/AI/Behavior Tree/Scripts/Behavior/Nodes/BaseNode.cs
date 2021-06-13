using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes
{
    [System.Serializable]
    public abstract class BaseNode : object
    {
        public int id;

        public bool inCall;

        public List<Transition> transitions;
        public Dictionary<string, bool> checkState;

        public abstract void Tick(BehaviorController setup);

        protected void ContinueTransitions(BehaviorController setup)
        {
            if (transitions == null) return;

            foreach (Transition t in transitions)
                t.Tick(setup);
        }

        public void AddTransition(Transition transition)
        {
            if (transitions == null)
                transitions = new List<Transition>();

            transitions.Add(transition);
        }

        public void AddCheckState(string key, bool value)
        {
            if (checkState == null)
                checkState = new Dictionary<string, bool>();

            if (!checkState.ContainsKey(key))
                checkState.Add(key, value);
        }

        protected bool CheckNodeReady(BaseNode n)
        {
            FieldInfo[] infos = n.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int i = 0;
            foreach (FieldInfo f in infos.Where(f => f.GetCustomAttribute(typeof(InputType)) as InputType != null))
            {
                i++;

                foreach (string c in n.checkState.Keys)
                {
                    if (!f.Name.Equals(c)) continue;

                    if (!n.checkState[c])
                        return false;
                }
            }

            return i == n.checkState.Keys.Count;
        }
    }
}