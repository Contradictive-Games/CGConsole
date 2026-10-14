using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CG.Console
{
    public static class ConsoleCommands
    {
        private static readonly Dictionary<string, CommandDefinition> commands = new();
        private static readonly HashSet<object> manualTargets = new();
        private static bool initialized;

        internal static string HelpText { get; private set; }

        private static ConsolePackageSettings _settings;
        private static ConsolePackageSettings settings
        {
            get
            {
                if (_settings == null) _settings = ConsolePackageSettings.GetOrCreateSettings();
                return _settings;
            }
        }


        #region Init

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            commands.Clear();
            manualTargets.Clear();
            initialized = false;
            _settings = null;
            HelpText = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInitialized()
        {
            if (initialized) return;
            initialized = true;
            DiscoverCommands();
        }

        private static void DiscoverCommands()
        {
            List<MethodInfo> methods = CommandDiscovery.FindMethodsWith<ConsoleCmdAttribute>();

            methods.Sort((a, b) => IsBuiltIn(a).CompareTo(IsBuiltIn(b)));

            bool requireInterface = settings != null && settings.RequireInterfaceForRegistration;

            foreach (MethodInfo method in methods)
            {
                var attr = method.GetCustomAttribute<ConsoleCmdAttribute>();
                string name = attr.CommandName.ToLowerInvariant().Trim();
                bool builtIn = IsBuiltIn(method);

                if (builtIn && settings != null && !settings.IsCommandEnabled(name)) continue;

                if (!builtIn && requireInterface && !method.IsStatic && !typeof(ICommandProvider).IsAssignableFrom(method.DeclaringType)) continue;

                if (!IsValidName(name)) continue;

                if (commands.TryGetValue(name, out CommandDefinition existing))
                {
                    if (!builtIn)
                    {
                        Debug.LogError($"[{Globals.k_PACKAGE_NAME}]: `{name}` is declared on both {existing.DeclaryingType.Name} and {method.DeclaringType.Name}");
                        continue;
                    }
                }

                commands.Add(name, new CommandDefinition(name, attr, method));
            }

            BuildHelpText();

            if (settings != null && settings.EnableLoggingForCommandRegistration)
            {
                Debug.Log($"[{Globals.k_PACKAGE_NAME}] Found {commands.Count}. Type `help` to list them");
            }
        }


        private static bool IsBuiltIn(MethodInfo method) => method.DeclaringType.Assembly == typeof(ConsoleCommands).Assembly;

        private static bool IsValidName(string name)
        {
            string pattern = settings != null && settings.UseCustomRegexForCommandNaming &&
                !string.IsNullOrEmpty(settings.Regex)
                ? settings.Regex
                : Globals.k_DefaultCommandNameRegex;

            if (Regex.IsMatch(name, pattern)) return true;

            Debug.LogError($"[{Globals.k_PACKAGE_NAME}] Invalid command name `{name}`");
            return false;
        }

        #endregion


        #region Command Registration

        public static IDisposable Register(object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target is UnityEngine.Object)
            {
                Debug.LogWarning($"[{Globals.k_PACKAGE_NAME}] {target} is a Unity object, commands are found automatically");
            }
            manualTargets.Add(target);
            return new Registration(target);
        }

        public static void Unregister(object target) => manualTargets.Remove(target);

        private sealed class Registration : IDisposable
        {
            private object target;
            public Registration(object target) => this.target = target;

            public void Dispose()
            {
                if (target == null) return;
                manualTargets.Remove(target);
                target = null;
            }
        }


        [Obsolete("Commands are discovered automatically, call is no longer needed")]
        public static void RegisterAllCommands(bool mustBeCommandProvider = true) => EnsureInitialized();

        [Obsolete("Commands are discovered automatically. Use Register(object) for plain C# objects")]
        public static void RegisterCommandsFrom(object target)
        {
            if (target is not UnityEngine.Object) Register(target);
        }

        #endregion


        #region Command Execution

        public static CommandResponse TryExecute(string input)
        {
            EnsureInitialized();

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return new CommandResponse(ResponseType.Invalid, "Command was empty");


            string name = parts[0].ToLowerInvariant();
            if (name.EndsWith("?")) return LogCommandHelp(name.TrimEnd('?'));


            if (!commands.TryGetValue(name, out CommandDefinition cmd))
            {
                return new CommandResponse(ResponseType.Error, $"Command `{name}` was not found");
            }

            if (!TryBindArgs(cmd, parts, out object[] args, out string bindError))
            {
                return new CommandResponse(ResponseType.Invalid, bindError);
            }

            var targets = new List<object>();
            if (!TryResolveTargets(cmd, null, targets))
            {
                return new CommandResponse(ResponseType.Error, $"No active {cmd.DeclaryingType.Name} found for `{name}`");
            }

            foreach (object target in targets)
            {
                CommandResponse response = Invoke(cmd, target, args);
                if (response.ResponseType != ResponseType.Success) return response;
            }

            return new CommandResponse(ResponseType.Success);
        }

        private static bool TryResolveTargets(CommandDefinition cmd, string selector, List<object> results)
        {
            if (cmd.IsStatic)
            {
                results.Add(null);
                return true;
            }

            if (cmd.IsUnityObject)
            {
                foreach (UnityEngine.Object obj in UnityEngine.Object.FindObjectsByType(cmd.DeclaryingType, FindObjectsSortMode.None))
                {
                    if (obj is Behaviour behaviour && !behaviour.isActiveAndEnabled) continue;
                    if (selector != null && !string.Equals(obj.name, selector, StringComparison.OrdinalIgnoreCase)) continue;
                    results.Add(obj);
                }
            }
            else
            {
                foreach (object target in manualTargets)
                {
                    if (cmd.DeclaryingType.IsInstanceOfType(target)) results.Add(target);
                }
            }

            if (cmd.TargetMode == CommandTarget.First && results.Count > 1) results.RemoveRange(1, results.Count - 1);

            return results.Count > 0;
        }


        private static CommandResponse Invoke(CommandDefinition cmd, object target, object[] args)
        {
            try
            {
                object result = cmd.Method.Invoke(target, args);
                if (result is CommandResponse response) return response;
                if (result != null) Debug.Log(result);
                return new CommandResponse(ResponseType.Success);
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                Debug.LogException(e.InnerException);
                return new CommandResponse(ResponseType.Error, e.InnerException.Message);
            }
        }

        private static bool TryBindArgs(CommandDefinition cmd, string[] parts, out object[] args, out string error)
        {
            ParameterInfo[] ps = cmd.Parameters;
            args = new object[ps.Length];
            error = null;
            int given = parts.Length - 1;

            if (given > ps.Length)
            {
                error = $"Too many args for `{cmd.Name}` (expected: {ps.Length})";
            }

            for (int i = 0; i < ps.Length; i++)
            {
                if (i >= given)
                {
                    if (ps[i].HasDefaultValue)
                    {
                        args[i] = ps[i].DefaultValue;
                        continue;
                    }
                    error = $"Not enough args for `{cmd.Name}` (expected: {ps.Length})";
                    return false;
                }

                string arg = parts[i + 1];
                Type type = ps[i].ParameterType;
                try
                {
                    args[i] = type.IsEnum
                        ? Enum.Parse(type, arg, true)
                        : Convert.ChangeType(arg, type, CultureInfo.InvariantCulture);
                }
                catch
                {
                    error = $"Failed to conver arg `{arg}` to type `{type.Name}`";
                    return false;
                }
            }

            return true;
        }


        private static CommandResponse LogCommandHelp(string name)
        {
            EnsureInitialized();

            if (!commands.TryGetValue(name, out CommandDefinition cmd))
            {
                return new CommandResponse(ResponseType.Invalid, $"No command found by name `{name}`");
            }
            
            string args = cmd.Parameters.Length == 0
                ? "No args required"
                : string.Join(" ", cmd.Parameters.Select(p => $"<{p.ParameterType.Name} {p.Name}>"));

            string description = string.IsNullOrWhiteSpace(cmd.Description)
                ? "No description provided"
                : cmd.Description;

            Debug.Log($"{description} // Args: {args}");
            return new CommandResponse(ResponseType.Info);
        }

        private static void BuildHelpText()
        {
            var sb = new StringBuilder("Commands List:\n");
            foreach (CommandDefinition cmd in commands.Values.Where(c => !c.HideFromHelpCommand).OrderBy(c => c.Name))
            {
                sb.Append(cmd.Name);
                if (!string.IsNullOrWhiteSpace(cmd.Description)) sb.Append("     (").Append(cmd.Description).Append(')');
                sb.Append('\n');
            }
            HelpText = sb.ToString();
        }

        public static List<string> GetCommandAutoComplete(string input)
        {
            EnsureInitialized();
            input = input.ToLowerInvariant();
            return commands.Values
                .Where(c => !c.HideFromAutoComplete && c.Name.Contains(input))
                .Select(c => c.Name)
                .ToList();
        }

        #endregion

    }
}