using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Necesario para usar TextMeshPro (evita pixeleado de UI)

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

        // 3. Crear Título del Juego con TextMeshPro (SDF, no se pixelea)
        GameObject tituloGO = new GameObject("Titulo");
        tituloGO.transform.SetParent(canvasGO.transform, false);
        
        TextMeshProUGUI txtTitulo = tituloGO.AddComponent<TextMeshProUGUI>();
        txtTitulo.text = "VULCAN";
        txtTitulo.fontSize = 130;
        txtTitulo.alignment = TextAlignmentOptions.Center;
        txtTitulo.color = new Color(0.9f, 0.25f, 0.1f); // Rojo lava brillante
        
        // Activar sombra y outline en TMPro para darle aspecto premium
        txtTitulo.outlineColor = Color.black;
        txtTitulo.outlineWidth = 0.2f;

        RectTransform tituloRect = tituloGO.GetComponent<RectTransform>();
        tituloRect.anchorMin = new Vector2(0.5f, 0.7f);
        tituloRect.anchorMax = new Vector2(0.5f, 0.7f);
        tituloRect.sizeDelta = new Vector2(800, 200);
        tituloRect.anchoredPosition = Vector2.zero;

        // 4. Crear Botón de Inicio
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

        // Reemplazar texto legado del botón por TextMeshProUGUI (más nítido)
        Text legacyText = buttonGO.GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            GameObject textGO = legacyText.gameObject;
            DestroyImmediate(legacyText); // Eliminar el texto borroso legado

            TextMeshProUGUI btnText = textGO.AddComponent<TextMeshProUGUI>();
            btnText.text = "INICIAR JUEGO";
            btnText.fontSize = 28;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.white;
            btnText.fontStyle = FontStyles.Bold;
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
