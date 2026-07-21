using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Estado del Juego")]
    public GriefPhase FaseActual { get; private set; } = GriefPhase.Procesar;
    public string HoraActual { get; private set; } = "02:00 AM";

    [Header("Referencias a Administradores")]
    public AtmosphereManager atmosphereManager;
    public UIManager uiManager;
    public LinternaJugador linternaJugador;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AsegurarSistemasRequeridos();
        IniciarFase(GriefPhase.Procesar, "02:00 AM", "Encuentra a tu hermano Lucas entre las ruinas");
    }

    private void AsegurarSistemasRequeridos()
    {
        if (atmosphereManager == null) atmosphereManager = FindFirstObjectByType<AtmosphereManager>();
        if (atmosphereManager == null)
        {
            GameObject atmoGO = new GameObject("AtmosphereManager");
            atmosphereManager = atmoGO.AddComponent<AtmosphereManager>();
        }

        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            GameObject uiGO = new GameObject("UIManager");
            uiManager = uiGO.AddComponent<UIManager>();
        }

        if (linternaJugador == null) linternaJugador = FindFirstObjectByType<LinternaJugador>();
        if (linternaJugador == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                linternaJugador = player.AddComponent<LinternaJugador>();
            }
            else
            {
                move25d playerScript = FindFirstObjectByType<move25d>();
                if (playerScript != null)
                {
                    linternaJugador = playerScript.gameObject.AddComponent<LinternaJugador>();
                }
            }
        }

        // Asegurar sistema de audio
        if (DisasterAudioSystem.Instance == null)
        {
            GameObject audioGO = new GameObject("DisasterAudioSystem");
            audioGO.AddComponent<DisasterAudioSystem>();
        }

        // Asegurar sistema de partículas de ceniza
        if (FindFirstObjectByType<EfectoCenizaVolcanica>() == null)
        {
            GameObject cenizaGO = new GameObject("EfectoCenizaVolcanica");
            cenizaGO.AddComponent<EfectoCenizaVolcanica>();
        }

        // Generar los 3 puntos clave de tragedia en el mapa si no existen
        AsegurarPuntosDeInteres();
    }

    private void AsegurarPuntosDeInteres()
    {
        PuntoDeInteres[] puntos = FindObjectsByType<PuntoDeInteres>(FindObjectsSortMode.None);
        bool tieneLucas = false, tieneMadre = false, tieneToby = false;

        foreach (var p in puntos)
        {
            if (p.idPunto == "lucas") tieneLucas = true;
            if (p.idPunto == "madre") tieneMadre = true;
            if (p.idPunto == "toby") tieneToby = true;
        }

        Vector3 posJugador = Vector3.zero;
        GameObject pObj = GameObject.FindWithTag("Player");
        if (pObj != null) posJugador = pObj.transform.position;

        // 1. Punto Lucas (Fase 1: Procesar)
        if (!tieneLucas)
        {
            GameObject puntoLucas = new GameObject("PuntoInteres_Lucas");
            puntoLucas.transform.position = posJugador + new Vector3(-12f, 0f, 15f);
            PuntoDeInteres pScript = puntoLucas.AddComponent<PuntoDeInteres>();
            pScript.idPunto = "lucas";
            pScript.faseRequerida = GriefPhase.Procesar;
            pScript.nombreZona = "Habitación de Lucas (Hermano)";
            pScript.tituloDescubrimiento = "Lucas... Mi hermano";
            pScript.narrativaDescubrimiento = "No... ¡LUCAS! Tu chaqueta de béisbol sobresale del cemento calcinado... Grito tu nombre pero la ceniza silencia mi lamento. Solo hay frío y oscuridad. Esto no puede estar pasando...";
            pScript.reflexionPsicologica = "Reflexión: Tu mente experimenta un shock y negación absoluta. El cerebro intenta bloquear el golpe protegiéndose de la insoportable realidad.";
            pScript.colorIndicador = new Color(0.95f, 0.25f, 0.25f);
        }

        // 2. Punto Madre/Padre (Fase 2: Aceptar)
        if (!tieneMadre)
        {
            GameObject puntoMadre = new GameObject("PuntoInteres_Madre");
            puntoMadre.transform.position = posJugador + new Vector3(25f, 0f, 22f);
            PuntoDeInteres pScript = puntoMadre.AddComponent<PuntoDeInteres>();
            pScript.idPunto = "madre";
            pScript.faseRequerida = GriefPhase.Aceptar;
            pScript.nombreZona = "Edificio Comunitario (Madre)";
            pScript.tituloDescubrimiento = "El colgante de Mamá";
            pScript.narrativaDescubrimiento = "Remuevo la tierra ardiente con mis manos hasta sangrar... solo para hallar tu colgante entre tus dedos fríos. Ya no hay gritos, solo lágrimas mudas. Mamá... la erupción te llevó y debo aceptarlo.";
            pScript.reflexionPsicologica = "Reflexión: Has cruzado el muro del shock. Al aceptar la devastación del ser amado, asimilas el dolor puro y comienzas el duelo genuino.";
            pScript.colorIndicador = new Color(0.85f, 0.35f, 0.85f);
        }

        // 3. Punto Mascota Toby (Fase 3: Reconstruir)
        if (!tieneToby)
        {
            GameObject puntoToby = new GameObject("PuntoInteres_Toby");
            puntoToby.transform.position = posJugador + new Vector3(8f, 0f, -25f);
            PuntoDeInteres pScript = puntoToby.AddComponent<PuntoDeInteres>();
            pScript.idPunto = "toby";
            pScript.faseRequerida = GriefPhase.Reconstruir;
            pScript.nombreZona = "Sótano del Refugio Viejo (Toby)";
            pScript.tituloDescubrimiento = "¡Toby está vivo! No estamos solos";
            pScript.narrativaDescubrimiento = "¡Un ladrido débil! Levanto las maderas y ahí estás, Toby. Sucio de ceniza pero batiendo la cola al verme. El primer rayo de sol ilumina el cielo y los supervivientes se acercan con ayuda.";
            pScript.reflexionPsicologica = "Reflexión: La esperanza florece. Afrontas las pérdidas sin olvidarlas, pero eliges vivir y reconstruir junto a quienes te acompañan.";
            pScript.colorIndicador = new Color(0.3f, 0.9f, 0.5f);
        }
    }

    public void IniciarFase(GriefPhase fase, string hora, string objetivo)
    {
        FaseActual = fase;
        HoraActual = hora;

        Color colorFase = Color.red;
        if (fase == GriefPhase.Aceptar) colorFase = new Color(0.85f, 0.45f, 0.85f);
        if (fase == GriefPhase.Reconstruir) colorFase = new Color(0.3f, 0.9f, 0.5f);

        if (atmosphereManager != null) atmosphereManager.TransicionarAFase(fase);
        if (linternaJugador != null) linternaJugador.AjustarPorFase(fase);
        if (uiManager != null) uiManager.ActualizarHUD(hora, $"Fase {((int)fase + 1)}: {fase}", colorFase, objetivo);
    }

    public void ProcesarPuntoDescubierto(PuntoDeInteres punto)
    {
        // Aplicar vibración de cámara/pantalla sutil si existe
        if (Camera.main != null)
        {
            StartCoroutine(EfectoSacudidaCamara(0.4f, 0.3f));
        }

        // Reproducir sonido según fase
        if (DisasterAudioSystem.Instance != null)
        {
            if (punto.faseRequerida == GriefPhase.Reconstruir)
            {
                DisasterAudioSystem.Instance.ReproducirAcordeEsperanza();
            }
            else
            {
                DisasterAudioSystem.Instance.ReproducirAcordeTragico();
            }
        }

        // Mostrar Modal
        if (uiManager != null)
        {
            uiManager.MostrarModalNarrativo(punto);
        }
    }

    public void AvanzarFaseDespuesDeDescubrimiento(PuntoDeInteres punto)
    {
        if (punto.idPunto == "lucas")
        {
            IniciarFase(GriefPhase.Aceptar, "05:30 AM", "Busca a tu Madre cerca del Edificio Comunitario al este");
        }
        else if (punto.idPunto == "madre")
        {
            IniciarFase(GriefPhase.Reconstruir, "06:30 AM", "Sigue los ladridos al sur para encontrar a tu mascota Toby");
        }
        else if (punto.idPunto == "toby")
        {
            if (uiManager != null) uiManager.MostrarEpilogo();
        }
    }

    private System.Collections.IEnumerator EfectoSacudidaCamara(float duracion, float magnitud)
    {
        Vector3 posOriginal = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;

            Camera.main.transform.localPosition = new Vector3(posOriginal.x + x, posOriginal.y + y, posOriginal.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = posOriginal;
    }
}
