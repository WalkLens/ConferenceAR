using UnityEngine;
using UnityEngine.InputSystem; // Unity Input System
using Microsoft.MixedReality.WebView; // WebView2 Plugin

public class WebViewExample : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera; // 메인 카메라 (필수)
    private IWithMouseEvents webViewMouseEvents;
    private IWebView webView;

    // Unity Input System InputAction (Inspector에서 연결 필요)
    [SerializeField]
    private InputAction pointerClickAction;

    private void OnEnable()
    {
        // InputAction 활성화
        pointerClickAction.Enable();
        pointerClickAction.performed += OnPointerClick;
    }

    private void OnDisable()
    {
        // InputAction 비활성화
        pointerClickAction.performed -= OnPointerClick;
        pointerClickAction.Disable();
    }

    private void Start()
    {
        // WebView 초기화
        webView = InitializeWebView();
        webViewMouseEvents = webView as IWithMouseEvents;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnPointerClick(InputAction.CallbackContext context)
    {
        // Raycast를 통해 클릭 위치 감지
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 클릭된 객체가 WebView 영역인지 확인
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // WebView 좌표로 변환
                Vector2 webViewCoord = ConvertToWebViewSpace(hit.textureCoord);

                // WebView 이벤트 전달
                WebViewMouseEventData mouseEvent = new WebViewMouseEventData
                {
                    X = (int)webViewCoord.x,
                    Y = (int)webViewCoord.y,
                    Type = WebViewMouseEventData.EventType.MouseDown,
                    Button = WebViewMouseEventData.MouseButton.ButtonLeft,
                    TertiaryAxisDeviceType = WebViewMouseEventData.TertiaryAxisDevice.PointingDevice
                };

                webViewMouseEvents?.MouseEvent(mouseEvent);
            }
        }
    }

    /// <summary>
    /// WebView 초기화 (가상 메서드)
    /// </summary>
    private IWebView InitializeWebView()
    {
        // WebView 초기화 로직 작성
        return null; // 실제 WebView 초기화 코드로 대체
    }

    /// <summary>
    /// WebView의 좌표 공간으로 변환
    /// </summary>
    private Vector2 ConvertToWebViewSpace(Vector2 textureCoord)
    {
        // WebView의 크기 및 위치에 따라 변환 로직 작성
        return new Vector2(textureCoord.x * 1920, textureCoord.y * 1080); // 예: 1920x1080 WebView 크기 기준
    }
}
