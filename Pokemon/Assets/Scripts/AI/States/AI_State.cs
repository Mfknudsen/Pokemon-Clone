using UnityEngine;

namespace Mfknudsen.AI.States
{
    public abstract class AI_State: ScriptableObject
    {
        protected AI_Controller controller;
        
        // ReSharper disable once ParameterHidesMember
        public virtual void StartState(AI_Controller controller)
        {
            this.controller = controller;
        }

        public abstract void Update();
    }
}
