using System.Collections;
using UnityEngine;

public abstract class Human : MonoBehaviour
{
    public string[] needReaction;
    protected abstract string[] GetReactions();

    [Header("Old fields")]
    public GameObject background;
    public GameObject[] emotion;
    public string name;
    public int reactionState; // 2 - придет на мессу, 1 - не придет
    public bool interact = false;

    [Header("Walk / Idle visual switch")]
    [Tooltip("Объект с анимацией ходьбы. Включается, когда прохожий двигается.")]
    [SerializeField] private GameObject walkVisualObject;

    [Tooltip("Картинка/анимация ожидания. Включается, когда прохожий остановлен после попадания листовки.")]
    [SerializeField] private GameObject idleVisualObject;

    [Tooltip("Можно не заполнять. Если заполнено, скрипт включает/выключает bool параметр ходьбы.")]
    [SerializeField] private Animator walkAnimator;

    [SerializeField] private string walkingBoolName = "IsWalking";

    [Header("SIMPLE direction flip")]
    [Tooltip("Включить простой поворот персонажа по X.")]
    [SerializeField] private bool useSimpleDirectionFlip = true;

    [Tooltip("Сюда перетащи ТОЛЬКО объект Run / объект анимации, который надо разворачивать по X. Скрипт меняет только знак Scale X у этого объекта.")]
    [SerializeField] private Transform visualRootToFlip;

    [Tooltip("Если Scale X > 0 означает, что персонаж смотрит вправо, оставь true. Если наоборот — false.")]
    [SerializeField] private bool positiveScaleXFacingRight = true;

    [Tooltip("Обычное движение по улице. Если прохожие идут справа налево, ставь -1. Если слева направо, ставь 1.")]
    [SerializeField] private float streetMoveDirectionX = -1f;

    [Tooltip("Если включено, скрипт меняет ТОЛЬКО знак Scale X и не трогает Y/Z. Это убирает искажения.")]
    [SerializeField] private bool changeOnlyScaleXSign = true;

    [Header("Flyer hit setup")]
    [SerializeField] private Transform facePoint;
    [SerializeField] private SpriteRenderer bubbleFlyerIcon;
    [SerializeField] private float flyerInBubbleTime = 0.35f;
    [SerializeField] private float stopAfterHitTime = 0.35f;

    [Header("Messa door movement")]
    [Tooltip("Необязательно. Если пусто, правильный прохожий пойдет к объекту MessaDoorPoint на сцене.")]
    [SerializeField] private Transform doorTargetOverride;

    [Tooltip("ОТДЕЛЬНАЯ скорость движения к двери ТОЛЬКО ПО X. Если медленно — ставь 7-12.")]
    [SerializeField] private float messaDoorMoveSpeed = 7f;

    [Tooltip("Когда X прохожего почти равен X двери, он считается вошедшим в мессу.")]
    [SerializeField] private float doorStopDistance = 0.05f;

    [Header("Police")]
    [SerializeField] private GameObject policeWarningIcon;

    private bool canMove = true;
    private bool alreadyReceivedFlyer = false;
    private bool waitingForFlyerHit = false;
    private bool goingToMessa = false;
    private Coroutine receiveFlyerCoroutine;

    private bool hasWantedFacing = false;
    private bool wantedFaceRight = false;
    private float safeAbsScaleX = 1f;

    public bool IsGoingToMessa => goingToMessa;
    public bool AlreadyReceivedFlyer => alreadyReceivedFlyer;
    public bool IsInPoliceZone { get; private set; }

    protected virtual void Awake()
    {
        needReaction = GetReactions();

        Transform flipTarget = GetFlipTarget();
        if (flipTarget != null)
            safeAbsScaleX = Mathf.Max(0.0001f, Mathf.Abs(flipTarget.localScale.x));

        HideBubbleFlyerIcon();
        SetPoliceZone(false);
        SetCanMove(true);
        FaceStreetDirection();
    }

    private void LateUpdate()
    {
        // Важно: LateUpdate идет после Animator, поэтому анимация не перезатирает поворот.
        ApplyWantedFacing();
    }

    public void SetPoliceZone(bool value)
    {
        IsInPoliceZone = value;

        if (policeWarningIcon != null)
            policeWarningIcon.SetActive(value);
    }

    public bool CanMove()
    {
        return canMove;
    }

    private void SetCanMove(bool value)
    {
        canMove = value;
        UpdateWalkIdleVisual();
    }

    private void UpdateWalkIdleVisual()
    {
        if (walkVisualObject != null)
            walkVisualObject.SetActive(canMove);

        if (idleVisualObject != null)
            idleVisualObject.SetActive(!canMove);

        if (walkAnimator != null && !string.IsNullOrEmpty(walkingBoolName))
            walkAnimator.SetBool(walkingBoolName, canMove);
    }

    public bool CanReceiveFlyer()
    {
        return interact && !alreadyReceivedFlyer && !waitingForFlyerHit;
    }

    public bool CanBeMegafonRecruited()
    {
        return !alreadyReceivedFlyer && !waitingForFlyerHit;
    }

    public bool ReserveForFlyerThrow()
    {
        if (!CanReceiveFlyer())
            return false;

        waitingForFlyerHit = true;
        interact = false;
        SetCanMove(false);

        if (background != null)
            background.SetActive(false);

        HideAllEmotions();
        return true;
    }

    public Vector3 GetFacePosition()
    {
        if (facePoint != null)
            return facePoint.position;

        return transform.position + new Vector3(0f, 0.6f, 0f);
    }

    public void ReceiveFlyer(string reaction, Sprite bubbleSprite)
    {
        if (alreadyReceivedFlyer)
            return;

        if (!waitingForFlyerHit)
        {
            if (!CanReceiveFlyer())
                return;

            ReserveForFlyerThrow();
        }

        StartReceiveFlyerRoutine(reaction, bubbleSprite);
    }

    public void ReceiveFlyerFromMegafon(string reaction, Sprite bubbleSprite)
    {
        if (!CanBeMegafonRecruited())
            return;

        waitingForFlyerHit = false;
        interact = false;

        if (background != null)
            background.SetActive(false);

        StartReceiveFlyerRoutine(reaction, bubbleSprite);
    }

    public void SetReaction(string reaction)
    {
        ReceiveFlyer(reaction, null);
    }

    private void StartReceiveFlyerRoutine(string reaction, Sprite bubbleSprite)
    {
        if (receiveFlyerCoroutine != null)
            StopCoroutine(receiveFlyerCoroutine);

        receiveFlyerCoroutine = StartCoroutine(ReceiveFlyerRoutine(reaction, bubbleSprite));
    }

    private IEnumerator ReceiveFlyerRoutine(string reaction, Sprite bubbleSprite)
    {
        alreadyReceivedFlyer = true;
        waitingForFlyerHit = false;
        interact = false;
        SetCanMove(false);

        HideAllEmotions();
        ShowBubbleFlyerIcon(bubbleSprite);

        yield return new WaitForSeconds(flyerInBubbleTime);

        HideBubbleFlyerIcon();

        bool isCorrectReaction = needReaction != null && needReaction.Length > 0 && reaction == needReaction[0];

        if (isCorrectReaction)
        {
            gameObject.layer = 2; // Ignore Raycast: второй раз кинуть нельзя
            reactionState = 2;
            goingToMessa = true;
            FaceDoorDirection();
        }
        else
        {
            reactionState = 1;
            goingToMessa = false;
            FaceStreetDirection();
        }

        if (needReaction != null && needReaction.Length > 0)
            Debug.Log("Нужно: " + needReaction[0] + " | Дали: " + reaction + " | Результат: " + reactionState + " | В мессу: " + goingToMessa);
        else
            Debug.Log("У прохожего не настроен needReaction. Дали: " + reaction);

        if (emotion != null && reactionState >= 0 && reactionState < emotion.Length && emotion[reactionState] != null)
            emotion[reactionState].SetActive(true);

        yield return new WaitForSeconds(stopAfterHitTime);

        SetCanMove(true);
        receiveFlyerCoroutine = null;
    }

    protected void MoveHuman(float speed)
    {
        if (!CanMove())
            return;

        if (goingToMessa)
        {
            Transform doorTarget = GetDoorTarget();

            if (doorTarget != null)
            {
                float targetX = doorTarget.position.x;
                float directionX = targetX - transform.position.x;

                FaceByDirectionX(directionX);

                float newX = Mathf.MoveTowards(
                    transform.position.x,
                    targetX,
                    messaDoorMoveSpeed * Time.deltaTime
                );

                // Важно: к двери идём ТОЛЬКО по X. Y/Z не трогаем, поэтому персонаж не ползёт вверх.
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);

                // Дошёл по X до двери — сразу считаем, что он зашёл в мессу.
                if (Mathf.Abs(targetX - transform.position.x) <= doorStopDistance)
                {
                    SetCanMove(false);

                    if (MessaDoorTrigger.Current != null)
                        MessaDoorTrigger.Current.AcceptHuman(this);
                }

                return;
            }
        }

        FaceStreetDirection();
        float x = Time.deltaTime * speed * Mathf.Abs(streetMoveDirectionX);
        transform.Translate(new Vector2(Mathf.Sign(streetMoveDirectionX) * x, 0f));
    }

    private Transform GetDoorTarget()
    {
        if (doorTargetOverride != null)
            return doorTargetOverride;

        return MessaDoorPoint.Current;
    }

    public bool IsNearDoor()
    {
        Transform doorTarget = GetDoorTarget();
        if (doorTarget == null)
            return false;

        return Mathf.Abs(transform.position.x - doorTarget.position.x) <= doorStopDistance;
    }

    private void FaceDoorDirection()
    {
        Transform doorTarget = GetDoorTarget();
        if (doorTarget == null)
            return;

        FaceByDirectionX(doorTarget.position.x - transform.position.x);
    }

    private void FaceStreetDirection()
    {
        FaceByDirectionX(streetMoveDirectionX);
    }

    private void FaceByDirectionX(float directionX)
    {
        if (!useSimpleDirectionFlip)
            return;

        if (Mathf.Abs(directionX) < 0.001f)
            return;

        wantedFaceRight = directionX > 0f;
        hasWantedFacing = true;
    }

    private void ApplyWantedFacing()
    {
        if (!useSimpleDirectionFlip || !hasWantedFacing)
            return;

        Transform target = GetFlipTarget();
        if (target == null)
            return;

        Vector3 scale = target.localScale;

        float absX = Mathf.Abs(scale.x);
        if (absX < 0.0001f)
            absX = safeAbsScaleX;

        float sign = positiveScaleXFacingRight == wantedFaceRight ? 1f : -1f;

        if (changeOnlyScaleXSign)
        {
            // Самый безопасный вариант: меняем только знак X, Y/Z не трогаем.
            scale.x = absX * sign;
            target.localScale = scale;
        }
        else
        {
            target.localScale = new Vector3(absX * sign, scale.y, scale.z);
        }
    }

    private Transform GetFlipTarget()
    {
        // Специально НЕ используем fallback на весь walkVisualObject/idleVisualObject.
        // Чтобы не искажать картинки, скрипт меняет Scale X только у объекта, который ты явно перетащил сюда — обычно Run.
        return visualRootToFlip;
    }

    private void ShowBubbleFlyerIcon(Sprite bubbleSprite)
    {
        if (bubbleFlyerIcon == null)
            return;

        if (bubbleSprite != null)
            bubbleFlyerIcon.sprite = bubbleSprite;

        bubbleFlyerIcon.gameObject.SetActive(true);
    }

    private void HideBubbleFlyerIcon()
    {
        if (bubbleFlyerIcon != null)
            bubbleFlyerIcon.gameObject.SetActive(false);
    }

    private void HideAllEmotions()
    {
        if (emotion == null)
            return;

        for (int i = 0; i < emotion.Length; i++)
        {
            if (emotion[i] != null)
                emotion[i].SetActive(false);
        }
    }

    public void OnMouseEnter()
    {
        if (background != null && CanReceiveFlyer())
            background.SetActive(true);
    }

    public void OnMouseExit()
    {
        if (background != null)
            background.SetActive(false);
    }
}
