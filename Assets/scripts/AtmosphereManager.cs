using System.Collections;
using UnityEngine;

public class AtmosphereManager : MonoBehaviour
{
    public static AtmosphereManager Instance { get; private set; }

    [Header("Luces Principales")]
    public Light luzDireccional; // El Sol/Luna principal en la escena
    public Camera camaraPrincipal;

    [Header("Configuración Fase 1: Procesar (02:00 AM)")]
    public Color colorAmbienteFase1 = new Color(0.12f, 0.13f, 0.18f); // Luz nocturna azul tenue
    public Color colorLuzSolFase1 = new Color(0.55f, 0.65f, 0.90f);
    public float intensidadSolFase1 = 0.30f;
    public Color colorNieblaFase1 = new Color(0.07f, 0.08f, 0.12f);
    public Color colorFondoCamaraFase1 = new Color(0.05f, 0.06f, 0.10f);
    public float densidadNieblaFase1 = 0.02f;

    [Header("Configuración Fase 2: Aceptar (03:30 AM)")]

    public Color colorAmbienteFase2 = new Color(0.16f, 0.18f, 0.24f);
    public Color colorLuzSolFase2 = new Color(0.65f, 0.72f, 0.90f);
    public float intensidadSolFase2 = 0.38f;

    public Color colorNieblaFase2 = new Color(0.11f, 0.13f, 0.20f);
    public Color colorFondoCamaraFase2 = new Color(0.10f, 0.14f, 0.24f);
    public float densidadNieblaFase2 = 0.016f;

    [Header("Configuración Fase 3: Reconstruir (04:30 AM)")]

    public Color colorAmbienteFase3 = new Color(0.28f, 0.30f, 0.34f);
    public Color colorLuzSolFase3 = new Color(0.95f, 0.82f, 0.70f);
    public float intensidadSolFase3 = 0.60f;

    public Color colorNieblaFase3 = new Color(0.22f, 0.24f, 0.28f);
    public Color colorFondoCamaraFase3 = new Color(0.22f, 0.30f, 0.42f);
    public float densidadNieblaFase3 = 0.010f;

    [Header("Transición")]
    public float duracionTransicion = 3.5f;

    private Coroutine corrutinaTransicion;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (camaraPrincipal == null) camaraPrincipal = Camera.main;
        if (luzDireccional == null)
        {
            Light[] luces = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var l in luces)
            {
                if (l.type == LightType.Directional)
                {
                    luzDireccional = l;
                    break;
                }
            }
        }

        // Habilitar Modo Plano de Luz Ambiental (Sin Reflexiones de Skybox)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.reflectionIntensity = 0f;
        RenderSettings.skybox = null;

        // Habilitar Niebla Oscura de escena
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;

        if (camaraPrincipal != null)
        {
            camaraPrincipal.clearFlags = CameraClearFlags.SolidColor;
            camaraPrincipal.backgroundColor = Color.black;
        }

        // Inicializar en Fase 1
        AplicarFaseInstantanea(GriefPhase.Procesar);
    }

    public void TransicionarAFase(GriefPhase fase)
    {
        if (corrutinaTransicion != null) StopCoroutine(corrutinaTransicion);

        Color targetAmbiente, targetSolColor, targetNieblaColor, targetCamFondo;
        float targetSolIntensidad, targetNieblaDensidad;

        ObtenerValoresFase(fase, out targetAmbiente, out targetSolColor, out targetSolIntensidad, out targetNieblaColor, out targetCamFondo, out targetNieblaDensidad);

        corrutinaTransicion = StartCoroutine(RutinaTransicion(targetAmbiente, targetSolColor, targetSolIntensidad, targetNieblaColor, targetCamFondo, targetNieblaDensidad));
    }

    public void AplicarFaseInstantanea(GriefPhase fase)
    {
        Color targetAmbiente, targetSolColor, targetNieblaColor, targetCamFondo;
        float targetSolIntensidad, targetNieblaDensidad;

        ObtenerValoresFase(fase, out targetAmbiente, out targetSolColor, out targetSolIntensidad, out targetNieblaColor, out targetCamFondo, out targetNieblaDensidad);

        RenderSettings.ambientLight = targetAmbiente;
        RenderSettings.fogColor = targetNieblaColor;
        RenderSettings.fogDensity = targetNieblaDensidad;

        if (luzDireccional != null)
        {
            luzDireccional.color = targetSolColor;
            luzDireccional.intensity = targetSolIntensidad;
            if (targetSolIntensidad <= 0.01f) luzDireccional.enabled = false;
            else luzDireccional.enabled = true;
        }

        if (camaraPrincipal != null)
        {
            camaraPrincipal.clearFlags = CameraClearFlags.SolidColor;
            camaraPrincipal.backgroundColor = targetCamFondo;
        }
    }

    private void ObtenerValoresFase(GriefPhase fase, out Color ambiente, out Color solColor, out float solIntensidad, out Color nieblaColor, out Color camFondo, out float nieblaDensidad)
    {
        switch (fase)
        {
            case GriefPhase.Aceptar:
                ambiente = colorAmbienteFase2;
                solColor = colorLuzSolFase2;
                solIntensidad = intensidadSolFase2;
                nieblaColor = colorNieblaFase2;
                camFondo = colorFondoCamaraFase2;
                nieblaDensidad = densidadNieblaFase2;
                break;

            case GriefPhase.Reconstruir:
                ambiente = colorAmbienteFase3;
                solColor = colorLuzSolFase3;
                solIntensidad = intensidadSolFase3;
                nieblaColor = colorNieblaFase3;
                camFondo = colorFondoCamaraFase3;
                nieblaDensidad = densidadNieblaFase3;
                break;

            case GriefPhase.Procesar:
            default:
                ambiente = colorAmbienteFase1;
                solColor = colorLuzSolFase1;
                solIntensidad = intensidadSolFase1;
                nieblaColor = colorNieblaFase1;
                camFondo = colorFondoCamaraFase1;
                nieblaDensidad = densidadNieblaFase1;
                break;
        }
    }

    private IEnumerator RutinaTransicion(Color targetAmbiente, Color targetSolColor, float targetSolIntensidad, Color targetNieblaColor, Color targetCamFondo, float targetNieblaDensidad)
    {
        Color startAmbiente = RenderSettings.ambientLight;
        Color startSolColor = luzDireccional != null ? luzDireccional.color : Color.black;
        float startSolIntensidad = luzDireccional != null ? luzDireccional.intensity : 0f;
        Color startNieblaColor = RenderSettings.fogColor;
        float startNieblaDensidad = RenderSettings.fogDensity;
        Color startCamFondo = camaraPrincipal != null ? camaraPrincipal.backgroundColor : Color.black;

        float tiempo = 0f;
        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tiempo / duracionTransicion);

            RenderSettings.ambientLight = Color.Lerp(startAmbiente, targetAmbiente, t);
            RenderSettings.fogColor = Color.Lerp(startNieblaColor, targetNieblaColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(startNieblaDensidad, targetNieblaDensidad, t);

            if (luzDireccional != null)
            {
                luzDireccional.color = Color.Lerp(startSolColor, targetSolColor, t);
                luzDireccional.intensity = Mathf.Lerp(startSolIntensidad, targetSolIntensidad, t);
            }

            if (camaraPrincipal != null)
            {
                camaraPrincipal.backgroundColor = Color.Lerp(startCamFondo, targetCamFondo, t);
            }

            yield return null;
        }

        AplicarValoresFinales(targetAmbiente, targetSolColor, targetSolIntensidad, targetNieblaColor, targetCamFondo, targetNieblaDensidad);
    }

    private void AplicarValoresFinales(Color ambiente, Color solColor, float solIntensidad, Color nieblaColor, Color camFondo, float nieblaDensidad)
    {
        RenderSettings.ambientLight = ambiente;
        RenderSettings.fogColor = nieblaColor;
        RenderSettings.fogDensity = nieblaDensidad;
        if (luzDireccional != null)
        {
            luzDireccional.color = solColor;
            luzDireccional.intensity = solIntensidad;
        }
        if (camaraPrincipal != null)
        {
            camaraPrincipal.backgroundColor = camFondo;
        }
    }
}
