using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class MultiLangReceiver : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI outputText;

    [PunRPC]
    public void OnReceiveAllTranslations(string originalText, string translationsJson)
    {
        Debug.Log($"[Receiver] Original: {originalText}");
        Debug.Log($"[Receiver] JSON: {translationsJson}");

        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(translationsJson);



        string myLangCode = GetMyLanguageCode();
        

        if (dict.TryGetValue(myLangCode, out string translation))
        {
            Debug.Log($"[Receiver] My Translation ({myLangCode}): {translation}");
            if (outputText != null)
            {
                outputText.text = $"Original:\n{originalText}\n\n" +
                                  $"[{myLangCode}] {translation}";
            }
        }
        else
        {
            Debug.LogWarning($"No exact match for {myLangCode} in translations dict.");
        }
        
    }

    // 예) Photon CustomProperties에서 "LanguageCode" 불러오기
    private string GetMyLanguageCode()
    {
        var props = PhotonNetwork.LocalPlayer.CustomProperties;
        Debug.Log(props["LanguageCode"]);
        
        if (props.ContainsKey("LanguageCode"))
        {
            string shortKey = props["LanguageCode"] as string;
            switch (shortKey)
            {
                case "ru-RU": return "ru";
                case "es-ES": return "es";
                case "de-DE": return "de";
                case "zh-HK": return "yue"; // 광둥어 → 홍콩 중국어
                case "ko-KR": return "ko";
                case "en-US": return "en";
                case "ja-JP": return "ja";
                // etc...
                default: return shortKey; // 혹시 매핑이 없으면 그대로
            }
        }
        return null;
    }
}
