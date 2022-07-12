#region Packages

using Mfknudsen.World.Overworld.Sources;
using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Actions
{
    public class FindClosestSource<T> : ActionTask where T : Source
    {
        public BBParameter<Transform> result;
        public BBParameter<T> toFind;

        protected override void OnExecute()
        {
            
            EndAction(result.value != null);
        }
    }
}