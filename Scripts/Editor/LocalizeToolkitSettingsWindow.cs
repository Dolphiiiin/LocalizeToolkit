using UnityEditor;
using UnityEngine;

namespace LocalizeToolkit.Editor
{
    public class LocalizeToolkitSettingsWindow : EditorWindow
    {
        private string token;

        [MenuItem("Tools/Localize Toolkit/Settings")]
        public static void ShowWindow()
        {
            GetWindow<LocalizeToolkitSettingsWindow>("Localize Toolkit");
        }

        private void OnEnable()
        {
            // ウィンドウが開かれたときに、EditorPrefsからTokenの値を復元します。
            token = EditorPrefs.GetString("LOCALIZE_TOOLKIT_TOKEN", "");
        }

        private void OnGUI()
        {
            GUILayout.Label("API Key", EditorStyles.boldLabel);
            token = EditorGUILayout.TextField("API Key", token);

            if (GUILayout.Button("Save"))
            {
                // Saveボタンが押されたときに、TokenをEditorPrefsに保存します。
                EditorPrefs.SetString("LOCALIZE_TOOLKIT_TOKEN", token);
                Debug.Log("API Key saved");
            }
        }
    }
}
