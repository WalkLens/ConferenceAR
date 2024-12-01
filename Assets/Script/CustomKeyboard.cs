using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MRTK.Tutorials.MultiUserCapabilities;

public class CustomKeyboard : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    public GameObject placeHolder;

    public void OnKeyPress(string character)
    {
        if (placeHolder.activeSelf)
        {
            placeHolder.SetActive(false);
        }
        inputField.text += character;
    }

    public void OnBackspace()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void OnEnter()
    {
        // Enter key functionality
        PhotonLobby.Lobby.input_PIN = inputField;
        PhotonLobby.Lobby.CheckPIN();
    }
}
