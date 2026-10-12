#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CG.Console.Editor
{
    [CustomEditor(typeof(ConsolePackageSettings))]
    internal sealed class ConsolePackageSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty enableLogging;
        private SerializedProperty defaultCommands;
        private SerializedProperty forceInterfaceRequirement;
        private SerializedProperty useCustomRegex;
        private SerializedProperty customRegexString;

        private void OnEnable()
        {
            if (target == null || serializedObject == null) return;

            enableLogging = serializedObject.FindProperty("EnableLoggingForCommandRegistration");
            defaultCommands = serializedObject.FindProperty("DefaultCommands");
            forceInterfaceRequirement = serializedObject.FindProperty("RequireInterfaceForRegistration");
            useCustomRegex = serializedObject.FindProperty("UseCustomRegexForCommandNaming");
            customRegexString = serializedObject.FindProperty("Regex");
        }

        public override void OnInspectorGUI()
        {
            if (target == null || serializedObject == null)
            {
                EditorGUILayout.HelpBox("We currently do not have any settings scriptable object", MessageType.Warning);
                return;
            }
            serializedObject.Update();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("CG Console Package Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("These settings control the behavior of the CG Console package.", MessageType.Info);
            EditorGUILayout.Space(5);

            float originalLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * .4f;
            EditorGUILayout.PropertyField(enableLogging);
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(forceInterfaceRequirement);
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(useCustomRegex);
            EditorGUIUtility.labelWidth = originalLabelWidth;
            if (useCustomRegex.boolValue)
            {
                EditorGUILayout.HelpBox("Warning: This can potentially break some of the default behaviors with the console. It's best to at least ensure that your own commands can not end with a `?`", MessageType.Warning);
            }
            EditorGUI.BeginDisabledGroup(useCustomRegex.boolValue == false);
            EditorGUILayout.PropertyField(customRegexString);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(15);


            EditorGUILayout.LabelField("Default Commands", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));
            if (GUILayout.Button("Enable All", GUILayout.Height(18)) && defaultCommands.arraySize != 0)
            {
                for (int i = 0; i < defaultCommands.arraySize; i++)
                {
                    SerializedProperty command = defaultCommands.GetArrayElementAtIndex(i);
                    SerializedProperty isEnabled = command.FindPropertyRelative("Enabled");
                    isEnabled.boolValue = true;
                }
            }
            if (GUILayout.Button("Disable All", GUILayout.Height(18)) && defaultCommands.arraySize != 0)
            {
                for (int i = 0; i < defaultCommands.arraySize; i++)
                {
                    SerializedProperty command = defaultCommands.GetArrayElementAtIndex(i);
                    SerializedProperty isEnabled = command.FindPropertyRelative("Enabled");
                    isEnabled.boolValue = false;
                }
            }
            EditorGUILayout.EndHorizontal();


            if (defaultCommands.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No default commands configured", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < defaultCommands.arraySize; i++)
                {
                    SerializedProperty command = defaultCommands.GetArrayElementAtIndex(i);
                    SerializedProperty commandName = command.FindPropertyRelative("CommandName");
                    SerializedProperty isEnabled = command.FindPropertyRelative("Enabled");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PropertyField(isEnabled, GUIContent.none, GUILayout.Width(20));
                    EditorGUILayout.LabelField(commandName.stringValue, GUILayout.ExpandWidth(true));

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Space(5);


            EditorGUI.indentLevel--;
            EditorGUILayout.Space(10);


            // Reset Button
            if (GUILayout.Button("Reset All Settings to Defaults", GUILayout.Height(25), GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)))
            {
                if (EditorUtility.DisplayDialog("Reset Settings",
                    "Are you sure you want to reset all settings to their default values?",
                    "Reset", "Cancel"))
                {
                    (target as ConsolePackageSettings)?.ResetToDefaults();
                    EditorUtility.SetDirty(target);
                }
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif