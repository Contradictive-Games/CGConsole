using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CG.Console
{
    [AddComponentMenu(Globals.k_PACKAGE_NAME + "/Console Window")]
    public sealed class ConsoleWindow : MonoBehaviour
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


        #region Enable/Disable

        private void OnEnable()
        {
            Application.logMessageReceived += CreateConsoleOutput;
            if(consoleInput != null) {
                consoleInput.ActivateInputField();
            }
        }


        private void OnDisable()
        {
            Application.logMessageReceived -= CreateConsoleOutput;
            if(consoleInput != null) {
                consoleInput.DeactivateInputField();
            }
        }

        #endregion


        #region Initialization

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else
            {
                Destroy(gameObject);
                return;
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
            if(consoleInput != null)
            {
                consoleInput.onSubmit.AddListener(OnCommandSubmitted);
                if(autoCompleteEnabled && autoCompletePopup != null && autoCompleteText!= null)
                {
                    consoleInput.onValueChanged.AddListener(OnInputFieldUpdated);
                    autoCompletePopup.SetActive(false);
                }
            }
        }


        private void Start()
        {
            ConsoleCommands.RegisterAllCommands();
        }

        #endregion


        #region LogHandling

        private void CreateConsoleOutput(string message, string stackTrace = "", LogType type = LogType.Log)
        {
            if (consoleOutputContainer.childCount >= (Settings?.MaxLines ?? Globals.k_MaxConsoleWindowLines))
            {
                Destroy(consoleOutputContainer.GetChild(0).gameObject);
            }

            TMP_Text output = Instantiate(consoleOutputPrefab, consoleOutputContainer);
            string log = $"<color=#{Settings.GetLogColorByLogType(type)}>[{DateTime.Now:HH:mm:ss}]: {message} {(Settings.ShowStackTrace ? $"Stack Trace: {stackTrace}" : "")}</color>";

            output.SetText(log);

            Canvas.ForceUpdateCanvases();
            if(scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
        }

        #endregion


        #region InputField Handling

        private void OnCommandSubmitted(string input)
        {
            CommandResponse response = ConsoleCommands.TryExecute(input);
            if(Settings.ShowCommandResponseLogs && !String.IsNullOrWhiteSpace(response.Message)) 
            {
                CreateConsoleOutput(response.Message, type: response.ResponseType != ResponseType.Success 
                    ? LogType.Error 
                    : LogType.Assert
                );
            }

            if(response.ResponseType == ResponseType.Success)
            {
                consoleInput.text = "";
                autoCompletePopup.SetActive(false);
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

    }
}