using Unity.Netcode;
using UnityEngine;

public class JogadorMedições : NetworkBehaviour
{
    LineRenderer lineRenderer;
    public NetworkVariable<Vector3> Ponto1 = new NetworkVariable<Vector3>(new(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> Ponto2 = new NetworkVariable<Vector3>(new(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();       
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, Ponto1.Value);
        lineRenderer.SetPosition(1, Ponto2.Value);
    }
}
