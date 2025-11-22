using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Token : NetworkBehaviour
{
    public bool selecionado;

    Collider col;
    Renderer rd;
    public NetworkVariable<Color> cor = new NetworkVariable<Color>(Color.white,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public Renderer baseRd;

    private void Start()
    {
        rd = GetComponent<Renderer>();
        col = GetComponent<Collider>();

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Aplica cor inicial
        baseRd.material.color = cor.Value;

        // Registra callback
        cor.OnValueChanged += OnColorChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Remove callback quando o objeto despawna da rede
        cor.OnValueChanged -= OnColorChanged;
    }



    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void MoverRpc(Vector3 destino, RpcParams rpcParams = default)
    {
        float altura = col.bounds.extents.y;
        transform.position = new Vector3(destino.x, destino.y + altura / 2f, destino.z);
    }

    public void Cor(Color color) //Usado para definir a cor dos tokens, é chamado no servidor pelo script JogadorControlador
    {
        cor.Value = color;
    }
    private void OnColorChanged(Color oldColor, Color newColor)
    {
        if (baseRd != null)
            baseRd.material.color = newColor;
    }
}
