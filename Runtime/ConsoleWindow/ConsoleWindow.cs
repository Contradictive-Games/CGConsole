using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CG.Console
{
    [AddComponentMenu("ContradictiveGames/Console/Console Window")]
    public class ConsoleWindow : MonoBehaviour
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
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else
            {
                Destroy(gameObject);
            }


            if(consoleOutputPrefab == null)
            {
                Debug.LogWarning($"We are creating a basic ConsoleOutput Prefab, as none we assigned within the inspector");
                GameObject obj = new GameObject("consoleoutput");
                consoleOutputPrefab = obj.AddComponent<TMP_Text>();
                consoleOutputPrefab.fontSize = Settings.TextSize;
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


        protected virtual void Start()
        {
            ConsoleCommands.RegisterAllCommands();
        }

        #endregion


        #region Validation

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if(consoleInput == null) Debug.LogWarning($"We currently do not have any input field added to the Developer Console", this);
            if(consoleOutputPrefab == null) Debug.LogWarning($"We currently do not have any console output prefab created within the inspector. One will be created by default but it is recommended to make one yourself", this);
            if(consoleOutputContainer == null) Debug.LogError($"We currently do not have a parent container for console output", this);
            if(scrollRect == null) Debug.LogWarning($"We currently do not have a scroll rect set for the console", this);
            if(autoCompleteEnabled && autoCompletePopup == null) Debug.LogWarning("We have enabled auto-complete but we don't have anything to display our command auto-completion", this);
            if(autoCompleteEnabled && autoCompleteText == null) Debug.LogWarning("We have enabled auto-complete but we don't have a text component setup to display our command auto-completes", this);
        }
#endif

        #endregion


        #region LogHandling

        protected virtual void CreateConsoleOutput(string message, string stackTrace = "", LogType type = LogType.Log)
        {
            if (consoleOutputContainer.childCount >= Settings.MaxLines)
            {
                Destroy(consoleOutputContainer.GetChild(0).gameObject);
            }

            TMP_Text output = Instantiate(consoleOutputPrefab, consoleOutputContainer);
            string log = $"<color=#{Settings.GetLogColorByLogType(type)}>[{DateTime.Now.ToString("HH:mm:ss")}]: {message} {(Settings.ShowStackTrace ? $"Stack Trace: {stackTrace}" : "")}</color>";

            output.SetText(log);

            Canvas.ForceUpdateCanvases();
            if(scrollRect != null) scrollRect.verticalNormalizedPosition = 0f;
        }

        #endregion


        #region InputField Handling

        protected virtual void OnCommandSubmitted(string input)
        {
            CommandResponse response = ConsoleCommands.TryExecute(input);
            if(Settings.ShowCommandResponseLogs && !String.IsNullOrWhiteSpace(response.Message)) 
            {
                CreateConsoleOutput(response.Message, "", response.ResponseType != ResponseType.Success ? LogType.Error : LogType.Assert);
            }

            if(response.ResponseType == ResponseType.Success)
            {
                consoleInput.text = "";
                autoCompletePopup.SetActive(false);
            }

            consoleInput.ActivateInputField();
        }


        protected virtual void OnInputFieldUpdated(string input)
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