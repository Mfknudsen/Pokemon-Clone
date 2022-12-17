#region Packages

using NodeCanvas.Framework;
using Runtime.Battle.Systems;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class StartPlayerBattle : ActionTask<BattleStarter>
    {
        protected override void OnExecute()
        {
            this.agent.TriggerBattle();

            this.EndAction();
        }
    }
}