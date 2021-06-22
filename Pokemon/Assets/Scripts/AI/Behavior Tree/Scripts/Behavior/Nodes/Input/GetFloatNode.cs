namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Float", "Float Input")]
    public class GetFloatNode : InputNode
    {
        [OutputType(VariableType.Float, "Float Input")]
        public object input = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}