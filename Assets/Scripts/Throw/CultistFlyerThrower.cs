using System;
using System.Collections;
using UnityEngine;

public class CultistFlyerThrower : MonoBehaviour
{
    [Header("Visual switch")]
    [SerializeField] private GameObject idleSpriteObject;
    [SerializeField] private GameObject throwRigObject;
    [SerializeField] private Animator throwAnimator;
    [SerializeField] private string throwAnimationName = "Cultist_Throw";

    [Header("Animation Events")]
    [Tooltip("Включи true, если в анимации броска стоят события Anim_SpawnFlyerInHand, Anim_ReleaseFlyer, Anim_FinishThrow.")]
    [SerializeField] private bool useAnimationEvents = true;

    [Header("Fallback timing if Animation Events are OFF")]
    [SerializeField] private float releaseDelay = 0.25f;
    [SerializeField] private float throwDuration = 0.8f;

    [Header("Flyer")]
    [SerializeField] private Transform handPoint;
    [SerializeField] private SpriteRenderer handFlyerRenderer;

    [Tooltip("ЕДИНЫЙ prefab летящей листовки для всех типов. Скорость и размер настраиваются только на нём.")]
    [SerializeField] private FlyingFlyer sharedFlyingFlyerPrefab;

    [SerializeField] private FlyerVisualDatabase flyerVisualDatabase;
    [SerializeField] private AudioClip faceHitSound;

    private bool isThrowing;
    private bool flyerReleased;
    private Human pendingTarget;
    private string pendingFlyerId;
    private Action pendingOnHit;
    private Coroutine fallbackRoutine;

    public bool IsThrowing
    {
        get { return isThrowing; }
    }

    private void Start()
    {
        ShowIdle();
        HideHandFlyer();
    }

    public void ThrowFlyerTo(Human target, string flyerId, Action onHit)
    {
        if (isThrowing)
            return;

        if (target == null)
            return;

        pendingTarget = target;
        pendingFlyerId = flyerId;
        pendingOnHit = onHit;
        flyerReleased = false;
        isThrowing = true;

        ShowThrow();

        if (!useAnimationEvents)
        {
            fallbackRoutine = StartCoroutine(FallbackThrowRoutine());
        }
    }

    private IEnumerator FallbackThrowRoutine()
    {
        Anim_SpawnFlyerInHand();

        yield return new WaitForSeconds(releaseDelay);

        Anim_ReleaseFlyer();

        float remainingTime = Mathf.Max(0f, throwDuration - releaseDelay);
        yield return new WaitForSeconds(remainingTime);

        Anim_FinishThrow();
    }

    // Animation Event: поставить в начале броска, когда листовка должна появиться в руке.
    public void Anim_SpawnFlyerInHand()
    {
        if (!isThrowing)
            return;

        ShowHandFlyer(pendingFlyerId);
    }

    // Animation Event: поставить в кадре, когда рука отпускает листовку.
    public void Anim_ReleaseFlyer()
    {
        if (!isThrowing)
            return;

        if (flyerReleased)
            return;

        flyerReleased = true;
        ReleaseFlyer(pendingTarget, pendingFlyerId, pendingOnHit);
    }

    // Animation Event: поставить в конце броска.
    public void Anim_FinishThrow()
    {
        if (!isThrowing)
            return;

        if (fallbackRoutine != null)
        {
            StopCoroutine(fallbackRoutine);
            fallbackRoutine = null;
        }

        ShowIdle();
        HideHandFlyer();

        pendingTarget = null;
        pendingFlyerId = null;
        pendingOnHit = null;
        flyerReleased = false;
        isThrowing = false;
    }

    private void ShowIdle()
    {
        if (idleSpriteObject != null)
            idleSpriteObject.SetActive(true);

        if (throwRigObject != null)
            throwRigObject.SetActive(false);
    }

    private void ShowThrow()
    {
        if (idleSpriteObject != null)
            idleSpriteObject.SetActive(false);

        if (throwRigObject != null)
            throwRigObject.SetActive(true);

        if (throwAnimator != null && !string.IsNullOrEmpty(throwAnimationName))
            throwAnimator.Play(throwAnimationName, 0, 0f);
    }

    private void ShowHandFlyer(string flyerId)
    {
        if (handFlyerRenderer == null)
            return;

        Sprite flyerSprite = flyerVisualDatabase != null ? flyerVisualDatabase.GetFlyerSprite(flyerId) : null;

        if (flyerSprite != null)
            handFlyerRenderer.sprite = flyerSprite;

        handFlyerRenderer.enabled = true;
    }

    private void HideHandFlyer()
    {
        if (handFlyerRenderer != null)
            handFlyerRenderer.enabled = false;
    }

    private void ReleaseFlyer(Human target, string flyerId, Action onHit)
    {
        HideHandFlyer();

        if (target == null)
            return;

        if (sharedFlyingFlyerPrefab == null)
        {
            Sprite bubbleSprite = flyerVisualDatabase != null ? flyerVisualDatabase.GetBubbleSprite(flyerId) : null;
            target.ReceiveFlyer(flyerId, bubbleSprite);

            if (faceHitSound != null)
                AudioSource.PlayClipAtPoint(faceHitSound, target.GetFacePosition());

            if (onHit != null)
                onHit.Invoke();

            return;
        }

        Vector3 spawnPosition = handPoint != null ? handPoint.position : transform.position;
        FlyingFlyer flyingFlyer = Instantiate(sharedFlyingFlyerPrefab, spawnPosition, Quaternion.identity);
        flyingFlyer.Init(target, flyerId, flyerVisualDatabase, faceHitSound, onHit);
    }
}
