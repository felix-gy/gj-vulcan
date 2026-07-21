using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform jugador; // Arrastra tu objeto Player aquí

    [Header("Configuración de 2.5D Estrecha")]
    public Vector3 offset = new Vector3(0f, 2.0f, -2.2f); // Súper cercano al personaje y pies
    public float suavidad = 10f;
    public float fovDeseado = 30f; // Campo de visión estrecho y enfocado

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        AsegurarJugador();
        AjustarEnfoqueCamara();
    }

    private void AjustarEnfoqueCamara()
    {
        if (cam != null)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize = 2.2f;
            }
            else
            {
                cam.fieldOfView = fovDeseado;
            }
            // Inclinar cámara hacia abajo hacia el personaje
            cam.transform.rotation = Quaternion.Euler(32f, 0f, 0f);
        }
    }

    private void AsegurarJugador()
    {
        if (jugador == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) jugador = p.transform;
            else
            {
                move25d pScript = FindFirstObjectByType<move25d>();
                if (pScript != null) jugador = pScript.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (jugador == null)
        {
            AsegurarJugador();
            if (jugador == null) return;
        }

        AjustarEnfoqueCamara();

        // Calculamos la posición exacta cercana al personaje
        Vector3 posicionDeseada = jugador.position + offset;

        // Movimiento fluido de persecución cercana
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavidad * Time.deltaTime);
    }
}