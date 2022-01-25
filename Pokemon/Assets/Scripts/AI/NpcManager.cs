#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Settings.Manager;

#endregion

namespace Mfknudsen.AI
{
    public class NpcManager : Manager
    {
        #region Values

        public static NpcManager instance;

        private readonly List<NpcController> controllers = new List<NpcController>();

        #endregion

        #region In

        public override IEnumerator Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
            
            yield break;
        }

        public void AddController(NpcController add)
        {
            if (add == null || controllers.Contains(add))
                return;

            controllers.Add(add);
        }

        public void RemoveController(NpcController remove)
        {
            if (remove == null || !controllers.Contains(remove))
                return;

            controllers.Remove(remove);
        }

        #endregion
    }
}