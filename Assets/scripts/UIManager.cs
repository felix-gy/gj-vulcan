using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Referencias HUD")]
    public Text textHora;
    public Text textFase;
    public Text textObjetivo;
    public Text textPromptInteraccion;
    public Text textEstadoLinterna;

    [Header("Modal de Descubrimiento Narrativo")]
    public GameObject modalContainer;
    public Text modalBadgeFase;
    public Text modalTitulo;
    public Text modalDescripcion;
    public Text modalReflexion;
    public Button btnModalContinuar;

    [Header("Pantalla Epílogo (Final amanecer)")]
    public GameObject epilogoContainer;
    public Text epilogoTitulo;
    public Text epilogoMensaje;
    public Button btnReiniciar;

    public PuntoDeInteres PuntoActualEnFoco { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AsegurarInterfazCanvas();
        OcultarModal();
        if (epilogoContainer != null) epilogoContainer.SetActive(false);

        // Desactivar elementos del HUD según requerimientos del usuario
        if (textHora != null) textHora.gameObject.SetActive(false);
        if (textFase != null) textFase.gameObject.SetActive(false);
        if (textObjetivo != null) textObjetivo.gameObject.SetActive(false);
        if (textEstadoLinterna != null) textEstadoLinterna.gameObject.SetActive(false);

        // Ocultar también el panel de fondo del HUD superior para limpiar la pantalla
        if (textHora != null && textHora.transform.parent != null && textHora.transform.parent.name == "HUD_TopPanel")
        {
            textHora.transform.parent.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Permitir avanzar el modal con ENTER, NumpadEnter, Espacio o E
        if (modalContainer != null && modalContainer.activeSelf)
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.enterKey.wasPressedThisFrame ||
                    Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                    Keyboard.current.spaceKey.wasPressedThisFrame ||
                    Keyboard.current.eKey.wasPressedThisFrame)
                {
                    OnBotonContinuarClick();
                }
            }
        }
    }

    public void ActualizarHUD(string hora, string faseNombre, Color faseColor, string objetivo)
    {
        if (textHora != null) textHora.text = hora;
        if (textFase != null)
        {
            textFase.text = faseNombre.ToUpper();
            textFase.color = faseColor;
        }
        if (textObjetivo != null) textObjetivo.text = objetivo;
    }

    public void ActualizarEstadoLinterna(bool encendida)
    {
        if (textEstadoLinterna != null)
        {
            textEstadoLinterna.text = encendida ? "LINTERNA [F]: ENCENDIDA" : "LINTERNA [F]: APAGADA";
            textEstadoLinterna.color = encendida ? new Color(1f, 0.9f, 0.5f) : new Color(0.6f, 0.6f, 0.6f);
        }
    }

    public void MostrarPromptInteraccion(bool mostrar, string mensaje)
    {
        if (textPromptInteraccion != null)
        {
            textPromptInteraccion.gameObject.SetActive(mostrar);
            textPromptInteraccion.text = mensaje;
        }
    }

    public void MostrarModalNarrativo(PuntoDeInteres punto)
    {
        PuntoActualEnFoco = punto;
        if (modalContainer == null) return;

        modalContainer.SetActive(true);

        if (modalBadgeFase != null)
        {
            modalBadgeFase.text = $"FASE DE DUELO: {punto.faseRequerida.ToString().ToUpper()}";
            modalBadgeFase.color = punto.colorIndicador;
        }
        if (modalTitulo != null) modalTitulo.text = punto.tituloDescubrimiento;
        if (modalDescripcion != null) modalDescripcion.text = $"\"{punto.narrativaDescubrimiento}\"";
        if (modalReflexion != null) modalReflexion.text = punto.reflexionPsicologica;

        if (btnModalContinuar != null)
        {
            btnModalContinuar.onClick.RemoveAllListeners();
            btnModalContinuar.onClick.AddListener(OnBotonContinuarClick);
        }
    }

    private void OnBotonContinuarClick()
    {
        OcultarModal();
        if (GameManager.Instance != null && PuntoActualEnFoco != null)
        {
            GameManager.Instance.AvanzarFaseDespuesDeDescubrimiento(PuntoActualEnFoco);
        }
        PuntoActualEnFoco = null;
    }

    public void OcultarModal()
    {
        if (modalContainer != null) modalContainer.SetActive(false);
    }

    public void MostrarEpilogo()
    {
        if (modalContainer != null) modalContainer.SetActive(false);
        if (epilogoContainer != null)
        {
            epilogoContainer.SetActive(true);
            if (btnReiniciar != null)
            {
                btnReiniciar.onClick.RemoveAllListeners();
                btnReiniciar.onClick.AddListener(() => {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                    );
                });
            }
        }
    }

    private void AsegurarInterfazCanvas()
    {
        // Asegurar que exista un EventSystem con el InputSystemUIInputModule para procesar clics de mouse/pantalla
        EventSystem es = FindFirstObjectByType<EventSystem>();
        if (es == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<InputSystemUIInputModule>();
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas_GameJamUI");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Crear Panel Top HUD si no existen referencias asignadas
        if (textHora == null || textFase == null || textObjetivo == null)
        {
            CrearHUDDinamicamente(canvas.transform);
        }

        if (modalContainer == null)
        {
            CrearModalDinamicamente(canvas.transform);
        }

        if (epilogoContainer == null)
        {
            CrearEpilogoDinamicamente(canvas.transform);
        }
    }

    private void CrearHUDDinamicamente(Transform parent)
    {
        // Panel superior HUD
        GameObject hudPanel = new GameObject("HUD_TopPanel", typeof(RectTransform), typeof(Image));
        hudPanel.transform.SetParent(parent, false);
        RectTransform rtPanel = hudPanel.GetComponent<RectTransform>();
        rtPanel.anchorMin = new Vector2(0f, 1f);
        rtPanel.anchorMax = new Vector2(1f, 1f);
        rtPanel.pivot = new Vector2(0.5f, 1f);
        rtPanel.sizeDelta = new Vector2(0f, 90f);
        rtPanel.anchoredPosition = Vector2.zero;

        Image imgPanel = hudPanel.GetComponent<Image>();
        imgPanel.color = new Color(0.03f, 0.03f, 0.05f, 0.85f);

        // Texto Hora
        GameObject goHora = new GameObject("TextHora", typeof(RectTransform), typeof(Text));
        goHora.transform.SetParent(hudPanel.transform, false);
        RectTransform rtHora = goHora.GetComponent<RectTransform>();
        rtHora.anchorMin = new Vector2(0.02f, 0.5f);
        rtHora.anchorMax = new Vector2(0.25f, 0.9f);
        textHora = goHora.GetComponent<Text>();
        textHora.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textHora.fontSize = 20;
        textHora.fontStyle = FontStyle.Bold;
        textHora.color = new Color(1f, 0.6f, 0.2f);
        textHora.text = "02:00 AM";

        // Texto Fase
        GameObject goFase = new GameObject("TextFase", typeof(RectTransform), typeof(Text));
        goFase.transform.SetParent(hudPanel.transform, false);
        RectTransform rtFase = goFase.GetComponent<RectTransform>();
        rtFase.anchorMin = new Vector2(0.02f, 0.1f);
        rtFase.anchorMax = new Vector2(0.35f, 0.5f);
        textFase = goFase.GetComponent<Text>();
        textFase.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textFase.fontSize = 15;
        textFase.fontStyle = FontStyle.Bold;
        textFase.color = new Color(0.9f, 0.3f, 0.3f);
        textFase.text = "FASE 1: PROCESAR";

        // Texto Objetivo
        GameObject goObj = new GameObject("TextObjetivo", typeof(RectTransform), typeof(Text));
        goObj.transform.SetParent(hudPanel.transform, false);
        RectTransform rtObj = goObj.GetComponent<RectTransform>();
        rtObj.anchorMin = new Vector2(0.4f, 0.2f);
        rtObj.anchorMax = new Vector2(0.98f, 0.8f);
        textObjetivo = goObj.GetComponent<Text>();
        textObjetivo.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textObjetivo.fontSize = 16;
        textObjetivo.alignment = TextAnchor.MiddleRight;
        textObjetivo.color = new Color(1f, 0.9f, 0.4f);
        textObjetivo.text = "Encuentra a tu hermano Lucas en las ruinas";

        // Texto Prompt Interacción (Centro abajo)
        GameObject goPrompt = new GameObject("TextPrompt", typeof(RectTransform), typeof(Text));
        goPrompt.transform.SetParent(parent, false);
        RectTransform rtPrompt = goPrompt.GetComponent<RectTransform>();
        rtPrompt.anchorMin = new Vector2(0.15f, 0.12f);
        rtPrompt.anchorMax = new Vector2(0.85f, 0.2f);
        textPromptInteraccion = goPrompt.GetComponent<Text>();
        textPromptInteraccion.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textPromptInteraccion.fontSize = 22;
        textPromptInteraccion.fontStyle = FontStyle.Bold;
        textPromptInteraccion.alignment = TextAnchor.MiddleCenter;
        textPromptInteraccion.color = Color.yellow;
        textPromptInteraccion.gameObject.SetActive(false);

        // Estado Linterna (Abajo izquierda)
        GameObject goLinterna = new GameObject("TextEstadoLinterna", typeof(RectTransform), typeof(Text));
        goLinterna.transform.SetParent(parent, false);
        RectTransform rtLinterna = goLinterna.GetComponent<RectTransform>();
        rtLinterna.anchorMin = new Vector2(0.02f, 0.02f);
        rtLinterna.anchorMax = new Vector2(0.35f, 0.08f);
        textEstadoLinterna = goLinterna.GetComponent<Text>();
        textEstadoLinterna.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textEstadoLinterna.fontSize = 15;
        textEstadoLinterna.color = new Color(1f, 0.9f, 0.5f);
        textEstadoLinterna.text = "LINTERNA [F]: ENCENDIDA";
    }

    private void CrearModalDinamicamente(Transform parent)
    {
        modalContainer = new GameObject("ModalNarrativo", typeof(RectTransform), typeof(Image));
        modalContainer.transform.SetParent(parent, false);
        RectTransform rtModal = modalContainer.GetComponent<RectTransform>();
        rtModal.anchorMin = new Vector2(0.12f, 0.12f);
        rtModal.anchorMax = new Vector2(0.88f, 0.88f);

        Image imgModal = modalContainer.GetComponent<Image>();
        imgModal.color = new Color(0.05f, 0.03f, 0.06f, 0.96f);

        // Title Box
        GameObject goTitle = new GameObject("ModalTitle", typeof(RectTransform), typeof(Text));
        goTitle.transform.SetParent(modalContainer.transform, false);
        RectTransform rtTitle = goTitle.GetComponent<RectTransform>();
        rtTitle.anchorMin = new Vector2(0.05f, 0.80f);
        rtTitle.anchorMax = new Vector2(0.95f, 0.95f);
        modalTitulo = goTitle.GetComponent<Text>();
        modalTitulo.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        modalTitulo.fontSize = 26;
        modalTitulo.fontStyle = FontStyle.Bold;
        modalTitulo.color = new Color(0.95f, 0.35f, 0.25f);
        modalTitulo.text = "Título Descubrimiento";

        // Badge Fase
        GameObject goBadge = new GameObject("ModalBadge", typeof(RectTransform), typeof(Text));
        goBadge.transform.SetParent(modalContainer.transform, false);
        RectTransform rtBadge = goBadge.GetComponent<RectTransform>();
        rtBadge.anchorMin = new Vector2(0.05f, 0.72f);
        rtBadge.anchorMax = new Vector2(0.95f, 0.80f);
        modalBadgeFase = goBadge.GetComponent<Text>();
        modalBadgeFase.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        modalBadgeFase.fontSize = 14;
        modalBadgeFase.fontStyle = FontStyle.Bold;
        modalBadgeFase.color = Color.red;

        // Descripcion Text
        GameObject goDesc = new GameObject("ModalDesc", typeof(RectTransform), typeof(Text));
        goDesc.transform.SetParent(modalContainer.transform, false);
        RectTransform rtDesc = goDesc.GetComponent<RectTransform>();
        rtDesc.anchorMin = new Vector2(0.05f, 0.35f);
        rtDesc.anchorMax = new Vector2(0.95f, 0.70f);
        modalDescripcion = goDesc.GetComponent<Text>();
        modalDescripcion.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        modalDescripcion.fontSize = 17;
        modalDescripcion.color = new Color(0.9f, 0.9f, 0.95f);
        modalDescripcion.alignment = TextAnchor.UpperLeft;

        // Reflection Text
        GameObject goRef = new GameObject("ModalReflexion", typeof(RectTransform), typeof(Text));
        goRef.transform.SetParent(modalContainer.transform, false);
        RectTransform rtRef = goRef.GetComponent<RectTransform>();
        rtRef.anchorMin = new Vector2(0.05f, 0.18f);
        rtRef.anchorMax = new Vector2(0.95f, 0.33f);
        modalReflexion = goRef.GetComponent<Text>();
        modalReflexion.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        modalReflexion.fontSize = 14;
        modalReflexion.fontStyle = FontStyle.Italic;
        modalReflexion.color = new Color(0.7f, 0.75f, 0.85f);

        // Boton Continuar
        GameObject goBtn = new GameObject("BtnContinuar", typeof(RectTransform), typeof(Image), typeof(Button));
        goBtn.transform.SetParent(modalContainer.transform, false);
        RectTransform rtBtn = goBtn.GetComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(0.3f, 0.03f);
        rtBtn.anchorMax = new Vector2(0.7f, 0.15f);

        Image imgBtn = goBtn.GetComponent<Image>();
        imgBtn.color = new Color(0.7f, 0.2f, 0.15f);

        btnModalContinuar = goBtn.GetComponent<Button>();

        GameObject goBtnText = new GameObject("TextBtn", typeof(RectTransform), typeof(Text));
        goBtnText.transform.SetParent(goBtn.transform, false);
        RectTransform rtBtnText = goBtnText.GetComponent<RectTransform>();
        rtBtnText.anchorMin = Vector2.zero;
        rtBtnText.anchorMax = Vector2.one;
        Text tBtn = goBtnText.GetComponent<Text>();
        tBtn.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tBtn.fontSize = 18;
        tBtn.fontStyle = FontStyle.Bold;
        tBtn.alignment = TextAnchor.MiddleCenter;
        tBtn.color = Color.white;
        tBtn.text = "ACEPTAR Y AVANZAR";
    }

    private void CrearEpilogoDinamicamente(Transform parent)
    {
        epilogoContainer = new GameObject("PantallaEpilogo", typeof(RectTransform), typeof(Image));
        epilogoContainer.transform.SetParent(parent, false);
        RectTransform rtEpilogo = epilogoContainer.GetComponent<RectTransform>();
        rtEpilogo.anchorMin = Vector2.zero;
        rtEpilogo.anchorMax = Vector2.one;

        Image imgEpilogo = epilogoContainer.GetComponent<Image>();
        imgEpilogo.color = new Color(0.08f, 0.06f, 0.04f, 0.98f);

        GameObject goTitle = new GameObject("EpilogoTitle", typeof(RectTransform), typeof(Text));
        goTitle.transform.SetParent(epilogoContainer.transform, false);
        RectTransform rtTitle = goTitle.GetComponent<RectTransform>();
        rtTitle.anchorMin = new Vector2(0.1f, 0.75f);
        rtTitle.anchorMax = new Vector2(0.9f, 0.9f);
        epilogoTitulo = goTitle.GetComponent<Text>();
        epilogoTitulo.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        epilogoTitulo.fontSize = 32;
        epilogoTitulo.fontStyle = FontStyle.Bold;
        epilogoTitulo.alignment = TextAnchor.MiddleCenter;
        epilogoTitulo.color = new Color(1f, 0.75f, 0.3f);
        epilogoTitulo.text = "EL AMANECER DE LA ESPERANZA";

        GameObject goMsg = new GameObject("EpilogoMsg", typeof(RectTransform), typeof(Text));
        goMsg.transform.SetParent(epilogoContainer.transform, false);
        RectTransform rtMsg = goMsg.GetComponent<RectTransform>();
        rtMsg.anchorMin = new Vector2(0.12f, 0.25f);
        rtMsg.anchorMax = new Vector2(0.88f, 0.70f);
        epilogoMensaje = goMsg.GetComponent<Text>();
        epilogoMensaje.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        epilogoMensaje.fontSize = 18;
        epilogoMensaje.alignment = TextAnchor.MiddleCenter;
        epilogoMensaje.color = new Color(0.95f, 0.9f, 0.8f);
        epilogoMensaje.text = "El manto asfixiante de la erupción comienza a aclararse cuando el sol se alza en el horizonte. Los rayos dorados tiñen de calidez la ceniza del suelo.\n\n" +
                              "Aunque has perdido pilares en tu vida, la presencia viva de tu leal mascota Toby y los supervivientes te recuerdan que sigues aquí.\n\n" +
                              "\"La reconstrucción no niega la tragedia. La abraza como cicatrices de un camino donde prometemos sobrevivir, recordar y volver a florecer.\"";

        GameObject goBtn = new GameObject("BtnReiniciar", typeof(RectTransform), typeof(Image), typeof(Button));
        goBtn.transform.SetParent(epilogoContainer.transform, false);
        RectTransform rtBtn = goBtn.GetComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(0.35f, 0.08f);
        rtBtn.anchorMax = new Vector2(0.65f, 0.18f);

        Image imgBtn = goBtn.GetComponent<Image>();
        imgBtn.color = new Color(0.2f, 0.6f, 0.35f);

        btnReiniciar = goBtn.GetComponent<Button>();

        GameObject goBtnText = new GameObject("TextBtnReiniciar", typeof(RectTransform), typeof(Text));
        goBtnText.transform.SetParent(goBtn.transform, false);
        RectTransform rtBtnText = goBtnText.GetComponent<RectTransform>();
        rtBtnText.anchorMin = Vector2.zero;
        rtBtnText.anchorMax = Vector2.one;
        Text tBtn = goBtnText.GetComponent<Text>();
        tBtn.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tBtn.fontSize = 18;
        tBtn.fontStyle = FontStyle.Bold;
        tBtn.alignment = TextAnchor.MiddleCenter;
        tBtn.color = Color.white;
        tBtn.text = "VOLVER A EMPEZAR";
    }
}
