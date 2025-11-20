using System.Threading.Tasks; 
using UnityEngine;
using Unity.Netcode; 
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core; 
using Unity.Services.Authentication;
using Unity.Services.Relay; 
using Unity.Services.Relay.Models; 

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    //private UnityTransport utp;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Relay conectado com sucesso!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao inicializar Unity Services: " + e.Message);
        }
    }

    public async Task<bool> JoinRelayWithCode(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
        return true;
    }
}
