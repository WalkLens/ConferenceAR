using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;


[System.Serializable]
public class UserData
{
    public string pin;
    public string name;
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
}
public class DatabaseManager : MonoBehaviour
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
    
    private static DatabaseManager instance;
    public static DatabaseManager Instance { get; private set;}
    private string address;
    private Dictionary<string, string> userData;
    
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("DatabaseManager already exists. This instance will be destroyed.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ConnectDB()
    {
        return false;
    }

    public bool isPINDuplicate()
    {
        return false;
    }

    public void TestRegisterUser()
    {
        RegisterProfile();
    }

    public bool RegisterProfile()
    {
        // 요청 URL
        string apiUrl = "http://127.0.0.1:8000/users/";

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

        // JSON 데이터 생성
        string jsonData = JsonUtility.ToJson(userData);
        Debug.Log("Serialized JSON Data: " + jsonData); // 디버깅용 로그 출력

        // UnityWebRequest 객체 생성
        using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
        {
            // 헤더 설정
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // JSON 데이터를 전송할 수 있도록 설정
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 요청 전송 및 완료 대기
            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                // 요청이 완료될 때까지 대기
            }

            // 요청 성공 여부 확인
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User registered successfully: " + webRequest.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError("Failed to register user: " + webRequest.error);
                return false;
            }
        }
    }

    public bool EditProfile()
    {
        return false;
    }

    public Dictionary<string, string> FindUser(string PIN)
    {
        return userData;
    }
}
