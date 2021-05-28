#region SDK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace AI.BehaviourTreeEditor
{
    public enum VariableType
    {
        DEFAULT,
        Any,
        Int,
        Float,
        Double,
        String,
        Vector2,
        Vector3,
        Transform,
        Script
    }

    public enum ScriptType
    {
        DEFAULT,
        TrainerTeam,
        Pokemon,
        PokeMove,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InputType : InformationType
    {
        public bool receiveMultiply;

        public InputType(VariableType varType, string name, bool receiveMultiply,
            ScriptType scriptType = ScriptType.DEFAULT)
        {
            this.varType = varType;
            this.name = name;
            this.scriptType = scriptType;
            this.receiveMultiply = receiveMultiply;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutputType : InformationType
    {
        public OutputType(VariableType varType, string name, ScriptType scriptType = ScriptType.DEFAULT)
        {
            this.varType = varType;
            this.name = name;
            this.scriptType = scriptType;
        }
    }

    public class InformationType : Attribute
    {
        public string name;
        public VariableType varType;
        public ScriptType scriptType;
    }
}