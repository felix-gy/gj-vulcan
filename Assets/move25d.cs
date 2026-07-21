using UnityEngine;
using UnityEngine.InputSystem; // OBLIGATORIO: Librería del Input System

public class move25d : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody rb;
    private Vector3 movimiento;

    [Header("Referencia al Sprite")]
    public SpriteRenderer spriteRenderer;

    [Header("Componentes de Atmósfera y Linterna")]
    public LinternaJugador linterna;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Si no hay linterna asignada, buscarla o añadirla
        if (linterna == null) linterna = GetComponent<LinternaJugador>();
        if (linterna == null) linterna = gameObject.AddComponent<LinternaJugador>();
    }

    void Start()
    {
        // Asegurar que el GameManager exista en la escena
        if (GameManager.Instance == null)
        {
            GameObject gmGO = new GameObject("GameManager_AutoInit");
            gmGO.AddComponent<GameManager>();
        }
    }

    // Se ejecuta con el nuevo Input System (Acción Move)
    public void OnMove(InputValue value)
    {
        Vector2 input2D = value.Get<Vector2>();

        // Conversión a 2.5D: X al eje X, Y al eje Z (profundidad 3D)
        movimiento = new Vector3(input2D.x, 0f, input2D.y);
    }

    void Update()
    {
        // Girar el sprite según dirección horizontal
        if (movimiento.x < 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = true;  // Mira a la izquierda
        }
        else if (movimiento.x > 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = false; // Mira a la derecha
        }

        // Orientar la luz foco de la linterna según la dirección del movimiento
        if (linterna != null && movimiento.sqrMagnitude > 0.01f)
        {
            linterna.OrientarHacia(movimiento);
        }

        // Alternar linterna con F (Nuevo Input System)
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame && linterna != null)
        {
            linterna.ToggleLinterna();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ActualizarEstadoLinterna(linterna.encendida);
            }
        }
    }

    void FixedUpdate()
    {
        // Movimiento físico 3D
        if (rb != null)
        {
            rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
        }
    }
}