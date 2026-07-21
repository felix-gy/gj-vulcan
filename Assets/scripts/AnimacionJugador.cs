using UnityEngine;
using UnityEngine.InputSystem;

public class AnimacionJugador : MonoBehaviour
{
    [Header("Sprites W - Arriba")]
    public Sprite[] spritesArriba;

    [Header("Sprites S - Abajo")]
    public Sprite[] spritesAbajo;

    [Header("Sprites A - Izquierda")]
    public Sprite[] spritesIzquierda;

    [Header("Sprites D - Derecha")]
    public Sprite[] spritesDerecha;

    [Header("Velocidad de Animación")]
    public float fps = 8f;

    private SpriteRenderer spriteRenderer;
    private Sprite[] spritesActuales;
    private int frameActual = 0;
    private float timer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Iniciamos mirando hacia abajo por defecto
        spritesActuales = spritesAbajo;
        AplicarFrame();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool w = Keyboard.current.wKey.isPressed;
        bool s = Keyboard.current.sKey.isPressed;
        bool a = Keyboard.current.aKey.isPressed;
        bool d = Keyboard.current.dKey.isPressed;

        // Comprobamos si el jugador se está moviendo
        bool enMovimiento = w || s || a || d;

        if (enMovimiento)
        {
            if (w) CambiarSprites(spritesArriba);
            else if (s) CambiarSprites(spritesAbajo);
            else if (a) CambiarSprites(spritesIzquierda);
            else if (d) CambiarSprites(spritesDerecha);

            // Solo avanzamos los frames si el personaje camina
            AvanzarFrame();
        }
        else
        {
            // AL SOLTAR LAS TECLAS:
            // Opcional: Si quieres que al detenerse vuelva a su postura inicial 
            // de esa misma dirección (el frame 0, con los pies juntos), descomenta la siguiente línea:
            // DetenerEnPrimerFrame();
            
            // Si lo dejas tal como está, se quedará quieto exactamente 
            // en el último frame en el que iba caminando.
        }
    }

    private void CambiarSprites(Sprite[] nuevosSprites)
    {
        if (spritesActuales != nuevosSprites)
        {
            spritesActuales = nuevosSprites;
            frameActual = 0;
            timer = 0f;
            AplicarFrame();
        }
    }

    private void AvanzarFrame()
    {
        if (spritesActuales == null || spritesActuales.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fps)
        {
            timer = 0f;
            frameActual = (frameActual + 1) % spritesActuales.Length;
            AplicarFrame();
        }
    }

    private void AplicarFrame()
    {
        if (spritesActuales == null || spritesActuales.Length == 0) return;
        if (frameActual >= spritesActuales.Length) frameActual = 0;
        if (spritesActuales[frameActual] != null)
            spriteRenderer.sprite = spritesActuales[frameActual];
    }

    // Función auxiliar por si prefieres que al soltar la tecla 
    // se quede mirando a esa dirección pero en postura de reposo (frame 0)
    private void DetenerEnPrimerFrame()
    {
        if (frameActual != 0)
        {
            frameActual = 0;
            AplicarFrame();
        }
    }
}