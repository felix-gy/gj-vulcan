using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro en la UI
using UnityEngine.UI; // ¡NUEVO! Necesario para manipular componentes de Imagen en UI
using UnityEngine.InputSystem; // Necesario para el nuevo Input System

public class RocaInteractiva : MonoBehaviour
{
    [Header("Conexión con la UI")]
    public GameObject panelDialogo; // Arrastra aquí tu PanelDialogo
    public TextMeshProUGUI textoDialogo; // Arrastra aquí tu TextoDialogo
    public Image imagenUI; // ¡NUEVO! Arrastra aquí el componente Image de tu PanelDialogo

    [Header("Contenido del Diálogo")]
    public Sprite imagenPersonalizada; // ¡NUEVO! Arrastra aquí la foto/sprite propia de ESTA roca

    [TextArea(3, 5)]
    public string mensaje = "¡Parece que hay algo grabado en esta roca...! Es una pista.";

    private bool jugadorCerca = false;
    private bool dialogoAbierto = false;

    void Update()
    {
        // Si el jugador está cerca y presiona la tecla Enter (Nuevo Input System)...
        if (jugadorCerca && Keyboard.current.enterKey.wasPressedThisFrame)
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
            // Apagamos el panel
            panelDialogo.SetActive(false);
            // Time.timeScale = 1f;
        }
    }

    // Se activa cuando el jugador entra al área de la roca
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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

            // Si el jugador se aleja, nos aseguramos de cerrar la ventana
            if (dialogoAbierto)
            {
                dialogoAbierto = false;
                panelDialogo.SetActive(false);
                // Time.timeScale = 1f;
            }
        }
    }
}