using UnityEngine;
using UnityEngine.UI;

public class JogadorUi : MonoBehaviour
{
    public Button SetarCorBotao;
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public Color corAtual;
    public Image preview;

    public JogadorControlador jogg;
    private void Start()
    {
        sliderR.onValueChanged.AddListener(_ => UpdateColor());
        sliderG.onValueChanged.AddListener(_ => UpdateColor());
        sliderB.onValueChanged.AddListener(_ => UpdateColor());
        SetarCorBotao.onClick.AddListener(SetarCor);

        UpdateColor();
    }

    void UpdateColor()
    {
        Color c = new Color(
            sliderR.value / 255f,
            sliderG.value / 255f,
            sliderB.value / 255f
        );

        corAtual = c;
        preview.color = corAtual;

    }

    void SetarCor()
    {
        jogg.cor = corAtual;
        jogg.DefinirCorRpc(corAtual);
    }


}
