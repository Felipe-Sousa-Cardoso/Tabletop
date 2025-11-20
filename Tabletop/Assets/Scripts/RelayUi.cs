using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RelayUI : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField joinCodeInput;
    public TextMeshProUGUI joinCodeDisplay;
    public GameObject Canvas;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
        Application.targetFrameRate = 60;
    }

    async void OnHostClicked()
    {
        string code = await RelayManager.Instance.CreateRelayAndHost();

        if (code != null)
        {
            joinCodeDisplay.text = "Join Code: " + code;
            Canvas.SetActive(false);
        }
    }

    async void OnJoinClicked()
    {
        string code = joinCodeInput.text.Trim().ToUpper();

        if (await RelayManager.Instance.JoinRelayWithCode(code))
        {
            joinCodeDisplay.text = "Conectado!";
            Canvas.SetActive(false);
        }
        else
        {
            joinCodeDisplay.text = "Falha ao conectar.";
        }

    }
}
