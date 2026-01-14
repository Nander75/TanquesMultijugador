using Unity.Netcode;
using UnityEngine;

public class RotacionTorreta : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;

        //Generar un rayo (ray) a partir de un punto en la pantalla.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direccionARaton = (hit.point - transform.position).normalized;
            direccionARaton.y = 0; //Quito verticalidad.

            RotarTorretaServerRpc(direccionARaton);
        }
    }

    [ServerRpc]
    private void RotarTorretaServerRpc(Vector3 direccionARaton)
    {
        transform.forward = direccionARaton;
    }

}
