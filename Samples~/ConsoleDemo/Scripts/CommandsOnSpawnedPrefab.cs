using UnityEngine;

namespace CG.Console
{
    public class CommandsOnSpawnedPrefab : MonoBehaviour, ICommandProvider
    {
        private void OnEnable()
        {
            Debug.Log("I have been spawned, try to type the command order_66 - it should work now!");
            RegisterCommands();
        }


        [ConsoleCmd("order_66")]
        private void LogAStatementFromPrefab()
        {
            Debug.Log("We have executed Order 66.");
        }
     
     
        public void RegisterCommands()
        {
            ConsoleCommands.RegisterCommandsFrom(this);
        }
    
    }
}