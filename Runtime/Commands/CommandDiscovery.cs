using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CG.Console
{
    internal static class CommandDiscovery
    {
        private const BindingFlags k_Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        
        public static List<MethodInfo> FindMethodsWith<TAttribute>() where TAttribute : Attribute
        {
            var results = new List<MethodInfo>();
            Assembly consoleAssembly = typeof(ConsoleCommands).Assembly;
            string consoleName = consoleAssembly.GetName().Name;

            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly != consoleAssembly && !References(assembly, consoleName)) continue;

                foreach(Type type in GetTypesSafe(assembly))
                {
                    if(type.ContainsGenericParameters) continue;
                    foreach(MethodInfo method in type.GetMethods(k_Flags))
                    {
                        if(method.IsDefined(typeof(TAttribute), false)) results.Add(method);
                    }
                }
            }

            return results;
        }

        private static bool References(Assembly assembly, string name)
        {
            foreach(AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if(reference.Name == name) return true;
            }
            return false;
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}