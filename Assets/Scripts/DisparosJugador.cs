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
    
    private NetworkVariable<int> municion = new NetworkVariable<int>(10);
    
    [SerializeField] private GameObject prefabMunicion;

    //En términos de red, este es como el Awake/Start
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsLocalPlayer) return; //Todo esto sólo tiene sentido en término de máquina local.

        fuerzaAcumulada = fuerzaMinima;
        UIManager.Instance.AmmoText.text = "Munición: " + municion.Value;

        municion.OnValueChanged += ActualizarMunicionTexto;
    }

    private void ActualizarMunicionTexto(int previousValue, int newValue)
    {
        UIManager.Instance.AmmoText.text = "Munición: " + newValue;
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

        if (municion.Value > 0)
        {

            NetworkObject copia = Instantiate(proyectilPrefab, spawnPoint.position, spawnPoint.rotation);
            copia.Spawn(); //OBLIGATORIO EN TERMINOS DE RED. (SPAWNS Y DESTRUCCIONES)

            //Aplicar una fuerza tipo IMPULSO a la copia en su dirección: transform.forward
            copia.GetComponent<Rigidbody>().AddForce(copia.transform.forward * resultadoFuerza, ForceMode.Impulse);

            municion.Value--;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CajaMunicion"))
        {
            if (!IsServer) return;
            
            if (municion.Value == 10) return; //Si ya tienes munición completa, no hagas nada.

            municion.Value += 3;
            if (municion.Value > 10)
            {
                municion.Value = 10;
            }

            Debug.Log("Munición actual: " + municion.Value);
            other.GetComponent<NetworkObject>().Despawn();

        }

    }

}
