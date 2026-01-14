using Unity.Netcode;
using UnityEngine;

public class DisparosJugador : NetworkBehaviour
{

    [SerializeField] private NetworkObject proyectilPrefab;
    
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float fuerzaAcumulada;
    [SerializeField] private float fuerzaMinima;
    [SerializeField] private float limiteFuerza;
    [SerializeField] private float ratioPorSegundo;

    //En términos de red, este es como el Awake/Start
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        fuerzaAcumulada = fuerzaMinima;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!IsLocalPlayer) return;

        if (Input.GetMouseButton(0))
        {
            fuerzaAcumulada += ratioPorSegundo * Time.deltaTime;            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (fuerzaAcumulada >= limiteFuerza)
            {
                fuerzaAcumulada = limiteFuerza;
            }
            SpawnProyectilServerRpc(fuerzaAcumulada);
            fuerzaAcumulada = fuerzaMinima;
        }
        
    }

    [ServerRpc]
    private void SpawnProyectilServerRpc(float resultadoFuerza)
    {
        NetworkObject copia = Instantiate(proyectilPrefab, spawnPoint.position, spawnPoint.rotation);
        copia.Spawn(); //OBLIGATORIO EN TERMINOS DE RED. (SPAWNS Y DESTRUCCIONES)

        //Aplicar una fuerza tipo IMPULSO a la copia en su dirección: transform.forward
        copia.GetComponent<Rigidbody>().AddForce(copia.transform.forward * resultadoFuerza, ForceMode.Impulse);

    }

}
