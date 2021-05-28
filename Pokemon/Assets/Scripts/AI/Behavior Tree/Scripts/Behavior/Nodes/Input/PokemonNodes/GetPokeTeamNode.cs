using System.Collections;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using AI.BehaviourTreeEditor;
using UnityEngine;

public class GetPokeTeamNode : InputNode
{
    [OutputType(VariableType.Script, "Team Input", ScriptType.TrainerTeam)]
    public object input = (Trainer.Team)null;
}
