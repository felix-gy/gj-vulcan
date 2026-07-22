using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro en la UI
using UnityEngine.UI; // Necesario para manipular componentes de Imagen en UI
using UnityEngine.InputSystem; // Necesario para el nuevo Input System

public class RocaInteractiva : MonoBehaviour
{
    [Header("Conexión con la UI de Diálogo")]
    public GameObject panelDialogo; // Panel grande donde sale la foto y la historia
    public TextMeshProUGUI textoDialogo; 
    public Image imagenUI; 

    [Header("UI de Indicador (Presiona Enter)")]
    public GameObject textoIndicador; // Un texto simple en pantalla que dice: "Presiona Enter"

    [Header("Efecto Visual en el Objeto (Opcional)")]
    public GameObject luzOEfectoPista; // Luz tenue o partículas en la roca que se apagan al usarla

    [Header("Contenido del Diálogo")]
    public Sprite imagenPersonalizada; 

    [TextArea(3, 5)]
    public string mensaje = "¡Parece que hay algo grabado en esta roca...! Es una pista.";

    private bool jugadorCerca = false;
    private bool dialogoAbierto = false;
    private bool yaFueUsado = false; 

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
            // Ocultamos el mensaje de "Presiona Enter" mientras leemos
            if (textoIndicador != null) textoIndicador.SetActive(false);

            // 1. Asignamos el texto
            textoDialogo.text = mensaje;

            // 2. Asignamos la imagen
            if (imagenUI != null)
            {
                if (imagenPersonalizada != null)
                {
                    imagenUI.sprite = imagenPersonalizada;
                    imagenUI.gameObject.SetActive(true);
                }
                else
                {
                    imagenUI.gameObject.SetActive(false); 
                }
            }

            // 3. Encendemos el panel principal
            panelDialogo.SetActive(true);
        }
        else
        {
            // Apagamos el panel y consumimos el objeto para siempre
            panelDialogo.SetActive(false);
            yaFueUsado = true;

            // Apagamos la luz/brasas de la roca para que el jugador sepa que ya la inspeccionó
            if (luzOEfectoPista != null) luzOEfectoPista.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!yaFueUsado && other.CompareTag("Player"))
        {
            jugadorCerca = true;
            
            // Muestra el aviso "Presiona Enter" en pantalla
            if (textoIndicador != null) textoIndicador.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            // Oculta el aviso "Presiona Enter"
            if (textoIndicador != null) textoIndicador.SetActive(false);

            // Si el jugador se aleja mientras leía, cerramos todo
            if (dialogoAbierto)
            {
                dialogoAbierto = false;
                panelDialogo.SetActive(false);
                yaFueUsado = true;

                if (luzOEfectoPista != null) luzOEfectoPista.SetActive(false);
            }
        }
    }
}