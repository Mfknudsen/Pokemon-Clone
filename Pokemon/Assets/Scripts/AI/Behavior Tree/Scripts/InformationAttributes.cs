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
        private readonly float width;

        public NodeAttribute(string menuName, string displayName)
        {
            this.menuName = menuName;
            this.displayName = displayName;
            width = 215;
        }
        
        public NodeAttribute(string menuName, string displayName, float width)
        {
            this.menuName = menuName;
            this.displayName = displayName;
            this.width = width;
        }

        public string GetMenuName()
        {
            return menuName;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public float GetWidth()
        {
            return width;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutCaller : Attribute
    {
        public readonly string display;

        public OutCaller(string display)
        {
            this.display = display;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InputType : InformationType
    {
        public InputType(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutputType : InformationType
    {
        public readonly bool show;
        
        public OutputType(string name, Type type, bool show = false)
        {
            this.name = name;
            this.type = type;
            this.show = show;
        }
    }

    public class InformationType : Attribute
    {
        public string name;
        public Type type;
    }
}