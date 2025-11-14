using UnityEngine;
using Unity.Netcode;

public class Começo : MonoBehaviour
{
    void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Host"))
                NetworkManager.Singleton.StartHost();

            if (GUI.Button(new Rect(10, 60, 200, 40), "Client"))
                NetworkManager.Singleton.StartClient();
        }
    }
}
