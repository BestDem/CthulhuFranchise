using UnityEngine;
using TMPro;

public class Messa : MonoBehaviour
{
    
    public float Money;

    [HideInInspector] public int CurrentDay = 1;
    [SerializeField] private int[] auditory = new int[5];
    [SerializeField] private int[] newAdepts = new int[5];
    [SerializeField] private int[] oldAdepts = new int[5];
    private int[] defaultAuditoryValues = new int[5];

    [Header("Базовая конверсия")]
    [SerializeField] private float[] baseConversion = new float[5] { 0.30f, 0.52f, 0.34f, 0.30f, 0.38f };

    [Header("Базовый доход")]
    [SerializeField] private float[] baseIncome = new float[5] { 4.0f, 0.8f, 1.4f, 1.1f, 1.3f };

    [Header("Улучшения")]
    [SerializeField] private UpgradesBridge BooleanUpgrades;

    [Header("Множители проповеди")]
    [SerializeField] private float badMultiplier = 0.68f;
    [SerializeField] private float normalMultiplier = 1.0f;
    [SerializeField] private float goodMultiplier = 1.22f;
    [SerializeField] private float excellentMultiplier = 1.35f;

    [Header("Пороги")]
    [SerializeField] private float badThreshold = 0.18f;
    [SerializeField] private float goodThreshold = 0.34f;
    [SerializeField] private float excellentThreshold = 0.50f;

    [Header("Бонусы")]
    [SerializeField] private float cookieBonus = 0.12f;
    [SerializeField] private float altarGoodBonus = 0.06f;
    [SerializeField] private float altarExcellentBonus = 0.08f;
    [SerializeField] private float premiumFlyerBonus = 0.05f;

    [SerializeField] private float esotericBonusPerUnit = 0.02f;
    [SerializeField] private float esotericBonusCap = 0.20f;

    [Header("Лимиты")]
    [SerializeField] private float maxConversionChance = 0.90f;

    [Header("Отток")]
    [SerializeField] private float baseChurn = 0.08f;
    [SerializeField] private float pensionerChurnReduce = 0.01f;
    [SerializeField] private float minChurn = 0.02f;
    [SerializeField] private float choirMultiplier = 0.5f;

    [Header("Доход")]
    [SerializeField] private float premiumIncomeMultiplier = 1.25f;
    [SerializeField] private float paidFrontRowBonus = 1.5f;
    [SerializeField] private float oldAdeptIncomeMultiplier = 0.45f;
    [SerializeField] private float candlesMultiplier = 1.20f;
    [SerializeField] private float auditMultiplier = 1.10f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI DayLabel;
    [SerializeField] private TextMeshProUGUI MoneyLabel;
    [SerializeField] private TextMeshProUGUI OldAdeptsCountLabel;
    [SerializeField] private TextMeshProUGUI NewAdeptsCountLabel;
    [SerializeField] private TextMeshProUGUI AuditoryCountLabel;

    [SerializeField] private TextMeshProUGUI AdeptWorkersLabel;
    [SerializeField] private TextMeshProUGUI AdeptStudentsLabel;
    [SerializeField] private TextMeshProUGUI AdeptPensionersLabel;
    [SerializeField] private TextMeshProUGUI AdeptBloggersLabel;
    [SerializeField] private TextMeshProUGUI AdeptEsotericsLabel;

    [SerializeField] private TextMeshProUGUI AuditoryWorkersLabel;
    [SerializeField] private TextMeshProUGUI AuditoryStudentsLabel;
    [SerializeField] private TextMeshProUGUI AuditoryPensionersLabel;
    [SerializeField] private TextMeshProUGUI AuditoryBloggersLabel;
    [SerializeField] private TextMeshProUGUI AuditoryEsotericsLabel;

    private int TotalCount(int[] array)
    {
        int count = 0;
        foreach (int i in array) count += i;
        return count;
    }

    void Start()
    {
        defaultAuditoryValues = (int[])auditory.Clone();
        BooleanUpgrades.ApplyToStatic();
        UpdateUI();
    }

    public void SpellSermon(int peopleClass)
    {
        if (TotalCount(auditory) == 0) return;

        int totalVisitors = TotalCount(auditory);

        int randomIndex = Random.Range(0, totalVisitors);

        int selectedClass = -1;
        int counter = 0;

        for (int i = 0; i < 5; i++)
        {
            counter += auditory[i];
            if (randomIndex < counter)
            {
                selectedClass = i;
                break;
            }
        }

        if (selectedClass == -1) return;

        float share = (float)auditory[peopleClass] / totalVisitors;

        float multiplier;
        bool isBad = false;
        bool isGoodOrBetter = false;
        bool isExcellent = false;

        if (share < badThreshold)
        {
            multiplier = badMultiplier;
            isBad = true;
        }
        else if (share < goodThreshold)
        {
            multiplier = normalMultiplier;
        }
        else if (share < excellentThreshold)
        {
            multiplier = goodMultiplier;
            isGoodOrBetter = true;
        }
        else
        {
            multiplier = excellentMultiplier;
            isGoodOrBetter = true;
            isExcellent = true;
        }

        float esotericBonus = Mathf.Min(auditory[4] * esotericBonusPerUnit, esotericBonusCap);

        float chance = baseConversion[selectedClass] * multiplier;

        if (Upgrades.HasCookie && selectedClass == 1)
            chance += cookieBonus;

        if (Upgrades.HasAltar)
        {
            if (isExcellent) chance += altarExcellentBonus;
            else if (isGoodOrBetter) chance += altarGoodBonus;
        }

        chance += esotericBonus;

        if (Upgrades.HasPremiumFlyer)
            chance += premiumFlyerBonus;

        chance = Mathf.Min(chance, maxConversionChance);

        int converted = (Random.value <= chance) ? 1 : 0;

        int[] adeptsBefore = (int[])newAdepts.Clone();
        int totalAdeptsBefore = TotalCount(adeptsBefore);
        int lostTotal = 0;

        if (isBad)
        {
            float churn = baseChurn - auditory[2] * pensionerChurnReduce;
            churn = Mathf.Max(churn, minChurn);

            if (Upgrades.HasChoir) churn *= choirMultiplier;

            for (int i = 0; i < 5; i++)
            {
                int lost = Mathf.RoundToInt(adeptsBefore[i] * churn);
                newAdepts[i] = adeptsBefore[i] - lost;
                lostTotal += lost;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++) newAdepts[i] = adeptsBefore[i];
        }

        newAdepts[selectedClass] += converted;

        float visitorIncome = baseIncome[selectedClass];

        if (Upgrades.HasPremiumFlyer) visitorIncome *= premiumIncomeMultiplier;


        float oldAdeptIncome = totalAdeptsBefore * oldAdeptIncomeMultiplier;

        float frontRow = Upgrades.HasPaidFrontRow && selectedClass == 0 ? paidFrontRowBonus : 0f;

        float totalMoney = visitorIncome + oldAdeptIncome + frontRow;

        if (Upgrades.HasCandles && (isGoodOrBetter || isExcellent)) totalMoney *= candlesMultiplier;


        if (Upgrades.IsDayFive) totalMoney *= auditMultiplier;

        Money += totalMoney;

        auditory[selectedClass]--;

        if (TotalCount(auditory) <= 0) SwitchDay();
        UpdateUI();
    }
    private void InitiateAdepts()
    {
        int n = Mathf.Min(oldAdepts.Length, newAdepts.Length);    
        for(int i = 0; i < n; i++)
        {
            oldAdepts[i] += newAdepts[i];
            newAdepts[i] = 0;
        }
    }
    public void SwitchDay()
    {
        InitiateAdepts();
        auditory = (int[])defaultAuditoryValues.Clone();
        CurrentDay++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        DayLabel?.SetText($"День: {CurrentDay}");
        MoneyLabel?.SetText($"${Money:F2}");

        OldAdeptsCountLabel?.SetText($"Старые адепты: {TotalCount(oldAdepts)}");
        NewAdeptsCountLabel?.SetText($"Новые адепты: {TotalCount(newAdepts)}");

        AdeptWorkersLabel?.SetText($"Рабочие: {newAdepts[0]}");
        AdeptStudentsLabel?.SetText($"Студенты: {newAdepts[1]}");
        AdeptPensionersLabel?.SetText($"Пенсионеры: {newAdepts[2]}");
        AdeptBloggersLabel?.SetText($"Блогеры: {newAdepts[3]}");
        AdeptEsotericsLabel?.SetText($"Эзотерики: {newAdepts[4]}");

        AuditoryCountLabel?.SetText($"Аудитория: {TotalCount(auditory)}");
        AuditoryWorkersLabel?.SetText($"Рабочие: {auditory[0]}");
        AuditoryStudentsLabel?.SetText($"Студенты: {auditory[1]}");
        AuditoryPensionersLabel?.SetText($"Пенсионеры: {auditory[2]}");
        AuditoryBloggersLabel?.SetText($"Блогеры: {auditory[3]}");
        AuditoryEsotericsLabel?.SetText($"Эзотерики: {auditory[4]}");    
    }
}