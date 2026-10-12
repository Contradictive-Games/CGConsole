using UnityEngine;
using UnityEngine.SceneManagement;

namespace CG.Console
{
    internal static class DefaultCommands    
    {
        [ConsoleCmd("help", "List all available console commands", true, true)]
        private static void Help() => Debug.Log(ConsoleCommands.HelpText);

        [ConsoleCmd("load", "Load a level by name (non-async)")]
        private static void LoadScene(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene `{sceneName}` isn't in build settings");
                return;
            }
            Debug.Log($"Loading level: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }


        [ConsoleCmd("quit", "Exits the application")]
        private static void Quit()
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }


        [ConsoleCmd("set_intkey", "Set the value of a saved integer by the key name")]
        private static void SetIntKey(string keyName, int value) {
            PlayerPrefs.SetInt(keyName, value);
            PlayerPrefs.Save();
        }

        [ConsoleCmd("get_intkey")] private static void LogIntKey(string keyName) => Debug.Log($"Value for `{keyName}`: {PlayerPrefs.GetInt(keyName)}");
        

        [ConsoleCmd("set_floatkey", "Set the value of a saved float by the key name")]
        private static void SetFloatKey(string keyName, float value) {
            PlayerPrefs.SetFloat(keyName, value);
            PlayerPrefs.Save();
        }

        [ConsoleCmd("get_floatkey")] private static void LogFloatKey(string keyName) => Debug.Log($"Value for `{keyName}`: {PlayerPrefs.GetFloat(keyName)}");
        

        [ConsoleCmd("set_stringkey", "Set the value of a saved string by the key name")]
        private static void SetStringKey(string keyName, string value) {
            PlayerPrefs.SetString(keyName, value);
            PlayerPrefs.Save();
        }

        [ConsoleCmd("get_stringkey")] private static void LogStringKey(string keyName) => Debug.Log($"Value for `{keyName}`: {PlayerPrefs.GetString(keyName)}");

    }
}