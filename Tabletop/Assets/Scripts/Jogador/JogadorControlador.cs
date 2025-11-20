using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class JogadorControlador : NetworkBehaviour
{
    InputSystem_Actions action;

    public Vector3 targetPos;

    Token tokenSelecionado;

    int maskDoraycast; //Usado para detectar o mapa nas colisões
    private void MouseClicado(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()); //Faz um raio da camera até onde o mouse está

        if (Physics.Raycast(ray, out RaycastHit hit, 300f,maskDoraycast)) //Detecta colisão do raio da posição do mouse do jogador e salva as informações no hit, 300 é a distancia máxima
        {          
            if (hit.collider.CompareTag("Mapa"))
            {
                if (tokenSelecionado == null)
                {
                    targetPos = hit.point;
                }
                else
                {
                    tokenSelecionado.MoverRpc(hit.point);
                    tokenSelecionado = null;
                }
            }
            if (hit.collider.CompareTag("Tokens"))
            {
                tokenSelecionado = hit.collider.GetComponent<Token>();
            }
        }
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        action = new InputSystem_Actions();
        action.Enable(); // só habilita input no jogador local

        action.Player.Mouse.performed += MouseClicado;
        maskDoraycast = LayerMask.GetMask("Mapa", "Tokens");
        
    }
}
