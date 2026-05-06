using UnityEngine;
using TMPro;

public class ResultsWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI DailyBaseIncomeLabel;
    [SerializeField] private TextMeshProUGUI DailyTotalIncomeLabel;
    [SerializeField] private TextMeshProUGUI newAdeptsLabel;
    [SerializeField] private TextMeshProUGUI adeptsOutflowLabel;
    [SerializeField] private TextMeshProUGUI faithIncomeLabel;
    [SerializeField] private TextMeshProUGUI oldAdeptsCalculationLabel;
    [SerializeField] private TextMeshProUGUI oldAdeptsIncomeLabel;

    [SerializeField] private TextMeshProUGUI firstRowBonusLabel;
    [SerializeField] private TextMeshProUGUI candleBonusLabel;
    [SerializeField] private TextMeshProUGUI premiumFlyerBonusLabel;
    [SerializeField] private TextMeshProUGUI messaResultLabel;

    [SerializeField] private TextMeshProUGUI[] visitorsCountLabels;
    [SerializeField] private TextMeshProUGUI[] adeptsConversionLabels;
    [SerializeField] private TextMeshProUGUI[] newAdeptsCountLabels;
    [SerializeField] private TextMeshProUGUI[] incomeCalculationLabels;
    [SerializeField] private TextMeshProUGUI[] baseIncomeLabels;

    
    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        try
        {
            newAdeptsLabel?.SetText(Messa.Instance.GetNewAdeptsCount());
            adeptsOutflowLabel?.SetText(Messa.Instance.GetAdeptsOutflow());
            faithIncomeLabel?.SetText($"{Messa.Instance.GetFaithIncome()}");
            messaResultLabel?.SetText(Messa.Instance.MessaResult);

            if (Messa.Instance.IsUnlocked(Upgrades.PaidFrontRow))
            {
                firstRowBonusLabel?.SetText($"+{Messa.Instance.PaidFrontRowBonus * Messa.Instance.Auditory[0]}");
            }
            else firstRowBonusLabel?.SetText("нет улучшения");

            if (Messa.Instance.IsUnlocked(Upgrades.PremiumCandles))
            {
                candleBonusLabel?.SetText($"*{Messa.Instance.CandlesMultiplier}");
            }
            else candleBonusLabel?.SetText("нет улучшения");

            if (Messa.Instance.IsUnlocked(Upgrades.PremiumFlyer))
            {
                premiumFlyerBonusLabel?.SetText($"*{Messa.Instance.PremiumFlyerBonus} ");
            }
            else premiumFlyerBonusLabel?.SetText("нет улучшения");

            for (int i = 0; i < 5; i++)
            {
                visitorsCountLabels[i]?.SetText($"{Messa.Instance.Auditory[i]}");
                adeptsConversionLabels[i]?.SetText($"{(int)(Messa.Instance.BaseConversion[i] * Messa.Instance.ConversionMultiplier * 100f)}%");
                newAdeptsCountLabels[i]?.SetText($"{Messa.Instance.NewAdepts[i]}");
                incomeCalculationLabels[i]?.SetText($"{Messa.Instance.Auditory[i]} * {Messa.Instance.BaseIncome[i]}");
                baseIncomeLabels[i]?.SetText($"${Messa.Instance.Auditory[i] * Messa.Instance.BaseIncome[i]}");
            }
            oldAdeptsCalculationLabel?.SetText($"{Messa.Instance.GetOldAdeptsCount()} * {Messa.Instance.OldAdeptIncomeMultiplier}");
            oldAdeptsIncomeLabel?.SetText($"${Messa.Instance.GetOldAdeptsCount() * Messa.Instance.OldAdeptIncomeMultiplier}");
            DailyBaseIncomeLabel?.SetText($"${(int)Messa.Instance.DailyBaseIncome}");
            DailyTotalIncomeLabel?.SetText($"Итоговый доход: ${(int)Messa.Instance.DailyMoneyIncome}");
        }
        catch { }     
    }
}