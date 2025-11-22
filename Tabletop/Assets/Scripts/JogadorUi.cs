using System;
using UnityEngine;
using UnityEngine.UI;

public class JogadorUi : MonoBehaviour
{
    [Header("Ajuste de cor")]
    [SerializeField] UiCores Uicores;

    //Lista

    public JogadorControlador jogg;
    private void Start()
    {
        Uicores.sliderR.onValueChanged.AddListener(_ => UpdateColor());
        Uicores.sliderG.onValueChanged.AddListener(_ => UpdateColor());
        Uicores.sliderB.onValueChanged.AddListener(_ => UpdateColor());
        Uicores.SetarCorBotao.onClick.AddListener(SetarCor);
        UpdateColor();
    }
    #region cor
    void UpdateColor()
    {
        Color c = new Color(
            Uicores.sliderR.value / 255f,
            Uicores.sliderG.value / 255f,
            Uicores.sliderB.value / 255f
        );
        Uicores.corAtual = c;
        Uicores.preview.color = Uicores.corAtual;
    }
    void SetarCor()
    {
        jogg.cor = Uicores.corAtual;
        jogg.DefinirCorRpc(Uicores.corAtual);
    }
    #endregion
    public void PosicionarLista(Vector3 posicao)
    {

    }
}
[Serializable]
public class UiCores
{
    public Button SetarCorBotao;
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public Color corAtual;
    public Image preview;
}
