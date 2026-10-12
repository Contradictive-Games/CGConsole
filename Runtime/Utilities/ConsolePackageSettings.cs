using System;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace CG.Console
{
    [CreateAssetMenu(fileName = "Console Package Settings", menuName = Globals.k_PACKAGE_NAME + "/Package Settings")]
    public sealed class ConsolePackageSettings : ScriptableObject
    {
        [Serializable]
        public struct DefaultCommandSetting
        {
            public string CommandName;
            public bool Enabled;

            public DefaultCommandSetting(string name, bool enabled = true)
            {
                CommandName = name;
                Enabled = enabled;
            }
        }

        [Header("Logging")]
        [Tooltip("If you want to see the logs for every time a command is registered")]
        public bool EnableLoggingForCommandRegistration = false;

        [Header("ICommandProvider Interface")]
        [Tooltip("If you want to enforce that every time you register a command, that class must implement the ICommandProvider interface")]
        public bool RequireInterfaceForRegistration = true;

        [Header("Command Naming")]
        [Tooltip("If you want to name your commands with more than just the basic alphanumeric and underscores, you can create your own custom regex")]
        public bool UseCustomRegexForCommandNaming = false;
        [Tooltip("Your custom regex string")]
        public string Regex = "^[a-zA-Z0-9_]+$";

        [Header("Default Commands")]
        [Tooltip("You can toggle which of the default commands you would like added")]
        [SerializeField]
        private DefaultCommandSetting[] DefaultCommands = CreateDefaulstList();


        public static ConsolePackageSettings GetOrCreateSettings()
        {
            ConsolePackageSettings settings = null;

#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<ConsolePackageSettings>(Globals.k_SettingsAssetPath);

            if(settings == null)
            {
                if(!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
                if(!AssetDatabase.IsValidFolder(Globals.k_SettingsAssetDirectory)) AssetDatabase.CreateFolder("Assets/Resources", Globals.k_PACKAGE_NAME);

                settings = CreateInstance<ConsolePackageSettings>();
                settings.ResetToDefaults();
                AssetDatabase.CreateAsset(settings, Globals.k_SettingsAssetPath);
                AssetDatabase.SaveAssets();
            }
#else
            settings = Resources.Load<ConsolePackageSettings>(Globals.k_SettingsResourcePath);
#endif
            return settings;
        }

#if UNITY_EDITOR
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif


        public bool IsCommandEnabled(string commandName)
        {
            foreach (var cmd in DefaultCommands)
            {
                if (cmd.CommandName == commandName && cmd.Enabled) return true;
            }
            return false;
        }



        [ContextMenu("Reset All Settings to Defaults")]
        public void ResetToDefaults()
        {
            EnableLoggingForCommandRegistration = false;
            RequireInterfaceForRegistration = true;
            UseCustomRegexForCommandNaming = false;
            Regex = Globals.k_DefaultCommandNameRegex;
            DefaultCommands = CreateDefaulstList();
        }

        internal static DefaultCommandSetting[] CreateDefaulstList()
        {
            return new DefaultCommandSetting[]
            {
                new DefaultCommandSetting("help", true),
                new DefaultCommandSetting("clear", true),
                new DefaultCommandSetting("quit", true),
                new DefaultCommandSetting("load", true),
                new DefaultCommandSetting("set_intkey", true),
                new DefaultCommandSetting("set_floatkey", true),
                new DefaultCommandSetting("set_stringkey", true),
                new DefaultCommandSetting("get_intkey", true),
                new DefaultCommandSetting("get_floatkey", true),
                new DefaultCommandSetting("get_stringkey", true),
            };
        }
    }
}