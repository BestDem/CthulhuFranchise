using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlyingFlyer : MonoBehaviour
{
    [Header("Shared flyer flight")]
    [Tooltip("Единая скорость для ВСЕХ листовок, потому что используется один общий prefab.")]
    [SerializeField] private float flySpeed = 36f;

    [SerializeField] private float hitDistance = 0.15f;

    [Header("Hit")]
    [Tooltip("Сколько секунд эта же листовка висит на лице после попадания.")]
    [SerializeField] private float faceStickTime = 0.18f;

    [Tooltip("Если включено, листовка исчезает сразу после попадания. Если выключено, она чуть висит на лице и потом удаляется.")]
    [SerializeField] private bool hideInstantlyOnHit = false;

    [Header("Optional fallback sprite. Used if database has empty sprite.")]
    [SerializeField] private Sprite flyerSpriteOverride;

    private SpriteRenderer spriteRenderer;
    private Human target;
    private string flyerId;
    private FlyerVisualDatabase visualDatabase;
    private AudioClip hitSound;
    private Action onHit;
    private bool hasHit;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Human newTarget, string newFlyerId, FlyerVisualDatabase newVisualDatabase, AudioClip newHitSound, Action newOnHit)
    {
        target = newTarget;
        flyerId = newFlyerId;
        visualDatabase = newVisualDatabase;
        hitSound = newHitSound;
        onHit = newOnHit;
        hasHit = false;

        Sprite flyerSprite = GetFlyerSprite();
        if (flyerSprite != null)
            spriteRenderer.sprite = flyerSprite;

        spriteRenderer.enabled = true;
    }

    private void Update()
    {
        if (hasHit)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target.GetFacePosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flySpeed * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (Vector3.Distance(transform.position, targetPosition) <= hitDistance)
            HitTarget();
    }

    private void HitTarget()
    {
        if (hasHit)
            return;

        hasHit = true;

        Vector3 facePosition = target != null ? target.GetFacePosition() : transform.position;
        transform.position = facePosition;

        if (hideInstantlyOnHit && spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (target != null)
        {
            Sprite bubbleSprite = visualDatabase != null ? visualDatabase.GetBubbleSprite(flyerId) : null;
            target.ReceiveFlyer(flyerId, bubbleSprite);
        }

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, facePosition);

        if (onHit != null)
            onHit.Invoke();

        Destroy(gameObject, Mathf.Max(0.01f, faceStickTime));
    }

    private Sprite GetFlyerSprite()
    {
        Sprite flyerSprite = null;

        if (visualDatabase != null)
            flyerSprite = visualDatabase.GetFlyerSprite(flyerId);

        if (flyerSprite == null)
            flyerSprite = flyerSpriteOverride;

        return flyerSprite;
    }
}
