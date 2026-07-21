using UnityEngine;

public class EfectoCenizaVolcanica : MonoBehaviour
{
    [Header("Configuración de Partículas de Ceniza")]
    public int cantidadParticulas = 300;
    public Vector3 areaGeneracion = new Vector3(30f, 15f, 30f);

    private ParticleSystem ps;

    void Start()
    {
        CrearSistemaParticulas();
    }

    private void CrearSistemaParticulas()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            ps = gameObject.AddComponent<ParticleSystem>();
        }

        var main = ps.main;
        main.maxParticles = cantidadParticulas;
        main.startLifetime = 8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.25f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.5f, 0.45f, 0.45f, 0.7f), // Ceniza gris
            new Color(0.95f, 0.35f, 0.15f, 0.9f) // Brasa incandescente roja
        );
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.05f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = areaGeneracion;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-1.2f, -0.2f);
        velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);

        var emission = ps.emission;
        emission.rateOverTime = 40f;

        // Asignar un Renderer con material por defecto si no lo tiene
        ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
        if (renderer != null && renderer.sharedMaterial == null)
        {
            Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.sharedMaterial = mat;
        }

        ps.Play();
    }

    void Update()
    {
        // Seguir la posición del jugador o cámara para mantener la ceniza a su alrededor
        Transform objetivo = Camera.main != null ? Camera.main.transform : transform;
        if (objetivo != null)
        {
            Vector3 pos = objetivo.position;
            pos.y += 5f; // Mantener elevado sobre la cámara
            transform.position = pos;
        }
    }
}
