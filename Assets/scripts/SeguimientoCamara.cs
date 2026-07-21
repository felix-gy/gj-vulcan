using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    [Header("Posición Isométrica")]
    public Vector3 offsetPosicion = new Vector3(0f, 4.5f, -3.5f);
    public float anguloInclinacion = 45f;
    public float fovDeseado = 35f;

    [Header("Efecto de Respiración")]
    public bool respiracionActiva = true;
    public float amplitudRespiracion = 0.06f;
    public float velocidadRespiracion = 0.8f;

    [Header("Balanceo al Moverse")]
    public bool swayActivo = true;
    public float amplitudSway = 0.08f;
    public float suavidadSway = 3f;

    private Camera cam;
    private Rigidbody rb;
    private Vector3 posicionBase;
    private float swayActual = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
        rb = GetComponentInParent<Rigidbody>();

        transform.localPosition = offsetPosicion;
        transform.localRotation = Quaternion.Euler(anguloInclinacion, 0f, 0f);

        if (cam != null)
        {
            if (cam.orthographic)
                cam.orthographicSize = 4f;
            else
                cam.fieldOfView = fovDeseado;
        }

        posicionBase = transform.localPosition;
    }

    void LateUpdate()
    {
        Vector3 offset = Vector3.zero;

        if (respiracionActiva)
        {
            float respiracion = Mathf.Sin(Time.time * velocidadRespiracion) * amplitudRespiracion;
            offset.y += respiracion;
        }

        if (swayActivo && rb != null)
        {
            float velocidadX = rb.linearVelocity.x;
            float swayObjetivo = velocidadX * amplitudSway;
            swayActual = Mathf.Lerp(swayActual, swayObjetivo, Time.deltaTime * suavidadSway);
            offset.x += swayActual;
        }

        transform.localPosition = posicionBase + offset;
    }
}
