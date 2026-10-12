using UnityEngine;
using UnityEngine.SceneManagement;

namespace CG.Console
{
    public sealed class DefaultCommands : ICommandProvider
    {


        #region Load Level

        [ConsoleCmd("load", "Load a level by name")]
        private void LoadScene(string sceneName)
        {
            if (SceneManager.GetSceneByName(sceneName) != null)
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"Loading level: {sceneName}");
            }
        }

        #endregion


        #region Quit

        [ConsoleCmd("quit", "Exits the application")]
        private void Quit()
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion


        #region PlayerPrefs/Keys

        #region Int Keys

        [ConsoleCmd("set_intkey", "Set the value of a saved integer by the key name")]
        private void SetIntKey(string keyName, int value) {
            PlayerPrefs.SetInt(keyName, value);
            PlayerPrefs.Save();
        }

        [ConsoleCmd("get_intkey")] private void LogIntKey(string keyName) => Debug.Log($"Value for `{keyName}`: {PlayerPrefs.GetInt(keyName)}");
        
        #endregion


        #region Float Keys

        [ConsoleCmd("set_floatkey", "Set the value of a saved float by the key name")]
        private void SetFloatKey(string keyName, float value) {
            PlayerPrefs.SetFloat(keyName, value);
            PlayerPrefs.Save();
        }

        [ConsoleCmd("get_floatkey")] private void LogFloatKey(string keyName) => Debug.Log($"Value for `{keyName}`: {PlayerPrefs.GetFloat(keyName)}");
        
        #endregion


        #region String Keys

        [ConsoleCmd("set_stringkey", "Set the value of a saved string by the key name")]
        private void SetStringKey(string keyName, string value) {
            PlayerPrefs.SetString(keyName, value);
            PlayerPrefs.Save();
        }

        [ConsoleCmd("get_stringkey")] private void LogStringKey(string keyName) => Debug.Log($"Value for `{keyName}`: {PlayerPrefs.GetString(keyName)}");

        #endregion


        #endregion
    }
}