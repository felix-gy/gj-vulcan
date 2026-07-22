using System.Collections.Generic;

public static class RegistroPersonajes
{
    // Lista global que guarda los IDs de los personajes rescatados/hablados
    public static HashSet<string> personajesEncontrados = new HashSet<string>();

    // Método para limpiar el registro si el jugador vuelve a jugar desde el inicio
    public static void Reiniciar()
    {
        personajesEncontrados.Clear();
    }
}