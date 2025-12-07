using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class JogadorControlador : NetworkBehaviour
{
    Camera cam;

    [SerializeField] Vector3 pontoSelecionado;
    JogadorUi joggUi;
    InputSystem_Actions action;
    Token tokenSelecionado;

    [SerializeField] GameObject tokenPrefab;
    List<GameObject> tokensInstanciados = new();
    public Color cor;

    bool estadoDeInput = true;//Usado para controlar quando o jogador pode fazer inputs
    public bool EstadoDeInput { get => estadoDeInput; set => estadoDeInput = value; }

    int maskDoraycast; //Usado para detectar o mapa nas colisões

    

    private void Update()
    {
        if (!IsOwner) return;
        MovimentoCamera();
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        tokenSpawnRpc();
        tokenSpawnRpc();

        cam = Camera.main; //Define que a camera que será movimentada será a principal

        action = new InputSystem_Actions();// só habilita input no jogador local
        action.Enable(); 
        action.Player.Mouse.performed += MouseClicado;
        action.Player.Mouse2.performed += Mouse2Clicado;

        maskDoraycast = LayerMask.GetMask("Mapa", "Tokens");

        joggUi = FindFirstObjectByType<JogadorUi>(); //Usado para trocar informações com a UI
        joggUi.jogg = this;
        joggUi.Ajustes();
    }
    [Rpc(SendTo.Server)]
    public void tokenSpawnRpc()
    {
        GameObject token = Instantiate(tokenPrefab,Vector3.zero,Quaternion.identity);
        tokensInstanciados.Add(token);
        token.GetComponent<NetworkObject>().Spawn();
        token.GetComponent<Token>().nome.Value = "token" + tokensInstanciados.Count;
    }
    void MovimentoCamera()
    {
        if (!estadoDeInput) return;
        Vector3 move = cam.transform.right * action.Player.Movimento.ReadValue<Vector2>().x + //compõe o movimento da camera multiplicando os vetores de movimento pelos vetores da camera, de forma que ela
                                                                                              //se oriente para direçao da camera em vez dos eixos normais
            cam.transform.up * action.Player.Zoom.ReadValue<float>()+
            Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized * action.Player.Movimento.ReadValue<Vector2>().y;

        cam.transform.position += move;
        Vector3 rot = new Vector3(0, action.Player.RotacaoDaCameta.ReadValue<float>(), 0);
        Vector3 rotAtual = cam.transform.rotation.eulerAngles;
        rotAtual += rot;
        cam.transform.rotation = Quaternion.Euler(rotAtual);
    } //Responsável pode todos os movimentos de camera e pelo controle de input caso seja possível
    public void resetarCamera()
    {
        cam.transform.position = new Vector3(1, 16, -28);
        cam.transform.rotation = Quaternion.Euler(38, 0, 0);
    }//Chamado pela UI para resetar a posição da camera

    [Rpc(SendTo.Server)]
    public void DefinirCorRpc(Color cor)
    {
        foreach (GameObject t in tokensInstanciados)
        {
            t.GetComponent<Token>().Cor(cor);
        }
    }//Cor dos tokens
    private void MouseClicado(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //Faz um raio da camera até onde o mouse está

        RaycastHit[] hits = Physics.RaycastAll(ray, 300f, maskDoraycast); //Detecta todos os hits em uma esfera em volta do clique

        List<Token> tokensHit = new();
        foreach (var h in hits)
        {
            if (h.collider.CompareTag("Tokens"))
                tokensHit.Add(h.collider.GetComponent<Token>());
            if (h.collider.CompareTag("Mapa"))
            {
                pontoSelecionado =h.point;
            }
        } //Separa os hits que atingiram tokens e o hit que atingiu o terreno

        if (tokensHit.Count == 0)
        {
            if (tokenSelecionado != null)
            {
                tokenSelecionado.Selecionado(false);
                tokenSelecionado.MoverRpc(pontoSelecionado);
                tokenSelecionado = null;
            }//Se algum token já foi selecionado e não foi atingido nenhum outro token executa o movimento              
        }
        else if (tokensHit.Count ==1)
        {
            if (tokenSelecionado != null)
            {
                tokenSelecionado.Selecionado(false); //Remove a seleção do token anterior se ele existe
            }
            SelecionarToken(tokensHit[0]);
            
        }
        else
        {
            if (tokenSelecionado != null)
            {
                tokenSelecionado.Selecionado(false); //Remove a seleção do token anterior se ele existe
            }
            Vector3 posicaoNaTela = Mouse.current.position.ReadValue() - new Vector2(880, 540); //Desloca a posição do mouse com um pequeno offset ao lado do token
            joggUi.PosicionarLista(posicaoNaTela, tokensHit,1);
        }//Quando é acertado mais de um token
    }
    private void Mouse2Clicado(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //Faz um raio da camera até onde o mouse está

        RaycastHit[] hits = Physics.RaycastAll(ray, 300f, maskDoraycast); //Detecta todos os hits em uma esfera em volta do clique

        List<Token> tokensHit = new();
        foreach (var h in hits)
        {
            if (h.collider.CompareTag("Tokens"))
                tokensHit.Add(h.collider.GetComponent<Token>());
            if (h.collider.CompareTag("Mapa"))
            {
                pontoSelecionado = h.point;
            }
        } //Separa os hits que atingiram tokens e o hit que atingiu o terreno

        if (tokensHit.Count == 0)
        {
            joggUi.ApagarUISelecionarDefinir();
        }
        else if (tokensHit.Count >= 1)
        {
            Vector3 posicaoNaTela = Mouse.current.position.ReadValue() - new Vector2(880,540); //Desloca a posição do mouse com um pequeno offset ao lado do token
            joggUi.PosicionarLista(posicaoNaTela, tokensHit,2);
        }
    }
    public void SelecionarToken(Token token)
    {
        tokenSelecionado = token;
        tokenSelecionado.Selecionado(true);
    }
}
