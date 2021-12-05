#region Packages

using System.Collections.Generic;
using Mfknudsen.Settings.Manager;

#endregion

namespace Mfknudsen.AI
{
    public class NpcManager : Manager
    {
        #region Values

        public static NpcManager instance;

        private List<NpcController> controllers = new List<NpcController>();

        #endregion

        #region In

        public override void Setup()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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