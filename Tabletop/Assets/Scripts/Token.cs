using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Token : NetworkBehaviour
{
    public bool selecionado;
    Collider col;
    Renderer rd;


    private void Start()
    {
        rd = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }



    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void MoverRpc(Vector3 destino, RpcParams rpcParams = default)
    {

        float altura = col.bounds.extents.y;
        transform.position = new Vector3(destino.x, destino.y + altura / 2f, destino.z);
    }
}
