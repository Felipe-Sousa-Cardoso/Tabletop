using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Token : NetworkBehaviour
{
    JogadorControlador jog;
    public bool selecionado;
    Renderer rd;


    private void Start()
    {
        jog = GetComponent<JogadorControlador>();
        rd = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            if (selecionado)
            {
                rd.material.color = Color.green;
            }
            else
            {
                rd.material.color = Color.red;
            }
        }      
    }
    public void mover(Vector3 local)
    {
        print(local);
        transform.position = local;
    }
}
