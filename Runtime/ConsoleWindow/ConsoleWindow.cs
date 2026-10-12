using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CG.Console
{
    [AddComponentMenu(Globals.k_PACKAGE_NAME + "/Console Window")]
    public class ConsoleWindow : MonoBehaviour, ICommandProvider
    {
        public static ConsoleWindow Instance { get; private set; }

        [Header("Console Components")]
        [SerializeField] private TMP_InputField consoleInput;
        [SerializeField] private RectTransform consoleOutputContainer;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TMP_Text consoleOutputPrefab;

        [Header("Auto-Complete")]
        [SerializeField] private bool autoCompleteEnabled = true;
        [SerializeField] private GameObject autoCompletePopup;
        [SerializeField] private TMP_Text autoCompleteText;

        [Header("Settings")]
        public ConsoleWindowSettings Settings;

        private bool canAutoComplete;


        #region Enable/Disable

        protected virtual void OnEnable()
        {
            Application.logMessageReceived += CreateConsoleOutput;
            if(consoleInput != null) {
                consoleInput.ActivateInputField();
            }
        }


        protected virtual void OnDisable()
        {
            Application.logMessageReceived -= CreateConsoleOutput;
            if(consoleInput != null) {
                consoleInput.DeactivateInputField();
            }
        }

        #endregion


        #region Initialization

        protected virtual void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if(Settings == null)
            {
                Debug.LogWarning("[CG Console]: No ConsoleWindowSettings assigned, using defaults");
                Settings = ScriptableObject.CreateInstance<ConsoleWindowSettings>();
            }


            if(consoleOutputPrefab == null)
            {
                Debug.LogWarning($"We are creating a basic ConsoleOutput Prefab, as none were assigned within the inspector");
                GameObject obj = new GameObject("consoleoutput");
                consoleOutputPrefab = obj.AddComponent<TextMeshProUGUI>();
                consoleOutputPrefab.fontSize = Settings.TextSize;
                obj.transform.SetParent(transform);
                consoleOutputPrefab.gameObject.SetActive(false);
            }

            canAutoComplete = autoCompleteEnabled && autoCompletePopup != null && autoCompleteText != null;
            if(consoleInput != null)
            {
                consoleInput.onSubmit.AddListener(OnCommandSubmitted);
                if(canAutoComplete)
                {
                    consoleInput.onValueChanged.AddListener(OnInputFieldUpdated);
                    autoCompletePopup.SetActive(false);
                }
            }

        }


        private void Start()
        {
            if(Instance != this) return;
            ConsoleCommands.RegisterAllCommands();
        }

        #endregion


        #region LogHandling

        private void CreateConsoleOutput(string message, string stackTrace = "", LogType type = LogType.Log)
        {
            while(consoleOutputContainer.childCount > 0 && consoleOutputContainer.childCount >= Settings.MaxLines)
            {
                Transform oldest = consoleOutputContainer.GetChild(0);
                oldest.SetParent(null);
                Destroy(oldest.gameObject);
            }

            TMP_Text output = Instantiate(consoleOutputPrefab, consoleOutputContainer);
            output.gameObject.SetActive(true);
            string log = $"<color=#{Settings.GetLogColorByLogType(type)}>[{DateTime.Now:HH:mm:ss}]: {message} {(Settings.ShowStackTrace ? $"Stack Trace: {stackTrace}" : "")}</color>";

            output.SetText(log);

            Canvas.ForceUpdateCanvases();
            if(scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
        }

        #endregion


        #region InputField Handling

        private static LogType ToLogType(ResponseType type) => type switch
        {
            ResponseType.Success => LogType.Assert,
            ResponseType.Info => LogType.Log,
            ResponseType.Invalid => LogType.Warning,
            _ => LogType.Error
        };

        private void OnCommandSubmitted(string input)
        {
            CommandResponse response = ConsoleCommands.TryExecute(input);
            if(Settings.ShowCommandResponseLogs && !String.IsNullOrWhiteSpace(response.Message)) 
            {
                CreateConsoleOutput(response.Message, type: ToLogType(response.ResponseType));
            }

            if(response.ResponseType == ResponseType.Success)
            {
                consoleInput.text = "";
                if(canAutoComplete) autoCompletePopup.SetActive(false);
            }

            consoleInput.ActivateInputField();
        }


        private void OnInputFieldUpdated(string input)
        {
            if(input.Length == 0 || input.Contains(" ")) {
                if(autoCompletePopup.activeInHierarchy) autoCompletePopup.SetActive(false);
                return;
            }
            
            HashSet<string> commands = ConsoleCommands.GetCommandAutoComplete(input);
            if(commands.Count == 0) {
                autoCompletePopup.SetActive(false);
                autoCompleteText.SetText("");
                return;
            }

            autoCompletePopup.SetActive(true);
            string txt = "";
            foreach(var cmd in commands)
            {
                txt += cmd + "\n";
            }
            autoCompleteText.SetText(txt);
        }

        #endregion


        protected virtual void OnDestroy()
        {
            if(Instance == this) Instance = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInstance() => Instance = null;

    }
}