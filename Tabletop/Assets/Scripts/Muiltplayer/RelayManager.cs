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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // destrói o duplicado
            return;
        }

        Instance = this;
    }

    private async void Start()
    {
        await InitializeUnityServices();
        if (NetworkManager.Singleton.IsServer)
        {
            // Conecta o evento para que o servidor gerencie o spawn de tokens
        }
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Uninitialized)
                return;

            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("Relay pronto.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro ao iniciar Relay: " + e);
        }
    }
    public async Task<string> CreateRelayAndHost()
    {    
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(8);       Debug.Log("2h");
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        var relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async Task<bool> JoinRelayWithCode(string joinCode)
    {
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

       // return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();

        bool result = NetworkManager.Singleton.StartClient();
        return result;
    }
}
