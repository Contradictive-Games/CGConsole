using UnityEngine;
using UnityEngine.UI;

namespace CG.Console
{
    public class ExampleCommandExecutor : MonoBehaviour
    {
        [Header("Prefab")]
        public GameObject Prefab;
        public Button ExecuteCommandFromSpawnedPrefabButton;
        public Button LogWarningButton;
        public Button LogErrorButton;

        private void Start()
        {
            if(ExecuteCommandFromSpawnedPrefabButton != null)
            {
                ExecuteCommandFromSpawnedPrefabButton.onClick.AddListener(
                    delegate { LogAStatementFromASpawnedPrefab(); }
                );
            }
            if (LogWarningButton != null) LogWarningButton.onClick.AddListener(() => ConsoleCommands.TryExecute("log_warning"));
            if (LogErrorButton != null) LogErrorButton.onClick.AddListener(() => ConsoleCommands.TryExecute("log_error"));
        }

        public void LogStatementButtonPress()
        {
            ConsoleCommands.TryExecute("log_nice");
        }

        public void LogNumberFiveButtonPress()
        {
            ConsoleCommands.TryExecute("log_number 5");
        }


        public void SpawnAnObjectWithCommands()
        {
            Instantiate(Prefab);
            ExecuteCommandFromSpawnedPrefabButton.interactable = true;
        }

        private void LogAStatementFromASpawnedPrefab()
        {
            ConsoleCommands.TryExecute("order_66");
        }
        
    }
}