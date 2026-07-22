using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuInicioManager : MonoBehaviour
{
    private Vector3 originalCameraPosition;
    private Camera mainCamera;
    private bool starting = false;

    [Header("Configuración del Temblor")]
    public float duracionTemblor = 1.5f;
    public float magnitudTemblor = 0.5f;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }

        CrearInterfazMenu();
    }

    void CrearInterfazMenu()
    {
        // 1. Crear Canvas si no existe
        GameObject canvasGO = new GameObject("MenuCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();

        // 2. Crear EventSystem si no existe en la escena para recibir clicks
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }



        // 4. Crear Título del Juego
        GameObject tituloGO = new GameObject("Titulo");
        tituloGO.transform.SetParent(canvasGO.transform, false);
        Text txtTitulo = tituloGO.AddComponent<Text>();
        txtTitulo.text = "VULCAN";
        
        // Obtener una fuente predeterminada con bloque try-catch para evitar excepciones de carga
        Font defaultFont = null;
        try
        {
            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch {}

        if (defaultFont == null)
        {
            try
            {
                defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            catch {}
        }
        txtTitulo.font = defaultFont;
        txtTitulo.fontSize = 120;
        txtTitulo.alignment = TextAnchor.MiddleCenter;
        txtTitulo.color = new Color(0.9f, 0.25f, 0.1f); // Rojo lava brillante

        // Agregar una sombra sutil al título
        Shadow shadow = tituloGO.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(5, -5);

        RectTransform tituloRect = tituloGO.GetComponent<RectTransform>();
        tituloRect.anchorMin = new Vector2(0.5f, 0.7f);
        tituloRect.anchorMax = new Vector2(0.5f, 0.7f);
        tituloRect.sizeDelta = new Vector2(800, 200);
        tituloRect.anchoredPosition = Vector2.zero;

        // 5. Crear Botón de Inicio
        DefaultControls.Resources uiResources = new DefaultControls.Resources();
        GameObject buttonGO = DefaultControls.CreateButton(uiResources);
        buttonGO.name = "BotonInicio";
        buttonGO.transform.SetParent(canvasGO.transform, false);

        // Estilizar el botón
        Image btnImage = buttonGO.GetComponent<Image>();
        btnImage.color = new Color(0.8f, 0.3f, 0.2f);
        
        Button button = buttonGO.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.8f, 0.3f, 0.2f);
        colors.highlightedColor = new Color(1f, 0.4f, 0.3f);
        colors.pressedColor = new Color(0.6f, 0.2f, 0.1f);
        colors.selectedColor = new Color(0.8f, 0.3f, 0.2f);
        button.colors = colors;

        // Posicionar el botón
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.4f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.4f);
        buttonRect.sizeDelta = new Vector2(300, 80);
        buttonRect.anchoredPosition = Vector2.zero;

        // Estilizar texto del botón
        Text btnText = buttonGO.GetComponentInChildren<Text>();
        if (btnText != null)
        {
            btnText.text = "INICIAR JUEGO";
            btnText.font = defaultFont;
            btnText.fontSize = 28;
            btnText.color = Color.white;
            btnText.fontStyle = FontStyle.Bold;
        }

        // Asignar el evento Click al botón
        button.onClick.AddListener(OnBotonInicioClick);
    }

    void OnBotonInicioClick()
    {
        if (starting) return;
        starting = true;
        StartCoroutine(SecuenciaInicio());
    }

    IEnumerator SecuenciaInicio()
    {
        float elapsed = 0f;

        // Temblor de cámara
        while (elapsed < duracionTemblor)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitudTemblor;
            float yOffset = Random.Range(-1f, 1f) * magnitudTemblor;

            if (mainCamera != null)
            {
                mainCamera.transform.position = originalCameraPosition + new Vector3(xOffset, yOffset, 0f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restaurar posición original de la cámara
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
        }

        // Cargar la escena principal
        SceneManager.LoadScene("SampleScene");
    }
}
