using Unity.Netcode;
using UnityEngine;

//Cada vez que necesitemos implementar código referido a comportamiento en red, debe heredar de NetworkBehaviour
//en lugar de MonoBehaviour.
public class MovimientoJugador : NetworkBehaviour
{
    [SerializeField] float velocidad;
    [SerializeField] float velocidadRotación = 150;

    private float hInput;
    private float vInput;
    private Vector3 direccion;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Solo cada jugador debe tramitar su propio input.
        //No puedo establecer el input de los demás jugadores.
        if (!IsLocalPlayer) return;
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        direccion = transform.forward * vInput;

        MoverServerRpc(direccion, velocidad);
        RotarServerRpc(hInput, velocidadRotación);
    }

    //RPC: Remote Procedure Call (Ejecución de funciones remotas). Comunicar cliente-servidor.
    //O Servidor-Cliente.
    //RPCServer: Aquellas funciones que son ejecutadas en SERVIDOR.
    [ServerRpc] //Significa que esto se lanza en el contexto del servidor.
    private void MoverServerRpc(Vector3 dir, float vel) //OBLIGATORIAMENTE hay que terminar la función con "ServerRpc" 
    {
        //Describes la dirección y velocidad directamente pero respetando físicas.
        rb.linearVelocity = dir * vel;
    }

    [ServerRpc]
    private void RotarServerRpc(float hInput, float vel)
    {
        transform.Rotate(new Vector3(0, hInput, 0) * vel * Time.deltaTime);
    }

}
