using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class JogadorUi : MonoBehaviour
{
    [SerializeField] UiCores Uicores;
    [SerializeField] UiInterativaSelecionartokens UiInterativaSelecioanar;
    [SerializeField] UiInterativaDefinirInfoToken UiInterativaDefinir;

    public JogadorControlador jogg;
    private void Start()
    {
        Uicores.sliderR.onValueChanged.AddListener(_ => UpdateColor());
        Uicores.sliderG.onValueChanged.AddListener(_ => UpdateColor());
        Uicores.sliderB.onValueChanged.AddListener(_ => UpdateColor());
        Uicores.SetarCorBotao.onClick.AddListener(SetarCor);
        UpdateColor();
        DefinirbotoesSelecionar();
        DefinirBotoesDefinir();
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
    #region Ui interativa
    public void PosicionarLista(Vector3 posicao, List<Token> tokens)
    {
        UiInterativaSelecioanar.tokenSSelecionado = tokens.ToArray();
        for (int i = 0; i < 3; i++)
        {
            UiInterativaSelecioanar.BotoesDeSelecao[i].gameObject.SetActive(false);
            UiInterativaSelecioanar.textoBotes[i].text = "";
            UiInterativaSelecioanar.mostradorCores[i].color = Color.white;          
        }//Reseta os valores dos botões

        UiInterativaSelecioanar.Tabela.gameObject.SetActive(true); //Ativa a tabela e a move para perto do token
        UiInterativaSelecioanar.Tabela.rectTransform.localPosition = posicao;
       
        UiInterativaDefinir.Base.gameObject.SetActive(false); //Desativa a Ui usada para mostrar e definir variáveis
        UiInterativaDefinir.Base.rectTransform.localPosition = posicao;
        int N = 0;
        foreach (Token t in tokens)
        {
            if (N < 3)
            {
                UiInterativaSelecioanar.BotoesDeSelecao[N].gameObject.SetActive(true);
                UiInterativaSelecioanar.textoBotes[N].text = t.nome.Value.ToString();
                UiInterativaSelecioanar.mostradorCores[N].color = t.cor.Value;
                N++;
            }
        }//Coloca os valores nos botões dos tokens selecionados
    }//Posiciona ambas as Ui de selecionar e definir um pouco ao lado dos tokens e preenche os botões
    void DefinirbotoesSelecionar()
    {
        // Limpar listeners existentes para evitar duplicação (BOA PRÁTICA!)
        foreach (Button b in UiInterativaSelecioanar.BotoesDeSelecao)
        {
            b.onClick.RemoveAllListeners();
        }
        int N = 0;
        foreach (Button b in UiInterativaSelecioanar.BotoesDeSelecao)
        {
            // Variável local que captura o valor atual de N.
            // ESSENCIAL para que o lambda funcione corretamente.
            int indiceAtual = N;

            // Adiciona um Listener que chama o novo método
            // e passa o índice capturado (indiceAtual).
            b.onClick.AddListener(() => BotaoDeSelecaoClicado(indiceAtual));
            N++;
        }
    }
    private void BotaoDeSelecaoClicado(int indiceDoBotao)
    {
        // A variável "indiceDoBotao" agora contém 0, 1, ou 2.
        // Você pode usar isso para saber qual token na lista original foi clicado.
        UiInterativaSelecioanar.botaoSelecionado = indiceDoBotao-1; //guarda o valor que qual botão foi selecionado
        UiInterativaSelecioanar.tokenSelecionado = UiInterativaSelecioanar.tokenSSelecionado[indiceDoBotao]; //Guarda qual token foi selecionado

        // Agora você pode usar esse índice para pegar o Token correto
        // da lista original que gerou o menu (você precisará armazenar essa lista).

        UiInterativaSelecioanar.Tabela.gameObject.SetActive(false); //desativa a hud de seleção
        UiInterativaDefinir.Base.gameObject.SetActive(true); //Ativa a hud de definir variáveis
        MostrarInformaçõesToken();
    }

    private void DefinirBotoesDefinir()
    {
        
        UiInterativaDefinir.barraVermelha.onClick.AddListener(botaoVermelho);
        UiInterativaDefinir.barraVerde.onClick.AddListener(botaoVerde);
        UiInterativaDefinir.barraAzul.onClick.AddListener(botaoAzul);

        UiInterativaDefinir.definirNome.onClick.AddListener(botaoNome);
    }

    void MostrarInformaçõesToken()
    {
        if (UiInterativaSelecioanar.tokenSelecionado != null)
        {
            //Define o máximo das barras como o y do vector2
            UiInterativaDefinir.sliderBarraVermelha.maxValue = UiInterativaSelecioanar.tokenSelecionado.barra1.Value.y;
            UiInterativaDefinir.sliderBarraVerde.maxValue = UiInterativaSelecioanar.tokenSelecionado.barra2.Value.y;
            UiInterativaDefinir.sliderBarraAzul.maxValue = UiInterativaSelecioanar.tokenSelecionado.barra3.Value.y;
            //Define o atual das barras como o x do vector2
            UiInterativaDefinir.sliderBarraVermelha.value = UiInterativaSelecioanar.tokenSelecionado.barra1.Value.x;
            UiInterativaDefinir.sliderBarraVerde.value = UiInterativaSelecioanar.tokenSelecionado.barra2.Value.x;
            UiInterativaDefinir.sliderBarraAzul.value = UiInterativaSelecioanar.tokenSelecionado.barra3.Value.x;
            //Define os textos de cada barra
            UiInterativaDefinir.textoBarravermelha.text = UiInterativaSelecioanar.tokenSelecionado.barra1.Value.x + "/" + UiInterativaSelecioanar.tokenSelecionado.barra1.Value.y;
            UiInterativaDefinir.textoBarraVerde.text = UiInterativaSelecioanar.tokenSelecionado.barra2.Value.x + "/" + UiInterativaSelecioanar.tokenSelecionado.barra2.Value.y;
            UiInterativaDefinir.textoBarraAzul.text = UiInterativaSelecioanar.tokenSelecionado.barra3.Value.x + "/" + UiInterativaSelecioanar.tokenSelecionado.barra3.Value.y;
            //Define o nome do token
            UiInterativaDefinir.nomeDoToken.text = UiInterativaSelecioanar.tokenSelecionado.nome.Value.ToString();
        }
    }

    void botaoVermelho()
    {
        string textoMin = UiInterativaDefinir.minimo.text;
        string textoMax = UiInterativaDefinir.maximo.text;

        // 1. Verifique se os campos não estão vazios
        if (!string.IsNullOrEmpty(textoMin) && !string.IsNullOrEmpty(textoMax))
        {
            //Pega os valores inteiros que foram escritos nos input field
            int valorMin = int.Parse(textoMin);
            int valorMax = int.Parse(textoMax);

            if (valorMin < valorMax)
            {
                 definirVermelhorRpc(valorMin, valorMax);
            }
        }
        MostrarInformaçõesToken();
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void definirVermelhorRpc(int valorAtual, int ValorMaximo)
    {
        UiInterativaSelecioanar.tokenSelecionado.barra1.Value = new Vector2(valorAtual,ValorMaximo);
    }
    void botaoVerde()
    {
        string textoMin = UiInterativaDefinir.minimo.text;
        string textoMax = UiInterativaDefinir.maximo.text;

        if (!string.IsNullOrEmpty(textoMin) && !string.IsNullOrEmpty(textoMax))
        {
            int valorMin = int.Parse(textoMin);
            int valorMax = int.Parse(textoMax);

            if (valorMin < valorMax)
            {
                definirVerdeRpc(valorMin, valorMax);
            }
        }
        MostrarInformaçõesToken();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void definirVerdeRpc(int valorAtual, int ValorMaximo)
    {
        UiInterativaSelecioanar.tokenSelecionado.barra2.Value =
            new Vector2(valorAtual, ValorMaximo);
    }
    void botaoAzul()
    {
        string textoMin = UiInterativaDefinir.minimo.text;
        string textoMax = UiInterativaDefinir.maximo.text;

        if (!string.IsNullOrEmpty(textoMin) && !string.IsNullOrEmpty(textoMax))
        {
            int valorMin = int.Parse(textoMin);
            int valorMax = int.Parse(textoMax);

            if (valorMin < valorMax)
            {
                definirAzulRpc(valorMin, valorMax);
            }
        }
        MostrarInformaçõesToken();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void definirAzulRpc(int valorAtual, int ValorMaximo)
    {
        UiInterativaSelecioanar.tokenSelecionado.barra3.Value =
            new Vector2(valorAtual, ValorMaximo);
    }
    
    void botaoNome()
    {      
        definirNomeRpc();     
        MostrarInformaçõesToken();
    }

    // O método RPC que o Cliente chama:
    [Rpc(SendTo.Server)]
    void definirNomeRpc()
    {
        UiInterativaSelecioanar.tokenSelecionado.nome.Value = UiInterativaDefinir.nome.text;
    }

    public void ApagarUISelecionarDefinir()
    {
        UiInterativaSelecioanar.Tabela.gameObject.SetActive(false); //desativa a hud de seleção
        UiInterativaDefinir.Base.gameObject.SetActive(false); //desativa a hud de definir variáveis
    } //Chamado do jogador controlador
    #endregion
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
[Serializable]
public class UiInterativaSelecionartokens
{
    public Image Tabela;
    public Button[] BotoesDeSelecao;
    public TextMeshProUGUI[] textoBotes;
    public RawImage[] mostradorCores;
    public int botaoSelecionado;
    public Token tokenSelecionado;
    public Token[] tokenSSelecionado;
}
[Serializable]
public class UiInterativaDefinirInfoToken
{
    public Image Base;
    //barras
    public Button barraVermelha;
    public Slider sliderBarraVermelha;
    public TextMeshProUGUI textoBarravermelha;
    public Button barraVerde;
    public Slider sliderBarraVerde;
    public TextMeshProUGUI textoBarraVerde;
    public Button barraAzul;
    public Slider sliderBarraAzul;
    public TextMeshProUGUI textoBarraAzul;
    //Nome e inputs
    public TextMeshProUGUI nomeDoToken;
    public Button definirNome;

    public TMP_InputField nome;
    public TMP_InputField minimo;
    public TMP_InputField maximo;
}
