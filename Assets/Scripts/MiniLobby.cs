using Unity.Netcode;
using UnityEngine;

public class MiniLobby : MonoBehaviour
{   
    public void OnclickHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void OnclickCliente()
    {
        NetworkManager.Singleton.StartClient();
    }
}
