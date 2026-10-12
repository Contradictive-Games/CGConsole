using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CG.Console
{
    public static class ConsoleCommands
    {
        internal readonly struct ConsoleCommand
        {
            internal readonly string Command;
            internal readonly string Description;
            internal readonly bool HideFromAutoComplete;
            internal readonly bool HideFromHelpCommand;
            internal readonly MethodInfo MethodToExecute;
            internal readonly ParameterInfo[] Parameters;
            internal readonly object Target;

            internal ConsoleCommand(string command, string description, bool hideFromAutoComplete, bool hideFromHelpCommand,
                MethodInfo methodToExecute, ParameterInfo[] parameters, object target)
            {
                Command = command;
                Description = description;
                HideFromAutoComplete = hideFromAutoComplete;
                HideFromHelpCommand = hideFromHelpCommand;
                MethodToExecute = methodToExecute;
                Parameters = parameters;
                Target = target;
            }
        }


        private static readonly Dictionary<string, ConsoleCommand> allCommands = new();
        private readonly static HashSet<string> commandNameList = new();
        private static DefaultCommands defaultCommandsInstance;
        private static ConsolePackageSettings settings
        {
            get
            {
                if (_settings == null) _settings = ConsolePackageSettings.GetOrCreateSettings();
                return _settings;
            }
        }

        private static ConsolePackageSettings _settings;

        private static string commandHelpString;

        private static bool initialized;

        private static void EnsureInitialized()
        {
            if(initialized) return;
            initialized = true;
            RegisterDefaultCommands();
        }


        #region Command Registration

        public static void RegisterAllCommands()
        {
            EnsureInitialized();


#if UNITY_2023_1_OR_NEWER
            MonoBehaviour[] objects = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
#else
            MonoBehaviour[] objects = GameObject.FindObjectsOfType<MonoBehaviour>();
#endif

            bool requireInterface = settings != null && settings.RequireInterfaceForRegistration;
            foreach (var obj in objects)
            {
                if (requireInterface && obj is not ICommandProvider) continue;

                RegisterCommandsFrom(obj);
            }

            if (
                settings != null &&
                settings.EnableLoggingForCommandRegistration
            )
            {
                Debug.Log($"(CG Console) Successfully registered {allCommands.Count} commands. Type `help` into the console to see all available commands.");
            }
        }


        public static void RegisterCommandsFrom(object target)
        {
            EnsureInitialized();
            bool requireInterface = settings != null && settings.RequireInterfaceForRegistration;

            if (requireInterface && target is not ICommandProvider) return;


            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string cmdName = "";
            bool isDefaultCommand = target is DefaultCommands;
            int registered = 0;
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ConsoleCmdAttribute>();
                if (attr == null) continue;

                cmdName = attr.CommandName.ToLowerInvariant().Trim();

                if (!CommandIsValid(cmdName, isDefaultCommand)) continue;


                AddNewConsoleCommand(cmdName, attr, method, target);
                registered++;

                if (!attr.HideFromAutoComplete) commandNameList.Add(cmdName);

            }

            if (
                settings != null &&
                settings.EnableLoggingForCommandRegistration)
            {

                Debug.Log($"[CG Console] Registered `{registered}` command(s) from {target}");
            }



            UpdateHelpString();
        }

        #endregion


        #region Command Execution

        public static CommandResponse TryExecute(string input)
        {
            EnsureInitialized();

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return new CommandResponse(ResponseType.Invalid, "Command was empty");
            }

            string cmd = parts[0].ToLower();
            if (cmd.EndsWith("?"))
            {
                return LogCommandHelp(cmd.Remove(cmd.Length - 1));
            }

            if (!allCommands.TryGetValue(cmd, out ConsoleCommand command))
            {
                return new CommandResponse(ResponseType.Error, $"Command `{cmd}` was not found, or was not registered properly");
            }

            bool deadTarget = command.Target == null || (command.Target is UnityEngine.Object unityObject && unityObject == null);
            if (deadTarget && !command.MethodToExecute.IsStatic)
            {
                allCommands.Remove(cmd);
                commandNameList.Remove(cmd);
                UpdateHelpString();
                return new CommandResponse(ResponseType.Error, $"Could not find any valid targets for `{cmd}`");
            }

            object[] parameters = new object[command.Parameters.Length];

            if(parts.Length - 1 > command.Parameters.Length) return new CommandResponse(ResponseType.Invalid, $"Too many args for `{cmd}`");
            
            for (int i = 0; i < command.Parameters.Length; i++)
            {
                if (i + 1 >= parts.Length)
                {
                    return new CommandResponse(ResponseType.Invalid, $"Not enough args for command '{cmd}'");
                }

                string arg = parts[i + 1];
                Type paramType = command.Parameters[i].ParameterType;
                try
                {
                    parameters[i] = paramType.IsEnum 
                        ? Enum.Parse(paramType, arg, true)
                        : Convert.ChangeType(arg, paramType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return new CommandResponse(ResponseType.Error, $"Failed to convert arg `{arg}` to type: {paramType.Name}");
                }
            }

            try
            {
                command.MethodToExecute.Invoke(command.Target, parameters);
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                Debug.LogException(e.InnerException);
                return new CommandResponse(ResponseType.Error, e.InnerException.Message);
            }

            return new CommandResponse(ResponseType.Success);
        }


        private static CommandResponse LogCommandHelp(string command)
        {
            if (allCommands.TryGetValue(command, out ConsoleCommand cmd))
            {
                string parameters = "Args: ";
                if (cmd.Parameters.Length == 0) parameters += "No args required";
                else
                {
                    foreach (var param in cmd.Parameters)
                    {
                        parameters += param.ParameterType + " ";
                    }
                }
                Debug.Log($"{(!String.IsNullOrWhiteSpace(cmd.Description) ? cmd.Description : "No description was provided.")} // {parameters}");
                return new CommandResponse(ResponseType.Info);
            }
            return new CommandResponse(ResponseType.Invalid, $"No command found by name: `{command}`");
        }

        #endregion


        #region Default Commands

        private static void RegisterDefaultCommands()
        {
            bool registerAll = settings == null;

            if (registerAll || settings.IsCommandEnabled("help"))
            {
                allCommands.TryAdd(
                    "help",
                    new ConsoleCommand(
                        "help",
                        "List all available console commands",
                        hideFromAutoComplete: true,
                        hideFromHelpCommand: true,
                        typeof(ConsoleCommands).GetMethod(nameof(ShowHelp), BindingFlags.Static | BindingFlags.NonPublic),
                        new ParameterInfo[0],
                        null
                ));
            }

            defaultCommandsInstance = new();
            RegisterCommandsFrom(defaultCommandsInstance);
        }


        private static void ShowHelp()
        {
            Debug.Log(commandHelpString);
        }


        #endregion


        #region Utilities

        private static ConsoleCommand AddNewConsoleCommand(string cmdName, ConsoleCmdAttribute attr, MethodInfo method, object target)
        {
            ParameterInfo[] @params = method.GetParameters();

            ConsoleCommand cmd = new ConsoleCommand
            (
                cmdName.ToLowerInvariant().Trim(),
                attr.Description,
                attr.HideFromAutoComplete,
                attr.HideFromHelpCommand,
                method,
                @params,
                target
            );
            allCommands.Add(cmdName, cmd);

            return cmd;
        }


        private static bool CommandIsValid(string cmdName, bool isDefault)
        {
            if (allCommands.ContainsKey(cmdName))
            {
                if(settings != null && settings.EnableLoggingForCommandRegistration) Debug.LogError($"Command `{cmdName}` has already been registered");
                return false;
            }
            if (settings == null) return true;

            if(isDefault && !settings.IsCommandEnabled(cmdName)) return false;

            string pattern = settings.UseCustomRegexForCommandNaming && !string.IsNullOrEmpty(settings.Regex)
                ? settings.Regex
                : Globals.k_DefaultCommandNameRegex;
            
            if(!Regex.IsMatch(cmdName, pattern))
            {
                Debug.LogError($"Received invalid command format for `{cmdName}`");
                return false;
            }

            return true;
        }


        public static HashSet<string> GetCommandAutoComplete(string input)
        {
            HashSet<string> commands = new();

            foreach (String s in commandNameList)
            {
                if (s.Contains(input)) commands.Add(s);
            }

            return commands;
        }


        private static void UpdateHelpString()
        {
            string response = "Commands List: \n";
            foreach (var (_, cmd) in allCommands)
            {
                if (cmd.HideFromHelpCommand) continue;
                response += $"{cmd.Command}" + (!String.IsNullOrWhiteSpace(cmd.Description) ? "     (" + cmd.Description + ")" : "") + "\n";
            }
            commandHelpString = response;

        }

        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            allCommands.Clear();
            commandNameList.Clear();
            initialized = false;
            _settings = null;
            commandHelpString = null;
            defaultCommandsInstance = null;
        }
    }
}