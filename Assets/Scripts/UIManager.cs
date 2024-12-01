using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public string pin;
    public string name_str;
    public string job;
    public string language;
    public string introduction_1;
    public string introduction_2;
    public string introduction_3;
    public string introduction_4;
    public string introduction_5;
    public string interest_1;
    public string interest_2;
    public string interest_3;
    public string interest_4;
    public string interest_5;
    public string introduction_text;
    public string url;
    public string photo_url;
    public bool autoaccept;
    
    public static UIManager Instance { get; private set;}
    private Dictionary<string, string> inputComponent;
    private Dictionary<string, string> inputData;
    private UserData userData;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            userData = new UserData();
        }
        else
        {
            Debug.LogWarning("UIManager already exists. This instance will be destroyed.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSaveButtonClicked()
    {
        // UserData 객체 생성 및 데이터 초기화
        UserData userData = new UserData
        {
            pin = pin,
            name = name_str,
            job = job,
            language = language,
            introduction_1 = introduction_1,
            introduction_2 = introduction_2,
            introduction_3 = introduction_3,
            introduction_4 = introduction_4,
            introduction_5 = introduction_5,
            interest_1 = interest_1,
            interest_2 = interest_2,
            interest_3 = interest_3,
            interest_4 = interest_4,
            interest_5 = interest_5,
            introduction_text = introduction_text,
            url = url,
            photo_url = photo_url,
            autoaccept = autoaccept
        };
        DatabaseManager.Instance.RegisterProfile(userData);
    }

    public string getInputData()
    {
        return "0";
    }
}
