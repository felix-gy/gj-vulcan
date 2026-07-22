using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro en la UI
using UnityEngine.UI; // Necesario para manipular componentes de Imagen en UI
using UnityEngine.InputSystem; // Necesario para el nuevo Input System

public class PersonajeInteraccion : MonoBehaviour
{
    [Header("Conexión con la UI")]
    public GameObject panelDialogo; // Arrastra aquí tu PanelDialogo
    public TextMeshProUGUI textoDialogo; // Arrastra aquí tu TextoDialogo
    public Image imagenUI; // Arrastra aquí el componente Image de tu PanelDialogo

    [Header("Contenido del Diálogo")]
    public Sprite imagenPersonalizada; // Arrastra aquí la foto/sprite propia de ESTA roca

    [Header("Personaje Escondido")]
    public GameObject personaje_a_revelar; // Arrastra aquí el personaje que quieres revelar


    [TextArea(3, 5)]
    public string mensaje = "¡Parece que hay algo grabado en esta roca...! Es una pista.";

    private bool jugadorCerca = false;
    private bool dialogoAbierto = false;
    private bool yaFueUsado = false; // ¡NUEVO! Controla que solo se use una vez

    void Update()
    {
        // Solo permite interactuar si NO ha sido usado antes
        if (!yaFueUsado && jugadorCerca && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            AlternarDialogo();
        }
    }

    void AlternarDialogo()
    {
        dialogoAbierto = !dialogoAbierto;

        if (dialogoAbierto)
        {
            // 1. Asignamos el texto
            textoDialogo.text = mensaje;

            // 2. Asignamos la imagen si la UI y la foto existen
            if (imagenUI != null)
            {
                if (imagenPersonalizada != null)
                {
                    imagenUI.sprite = imagenPersonalizada;
                    imagenUI.gameObject.SetActive(true); // La mostramos
                }
                else
                {
                    // Si esta roca en particular no tiene foto, ocultamos el cuadro de imagen
                    imagenUI.gameObject.SetActive(false); 
                }
            }

            // 3. Encendemos el panel
            panelDialogo.SetActive(true);
            // Time.timeScale = 0f; // Descomenta si deseas pausar el juego
        }
        else
        {
            // Apagamos el panel y bloqueamos la interacción para siempre
            panelDialogo.SetActive(false);
            personaje_a_revelar.SetActive(true);
            yaFueUsado = true; // ¡NUEVO! Marca el objeto como completado
            // Time.timeScale = 1f;
        }
    }

    // Se activa cuando el jugador entra al área de la roca
    private void OnTriggerEnter(Collider other)
    {
        if (!yaFueUsado && other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Presiona Enter para inspeccionar la roca.");
        }
    }

    // Se activa cuando el jugador se aleja de la roca
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            // Si el jugador se aleja mientras leía, cerramos el panel y consumimos el uso
            if (dialogoAbierto)
            {
                dialogoAbierto = false;
                panelDialogo.SetActive(false);
                personaje_a_revelar.SetActive(true);
                yaFueUsado = true; // ¡NUEVO!
                // Time.timeScale = 1f;
            }
        }
    }
}