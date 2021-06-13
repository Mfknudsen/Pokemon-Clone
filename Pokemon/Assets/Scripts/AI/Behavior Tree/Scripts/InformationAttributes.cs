﻿#region SDK

using System;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts
{
    public enum VariableType
    {
        DEFAULT,
        Any,
        Int,
        Float,
        Double,
        Bool,
        String,
        Vector2,
        Vector3,
        Transform,
        Script,
    }

    public enum ScriptType
    {
        DEFAULT,
        TrainerTeam,
        Pokemon,
        PokeMove,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutCaller : Attribute
    {
        public string display;

        public OutCaller(string display)
        {
            this.display = display;
        }
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