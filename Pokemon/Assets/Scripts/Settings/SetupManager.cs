#region Packages

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Mfknudsen.Settings
{
    public class SetupManager : MonoBehaviour
    {
        void Start()
        {
            List<ISetup> setups = new List<ISetup>();
            setups.AddRange(FindObjectsOfType(typeof(ISetup)) as ISetup[] ?? Array.Empty<ISetup>());
            setups.OrderBy(i => i.Priority()).ToList().ForEach(i => i.Setup());
        }
    }
}