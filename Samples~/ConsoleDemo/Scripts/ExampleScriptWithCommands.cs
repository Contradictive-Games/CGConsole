using UnityEngine;

namespace ContradictiveGames.CGConsole
{
    public class ExampleScriptWithCommands : MonoBehaviour, ICommandProvider
    {
        private void Awake() => Debug.Log("Hello from Awake!", this);
        
        [ConsoleCmd("log_nice")]
        private void LogANiceStatement() => Debug.Log("I hope you have a great day!");

        [ConsoleCmd("log_number", "Log any number you would like to see")]
        private void LogANumber(int number) => Debug.Log($"Great choice! Your number was: {number}");

        [ConsoleCmd("log_warning", "Log a scary warning")]
        private void LogAWarning() => Debug.LogWarning("We just logged a warning");

        [ConsoleCmd("log_error", "Log an even scarier error")]
        private void LogAnError() => Debug.LogError($"We just logged an error!");

        [ConsoleCmd(
            "log_secret", 
            "A command that logs a secret statement, this will not show up in auto-complete and not show up in `help`", 
            hideFromAutoComplete:true, 
            hideFromHelpCommand:true
        )]
        private void LogASecret() => Debug.Log("A Secret");
    }
}