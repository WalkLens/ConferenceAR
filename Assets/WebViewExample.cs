using UnityEngine;
using UnityEngine.InputSystem; // Unity Input System
using Microsoft.MixedReality.WebView; // WebView2 Plugin

public class WebViewExample : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera; // ���� ī�޶� (�ʼ�)
    private IWithMouseEvents webViewMouseEvents;
    private IWebView webView;

    // Unity Input System InputAction (Inspector���� ���� �ʿ�)
    [SerializeField]
    private InputAction pointerClickAction;

    private void OnEnable()
    {
        // InputAction Ȱ��ȭ
        pointerClickAction.Enable();
        pointerClickAction.performed += OnPointerClick;
    }

    private void OnDisable()
    {
        // InputAction ��Ȱ��ȭ
        pointerClickAction.performed -= OnPointerClick;
        pointerClickAction.Disable();
    }

    private void Start()
    {
        // WebView �ʱ�ȭ
        webView = InitializeWebView();
        webViewMouseEvents = webView as IWithMouseEvents;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnPointerClick(InputAction.CallbackContext context)
    {
        // Raycast�� ���� Ŭ�� ��ġ ����
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ŭ���� ��ü�� WebView �������� Ȯ��
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // WebView ��ǥ�� ��ȯ
                Vector2 webViewCoord = ConvertToWebViewSpace(hit.textureCoord);

                // WebView �̺�Ʈ ����
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
    /// WebView �ʱ�ȭ (���� �޼���)
    /// </summary>
    private IWebView InitializeWebView()
    {
        // WebView �ʱ�ȭ ���� �ۼ�
        return null; // ���� WebView �ʱ�ȭ �ڵ�� ��ü
    }

    /// <summary>
    /// WebView�� ��ǥ �������� ��ȯ
    /// </summary>
    private Vector2 ConvertToWebViewSpace(Vector2 textureCoord)
    {
        // WebView�� ũ�� �� ��ġ�� ���� ��ȯ ���� �ۼ�
        return new Vector2(textureCoord.x * 1920, textureCoord.y * 1080); // ��: 1920x1080 WebView ũ�� ����
    }
}
