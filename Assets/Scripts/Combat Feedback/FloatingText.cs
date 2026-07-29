using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 0.8f;

    private TextMeshPro textMesh;
    private float elapsed;
    private Color baseColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Init(string text, Color color)
    {
        textMesh.text = text;
        baseColor = color;
        textMesh.color = color;
        textMesh.enableWordWrapping = false;
        textMesh.overflowMode = TextOverflowModes.Overflow;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        elapsed += Time.deltaTime;

        float t = elapsed / lifetime;
        Color c = baseColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        textMesh.color = c;

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}