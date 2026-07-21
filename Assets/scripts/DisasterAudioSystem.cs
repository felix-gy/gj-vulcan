using UnityEngine;

public class DisasterAudioSystem : MonoBehaviour
{
    public static DisasterAudioSystem Instance { get; private set; }

    private AudioSource audioSourceAmbiente;
    private AudioSource audioSourceEfectos;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        AsegurarAudioSources();
    }

    void Start()
    {
        GenerarYSonardeAmbiente();
    }

    private void AsegurarAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            audioSourceAmbiente = sources[0];
            audioSourceEfectos = sources[1];
        }
        else
        {
            audioSourceAmbiente = gameObject.AddComponent<AudioSource>();
            audioSourceEfectos = gameObject.AddComponent<AudioSource>();
        }

        audioSourceAmbiente.loop = true;
        audioSourceAmbiente.volume = 0.35f;

        audioSourceEfectos.loop = false;
        audioSourceEfectos.volume = 0.7f;
    }

    private void GenerarYSonardeAmbiente()
    {
        // Generar un clip de audio de zumbido volcánico retumbante de baja frecuencia
        int sampleRate = 44100;
        float duration = 4.0f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            // Ruido sutil y frecuencia de 45Hz de sismo
            float sinWave = Mathf.Sin(2f * Mathf.PI * 45f * t);
            float noise = (Random.value * 2f - 1f) * 0.15f;
            samples[i] = (sinWave * 0.6f + noise) * 0.25f;
        }

        AudioClip clipAmbiente = AudioClip.Create("VolcanicDrone", sampleCount, 1, sampleRate, false);
        clipAmbiente.SetData(samples, 0);

        audioSourceAmbiente.clip = clipAmbiente;
        audioSourceAmbiente.Play();
    }

    public void ReproducirAcordeTragico()
    {
        // Generar acorde menor trágico (A2 y C3)
        int sampleRate = 44100;
        float duration = 2.5f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float freq1 = 110.0f;   // La (A2)
        float freq2 = 130.81f;  // Do (C3) - acorde menor trágico

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Exp(-2.2f * t); // Decaimiento suave
            float wave1 = Mathf.Sin(2f * Mathf.PI * freq1 * t);
            float wave2 = Mathf.Sin(2f * Mathf.PI * freq2 * t);
            samples[i] = (wave1 + wave2) * 0.35f * envelope;
        }

        AudioClip clipTragico = AudioClip.Create("TragicDiscovery", sampleCount, 1, sampleRate, false);
        clipTragico.SetData(samples, 0);

        audioSourceEfectos.PlayOneShot(clipTragico);
    }

    public void ReproducirAcordeEsperanza()
    {
        // Generar secuencia de acorde mayor de esperanza (Sol Mayor: G3, B3, D4, G4)
        int sampleRate = 44100;
        float duration = 3.0f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float[] freqs = new float[] { 196.00f, 246.94f, 293.66f, 392.00f };

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Exp(-1.5f * t);
            float sum = 0f;

            for (int f = 0; f < freqs.Length; f++)
            {
                float delay = f * 0.12f;
                if (t >= delay)
                {
                    float tLocal = t - delay;
                    sum += Mathf.Sin(2f * Mathf.PI * freqs[f] * tLocal) * Mathf.Exp(-1.8f * tLocal);
                }
            }

            samples[i] = sum * 0.25f * envelope;
        }

        AudioClip clipEsperanza = AudioClip.Create("HopeChime", sampleCount, 1, sampleRate, false);
        clipEsperanza.SetData(samples, 0);

        audioSourceEfectos.PlayOneShot(clipEsperanza);
    }
}
