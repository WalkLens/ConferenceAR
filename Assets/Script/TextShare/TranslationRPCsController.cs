using Photon.Pun; // Photon 사용
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Translation;
using TMPro;

public class TranslationRPCsController : MonoBehaviourPun
{
    public TranslateToLanguage FromLanguage = TranslateToLanguage.Korean;
    public TranslateToLanguage TargetLanguage = TranslateToLanguage.Russian;

    private string recognizedString = "";
    private string translatedString = "";

    private TranslationRecognizer translator;
    private string fromLanguage = "";
    private string toLanguage = "";

    public TextMeshProUGUI sharedTextUI; // 번역 결과를 표시할 TextMeshProUGUI

    void Start()
    {
        ConfigureLanguages();
    }

    void ConfigureLanguages()
    {
        // 언어 매핑 설정
        switch (FromLanguage)
        {
            case TranslateToLanguage.Korean: fromLanguage = "ko-KR"; break;
            case TranslateToLanguage.English: fromLanguage = "en-US"; break;
            case TranslateToLanguage.Japanese: fromLanguage = "ja-JP"; break;
            case TranslateToLanguage.Russian: fromLanguage = "ru-RU"; break;
            case TranslateToLanguage.Spanish: fromLanguage = "es-ES"; break;
            case TranslateToLanguage.German: fromLanguage = "de-DE"; break;
            case TranslateToLanguage.Chinese: fromLanguage = "zh-HK"; break;
        }

        switch (TargetLanguage)
        {
            case TranslateToLanguage.Korean: toLanguage = "ko"; break;
            case TranslateToLanguage.English: toLanguage = "en"; break;
            case TranslateToLanguage.Japanese: toLanguage = "ja"; break;
            case TranslateToLanguage.Russian: toLanguage = "ru"; break;
            case TranslateToLanguage.Spanish: toLanguage = "es"; break;
            case TranslateToLanguage.German: toLanguage = "de"; break;
            case TranslateToLanguage.Chinese: toLanguage = "zh-HK"; break;
        }
    }

    public async void StartTranslation()
    {
        if (translator == null)
        {
            var config = SpeechTranslationConfig.FromSubscription("YourSubscriptionKey", "YourRegion");
            config.SpeechRecognitionLanguage = fromLanguage;
            config.AddTargetLanguage(toLanguage);

            translator = new TranslationRecognizer(config);

            translator.Recognized += OnTranslationRecognized;
            await translator.StartContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }

    private void OnTranslationRecognized(object sender, TranslationRecognitionEventArgs e)
    {
        if (e.Result.Reason == ResultReason.TranslatedSpeech)
        {
            recognizedString = e.Result.Text;
            translatedString = e.Result.Translations[toLanguage];

            // 번역된 텍스트를 네트워크에 동기화
            photonView.RPC("BroadcastTranslation", RpcTarget.All, recognizedString, translatedString);
        }
    }

    [PunRPC]
    public void BroadcastTranslation(string original, string translation)
    {
        // 공유 UI에 번역된 텍스트 표시
        sharedTextUI.text += $"\n[Original]: {original}\n[Translated]: {translation}";
    }
}

