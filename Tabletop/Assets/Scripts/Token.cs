using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Token : NetworkBehaviour
{
    bool selecionado;

    Collider col; //colider da capsula 
    ParticleSystem Ps; 
    public NetworkVariable<Color> cor = new NetworkVariable<Color>(Color.white,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server); //Cor definida pelo jogador
    public NetworkVariable<FixedString32Bytes> nome = new NetworkVariable<FixedString32Bytes>("token", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); //Nome definido pelo jogador
    public NetworkVariable<Vector2> barra1 = new NetworkVariable<Vector2>(new(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); //Barra 1
    public NetworkVariable<Vector2> barra2 = new NetworkVariable<Vector2>(new(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); //Barra 2
    public NetworkVariable<Vector2> barra3 = new NetworkVariable<Vector2>(new(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); //Barra 3
    public Renderer baseRd; //Rederer da parte quadrada


    private void Start()
    {
        col = GetComponent<Collider>();
        Ps = GetComponent<ParticleSystem>();
    }
    #region network spawn e despawn
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Aplica cor inicial
        baseRd.material.color = cor.Value;
        if (Ps == null)
        {
            Ps = GetComponent<ParticleSystem>();
        }
        
        var main = Ps.main;
        main.startColor = cor.Value;
        
        

        // Registra callback
        cor.OnValueChanged += OnColorChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Remove callback quando o objeto despawna da rede
        cor.OnValueChanged -= OnColorChanged;
    }
    #endregion


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void MoverRpc(Vector3 destino, RpcParams rpcParams = default)
    {
        float altura = col.bounds.extents.y;
        transform.position = new Vector3(destino.x, destino.y + altura / 2f, destino.z);
    }
    #region Alteração das variáveis de rede
    public void Cor(Color color) //Usado para definir a cor dos tokens, é chamado no servidor pelo script JogadorControlador
    {
        cor.Value = color;
    }
    public void BarraVermelha(Vector2 valor)
    {
        barra1.Value = valor;
    }
    public void Nome(FixedString32Bytes valor)
    {
        nome.Value = valor;
    }
    #endregion
    private void OnColorChanged(Color oldColor, Color newColor) //Chamado sempre que a cor do jogador muda
    {
        var main = Ps.main;   
        main.startColor = newColor;

        if (baseRd != null)
            baseRd.material.color = newColor;
    }
    public void Selecionado(bool valor)//chamado pelo jogador controlador
    {
        selecionado = valor;
        if (selecionado)
        {
            Ps.Play();
        }
        else
        {
            Ps.Stop(true);
        }
    }
}
