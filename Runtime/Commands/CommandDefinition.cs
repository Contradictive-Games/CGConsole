using System;
using System.Reflection;

namespace CG.Console
{
    internal sealed class CommandDefinition
    {
        public readonly string Name;
        public readonly string Description;
        public readonly bool HideFromAutoComplete;
        public readonly bool HideFromHelpCommand;
        public readonly CommandTarget TargetMode;
        public readonly MethodInfo Method;
        public readonly ParameterInfo[] Parameters;
        public readonly bool IsUnityObject;

        public Type DeclaryingType => Method.DeclaringType;
        public bool IsStatic => Method.IsStatic;

        public CommandDefinition(string name, ConsoleCmdAttribute attr, MethodInfo method)
        {
            Name = name;
            Description = attr.Description;
            HideFromAutoComplete = attr.HideFromAutoComplete;
            HideFromHelpCommand = attr.HideFromHelpCommand;
            TargetMode = attr.Target;
            Method = method;
            Parameters = method.GetParameters();
            IsUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(method.DeclaringType);
        }
    }
}