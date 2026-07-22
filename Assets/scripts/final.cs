using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena
using UnityEngine.InputSystem;    // Necesario para el Nuevo Input System

public class final : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Escribe el nombre exacto de la escena a la que quieres ir.")]
    public string nombreEscenaDestino = "New Scene";

    // Se ejecuta una vez por frame
    void Update()
    {
        // Verifica si hay un teclado conectado y si se presionó Enter en este frame
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            CargarEscenaPersonalizada();
        }
    }

    // Método para cargar la escena
    public void CargarEscenaPersonalizada()
    {
        // Comprobamos que no hayamos dejado el nombre vacío
        if (!string.IsNullOrEmpty(nombreEscenaDestino))
        {
            // Carga la escena por su nombre
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.LogError("No has puesto el nombre de la escena de destino en el objeto: " + gameObject.name);
        }
    }
}