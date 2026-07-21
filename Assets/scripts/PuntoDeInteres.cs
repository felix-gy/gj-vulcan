using UnityEngine;
using UnityEngine.InputSystem;

public class PuntoDeInteres : MonoBehaviour
{
    [Header("Identificación y Fase")]
    public string idPunto; // "lucas", "madre", "toby"
    public GriefPhase faseRequerida = GriefPhase.Procesar;
    public bool yaBuscado = false;

    [Header("Textos Narrativos")]
    public string nombreZona = "Ruinas de la Habitación de Lucas";
    public string tituloDescubrimiento = "Lucas... Mi hermano";
    [TextArea(4, 8)]
    public string narrativaDescubrimiento = "No... no puede ser. Su chaqueta de béisbol sobresale del hormigón quemado. Grito su nombre pero las cenizas ahogan el eco...";
    [TextArea(3, 5)]
    public string reflexionPsicologica = "Reflexión: Tu cerebro experimenta una disociación e incredulidad inicial. Es la fase de shock tratando de protegerte del dolor inasimilable.";

    [Header("Indicador Visual")]
    public Light luzIndicadora;
    public Color colorIndicador = new Color(0.9f, 0.2f, 0.2f);
    public float distanciaInteraccion = 3.5f;

    private Transform jugadorTransform;
    private bool jugadorCerca = false;

    void Start()
    {
        AsegurarLuzIndicadora();
        BuscarJugador();
    }

    void Update()
    {
        BuscarJugadorSiEsNecesario();

        if (jugadorTransform == null) return;

        bool faseActiva = GameManager.Instance != null && GameManager.Instance.FaseActual == faseRequerida;
        bool interactuable = faseActiva && !yaBuscado;

        // Medir distancia con jugador
        float distancia = Vector3.Distance(transform.position, jugadorTransform.position);
        jugadorCerca = (distancia <= distanciaInteraccion) && interactuable;

        // Actualizar luz indicadora parpadeante
        if (luzIndicadora != null)
        {
            luzIndicadora.enabled = interactuable;
            if (interactuable)
            {
                float pulso = 0.5f + Mathf.Sin(Time.time * 4f) * 0.5f;
                luzIndicadora.intensity = 1.5f + pulso * 2f;
            }
        }

        // Notificar al UI Manager si el jugador está cerca
        if (jugadorCerca && UIManager.Instance != null)
        {
            UIManager.Instance.MostrarPromptInteraccion(true, $"Presiona [E] para buscar en {nombreZona}");

            // Detectar tecla E o Espacio para buscar (Input System)
            bool teclaPresionada = Keyboard.current != null && (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame);
            if (teclaPresionada)
            {
                Interactuar();
            }
        }
        else if (interactuable && UIManager.Instance != null && UIManager.Instance.PuntoActualEnFoco == this)
        {
            UIManager.Instance.MostrarPromptInteraccion(false, "");
        }
    }

    private void BuscarJugador()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            jugadorTransform = p.transform;
        }
        else
        {
            move25d playerScript = FindFirstObjectByType<move25d>();
            if (playerScript != null) jugadorTransform = playerScript.transform;
        }
    }

    private void BuscarJugadorSiEsNecesario()
    {
        if (jugadorTransform == null) BuscarJugador();
    }

    private void AsegurarLuzIndicadora()
    {
        if (luzIndicadora == null)
        {
            GameObject luzGO = new GameObject("LuzPuntoInteres");
            luzGO.transform.SetParent(transform);
            luzGO.transform.localPosition = new Vector3(0f, 1.5f, 0f);

            luzIndicadora = luzGO.AddComponent<Light>();
            luzIndicadora.type = LightType.Point;
            luzIndicadora.range = 7f;
            luzIndicadora.color = colorIndicador;
            luzIndicadora.intensity = 2f;
        }
    }

    public void Interactuar()
    {
        if (yaBuscado) return;

        yaBuscado = true;
        if (luzIndicadora != null) luzIndicadora.enabled = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.MostrarPromptInteraccion(false, "");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ProcesarPuntoDescubierto(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = colorIndicador;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }
}
