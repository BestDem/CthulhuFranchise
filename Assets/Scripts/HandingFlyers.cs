using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandingFlyers : MonoBehaviour
{
    public static HandingFlyers singltoneFlyer { get; private set; }

    [Header("UI")]
    [SerializeField] private Text suspicionText;
    [SerializeField] private TMP_Text suspicionTMPText;
    [Tooltip("Image с Type = Filled. Показывает подозрение полиции.")]
    [SerializeField] private Image suspicionFillImage;
    [SerializeField] private string suspicionPrefix = "Подозрение: ";

    [Header("Throw")]
    [SerializeField] private CultistFlyerThrower cultistThrower;

    [Header("Bridge")]
    [Tooltip("Если включено, мегафон и адвокат берутся из GameSessionBridge. Если моста нет, используются галки ниже.")]
    [SerializeField] private bool useBridgeUpgradeToggles = true;

    [Header("Megaphone")]
    [Tooltip("Если включено, правильная листовка дополнительно вербует ближайших прохожих такого же подходящего типа.")]
    [SerializeField] private bool isMegafon = false;
    [SerializeField] private float radiusMegafon = 2f;
    [SerializeField] private LayerMask layer;
    [SerializeField] private int maxMegafonExtraHumans = 1;
    [SerializeField] private bool megafonOnlyIfMainTargetCorrect = true;
    [SerializeField] private FlyerVisualDatabase flyerVisualDatabase;

    [Header("Police Suspicion")]
    public bool isPolice = true;
    [SerializeField] private bool suspicionOnlyInsidePoliceZone = true;
    [SerializeField] private ListReactions listReactions;
    [SerializeField] private float maxSuspicion = 7f;

    [Header("Devil Advocate")]
    [Tooltip("Если включено, при максимальном подозрении один раз за день сбрасывает подозрение до половины, а полиция не ловит игрока.")]
    [SerializeField] private bool hasDevilAdvocate = false;
    [Range(0f, 1f)]
    [SerializeField] private float devilAdvocateResetPercent = 0.5f;

    [Header("Police Catch")]
    [SerializeField] private DayController dayController;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private string policeCatchReason = "Полиция накрыла точку";
    [SerializeField] private string devilAdvocateMessage = "Адвокат дьявола сбил подозрение";

    private float currentSuspicion = 0f;
    private bool devilAdvocateUsedToday = false;
    private bool streetLocked = false;

    private bool IsMegafonActive =>
        useBridgeUpgradeToggles && GameSessionBridge.Instance != null
            ? GameSessionBridge.Instance.HasMegafon
            : isMegafon;

    private bool HasDevilAdvocateActive =>
        useBridgeUpgradeToggles && GameSessionBridge.Instance != null
            ? GameSessionBridge.Instance.HasDevilAdvocate
            : hasDevilAdvocate;

    private void Awake()
    {
        singltoneFlyer = this;
    }

    private void OnEnable()
    {
        DayController.dayEnd += OnDayEnd;
    }

    private void OnDisable()
    {
        DayController.dayEnd -= OnDayEnd;
    }

    private void Start()
    {
        UpdateSuspicionUI();
    }

    private void Update()
    {
        if (streetLocked)
            return;

        if (Input.GetMouseButtonDown(0))
            TryStartThrow();
    }

    private void OnDayEnd(bool ended)
    {
        if (ended)
        {
            streetLocked = true;
            return;
        }

        streetLocked = false;
        devilAdvocateUsedToday = false;
        currentSuspicion = 0f;
        UpdateSuspicionUI();
    }

    private void TryStartThrow()
    {
        if (FlyersController.singltoneFlyers == null)
            return;

        string selectedFlyer = FlyersController.singltoneFlyers.CurrentFlyer();
        if (string.IsNullOrEmpty(selectedFlyer))
            return;

        if (cultistThrower != null && cultistThrower.IsThrowing)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
        if (hit == null)
            return;

        Human human = hit.GetComponent<Human>();
        if (human == null)
            human = hit.GetComponentInParent<Human>();

        if (human == null)
            return;

        if (!human.ReserveForFlyerThrow())
            return;

        bool mainTargetCorrect = IsCorrectFlyerForHuman(human, selectedFlyer);

        TryAddPoliceSuspicion(human, selectedFlyer);

        FlyersController.singltoneFlyers.DeletFlyer();

        if (cultistThrower != null)
            cultistThrower.ThrowFlyerTo(human, selectedFlyer, () => ApplyMegafon(human, selectedFlyer, mainTargetCorrect));
        else
        {
            human.ReceiveFlyer(selectedFlyer, GetBubbleSprite(selectedFlyer));
            ApplyMegafon(human, selectedFlyer, mainTargetCorrect);
        }
    }

    private void ApplyMegafon(Human mainHuman, string selectedFlyer, bool mainTargetCorrect)
    {
        if (!IsMegafonActive || mainHuman == null)
            return;

        if (megafonOnlyIfMainTargetCorrect && !mainTargetCorrect)
            return;

        if (maxMegafonExtraHumans <= 0)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mainHuman.transform.position, radiusMegafon, layer);
        int recruitedCount = 0;
        Sprite bubbleSprite = GetBubbleSprite(selectedFlyer);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            Human otherHuman = hits[i].GetComponent<Human>();
            if (otherHuman == null)
                otherHuman = hits[i].GetComponentInParent<Human>();

            if (otherHuman == null || otherHuman == mainHuman)
                continue;

            if (!otherHuman.CanBeMegafonRecruited())
                continue;

            if (!IsCorrectFlyerForHuman(otherHuman, selectedFlyer))
                continue;

            otherHuman.ReceiveFlyerFromMegafon(selectedFlyer, bubbleSprite);
            recruitedCount++;

            if (recruitedCount >= maxMegafonExtraHumans)
                break;
        }

        if (recruitedCount > 0)
            Debug.Log("Мегафон завербовал дополнительных прохожих: " + recruitedCount);
    }

    private bool IsCorrectFlyerForHuman(Human human, string selectedFlyer)
    {
        return human != null && human.needReaction != null && human.needReaction.Length > 0 && selectedFlyer == human.needReaction[0];
    }

    private Sprite GetBubbleSprite(string selectedFlyer)
    {
        if (flyerVisualDatabase == null)
            return null;

        return flyerVisualDatabase.GetBubbleSprite(selectedFlyer);
    }

    private void TryAddPoliceSuspicion(Human human, string selectedFlyer)
    {
        if (!isPolice || human == null)
            return;

        if (suspicionOnlyInsidePoliceZone && !human.IsInPoliceZone)
            return;

        if (listReactions == null || listReactions.Suspicion == null || listReactions.Suspicion.Length < 6)
            return;

        float addAmount = GetSuspicionAmount(human, selectedFlyer);
        currentSuspicion = Mathf.Clamp(currentSuspicion + addAmount, 0f, maxSuspicion);
        Debug.Log("Подозрение полиции +" + addAmount + " = " + currentSuspicion + "/" + maxSuspicion);
        UpdateSuspicionUI();

        if (currentSuspicion >= maxSuspicion)
            OnMaxSuspicionReached();
    }

    private float GetSuspicionAmount(Human human, string selectedFlyer)
    {
        bool isCorrectFlyer = IsCorrectFlyerForHuman(human, selectedFlyer);

        if (!isCorrectFlyer)
            return listReactions.Suspicion[5];

        switch (selectedFlyer)
        {
            case "worker2": return listReactions.Suspicion[0];
            case "student2": return listReactions.Suspicion[1];
            case "retiree2": return listReactions.Suspicion[2];
            case "blogger2": return listReactions.Suspicion[3];
            case "esoteric2": return listReactions.Suspicion[4];
            default: return listReactions.Suspicion[5];
        }
    }

    private void OnMaxSuspicionReached()
    {
        if (HasDevilAdvocateActive && !devilAdvocateUsedToday)
        {
            devilAdvocateUsedToday = true;
            currentSuspicion = maxSuspicion * devilAdvocateResetPercent;
            Debug.Log(devilAdvocateMessage + ": " + currentSuspicion + "/" + maxSuspicion);
            UpdateSuspicionUI();
            return;
        }

        streetLocked = true;

        bool chaseStarted = PoliceController.StartClosestChase(playerTarget, OnPoliceCaughtPlayer);
        if (!chaseStarted)
            OnPoliceCaughtPlayer();
    }

    private void OnPoliceCaughtPlayer()
    {
        if (dayController != null)
            dayController.ForceEndDay(policeCatchReason);
    }

    private void UpdateSuspicionUI()
    {
        float normalized = maxSuspicion <= 0f ? 0f : Mathf.Clamp01(currentSuspicion / maxSuspicion);

        string text = suspicionPrefix + currentSuspicion.ToString("0.#") + " / " + maxSuspicion.ToString("0.#");

        if (suspicionText != null)
            suspicionText.text = text;

        if (suspicionTMPText != null)
            suspicionTMPText.text = text;

        if (suspicionFillImage != null)
            suspicionFillImage.fillAmount = normalized;
    }

    public void SetMegafonActive(bool value)
    {
        isMegafon = value;
    }

    public void SetDevilAdvocateActive(bool value)
    {
        hasDevilAdvocate = value;
    }
}
