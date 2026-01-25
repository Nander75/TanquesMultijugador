using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Este script pretende EXTENDER lo que ya hace el NetworkManager.
public class ManagerExtension : MonoBehaviour
{

    [SerializeField] private NetworkObject[] playerPrefabs;
    [SerializeField] private Button BotonHost;

    private int jugadoresMinimos = 2;
    private int jugadoresActuales = 0;

    private NetworkManager networkManager;

    private TankSpawnPoint tankSpawnPoint;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    //Este métedo se ejecuta al dar al botón de Host.
    public void InicializarSesion()
    {
        networkManager.StartHost(); //Se inicie la sesión y quien dió el botón hostea.

        if (!networkManager.IsServer) return;

        //En este punto, el host está esperando a que se conecten clientes.
        networkManager.OnClientConnectedCallback += SeConectoUnCliente;

        networkManager.SceneManager.OnLoadComplete += NuevaEscenaCargada;

    }

    private void NuevaEscenaCargada(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if(sceneName == "SampleScene") //Escena de los tanques
        {
            
            TankSpawnPoint[] spawnPoints = GameObject.FindObjectsByType<TankSpawnPoint>(FindObjectsSortMode.None);
            int indiceAleatorio;

            do
            {
                //Buscar punto aleatorio
                indiceAleatorio = UnityEngine.Random.Range(0, spawnPoints.Length);

            } while (spawnPoints[indiceAleatorio].Ocupado); //El que me ofrezcas sigue estando ocupado

            NetworkObject copia = Instantiate(playerPrefabs[Random.Range(0, playerPrefabs.Length)], 
                spawnPoints[indiceAleatorio].transform.position, 
                spawnPoints[indiceAleatorio].transform.rotation);
            
            copia.SpawnAsPlayerObject(clientId, true);
            
            spawnPoints[indiceAleatorio].Ocupado = true;

            CinemachineTargetGroup targetGroup = FindAnyObjectByType<CinemachineTargetGroup>();
            targetGroup.AddMember(copia.transform, 1, 0.5f); //Añado a este tanque al total de los que la cámara visualiza

            jugadoresActuales++;
        }
    }

    public void ConectarCliente()
    {
        NetworkManager.Singleton.StartClient();
    }

    //Se ejecuta cuando se conecta un cliente nuevo.
    private void SeConectoUnCliente(ulong idCliente)
    {

        //Cuando se supera el número mínimo de jugadores, hemos de cargar el nivel
        //de los tanques.
        if (networkManager.ConnectedClients.Count >= jugadoresMinimos)
        {
            networkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }

        jugadoresActuales++;
    }
}
