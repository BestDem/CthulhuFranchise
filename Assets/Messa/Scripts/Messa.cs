using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(PlaySoundsComponent))]
public class Messa : MonoBehaviour
{
    public int oldAdeptsCount;
    [Header("Основные параметры")]
    public float Money;   
    public float messaDuration = 3f;

    [HideInInspector] public float TotalIncome;
    [HideInInspector] public float DailyIncome;
    [HideInInspector] public int CurrentDay = 1;

    public int[] Auditory = new int[5];
    public int[] NewAdepts = new int[5];
    public int[] OldAdepts = new int[5];

    private int[] defaultAuditoryValues = new int[5];
    private PlaySoundsComponent sfxPlayer;


    [Header("Базовая конверсия")]
    public float[] BaseConversion = new float[5] { 0.30f, 0.52f, 0.34f, 0.30f, 0.38f };
    public float ConversionMultiplier;

    [Header("Базовый доход")]
    [HideInInspector] public float DailyBaseIncome;
    public float[] BaseIncome = new float[5] { 4.0f, 0.8f, 1.4f, 1.1f, 1.3f };
    
    public float OldAdeptIncomeMultiplier = 0.45f;

    [Header("Улучшения")]
    public UpgradeInfo[] UpgradeList;
    public UpgradePanel UpgradePanel1;
    public UpgradePanel UpgradePanel2;
    public UpgradePanel UpgradePanel3;

    [Header("Множители проповеди")]
    public float BadMultiplier = 0.68f;
    public float NormalMultiplier = 1.0f;
    public float GoodMultiplier = 1.22f;
    public float ExcellentMultiplier = 1.35f;

    [Header("Пороги")]
    public float BadThreshold = 0.18f;
    public float GoodThreshold = 0.34f;
    public float ExcellentThreshold = 0.50f;

    [Header("Бонусы")]
    public float CookieBonus = 0.12f;
    public float AltarGoodBonus = 0.06f;
    public float AltarExcellentBonus = 0.08f;
    public float PremiumFlyerBonus = 0.05f;
    public int AbyssAccountantBonus = 3;

    public float EsotericBonusPerUnit = 0.02f;
    public float EsotericBonusCap = 0.20f;

    [Header("Лимиты")]
    public float MaxConversionChance = 0.90f;

    [Header("Отток")]
    public float BaseChurn = 0.08f;
    public float PensionerChurnReduce = 0.01f;
    public float MinChurn = 0.02f;
    public float ChoirMultiplier = 0.5f;

    [Header("Доход")]  
    public float PaidFrontRowBonus = 1.5f;
    public float CandlesMultiplier = 1.20f;
    public float PremiumIncomeMultiplier = 1.25f;

    [Header("UI")]
    public GameObject StatsPanel;
    public TextMeshProUGUI DayLabel;
    public TextMeshProUGUI MoneyLabel;
    public TextMeshProUGUI OldAdeptsCountLabel;
    public TextMeshProUGUI AuditoryCountLabel;

    public TextMeshProUGUI AuditoryWorkersLabel;
    public TextMeshProUGUI AuditoryStudentsLabel;
    public TextMeshProUGUI AuditoryPensionersLabel;
    public TextMeshProUGUI AuditoryBloggersLabel;
    public TextMeshProUGUI AuditoryEsotericsLabel;
    public GameObject[] Menus;
    public static Messa Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        sfxPlayer = GetComponent<PlaySoundsComponent>();
    }
    private void Start()
    {
        OpenMenu((int)MenuID.MessaHall);
        defaultAuditoryValues = (int[])Auditory.Clone();
        UpdateUI();
    }
    private void OnEnable()
    {
        Start();
    }
    private void OpenMenu(MenuID menuID)
    {
        for (int i = 0; i < Menus.Length; i++) Menus[i]?.SetActive(i == (int)menuID);
        StatsPanel.SetActive(menuID != MenuID.PendingMessa);
    }
    private int TotalCount(int[] array)
    {
        int count = 0;
        foreach (int i in array) count += i;
        return count;
    }
    public int GetOldAdeptsCount()
    {
        return TotalCount(OldAdepts);
    }
    public string GetNewAdeptsCount()
    {
        return TotalCount(NewAdepts).ToString();
    }
    public int GetTotalAdeptsCount()
    {
        return TotalCount(OldAdepts) + TotalCount(NewAdepts);
    }
    public string GetAdeptsOutflow()
    {
        int outflow = GetTotalAdeptsCount() - oldAdeptsCount;
        return outflow == 0 ? $"Нет оттока" : $"{outflow}";
    }
    public void BuyUpgrade(int i)
    {      
        if (i >= UpgradeList.Length || Money < UpgradeList[i].Price) return;   
        Money -= UpgradeList[i].Price;
        UpgradeList[i].Unlocked = true;
        Debug.Log($"Куплено улучшение: {UpgradeList[i].Header.ToLower()}");
        Next();
    }
    public bool IsUnlocked(Upgrades upgrade)
    {
        return UpgradeList[(int)upgrade].Unlocked;
    }
    public void SpellSermon(int peopleClass)
    {
        oldAdeptsCount = TotalCount(OldAdepts);
        int totalVisitors = TotalCount(Auditory);
        if (totalVisitors == 0) return;

        float share = (float)Auditory[peopleClass] / totalVisitors;

        bool isBad = false;
        bool isGoodOrBetter = false;
        bool isExcellent = false;

        if (share < BadThreshold)
        {
            ConversionMultiplier = BadMultiplier;
            isBad = true;
        }
        else if (share < GoodThreshold)
        {
            ConversionMultiplier = NormalMultiplier;
        }
        else if (share < ExcellentThreshold)
        {
            ConversionMultiplier = GoodMultiplier;
            isGoodOrBetter = true;
        }
        else
        {
            ConversionMultiplier = ExcellentMultiplier;
            isGoodOrBetter = true;
            isExcellent = true;
        }
        float esotericBonus = Mathf.Min(Auditory[4] * EsotericBonusPerUnit, EsotericBonusCap);
        int[] adeptsBefore = (int[])OldAdepts.Clone();
        int totalAdeptsBefore = TotalCount(adeptsBefore);
        int[] newConverted = new int[5];

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < Auditory[i]; j++)
            {
                float chance = BaseConversion[i] * ConversionMultiplier;

                if (IsUnlocked(Upgrades.CookiesAfterMessa) && i == 1) chance += CookieBonus;

                if (IsUnlocked(Upgrades.BeautifulAltar))
                {
                    if (isExcellent) chance += AltarExcellentBonus;
                    else if (isGoodOrBetter) chance += AltarGoodBonus;
                }
                chance += esotericBonus;

                if (IsUnlocked(Upgrades.PremiumFlyer)) chance += PremiumFlyerBonus;
                chance = Mathf.Min(chance, MaxConversionChance);

                if (Random.value <= chance) newConverted[i]++;

            }
        }
        int lostTotal = 0;
        if (isBad)
        {
            float churn = BaseChurn - Auditory[2] * PensionerChurnReduce;
            churn = Mathf.Max(churn, MinChurn);

            if (IsUnlocked(Upgrades.Choir)) churn *= ChoirMultiplier;

            for (int i = 0; i < 5; i++)
            {
                int lost = Mathf.RoundToInt(adeptsBefore[i] * churn);
                OldAdepts[i] = adeptsBefore[i] - lost;
                lostTotal += lost;
            }
        }
        for (int i = 0; i < 5; i++) NewAdepts[i] += newConverted[i];

        float visitorIncome = 0f;

        for (int i = 0; i < 5; i++)
        {
            float income = BaseIncome[i] * Auditory[i];
            if (IsUnlocked(Upgrades.PremiumFlyer)) income *= PremiumIncomeMultiplier;
            visitorIncome += income;
        }
        float oldAdeptIncome = totalAdeptsBefore * OldAdeptIncomeMultiplier;

        float frontRow = IsUnlocked(Upgrades.PaidFrontRow) ? Auditory[0] * PaidFrontRowBonus : 0f;

        DailyBaseIncome = visitorIncome + oldAdeptIncome;
        DailyIncome = DailyBaseIncome + frontRow;

        if (IsUnlocked(Upgrades.PremiumCandles) && isGoodOrBetter) DailyIncome *= CandlesMultiplier;

        Money += DailyIncome;
        TotalIncome += DailyIncome;

        for (int i = 0; i < 5; i++) Auditory[i] = 0;

        StartCoroutine(MessaCoroutine());
    }
    private IEnumerator MessaCoroutine()
    {
        OpenMenu(MenuID.PendingMessa);
        sfxPlayer?.Play("");    
        yield return new WaitForSeconds(messaDuration);
        UpdateUI();
        OpenMenu(MenuID.MessaResults);
    }
    public void Next()
    {
        UpdateUI();
        if (Menus[(int)MenuID.PendingMessa].activeSelf)
        {
            StopAllCoroutines();
            OpenMenu(MenuID.MessaResults);
        }
        else if (Menus[(int)MenuID.MessaResults].activeSelf)
        {
            if (CurrentDay <= 4)
            {
                int j = (CurrentDay - 1) * 3;
                OpenMenu(MenuID.UpgradeShop);
                UpgradePanel1.BindUpgrade(j);
                UpgradePanel2.BindUpgrade(j + 1);
                UpgradePanel3.BindUpgrade(j + 2);
            } 
            else StartNewDay();
        }
        else if (Menus[(int)MenuID.UpgradeShop].activeSelf) StartNewDay();
    }
    private void StartNewDay()
    {
        OpenMenu(MenuID.MessaHall);
        InitiateAdepts();
        Auditory = (int[])defaultAuditoryValues.Clone();
        CurrentDay++;
        UpdateUI();
    }
    private void InitiateAdepts()
    {
        int n = Mathf.Min(OldAdepts.Length, NewAdepts.Length);
        for (int i = 0; i < n; i++)
        {
            OldAdepts[i] += NewAdepts[i];
            NewAdepts[i] = 0;
        }
    }
    public void UpdateUI()
    {
        DayLabel?.SetText($"День: {CurrentDay}");
        MoneyLabel?.SetText($"${(int)Money}");
        OldAdeptsCountLabel?.SetText($"Старые адепты: {TotalCount(OldAdepts)}");

        AuditoryCountLabel?.SetText($"Аудитория: {TotalCount(Auditory)}");
        AuditoryWorkersLabel?.SetText($"Офисники: {Auditory[0]}");
        AuditoryStudentsLabel?.SetText($"Студенты: {Auditory[1]}");
        AuditoryPensionersLabel?.SetText($"Пенсионеры: {Auditory[2]}");
        AuditoryBloggersLabel?.SetText($"Блогеры: {Auditory[3]}");
        AuditoryEsotericsLabel?.SetText($"Эзотерики: {Auditory[4]}");    
    }
}