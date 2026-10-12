namespace CG.Console
{
    internal static class Globals
    {
        public const string k_PACKAGE_NAME = "CG Console";
        public const string k_SettingsAssetDirectory = "Assets/Resources/" + k_PACKAGE_NAME;
        public const string k_SettingsAssetPath = k_SettingsAssetDirectory + "/CGConsolePackageSettings.asset";
        public const string k_SettingsResourcePath = k_PACKAGE_NAME + "/CGConsolePackageSettings";
        public const string k_DefaultCommandNameRegex = "^[a-zA-Z0-9_]+$";
        public const int k_MaxConsoleWindowLines = 200;
    }
}