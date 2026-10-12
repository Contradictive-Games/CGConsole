namespace CG.Console
{
    internal static class Globals
    {
        public const string k_PACKAGE_NAME = "CG Console";
        public const string k_SettingsAssetDirectory = "Assets/Resources/" + k_PACKAGE_NAME;
        public const string k_SettingsAssetPath = "Assets/Resources/CG Console/CGConsolePackageSettings.asset";
        public const string k_SettingsResourcePath = "CG Console/CGConsolePackageSettings";
        public const string k_DefaultCommandNameRegex = "^[a-zA-Z0-9_]+$";
        public const int k_MaxConsoleLogs = 200;
    }
}