using UnityEngine;
using UnityEngine.InputSystem;

public class LinternaJugador : MonoBehaviour
{
    [Header("Configuración de Linterna")]
    public Light linternaSpot;
    public Light luzAmbienteJugador;
    public bool encendida = true;
    public KeyCode teclaLinterna = KeyCode.F;

    [Header("Foco y Dirección")]
    public Transform pivotLinterna;
    public float velocidadRotacion = 10f;
    private Vector3 direccionActual = Vector3.forward;

    [Header("Efecto de Parpadeo Orgánico")]
    public bool efectoParpadeo = true;
    public float intensidadBaseSpot = 4f;
    public float anguloSpot = 45f;
    public float alcanceSpot = 18f;

    void Awake()
    {
        AsegurarComponentesLuz();
    }

    void Start()
    {
        ActualizarEstadoLuz();
    }

    void Update()
    {
        // Alternar linterna con F (Input System)
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleLinterna();
        }

        // Orientar linterna según dirección del movimiento del jugador
        ActualizarDireccionLinterna();

        // Aplicar parpadeo sutil en la oscuridad
        if (encendida && efectoParpadeo && linternaSpot != null)
        {
            float ruido = Mathf.PerlinNoise(Time.time * 8f, 0f) * 0.4f;
            linternaSpot.intensity = intensidadBaseSpot + (ruido - 0.2f);
        }
    }

    private void AsegurarComponentesLuz()
    {
        // Si no se asignó un Pivot, usar este transform
        if (pivotLinterna == null)
        {
            GameObject pivotGO = new GameObject("PivotLinterna");
            pivotGO.transform.SetParent(transform);
            pivotGO.transform.localPosition = new Vector3(0f, 1f, 0f);
            pivotLinterna = pivotGO.transform;
        }

        // Si no se asignó la luz Spot, crearla automáticamente
        if (linternaSpot == null)
        {
            GameObject spotGO = new GameObject("LuzSpotLinterna");
            spotGO.transform.SetParent(pivotLinterna);
            spotGO.transform.localPosition = Vector3.zero;
            spotGO.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);

            linternaSpot = spotGO.AddComponent<Light>();
            linternaSpot.type = LightType.Spot;
            linternaSpot.range = alcanceSpot;
            linternaSpot.spotAngle = anguloSpot;
            linternaSpot.intensity = intensidadBaseSpot;
            linternaSpot.color = new Color(1f, 0.92f, 0.75f); // Cálido amarillento linterna
            linternaSpot.shadows = LightShadows.Soft;
        }

        // Si no se asignó la luz ambiente del jugador, crear un PointLight sutil
        if (luzAmbienteJugador == null)
        {
            GameObject pointGO = new GameObject("LuzAmbienteJugador");
            pointGO.transform.SetParent(transform);
            pointGO.transform.localPosition = new Vector3(0f, 1f, 0f);

            luzAmbienteJugador = pointGO.AddComponent<Light>();
            luzAmbienteJugador.type = LightType.Point;
            luzAmbienteJugador.range = 3.5f;
            luzAmbienteJugador.intensity = 0.8f;
            luzAmbienteJugador.color = new Color(0.8f, 0.85f, 1f); // Azul sutil tenue
        }
    }

    public void OrientarHacia(Vector3 direccionMovimiento)
    {
        if (direccionMovimiento.sqrMagnitude > 0.01f)
        {
            // Solo tomar en cuenta el plano XZ para 2.5D
            direccionActual = new Vector3(direccionMovimiento.x, 0f, direccionMovimiento.z).normalized;
        }
    }

    private void ActualizarDireccionLinterna()
    {
        if (pivotLinterna == null || direccionActual == Vector3.zero) return;

        Quaternion rotacionDeseada = Quaternion.LookRotation(direccionActual, Vector3.up);
        // Inclinar ligeramente hacia abajo para iluminar el suelo y escombros
        rotacionDeseada *= Quaternion.Euler(12f, 0f, 0f);

        pivotLinterna.rotation = Quaternion.Slerp(pivotLinterna.rotation, rotacionDeseada, Time.deltaTime * velocidadRotacion);
    }

    public void ToggleLinterna()
    {
        encendida = !encendida;
        ActualizarEstadoLuz();
    }

    public void SetLinternaEstado(bool activar)
    {
        encendida = activar;
        ActualizarEstadoLuz();
    }

    private void ActualizarEstadoLuz()
    {
        if (linternaSpot != null)
        {
            linternaSpot.enabled = encendida;
        }
    }

    public void AjustarPorFase(GriefPhase fase)
    {
        if (linternaSpot == null) return;

        switch (fase)
        {
            case GriefPhase.Procesar:
                intensidadBaseSpot = 6.0f;
                linternaSpot.spotAngle = 35f; // Cono enfocado de linterna
                linternaSpot.range = 12f;     // Alcance enfocado en la noche negra
                if (luzAmbienteJugador)
                {
                    luzAmbienteJugador.range = 1.5f;
                    luzAmbienteJugador.intensity = 0.1f; // Mínimo brillo para que la linterna sea la única luz
                }
                break;

            case GriefPhase.Aceptar:
                intensidadBaseSpot = 4.0f;
                linternaSpot.spotAngle = 50f;
                linternaSpot.range = 18f;
                if (luzAmbienteJugador)
                {
                    luzAmbienteJugador.range = 3.5f;
                    luzAmbienteJugador.intensity = 0.8f;
                }
                break;

            case GriefPhase.Reconstruir:
                intensidadBaseSpot = 2.5f;
                linternaSpot.spotAngle = 70f;
                linternaSpot.range = 26f;
                if (luzAmbienteJugador)
                {
                    luzAmbienteJugador.range = 5.0f;
                    luzAmbienteJugador.intensity = 1.4f;
                }
                break;
        }
    }
}
