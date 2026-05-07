using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class Messa : MonoBehaviour
{
    [HideInInspector] public int oldAdeptsCount;
    [Header("Основные параметры")]
    
    public float messaDuration = 3f;
    public float Money;
    [HideInInspector] public float TotalMoneyIncome;
    [HideInInspector] public float DailyMoneyIncome;
    [HideInInspector] public int CurrentDay = 1;

    [HideInInspector] public int[] Auditory = new int[5];
    [HideInInspector] public int[] NewAdepts = new int[5];
    [HideInInspector] public int[] OldAdepts = new int[5];


    [Header("Базовая конверсия")]
    public float[] BaseConversion = new float[5] { 0.30f, 0.52f, 0.34f, 0.30f, 0.38f };
    public float ConversionMultiplier;

    [Header("Базовый доход")]
    [HideInInspector] public float DailyBaseIncome;
    public float[] BaseIncome = new float[5] { 4.0f, 0.8f, 1.4f, 1.1f, 1.3f };
    
    public float OldAdeptIncomeMultiplier = 0.45f;
    public string MessaResult = "Отличная месса";

    [Header("Улучшения")]
    public UpgradeInfo[] UpgradeList;
    public UpgradePanel[] UpgradePanels;

    [Header("Множители проповеди")]
    public float BadMultiplier = 0.68f;
    public float NormalMultiplier = 1.0f;
    public float GoodMultiplier = 1.22f;
    public float ExcellentMultiplier = 1.35f;

    [Header("Пороги")]
    public float BadThreshold = 0.18f;
    public float GoodThreshold = 0.34f;
    public float ExcellentThreshold = 0.50f;

    [Header("Цели")]
    public int needMoney = 50;
    public int needAdepts = 20;


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
    public GameObject UpgradeWindow;
    public TextMeshProUGUI EndGameStats;
    public TextMeshProUGUI EndGameHeader;

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
    [Header("Спрайты посетителей")]
    [SerializeField] private GameObject[] VisitorSprites;
  
    [Header("Музыка")]
    [SerializeField] private AudioClip messaMusic;

    public static Messa Instance;
    private void Awake()
    {
        Instance = this;
        
    }
    private void Start()
    {
        LoadFromBridge();
        BindUpgrades();
    }
    private void OnEnable()
    {
        LoadFromBridge();
        MusicPlayer.Instance.PlayMusic(messaMusic);
    }
    private void Update()
    {
        DayLabel?.SetText($"{CurrentDay}");
        MoneyLabel?.SetText($"${(int)Money}");
        OldAdeptsCountLabel?.SetText($"Старые адепты: {TotalCount(OldAdepts)}");

        AuditoryCountLabel?.SetText($"Аудитория: {TotalCount(Auditory)}");
        AuditoryWorkersLabel?.SetText($"Офисники: {Auditory[0]}");
        AuditoryStudentsLabel?.SetText($"Студенты: {Auditory[1]}");
        AuditoryPensionersLabel?.SetText($"Пенсионеры: {Auditory[2]}");
        AuditoryBloggersLabel?.SetText($"Блогеры: {Auditory[3]}");
        AuditoryEsotericsLabel?.SetText($"Эзотерики: {Auditory[4]}");

        for (int i = 0; i < VisitorSprites.Length; i++)
        {
            VisitorSprites[i].SetActive(Auditory[i] > 0 || OldAdepts[i] > 0);
        }
    }
    private void LoadFromBridge()
    {    
        if (GameSessionBridge.Instance == null) return;
        var input = GameSessionBridge.Instance.GetMessaInput();

        CurrentDay = input.currentDay;
        Money = input.currentMoney;

        Auditory[0] = input.officeVisitors;
        Auditory[1] = input.studentVisitors;
        Auditory[2] = input.retireeVisitors;
        Auditory[3] = input.bloggerVisitors;
        Auditory[4] = input.esotericVisitors;

        OpenMenu((int)MenuID.MessaHall);
    }
    private void BindUpgrades()
    {
        int n = Mathf.Min(UpgradeList.Length, UpgradePanels.Length);

        for (int i = 0; i < n; i++) 
        {
            if(!UpgradeList[i].Unlocked) UpgradePanels[i].BindUpgrade(i);
        }     
    }
    private void OpenMenu(MenuID menuID)
    {
        for (int i = 0; i < Menus.Length; i++) Menus[i]?.SetActive(i == (int)menuID);
        StatsPanel.SetActive(menuID != MenuID.PendingMessa);
    }
    
    public int TotalCount(int[] array)
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
        int outflow = oldAdeptsCount - TotalCount(OldAdepts);
        return outflow >= 0 ? $"Нет оттока" : $"{outflow}";
    }
    public void BuyUpgrade(int i)
    {
        if (UpgradeList[i].Unlocked) return;

        if (i >= UpgradePanels.Length || i >= UpgradeList.Length || Money < UpgradeList[i].Price) return;

        Money -= UpgradeList[i].Price;
        UpgradeList[i].Unlocked = true;
        UpgradePanels[i].DisableBuy();
    }
    public bool IsUnlocked(Upgrades upgrade)
    {
        return UpgradeList[(int)upgrade].Unlocked;
    }
    public void SpellSermon(int peopleClass)
    {
        oldAdeptsCount = TotalCount(OldAdepts);
        int totalVisitors = TotalCount(Auditory);

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
        DailyMoneyIncome = DailyBaseIncome + frontRow;

        if (IsUnlocked(Upgrades.PremiumCandles) && isGoodOrBetter) DailyMoneyIncome *= CandlesMultiplier;

        Money += DailyMoneyIncome;
        TotalMoneyIncome += DailyMoneyIncome;

        if (isExcellent)
        {
            MessaResult = "Отличная месса";
        }
        else if (isGoodOrBetter)
        {
            MessaResult = "Хорошая месса";
        }
        else if (isBad)
        {
            MessaResult = "Плохая месса";
        }
        StartCoroutine(MessaCoroutine());
    }
    private IEnumerator MessaCoroutine()
    {
        SFXPlayer.Instance.Play("Месса");
        OpenMenu(MenuID.PendingMessa);
        SFXPlayer.Instance.Play("");    
        yield return new WaitForSeconds(messaDuration);      
        OpenMenu(MenuID.MessaResults);
        SFXPlayer.Instance.Stop();
    }
    public void Next()
    {
        if (Menus[(int)MenuID.PendingMessa].activeSelf)
        {
            StopAllCoroutines();
            OpenMenu(MenuID.MessaResults);
            SFXPlayer.Instance.Stop();
        }
        else if (Menus[(int)MenuID.MessaResults].activeSelf)
        {
            if (CurrentDay > 4) 
            {
                StartNewDay();
            }
            else OpenMenu(MenuID.UpgradeShop);
        }
        else if (Menus[(int)MenuID.UpgradeShop].activeSelf) 
        {
            UpgradeWindow?.SetActive(false);
        }       
    }
    public void StartNewDay()
    {
        
        InitiateAdepts();
        if (CurrentDay == 5) 
        {
            EndGameHeader?.SetText(BuildEndGameResultHeader());
            EndGameStats?.SetText(BuildEndGameResultText());
        }  
        GameSessionBridge.Instance.ApplyMessaResult(Money,TotalCount(OldAdepts));
        MusicPlayer.Instance.PlayDefaultMusic();
        if (GameSessionBridge.Instance != null) GameSessionBridge.Instance.StartNewDay();
    }
    private void InitiateAdepts()
    {
        int n = Mathf.Min(OldAdepts.Length, NewAdepts.Length);
        for (int i = 0; i < n; i++)
        {
            OldAdepts[i] += NewAdepts[i];
            NewAdepts[i] = 0;
        }
        for (int i = 0; i < 5; i++) Auditory[i] = 0;
    }
    private string BuildEndGameResultHeader()
    {
        if (Money > needMoney && TotalCount(OldAdepts) > needAdepts)
        {
            SFXPlayer.Instance.Play("Успех3");
            return "Филиал принят";
        }
        SFXPlayer.Instance.Play("Проигрыш");
        return "Провал филиала";
    }
    private string BuildEndGameResultText()
    {
        string text = $"Привлечено адептов: {TotalCount(OldAdepts)} / {needAdepts}\n";
        text += $"Заработано денег: ${(int)Money} / ${needMoney}\n";
        return text;
    }
    
    public void AddAdepts(int value)
    {
        if (value == 0) return;

        if (value > 0)
        {
            int perType = Mathf.Max(1, value / NewAdepts.Length);

            for (int i = 0; i < NewAdepts.Length; i++) NewAdepts[i] += perType;
        }
        else
        {
            int remaining = -value;
            int perType = Mathf.Max(1, remaining / OldAdepts.Length);

            for (int i = 0; i < OldAdepts.Length; i++)
            {
                OldAdepts[i] -= perType;
                if (OldAdepts[i] < 0) OldAdepts[i] = 0;
            }
        }
    }
}