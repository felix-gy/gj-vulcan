using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform jugador; // Arrastra tu objeto Player aquí

    [Header("Configuración de 2.5D")]
    public Vector3 offset = new Vector3(0f, 6f, -8f); // Distancia hacia atrás y arriba
    public float suavidad = 5f; // Qué tan suave o fluido es el seguimiento

    void FixedUpdate()
    {
        if (jugador == null) return;

        // Calculamos la posición exacta a la que debe ir la cámara
        Vector3 posicionDeseada = jugador.position + offset;

        // Movemos la cámara suavemente hacia esa posición (efecto de persecución fluida)
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavidad * Time.deltaTime);
    }
}