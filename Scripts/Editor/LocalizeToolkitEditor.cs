using ClusterVR.CreatorKit.World.Implements.Localization;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace LocalizeToolkit.Editor
{
    [CustomEditor(typeof(LocalizeToolkit))]
    public class LocalizeToolkitEditor : UnityEditor.Editor
    {
        
        private bool _foldout = false;
        
        public override void OnInspectorGUI()
        {
            LocalizeToolkit localizeToolkit = (LocalizeToolkit)target;
            
            //ボタンを押してドキュメントを開く
            if(GUILayout.Button("Document / ドキュメントを開く"))
            {
                Application.OpenURL("https://toza.notion.site/Localize-Toolkit-2ef04cf16833482c9dc4ac9674948663?pvs=4");
            }
            
            //各言語のテキストを入力
            EditorGUI.indentLevel++;
            localizeToolkit._PrimaryLang = (LocalizeToolkit.LangCode)EditorGUILayout.EnumPopup("既定の言語", localizeToolkit._PrimaryLang);
            
            localizeToolkit._enText = EditorGUILayout.TextField("en", localizeToolkit._enText);
            localizeToolkit._jaText = EditorGUILayout.TextField("ja", localizeToolkit._jaText);
            localizeToolkit._koText = EditorGUILayout.TextField("ko", localizeToolkit._koText);
            localizeToolkit._zhCnText = EditorGUILayout.TextField("zh_cn", localizeToolkit._zhCnText);
            localizeToolkit._zhTwText = EditorGUILayout.TextField("zh_tw", localizeToolkit._zhTwText);
            
            EditorGUI.indentLevel--;
            
            
            _foldout = EditorGUILayout.Foldout(_foldout, "DeepL");
            if(_foldout)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox("使用にはDeepLのAPIキーが必要です\nDeepLの仕様の都合上、Zh-Tw(繁体字)への翻訳に対応していません", MessageType.Info);
                
                localizeToolkit.langCode = (LocalizeToolkit.LangCode)EditorGUILayout.EnumPopup("原文の言語", localizeToolkit.langCode);
                localizeToolkit._OriginalText = EditorGUILayout.TextField("原文", localizeToolkit._OriginalText);
            
                if(GUILayout.Button("Translate"))
                {
                    localizeToolkit.AutoTranslate();
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
            
            if (GUILayout.Button("Setup"))
            {
                localizeToolkit.Setup();
            }
            
            localizeToolkit._localizationTexts = (LocalizationTexts)EditorGUILayout.ObjectField("LocalizationTexts", localizeToolkit._localizationTexts, typeof(LocalizationTexts), true);
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            var currentPreviewLang = localizeToolkit._PreviewLang;
            
            localizeToolkit._PreviewLang = (LocalizeToolkit.LangCode)EditorGUILayout.EnumPopup("プレビューする言語", localizeToolkit._PreviewLang);
            if(localizeToolkit._PreviewLang != currentPreviewLang)
            {
                localizeToolkit.UpdatePreview();
                currentPreviewLang = localizeToolkit._PreviewLang;
            }
            
            if(GUILayout.Button("Update Preview"))
            {
                localizeToolkit.UpdatePreview();
            }
        }
    }
    
    

    
}
#endif
