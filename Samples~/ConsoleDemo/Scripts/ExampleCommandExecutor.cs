using UnityEngine;
using UnityEngine.UI;

namespace CG.Console
{
    internal sealed class ExampleCommandExecutor : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Button executeCommandFromSpawnedPrefabButton;
        [SerializeField] private Button logWarningButton;
        [SerializeField] private Button logErrorButton;

        private void Start()
        {
            if(executeCommandFromSpawnedPrefabButton != null)
            {
                executeCommandFromSpawnedPrefabButton.onClick.AddListener(
                    delegate { LogAStatementFromASpawnedPrefab(); }
                );
            }
            if (logWarningButton != null) logWarningButton.onClick.AddListener(() => ConsoleCommands.TryExecute("log_warning"));
            if (logErrorButton != null) logErrorButton.onClick.AddListener(() => ConsoleCommands.TryExecute("log_error"));
        }

        internal void LogStatementButtonPress()
        {
            ConsoleCommands.TryExecute("log_nice");
        }

        internal void LogNumberFiveButtonPress()
        {
            ConsoleCommands.TryExecute("log_number 5");
        }


        internal void SpawnAnObjectWithCommands()
        {
            Instantiate(prefab);
            executeCommandFromSpawnedPrefabButton.interactable = true;
        }

        private void LogAStatementFromASpawnedPrefab()
        {
            ConsoleCommands.TryExecute("order_66");
        }
        
    }
}