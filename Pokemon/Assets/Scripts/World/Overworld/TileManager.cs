#region Packages

using System.Collections.Generic;
using Mfknudsen.Settings.Manager;
using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld
{
    public class TileManager : Manager
    {
        #region Values

        public static TileManager instance;

        #endregion
        
        #region In

        public override void Setup()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        #endregion   
    }
}