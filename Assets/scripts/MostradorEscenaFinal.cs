using UnityEngine;
using System.Collections.Generic;

public class MostrarCombinacionFinal : MonoBehaviour
{
    [System.Serializable]
    public class Combinacion
    {
        public string nombreInformativo;      // Solo para ordenarte (ej: "Prota + Papá + Mamá")
        public List<string> idsRequeridos;     // Los IDs necesarios (ej: "papa", "mama")
        public GameObject imagenCombinacion;  // La UI/Objeto con la imagen de esta combinación
    }

    [Header("Imagen cuando NO rescataste a nadie")]
    public GameObject imagenProtaSola; // La foto de la prota en soledad

    [Header("Lista de Combinaciones Posibles")]
    public List<Combinacion> combinaciones;

    void Start()
    {
        // 1. Apagamos todas las imágenes primero para evitar superposiciones
        ApagarTodasLasImagenes();

        // 2. Buscamos la combinación que coincida exactamente con la memoria
        GameObject imagenAActivar = ObtenerImagenCorrespondiente();

        // 3. Activamos la foto correcta
        if (imagenAActivar != null)
        {
            imagenAActivar.SetActive(true);
        }
        else if (imagenProtaSola != null)
        {
            // Si no encontró combinación (o rescataste a 0 personas), muestra a la prota sola
            imagenProtaSola.SetActive(true);
        }
    }

    void ApagarTodasLasImagenes()
    {
        if (imagenProtaSola != null) 
            imagenProtaSola.SetActive(false);

        foreach (var c in combinaciones)
        {
            if (c.imagenCombinacion != null)
                c.imagenCombinacion.SetActive(false);
        }
    }

    GameObject ObtenerImagenCorrespondiente()
    {
        var rescatados = RegistroPersonajes.personajesEncontrados;

        // Si no rescataste a nadie, devuelve directo a la prota sola
        if (rescatados == null || rescatados.Count == 0) 
            return imagenProtaSola;

        // Recorremos las combinaciones buscando coincidencia exacta
        foreach (var c in combinaciones)
        {
            if (EsCoincidenciaExacta(c.idsRequeridos, rescatados))
            {
                return c.imagenCombinacion;
            }
        }

        return null; // Si no hay coincidencia exacta
    }

    bool EsCoincidenciaExacta(List<string> requeridos, HashSet<string> rescatados)
    {
        // Si la cantidad de personas rescatadas no coincide con esta combinación, no es
        if (requeridos.Count != rescatados.Count) 
            return false;

        // Comprobamos que cada ID requerido esté guardado en la memoria
        foreach (string id in requeridos)
        {
            if (!rescatados.Contains(id))
                return false;
        }

        return true;
    }
}