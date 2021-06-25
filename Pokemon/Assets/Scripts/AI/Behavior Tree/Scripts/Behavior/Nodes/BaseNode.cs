using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes
{
    [System.Serializable]
    public abstract class BaseNode : object
    {
        public int id;
        public bool ready = false, inCall, resetOnEnd;

        public List<Transition> transitions;
        public Dictionary<string, bool> checkState;

        public abstract void Tick(BehaviorController setup);

        protected abstract void Resets();

        protected void ContinueTransitions(BehaviorController setup)
        {
            ready = false;

            if (transitions == null) return;

            foreach (Transition t in transitions)
                t.Tick(setup);

            if (!resetOnEnd) return;

            Resets();
        }

        public void AddTransition(Transition transition)
        {
            transitions ??= new List<Transition>();

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
            if (!ready && inCall) return false;

            FieldInfo[] infos = n.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int i = 0;
            foreach (FieldInfo f in infos.Where(f => f.GetCustomAttribute(typeof(InputType)) as InputType != null))
            {
                i++;

                if (n.checkState.Keys.Where(c => f.Name.Equals(c)).Any(c => !n.checkState[c]))
                    return false;
            }

            return i == n.checkState.Keys.Count;
        }
    }
}