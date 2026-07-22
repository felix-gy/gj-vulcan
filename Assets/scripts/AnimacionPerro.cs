using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimacionPerro : MonoBehaviour
{
    [Header("Sprites de la Animación")]
    public Sprite[] sprites;

    [Header("Velocidad de Animación")]
    public float fps = 6f;

    private SpriteRenderer spriteRenderer;
    private int frameActual = 0;
    private float timer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sprites == null || sprites.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fps)
        {
            timer = 0f;
            frameActual = (frameActual + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[frameActual];
        }
    }
}
