using System.Collections;
using UnityEngine;

public class script_audio : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioSource audioSource;     // Arrastra aquí el componente AudioSource
    public AudioClip sonidoAReproducir; // Arrastra aquí tu archivo de sonido/música

    [Header("Tiempo de Espera")]
    [Tooltip("Tiempo en segundos. 180 segundos = 3 minutos.")]
    public float tiempoEnSegundos = 180f; 

    private bool yaSeReprodujo = false;

    void Start()
    {
        // Inicia la cuenta regresiva al cargar la escena
        StartCoroutine(EsperarYReproducir());
    }

    IEnumerator EsperarYReproducir()
    {
        // Espera los 3 minutos (180 segundos) sin congelar el juego
        yield return new WaitForSeconds(tiempoEnSegundos);

        if (!yaSeReprodujo)
        {
            // Reproduce el sonido si las referencias están asignadas
            if (audioSource != null && sonidoAReproducir != null)
            {
                audioSource.PlayOneShot(sonidoAReproducir);
                yaSeReprodujo = true;
                Debug.Log("¡Sonido de los 3 minutos reproducido con éxito!");
            }
            else if (audioSource != null)
            {
                audioSource.Play();
                yaSeReprodujo = true;
            }
            else
            {
                Debug.LogWarning("Falta asignar el AudioSource o el AudioClip en el Inspector.");
            }
        }
    }
}