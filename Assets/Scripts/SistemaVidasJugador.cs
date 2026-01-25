using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SistemaVidasJugador : NetworkBehaviour
{
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private Image barraVida;

    //Esta variable es una variable SINCRONIZADA. Si es modificada, los componentes de a red
    //(Cliente o Servidor) se pueden enterar de que ha sido cambiada.
    private NetworkVariable<float> vidaActual = new NetworkVariable<float>(0);

    //En términos de red, este es como el Awake/Start
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        if (IsServer) //Sólo si estoy en contexto de servidor
        {
            vidaActual.Value = vidaMaxima;
        }

        //Se disparará un evento cada vez que la vida (controlada por servidor) cambie de valor.
        vidaActual.OnValueChanged += ActualizarVida;

        barraVida.fillAmount = vidaActual.Value / vidaMaxima;
        barraVida.color = Color.Lerp(Color.red, Color.green, vidaActual.Value / vidaMaxima);
    }

    //Este método se ejecutará en los clientes cada vez que la vida (controlada por
    //el servidor) cambie de valor.
    private void ActualizarVida(float previousValue, float newValue)
    {
        barraVida.fillAmount = newValue / vidaMaxima;
        barraVida.color = Color.Lerp(Color.red, Color.green, newValue / vidaMaxima);
    }

    //Cuando sea impactado por un proyectil, bajar mi vida 20 puntos.
    //Y actualizar mi barra de vida (Sin networking).
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; //Si no eres el servidor, no deberías
        //Tramitar que es lo que ocurre ante el impacto de un proyectil.

        if (other.gameObject.CompareTag("Proyectil"))
        {
            Debug.Log("Me han dado");
            vidaActual.Value -= 20f;

            //Destruir el proyectil en términos de red
            other.GetComponent<NetworkObject>().Despawn();

            //Si mi vida llega a 0, destruirme en términos de red.
            if (vidaActual.Value <= 0)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
