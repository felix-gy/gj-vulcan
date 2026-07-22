using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PasosJugador : MonoBehaviour
{
    [Header("Sonidos de Pasos")]
    [Tooltip("Lista de clips de audio para los pasos. Se reproducirán al azar.")]
    public AudioClip[] clipsPasos;

    [Header("Configuración del Tiempo")]
    [Tooltip("Tiempo en segundos entre cada paso.")]
    public float intervaloPasos = 0.45f;

    [Header("Variación de Sonido")]
    [Range(0f, 0.3f)]
    [Tooltip("Variación aleatoria del tono (pitch) para que no suene repetitivo.")]
    public float variacionPitch = 0.1f;

    [Range(0f, 1f)]
    [Tooltip("Volumen de reproducción de los pasos.")]
    public float volumenPasos = 0.5f;

    private AudioSource audioSource;
    private move25d scriptMovimiento;
    private float timerPasos = 0f;

    void Awake()
    {
        // Obtener el componente AudioSource requerido
        audioSource = GetComponent<AudioSource>();
        
        // Configurar el AudioSource inicial
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Obtener la referencia al script de movimiento
        scriptMovimiento = GetComponent<move25d>();
    }

    void Start()
    {
        // Inicializar el temporizador al intervalo para que el primer paso suene casi al instante
        timerPasos = intervaloPasos;
    }

    void Update()
    {
        if (scriptMovimiento == null) return;

        // Comprobamos si el jugador se está moviendo usando el vector de movimiento del move25d
        bool seEstaMoviendo = scriptMovimiento.MovimientoActual.sqrMagnitude > 0.01f;

        if (seEstaMoviendo)
        {
            timerPasos += Time.deltaTime;

            if (timerPasos >= intervaloPasos)
            {
                ReproducirSonidoPaso();
                timerPasos = 0f; // Reiniciar temporizador
            }
        }
        else
        {
            // Al detenerse, restablecemos el temporizador al intervalo
            // de modo que si empieza a caminar de nuevo, suene el paso inmediatamente
            timerPasos = intervaloPasos;
        }
    }

    private void ReproducirSonidoPaso()
    {
        if (clipsPasos == null || clipsPasos.Length == 0) return;

        // Elegir un clip de la lista de manera aleatoria
        int indiceAleatorio = Random.Range(0, clipsPasos.Length);
        AudioClip clipElegido = clipsPasos[indiceAleatorio];

        if (clipElegido != null)
        {
            // Aplicar una variación aleatoria en el pitch (tono) para naturalidad
            audioSource.pitch = 1f + Random.Range(-variacionPitch, variacionPitch);
            
            // Reproducir sin interrumpir otros sonidos que estén en curso en el mismo AudioSource
            audioSource.PlayOneShot(clipElegido, volumenPasos);
        }
    }
}
