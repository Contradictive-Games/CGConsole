using UnityEngine;

namespace CG.Console
{
    [CreateAssetMenu(fileName = "Console Window Settings", menuName = Globals.k_PACKAGE_NAME + "/Console Window Settings")]
    public sealed class ConsoleWindowSettings : ScriptableObject
    {
        [Header("Input Field")]
        public Color InputFieldBackgroundColor = Color.white;
        public Color InputTextColor = Color.black;
        public Color PlaceHolderTextColor = Color.black;

        [Header("Output Text")]
        public int TextSize = 24;
        public Color NormalColor = Color.white;
        public Color AssertColor = Color.green;
        public Color WarningColor = Color.yellow;
        public Color ErrorColor = Color.red;
        public Color ExceptionColor = Color.red;

        [Header("Additional Settings")]
        [Tooltip("How many output logs can be held within the parent container at most?")]
        public int MaxLines = 800;

        [Tooltip("When we logging anything to the console - do we want to also display the StackTrace?")]
        public bool ShowStackTrace = false;

        [Tooltip("If you want to see the error for why a command couldn't execute, or see the success message upon it executing properly")]
        public bool ShowCommandResponseLogs = true;
    }
}
