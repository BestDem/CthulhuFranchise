using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PoliceZone : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("Опционально. Полупрозрачный круг зоны полиции.")]
    [SerializeField] private SpriteRenderer zoneRenderer;
    [Range(0f, 1f)]
    [SerializeField] private float visibleAlpha = 0.25f;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        zoneRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (zoneRenderer == null)
            zoneRenderer = GetComponent<SpriteRenderer>();

        ApplyVisualAlpha();
    }

    private void ApplyVisualAlpha()
    {
        if (zoneRenderer == null)
            return;

        Color c = zoneRenderer.color;
        c.a = visibleAlpha;
        zoneRenderer.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
            human.SetPoliceZone(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
            human.SetPoliceZone(false);
    }
}
