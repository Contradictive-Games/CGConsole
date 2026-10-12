using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ContradictiveGames.CGConsole
{
    public sealed class ExampleConsoleWindow : CGConsoleWindow
    {
        
        [Header("Settings")]
        private ExampleConsoleWindowSettings settings => Settings as ExampleConsoleWindowSettings;

        [Header("Panel Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image topBar;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image inputFieldBackground;
        [SerializeField] private TMP_Text inputFieldText;
        [SerializeField] private TMP_Text inputFieldPlaceholder;
        [SerializeField] private Image scrollBarHandle;
        [SerializeField] private Image scrollBarBackground;


        [ContextMenu("Apply Theme")]
        private void ApplyConsoleTheme()
        {
            if(settings == null) return;

            if(background != null) background.color = settings.BackgroundColor;
            if(titleText != null) titleText.color = settings.TitleTitleColor;
            if(topBar != null) topBar.color = settings.TopBarColor;
            if(inputFieldBackground != null) inputFieldBackground.color = settings.InputFieldBackgroundColor;
            if(inputFieldText != null) inputFieldText.color = settings.InputTextColor;
            if(inputFieldPlaceholder != null) inputFieldPlaceholder.color = settings.PlaceHolderTextColor;
            if(scrollBarHandle != null) scrollBarHandle.color = settings.ScrollbarHandleColor;
            if(scrollBarBackground != null) scrollBarBackground.color = settings.ScrollbarBackgroundColor;
        }


        protected override void OnValidate()
        {
            base.OnValidate();

            if(settings == null) Debug.LogError($"We currently do not have a console settings asset");
            else ApplyConsoleTheme();
        }
    }
}
