using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIToolkitManager : MonoBehaviour
{
    // 0. Common
    [SerializeField] private UIDocument uiDocument;
    
    private VisualElement root;
    private VisualElement container; // 컨텐츠를 감싸는 컨테이너
    private float screenWidth; // 각 뷰의 너비 (화면 크기)
    private int currentPage = 1;
    [SerializeField] private int totalPages = 7;
    
    private Button _submitButton;

    // 1. BasicInfoScreen
    private VisualElement _profileArea;
    
    // 2. KeywordScreen

    // 3. InterestScreen

    // 4. IntroduceScreen

    // 5. RegisterCompleteScreen

    // 6. SetPINScreen

    // 7. ConnectDeviceScreen
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;

        Debug.Log("연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());


        // 앱 내용 등록
        container = root.Q<VisualElement>("Content");

        _submitButton = root.Q<Button>("SubmitButton");

        _submitButton.RegisterCallback<ClickEvent>(evt => Submit());

        _profileArea = root.Q<VisualElement>("Photo");

        _profileArea.RegisterCallback<ClickEvent>(evt => UpdatePhoto());
    }

    private void UpdatePhoto()
    {
        Debug.Log("Update Photo");
    }

    private void Submit()
    {
        Debug.Log("Clicked!!");
        if (currentPage < totalPages)
        {
            currentPage++;
            UpdateContainerPosition();

            switch(currentPage){
                case 1:
                    _submitButton.text = "다음으로";
                    break;
                case 2:
                    _submitButton.text = "다음으로 (0/5)";
                    break;
                case 3:
                    _submitButton.text = "다음으로 (0/5)";
                    break;
                case 4:
                    _submitButton.text = "다음으로";
                    break;
                case 5:
                    _submitButton.text = "프로필을 확인했어요.";
                    break;
                case 6:
                    _submitButton.text = "TODO";
                    break;
                case 7:
                    _submitButton.text = "착용을 완료했어요.";
                    break;
            }
        }
    }

    private void UpdateContainerPosition()
    {
        // 새로운 위치 계산 (왼쪽으로 이동)
        float newX = -(currentPage - 1) * screenWidth;
        container.style.translate = new Translate(newX, 0, 0);
    }

    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
    }
}
