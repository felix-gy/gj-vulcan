using UnityEngine;

public enum GriefPhase
{
    Procesar,    // Fase 1: 02:00 AM - Shock, negación, noche oscura
    Aceptar,     // Fase 2: 05:30 AM - Dolor profundo, aceptación, inicio de luz gris
    Reconstruir  // Fase 3: 06:30 AM - Resiliencia, esperanza, amanecer dorado
}

[System.Serializable]
public class StoryPointData
{
    public string id;
    public GriefPhase phase;
    public string zoneName;
    public string discoveryTitle;
    [TextArea(3, 6)]
    public string description;
    [TextArea(2, 4)]
    public string psychologicalReflection;
    public Color themeColor;
}
