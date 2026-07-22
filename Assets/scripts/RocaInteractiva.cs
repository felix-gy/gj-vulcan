using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro en la UI
using UnityEngine.InputSystem; // Necesario para el nuevo Input System

public class RocaInteractiva : MonoBehaviour
{
    [Header("Conexión con la UI")]
    public GameObject panelDialogo; // Arrastra aquí tu PanelDialogo
    public TextMeshProUGUI textoDialogo; // Arrastra aquí tu TextoDialogo

    [Header("Contenido del Diálogo")]
    [TextArea(3, 5)]
    public string mensaje = "¡Parece que hay algo grabado en esta roca...! Es una pista.";

    private bool jugadorCerca = false;
    private bool dialogoAbierto = false;

    void Update()
    {
        // Si el jugador está cerca y presiona la tecla E (Nuevo Input System)...
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
            // Ponemos el texto en la UI, encendemos el panel y pausamos el juego
            textoDialogo.text = mensaje;
            panelDialogo.SetActive(true);
           // Time.timeScale = 0f; // Pausa el movimiento mientras lees
        }
        else
        {
            // Apagamos el panel y reanudamos el juego
            panelDialogo.SetActive(false);
            //Time.timeScale = 1f;
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

            // Si el jugador se aleja (por error), nos aseguramos de cerrar la ventana
            if (dialogoAbierto)
            {
                dialogoAbierto = false;
                panelDialogo.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}
