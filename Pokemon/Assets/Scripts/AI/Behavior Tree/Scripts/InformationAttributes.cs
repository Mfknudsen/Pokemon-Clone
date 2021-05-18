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
        Null,
        Any,
        Int,
        Float,
        String,
        Vector2,
        Vector3,
        Quaterion,
        Transform
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InputType : InformationType
    {
        public bool receiveMultiply;

        public InputType(VariableType varType, string name, bool receiveMultiply)
        {
            this.varType = varType;
            this.name = name;
            this.receiveMultiply = receiveMultiply;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class OutputType : InformationType
    {
        public OutputType(VariableType varType, string name)
        {
            this.varType = varType;
            this.name = name;
        }
    }

    public class InformationType : Attribute
    {
        public string name;
        public VariableType varType;
    }
}