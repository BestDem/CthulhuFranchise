using UnityEngine;
using TMPro;


public class Messa : MonoBehaviour
{
    public float Money;

    
    [SerializeField] private int[] adepts = new int[5];
    [SerializeField] private int[] auditory = new int[5];
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

    [Header("Пороги доли аудитории")]
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

    [Header("Бонусы дохода")]
    [SerializeField] private float premiumIncomeMultiplier = 1.25f;
    [SerializeField] private float paidFrontRowBonus = 1.5f;
    [SerializeField] private float oldAdeptIncomeMultiplier = 0.45f;
    [SerializeField] private float candlesMultiplier = 1.20f;
    [SerializeField] private float auditMultiplier = 1.10f;

    [Header("UI")]   
    [SerializeField] private TextMeshProUGUI AdeptsCountLabel;
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
        UpdateUI();
        BooleanUpgrades.ApplyToStatic();
    }
    public void SpellSermon(int peopleClass)
    {
        int totalVisitors = TotalCount(auditory);
        if (totalVisitors == 0) return;

        int matchedVisitors = auditory[peopleClass];
        float share = (float)matchedVisitors / totalVisitors;

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
        int esotericsCount = auditory[4];
        float esotericBonus = Mathf.Min(esotericsCount * esotericBonusPerUnit, esotericBonusCap);

        for (int i = 0; i < auditory.Length; i++)
        {
            float chance = baseConversion[i] * multiplier;

            if (Upgrades.HasCookie && i == 1) chance += cookieBonus;

            if (Upgrades.HasAltar)
            {
                if (isExcellent) chance += altarExcellentBonus;
                else if (isGoodOrBetter) chance += altarGoodBonus;
            }

            chance += esotericBonus;

            if (Upgrades.HasPremiumFlyer) chance += premiumFlyerBonus;

            chance = Mathf.Min(chance, maxConversionChance);

            int newAdepts = Mathf.RoundToInt(auditory[i] * chance);
            adepts[i] += newAdepts;
        }

        if (isBad)
        {
            float churnRate = baseChurn - auditory[2] * pensionerChurnReduce;
            churnRate = Mathf.Max(churnRate, minChurn);

            if (Upgrades.HasChoir) churnRate *= choirMultiplier;

            for (int i = 0; i < adepts.Length; i++)
            {
                int lost = Mathf.RoundToInt(adepts[i] * churnRate);
                adepts[i] -= lost;
            }
        }

        float visitorIncome = 0f;

        for (int i = 0; i < auditory.Length; i++)
        {
            float income = baseIncome[i] * auditory[i];
            if (Upgrades.HasPremiumFlyer) income *= premiumIncomeMultiplier;
            visitorIncome += income;
        }

        float oldAdeptIncome = TotalCount(adepts) * oldAdeptIncomeMultiplier;

        float paidFrontRow = Upgrades.HasPaidFrontRow ? auditory[0] * paidFrontRowBonus : 0f;

        float rawRevenue = visitorIncome + oldAdeptIncome + paidFrontRow;

        float revenueMultiplier = 1f;

        if (Upgrades.HasCandles && isGoodOrBetter) revenueMultiplier *= candlesMultiplier;
        if (Upgrades.IsDayFive) revenueMultiplier *= auditMultiplier;

        float finalRevenue = rawRevenue * revenueMultiplier;

        Money += finalRevenue;

        for (int i = 0; i < auditory.Length; i++) auditory[i] = 0;

        UpdateUI();
    }

    private void UpdateUI()
    {
        AuditoryWorkersLabel.SetText($"Рабочие: {auditory[0]}");
        AuditoryStudentsLabel.SetText($"Студенты: {auditory[1]}");
        AuditoryPensionersLabel.SetText($"Пенсионеры: {auditory[2]}");
        AuditoryBloggersLabel.SetText($"Блогеры: {auditory[3]}");
        AuditoryEsotericsLabel.SetText($"Эзотерики: {auditory[4]}");

        AdeptWorkersLabel.SetText($"Рабочие: {adepts[0]}");
        AdeptStudentsLabel.SetText($"Студенты: {adepts[1]}");
        AdeptPensionersLabel.SetText($"Пенсионеры: {adepts[2]}");
        AdeptBloggersLabel.SetText($"Блогеры: {adepts[3]}");
        AdeptEsotericsLabel.SetText($"Эзотерики: {adepts[4]}");

        AuditoryCountLabel.SetText($"Аудитория: {TotalCount(auditory)}");
        AdeptsCountLabel.SetText($"Адепты: {TotalCount(adepts)}");
    }
}