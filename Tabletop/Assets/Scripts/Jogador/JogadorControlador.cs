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

        maskDoraycast = LayerMask.GetMask("Mapa", "Tokens");

        joggUi = FindFirstObjectByType<JogadorUi>(); //Usado para trocar informações com a UI
        joggUi.jogg = this;
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
        Vector3 move = new Vector3(action.Player.Movimento.ReadValue<Vector2>().x, action.Player.Zoom.ReadValue<float>(), action.Player.Movimento.ReadValue<Vector2>().y);
        cam.transform.position += move;
        Vector3 rot = new Vector3(0, action.Player.RotacaoDaCameta.ReadValue<float>(), 0);
        Vector3 rotAtual = cam.transform.rotation.eulerAngles;
        rotAtual += rot;
        cam.transform.rotation = Quaternion.Euler(rotAtual);
    } //Responsável pode todos os movimentos de camera
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
            }
                
        }
        else if (tokensHit.Count >=1)
        {
            if (tokenSelecionado != null)
            {
                tokenSelecionado.Selecionado(false); //Remove a seleção do token anterior se ele existe
            }
            tokenSelecionado = tokensHit[0];
            tokenSelecionado.Selecionado(true);
        }      
    }
}
