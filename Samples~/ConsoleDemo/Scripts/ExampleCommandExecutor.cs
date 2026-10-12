using UnityEngine;
using UnityEngine.UI;

namespace ContradictiveGames.CGConsole
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
            if (LogWarningButton != null) LogWarningButton.onClick.AddListener(() => CGConsoleCommands.TryExecute("log_warning"));
            if (LogErrorButton != null) LogErrorButton.onClick.AddListener(() => CGConsoleCommands.TryExecute("log_error"));
        }

        public void LogStatementButtonPress()
        {
            CGConsoleCommands.TryExecute("log_nice");
        }

        public void LogNumberFiveButtonPress()
        {
            CGConsoleCommands.TryExecute("log_number 5");
        }


        public void SpawnAnObjectWithCommands()
        {
            Instantiate(Prefab);
            ExecuteCommandFromSpawnedPrefabButton.interactable = true;
        }

        private void LogAStatementFromASpawnedPrefab()
        {
            CGConsoleCommands.TryExecute("order_66");
        }
        
    }
}