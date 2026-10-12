using UnityEngine;

namespace ContradictiveGames.CGConsole
{
    [CreateAssetMenu(fileName = "Example Console Window Settings", menuName = "Contradictive Games/CGConsole/Demo/Example Console Window Settings")]
    public sealed class ExampleConsoleWindowSettings : CGConsoleWindowSettings
    {
        [Header("Console Panel")]
        public Color TopBarColor = Color.black;
        public Color TitleTitleColor = Color.white;
        public Color BackgroundColor = Color.gray;
        
        [Header("Scroll Bar")]
        public Color ScrollbarHandleColor = Color.white;
        public Color ScrollbarBackgroundColor = Color.gray;
    }
}