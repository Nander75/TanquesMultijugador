using UnityEngine;

public class RotacionTorreta : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Generar un rayo (ray) a partir de un punto en la pantalla.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Mi posición: transform.position
            //hit.transform.position
            //Sacar dirección entre ambos puntos
            //Y ALINIEAR mi transform.forward con esa dirección

            Vector3 direccion = hit.point - transform.position;
            direccion.y = 0;
            transform.forward = direccion.normalized;
            transform.position = direccion;
            
        }
    }
}
