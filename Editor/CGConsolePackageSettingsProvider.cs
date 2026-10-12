#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace CG.Console.Editor
{
    internal class ConsoleSettingsProvider : SettingsProvider
    {
        private SerializedObject serializedSettings;
        private UnityEditor.Editor editor;
        private ConsolePackageSettings settingsAsset;
        
        
        public ConsoleSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes) { }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settingsAsset = AssetDatabase.LoadAssetAtPath<ConsolePackageSettings>(Utilities.k_SettingsAssetPath);
            
            if (settingsAsset != null)
            {
                serializedSettings = new SerializedObject(settingsAsset);
                editor = UnityEditor.Editor.CreateEditor(settingsAsset);
            }
        }
        
        public override void OnGUI(string searchContext)
        {
            if (!settingsAsset)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.HelpBox(
                    "No settings asset found. Create one to customize and persist your CG Console settings.",
                    MessageType.Info
                );
                EditorGUILayout.Space(10);
                
                if (GUILayout.Button("Create Settings Asset", GUILayout.Height(35)))
                {
                    CreateSettingsAsset();
                }
                
                EditorGUILayout.Space(20);
                return;
            }
            
            if (serializedSettings == null || editor == null)
            {
                EditorGUILayout.HelpBox("Settings could not be loaded", MessageType.Error);
                return;
            }
            
            EditorGUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Settings Asset", settingsAsset, typeof(ConsolePackageSettings), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);
            
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                editor.OnInspectorGUI();
                
                if (changeCheck.changed)
                {
                    serializedSettings.ApplyModifiedProperties();
                    EditorUtility.SetDirty(settingsAsset);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        
        private void CreateSettingsAsset()
        {
            if (!Directory.Exists(Utilities.k_SettingsAssetDirectory))
            {
                Directory.CreateDirectory(Utilities.k_SettingsAssetDirectory);
                AssetDatabase.Refresh();
            }
            
            var newSettings = ScriptableObject.CreateInstance<ConsolePackageSettings>();
            newSettings.ResetToDefaults();
            
            AssetDatabase.CreateAsset(newSettings, Utilities.k_SettingsAssetDirectory + "/ConsolePackageSettings.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            settingsAsset = newSettings;
            serializedSettings = new SerializedObject(settingsAsset);
            editor = UnityEditor.Editor.CreateEditor(settingsAsset);
            
            Selection.activeObject = newSettings;
            EditorGUIUtility.PingObject(newSettings);
            
            Debug.Log($"CG Console Package settings asset created at: {Utilities.k_SettingsAssetDirectory}"); 
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateConsoleSettingsProvider()
        {
            var provider = new ConsoleSettingsProvider("Project/CG Console", SettingsScope.Project)
            {
                keywords = new[] { "Console", "CG", "ContradictiveGames", "Contradictive", "Command", "Debug", "Console", "Logging" }
            };
            return provider;
        }
    }
}
#endif