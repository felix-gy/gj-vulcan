using UnityEngine;

public class MostradorEscenaFinal : MonoBehaviour
{
    [System.Serializable]
    public struct PersonajeFinal
    {
        public string personajeID; // ID coincidente (ej: mama, hermano)
        public GameObject objetoEnPantalla; // La imagen 2D o modelo 3D que está en el fondo negro
    }

    [Header("Lista de Personajes en la Escena Final")]
    public PersonajeFinal[] personajesEnEscena;

    void Start()
    {
        // Al cargar la escena final, revisa la lista global
        foreach (var p in personajesEnEscena)
        {
            if (p.objetoEnPantalla != null)
            {
                // Si el ID está guardado en el registro, activa la imagen/modelo
                bool fueEncontrado = RegistroPersonajes.personajesEncontrados.Contains(p.personajeID);
                p.objetoEnPantalla.SetActive(fueEncontrado);
            }
        }
    }
}