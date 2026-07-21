using UnityEngine;
using UnityEngine.InputSystem; // 1. OBLIGATORIO: Añadimos la librería del nuevo Input System

public class move25d : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody rb;
    private Vector3 movimiento;
    
    [Header("Referencia al Sprite")]
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 2. NUEVA FUNCIÓN: Se ejecuta SOLA cuando presionas WASD, flechas o el joystick
    // Debe llamarse exactamente "OnMove" porque el mapa de controles por defecto se llama "Move"
    public void OnMove(InputValue value)
    {
        // Extraemos los datos del control en 2D (X para izquierda/derecha, Y para arriba/abajo)
        Vector2 input2D = value.Get<Vector2>();

        // 3. LA CONVERSIÓN CLAVE A 2.5D: 
        // Pasamos el input.x al eje X, pero el input.y (arriba/abajo del teclado) al eje Z (profundidad 3D)
        movimiento = new Vector3(input2D.x, 0f, input2D.y);
    }

    void Update()
    {
        // Ya no leemos teclas aquí. Solo usamos el 'movimiento' guardado para girar el sprite
        if (movimiento.x < 0)
        {
            spriteRenderer.flipX = true;  // Mira a la izquierda
        }
        else if (movimiento.x > 0)
        {
            spriteRenderer.flipX = false; // Mira a la derecha
        }
    }

    void FixedUpdate()
    {
        // El movimiento físico en 3D se mantiene intacto
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);
    }
}