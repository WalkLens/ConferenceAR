using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;

public class TranslationController : MonoBehaviour
{
    public delegate void TranslationRecognizerDelegate();

    public TranslateToLanguage FromLanguage = TranslateToLanguage.Korean;
    public TranslateToLanguage TargetLanguage = TranslateToLanguage.Russian;

    private string recognizedString = "Select a mode to begin.";
    private string translatedString = "";
    private object threadLocker = new object();
    private bool isPunEnabled;

    private TranslationRecognizer translator;

    private bool micPermissionGranted = false;
    ///private bool scanning = false;

    private readonly string[] multipleTargetLangCodes = { "ru-RU", "es-ES", "de-DE", "zh-HK", "ko-KR", "en-US", "ja-JP" };


    private string fromLanguage = "";
    private string toLanguage = "";

    private SpeechToTextController speechToTextController;
    public bool IsPunEnabled
    {
        set => isPunEnabled = value;
    }
    void Start()
    {
        speechToTextController = SpeechToTextController.speechToTextController;

        if (SpeechToTextController.speechToTextController.outputText == null)
        {
            Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else
        {
            micPermissionGranted = true;
        }

        speechToTextController.onSelectRecognitionMode += HandleOnSelectRecognitionMode;

        // 
        switch (FromLanguage)
        {
            case TranslateToLanguage.Russian:
                fromLanguage = "ru-RU";
                break;
            case TranslateToLanguage.Spanish:
                fromLanguage = "es-ES";
                break;
            case TranslateToLanguage.German:
                fromLanguage = "de-DE";
                break;
            case TranslateToLanguage.Chinese:
                fromLanguage = "zh-HK";
                break;
            case TranslateToLanguage.Korean:
                fromLanguage = "ko-KR";
                break;
            case TranslateToLanguage.English:
                fromLanguage = "en-US";
                break;
            case TranslateToLanguage.Japanese:
                fromLanguage = "ja-JP";
                break;
        }

        switch (TargetLanguage)
        {
            case TranslateToLanguage.Russian:
                toLanguage = "ru-RU";
                break;
            case TranslateToLanguage.Spanish:
                toLanguage = "es-ES";
                break;
            case TranslateToLanguage.German:
                toLanguage = "de-DE";
                break;
            case TranslateToLanguage.Chinese:
                toLanguage = "zh-HK";
                break;
            case TranslateToLanguage.Korean:
                toLanguage = "ko-KR";
                break;
            case TranslateToLanguage.English:
                toLanguage = "en-US";
                break;
            case TranslateToLanguage.Japanese:
                toLanguage = "ja-JP";
                break;
        }
    }

    public void HandleOnSelectRecognitionMode(RecognitionMode recognitionMode)
    {
        if (recognitionMode == RecognitionMode.Tralation_Recognizer)
        {
            recognizedString = fromLanguage + " -> " + toLanguage + "\n" + "...!";
            translatedString = "";
            BeginTranslating();
        }
        else
        {
            if (translator != null)
            {
                translator.StopContinuousRecognitionAsync();
            }
            translator = null;
            recognizedString = "";
            translatedString = "";
        }
    }

    public async void BeginTranslating()
    {
        if (micPermissionGranted)
        {
            CreateTranslationRecognizer();

            if (translator != null)
            {
                await translator.StartContinuousRecognitionAsync().ConfigureAwait(false);
            }
        }
        else
        {
            recognizedString = "This app cannot function without access to the microphone.";
        }
    }


    void CreateTranslationRecognizer()
    {
        if (translator == null)
        {
            SpeechTranslationConfig config = SpeechTranslationConfig.FromSubscription(speechToTextController.SpeechServiceAPIKey, speechToTextController.SpeechServiceRegion);
            config.SpeechRecognitionLanguage = fromLanguage;
            //config.AddTargetLanguage(toLanguage);

            foreach (string langCode in multipleTargetLangCodes)
            {
                config.AddTargetLanguage(langCode);
            }


            translator = new TranslationRecognizer(config);

            if (translator != null)
            {
                translator.Recognizing += HandleTranslatorRecognizing;
                translator.Recognized += HandleTranslatorRecognized;
                translator.Canceled += HandleTranslatorCanceled;
                translator.SessionStarted += HandleTranslatorSessionStarted;
                translator.SessionStopped += HandleTranslatorSessionStopped;
            }
        }
    }

    #region Translation Recognition Event Handlers
    private void HandleTranslatorRecognizing(object s, TranslationRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.TranslatingSpeech)
        {
            if (e.Result.Text != "")
            {
                recognizedString = e.Result.Text;

                translatedString = "";
                foreach (var kvp in e.Result.Translations)
                {
                    // kvp.Key = 언어 코드, kvp.Value = 번역 결과
                    translatedString += $"[{kvp.Key}]: {kvp.Value}\n";
                }
                /*
                foreach (var element in e.Result.Translations)
                {
                    translatedString = element.Value;
                }
                */
            }
        }
    }

    private void HandleTranslatorRecognized(object s, TranslationRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.TranslatedSpeech)
        {
            recognizedString = e.Result.Text;
            /*
            foreach (var element in e.Result.Translations)
            {
                translatedString = element.Value;
            }
            */
            translatedString = "";
            foreach (var kvp in e.Result.Translations)
            {
                translatedString += $"[{kvp.Key}]: {kvp.Value}\n";
            }
        }
    }

    private void HandleTranslatorCanceled(object s, TranslationRecognitionEventArgs e)
    {
    }

    private void HandleTranslatorSessionStarted(object s, SessionEventArgs e)
    {
    }

    public void HandleTranslatorSessionStopped(object s, SessionEventArgs e)
    {
    }
    #endregion

    private void Update()
    {
        PunUpdateTranslator();
    }

    void OnDestroy()
    {
        if (translator != null)
        {
            translator.Dispose();
        }
    }

    public void PunUpdateTranslator()
    {
        if (isPunEnabled)
            OnToggleTranslator?.Invoke();
        else
            UpdateTranslator();
    }

    public void UpdateTranslator()
    {
        if (speechToTextController.CurrentRecognitionMode() == RecognitionMode.Tralation_Recognizer)
        {
            if (recognizedString != "")
            {
                speechToTextController.UpdateRecognizedText(recognizedString);
                if (translatedString != "")
                {
                    speechToTextController.outputText.text += "\n\ntranslated :\n" + translatedString;
                }
            }
        }
    }

    public event TranslationRecognizerDelegate OnToggleTranslator;
}
