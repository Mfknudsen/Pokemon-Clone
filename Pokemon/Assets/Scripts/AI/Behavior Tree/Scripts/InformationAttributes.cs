#region SDK

using System;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts
{
    public enum VariableType
    {
        Default,
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
        Default,
        TrainerTeam,
        Pokemon,
        PokeMove,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        private readonly string menuName, displayName;

        public NodeAttribute(string menuName, string displayName)
        {
            this.menuName = menuName;
            this.displayName = displayName;
        }

        public string GetMenuName()
        {
            return menuName;
        }

        public string GetDisplayName()
        {
            return displayName;
        }
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

    [AttributeUsage(AttributeTargets.Field)]
    public class InputType : InformationType
    {
        public InputType(VariableType varType, string name)
        {
            this.varType = varType;
            this.name = name;

            scriptType = ScriptType.Default;
        }

        public InputType(VariableType varType, string name, ScriptType scriptType)
        {
            this.varType = varType;
            this.name = name;
            this.scriptType = scriptType;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutputType : InformationType
    {
        public OutputType(VariableType varType, string name)
        {
            this.varType = varType;
            this.name = name;
            
            scriptType = ScriptType.Default;
        }
        
        public OutputType(VariableType varType, string name, ScriptType scriptType)
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