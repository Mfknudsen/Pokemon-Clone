#region Packages

using NodeCanvas.Framework;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Actions
{
    public class AttackAction : ActionTask
    {
        public BBParameter<bool> meleeAttack;
        public BBParameter<NpcBase> npcBase;
        public BBParameter<GameObject> target;


        protected override void OnUpdate()
        {
            
            
            EndAction(true);
        }
    }
}