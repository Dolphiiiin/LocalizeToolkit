using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClusterVR.CreatorKit.World.Implements.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using LocalizedText = ClusterVR.CreatorKit.World.Implements.Localization.LocalizedText;

#if UNITY_EDITOR
namespace LocalizeToolkit
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Localize Toolkit")]
    [RequireComponent(typeof(LocalizedText)), DisallowMultipleComponent]
    public class LocalizeToolkit : MonoBehaviour
    {
        [Serializable]
        public class TranslatedText
        {
            public Translation[] translations;

            [Serializable]
            public class Translation
            {
                public string detected_source_language;
                public string text;
            }
        }
        
        
        public enum LangCode
        {
            en,
            ja,
            ko,
            zh_cn,
            zh_tw
        }

        [SerializeField] public LangCode langCode = LangCode.ja;
        [SerializeField] public LangCode _PrimaryLang = LangCode.ja;
        
        [SerializeField] public string _OriginalText;
        [SerializeField] public string _enText = "";
        [SerializeField] public string _jaText = "";
        [SerializeField] public string _koText = "";
        [SerializeField] public string _zhCnText = "";
        [SerializeField] public string _zhTwText = "";
        [SerializeField] public LocalizationTexts _localizationTexts;
        
        [SerializeField] public LangCode _PreviewLang;
        
        [Serializable]
        struct TextWithLangCode
        {
            [SerializeField, LangCode] string langCode;
            [SerializeField] string text;

            public string LangCode => langCode;
            public string Text => text;
        }
        
        //コンポーネントの初期化処理
        private void Awake()
        {
            UpdatePreview();
        }
        
        public void Setup()
        {
            if (GetComponent<LocalizedText>() == null)
            {
                gameObject.AddComponent<LocalizedText>();
            }
            
            
            //空白担っているテキストボックスを探索し、メッセージボックスを表示する
            List<string> blackTextList = new List<string>();
            if (_enText == "")
            {
                blackTextList.Add("en");
            }
            if (_jaText == "")
            {
                blackTextList.Add("ja");
            }
            if (_koText == "")
            {
                blackTextList.Add("ko");
            }
            if (_zhCnText == "")
            {
                blackTextList.Add("zh_cn");
            }
            if (_zhTwText == "")
            {
                blackTextList.Add("zh_tw");
            }

            if(blackTextList.Count == 5)
            {
                EditorUtility.DisplayDialog("Error", "訳文が入力されていません", "OK");
                return;
            }
            
            //空白のテキストがある場合はメッセージボックスを表示
            if (blackTextList.Count > 0)
            {
                string emptyTexts = "";
                foreach (var emptyText in blackTextList)
                {
                    emptyTexts += emptyText + ", ";
                }
                emptyTexts = emptyTexts.Substring(0, emptyTexts.Length - 2);
                if (EditorUtility.DisplayDialog("Empty Texts", $"訳文が入力されていない言語があります。: {emptyTexts}\n\n?", "Yes", "No"))
                {
                    Debug.Log("Continue");
                }
                else
                {
                    return;
                }
            }
            
            // 使用する言語設定,設定するテキストを配列に格納 *_PrimaryLangを要素の先頭にする
            string[] langCodes = { "ja", "en", "ko", "zh_cn", "zh_tw" };
            string[] texts = { _enText, _jaText, _koText, _zhCnText, _zhTwText };
            
            // blackTextListに_PrimaryLangを追加。
            // すでに存在する場合はエラー
            if (blackTextList.Contains(_PrimaryLang.ToString()))
            {
                EditorUtility.DisplayDialog("Error", "既定の言語に設定されている言語の訳文が設定されていません", "OK");
                return;
            }
            blackTextList.Add(_PrimaryLang.ToString());
            
            
            // 空白のテキストを除外
            foreach (var emptyText in blackTextList)
            {
                langCodes = langCodes.Where(x => x != emptyText).ToArray();
                texts = texts.Where(x => x != emptyText).ToArray();
            }
            
            // langCodesとtextsの先頭に_PrimaryLangを追加
            langCodes = langCodes.Prepend(_PrimaryLang.ToString()).ToArray();
            switch (_PrimaryLang.ToString())
            {
                case "en":
                    texts = texts.Prepend(_enText).ToArray();
                    break;
                case "ja":
                    texts = texts.Prepend(_jaText).ToArray();
                    break;
                case "ko":
                    texts = texts.Prepend(_koText).ToArray();
                    break;
                case "zh_cn":
                    texts = texts.Prepend(_zhCnText).ToArray();
                    break;
                case "zh_tw":
                    texts = texts.Prepend(_zhTwText).ToArray();
                    break;
            }
            
            //LocalizationTextsのインスタンスがない場合は作成する
            if(_localizationTexts == null)
            {
                Debug.LogError("_localizationTexts is null");
                _localizationTexts = ScriptableObject.CreateInstance<LocalizationTexts>();
                Debug.Log("Create LocalizationTexts");
            }
            
            // settingsに新しい設定を設定
            SetSettings(_localizationTexts, langCodes, texts);
            
            // LocalizedTextコンポーネントのlocalizationTextsに設定
            var localizedText = new SerializedObject(GetComponent<LocalizedText>());
            localizedText.FindProperty("localizationTexts").objectReferenceValue = _localizationTexts;
            localizedText.ApplyModifiedProperties();
        }
        
        //_PreviewLangが変更されたら呼ばれる
        public void UpdatePreview()
        {
            //Textコンポーネントのtextを_PreViewLangの言語のテキストに変更
            Text PreviewTextComponent = GetComponent<Text>();
            switch (_PreviewLang.ToString())
            {
                case "en":
                    PreviewTextComponent.text = _PreviewLang.ToString().Length > 0 ? _enText : "";
                    break;
                case "ja":
                    PreviewTextComponent.text = _PreviewLang.ToString().Length > 0 ? _jaText : "";
                    break;
                case "ko":
                    PreviewTextComponent.text = _PreviewLang.ToString().Length > 0 ? _koText : "";
                    break;
                case "zh_cn":
                    PreviewTextComponent.text = _PreviewLang.ToString().Length > 0 ? _zhCnText : "";
                    break;
                case "zh_tw":
                    PreviewTextComponent.text = _PreviewLang.ToString().Length > 0 ? _zhTwText : "";   
                    break;
            }
            
            //Textコンポーネントのtextを更新
            PreviewTextComponent.SetAllDirty();
            Canvas.ForceUpdateCanvases();
            UnityEditor.EditorUtility.SetDirty(PreviewTextComponent);
        }
        
        private void SetSettings(LocalizationTexts localizationTexts, string[] langCodes, string[] texts)
        {
            // リフレクションを使用してprivateなsettingsにアクセス
            Type type = typeof(LocalizationTexts);
            FieldInfo fieldInfo = type.GetField("settings", BindingFlags.NonPublic | BindingFlags.Instance);

            // 新しいLocalizationTexts.TextWithLangCode[]のインスタンスを作成
            var textWithLangCodeArray = Array.CreateInstance(type.GetNestedType("TextWithLangCode", BindingFlags.NonPublic), langCodes.Length);

            for (int i = 0; i < langCodes.Length; i++)
            {
                if(texts[i] == "")
                {
                    continue;
                }
                
                var textWithLangCodeInstance = Activator.CreateInstance(type.GetNestedType("TextWithLangCode", BindingFlags.NonPublic));
                type.GetNestedType("TextWithLangCode", BindingFlags.NonPublic).GetField("langCode", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(textWithLangCodeInstance, langCodes[i]);
                type.GetNestedType("TextWithLangCode", BindingFlags.NonPublic).GetField("text", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(textWithLangCodeInstance, texts[i]);
                textWithLangCodeArray.SetValue(textWithLangCodeInstance, i);
            }

            // settingsに新しい設定を設定
            fieldInfo.SetValue(localizationTexts, textWithLangCodeArray);
        }
        
        private IEnumerator TranslateText(string text, string targetLang, Action<string> callback)
        {
            //targetLangをDeep-Lの言語コードに変換
            //EN, JA, KO, ZH
            switch (targetLang)
            {
                case "en":
                    targetLang = "EN";
                    break;
                case "ja":
                    targetLang = "JA";
                    break;
                case "ko":
                    targetLang = "KO";
                    break;
                case "zh_cn":
                    targetLang = "ZH";
                    break;
                case "zh_tw":
                    targetLang = "zh_tw";
                    break;
            }
            
            
            string token = EditorPrefs.GetString("LOCALIZE_TOOLKIT_TOKEN", "");
            
            string json = "{\"text\":[\"" + text + "\"],\"target_lang\":\"" + targetLang + "\"}";

            UnityWebRequest request = new UnityWebRequest("https://api-free.deepl.com/v2/translate", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "DeepL-Auth-Key " + token);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                request.Dispose();
                throw new Exception("Error in TranslateText");
            }
            else
            {
                string translatedText = JsonUtility.FromJson<TranslatedText>(request.downloadHandler.text).translations[0].text;
                request.Dispose();
                callback(translatedText);
            }
        }

        public void AutoTranslate()
        {
            //OriginalTextが空白の場合は処理を終了
            if (_OriginalText == "")
            {
                Debug.LogError("OriginalText is empty");
                EditorUtility.DisplayDialog("Error", "Original Textが入力されていません", "OK");
                return;
            }
            
            
            if(EditorPrefs.GetString("LOCALIZE_TOOLKIT_TOKEN", "") == "")
            {
                EditorUtility.DisplayDialog("Error", "API Keyが設定されていません\nTools > Localize Toolkit > SettingsからAPI Keyを設定してください", "OK");
                return;
            }
            
            
            
            Debug.Log("AutoTranslate Start");

            string[] langCodes = { "en", "ja", "ko", "zh_cn", "zh_tw" };
            foreach (var lang in langCodes)
            {
                if (lang != langCode.ToString() && lang != "zh_tw")
                {
                    Debug.Log($"Start translation for {lang}");

                    try
                    {
                        StartCoroutine(TranslateText(_OriginalText, lang, translatedText =>
                        {
                            Debug.Log($"Translation completed for {lang}");


                            switch (lang)
                            {
                                case "en":
                                    _enText = translatedText;
                                    break;
                                case "ja":
                                    _jaText = translatedText;
                                    break;
                                case "ko":
                                    _koText = translatedText;
                                    break;
                                case "zh_cn":
                                    _zhCnText = translatedText;
                                    break;
                                case "zh_tw":
                                    _zhTwText = translatedText;
                                    break;
                            }
                            
                            if (_PreviewLang.ToString() == lang)
                            {
                                UpdatePreview();
                            }
                        }));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            switch (langCode.ToString())
            {
                case "en":
                    _enText = _OriginalText;
                    break;
                case "ja":
                    _jaText = _OriginalText;
                    break;
                case "ko":
                    _koText = _OriginalText;
                    break;
                case "zh_cn":
                    _zhCnText = _OriginalText;
                    break;
                case "zh_tw":
                    _zhTwText = _OriginalText;
                    break;
            }

            Debug.Log("AutoTranslate End");
        }
    }

}
#endif
