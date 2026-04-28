using UnityEngine;
using TMPro;

public class ReportWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TotalAdeptsLabel;
    [SerializeField] private TextMeshProUGUI NewAdeptsLabel;
    [SerializeField] private TextMeshProUGUI DailyIncomeLabel;
    [SerializeField] private TextMeshProUGUI TotalIncomeLabel;
    [SerializeField] private TextMeshProUGUI SuspicionLabel;
    [SerializeField] private TextMeshProUGUI DetentionLabel;

    private void Start()
    {  
        TotalAdeptsLabel?.SetText($"Адептов всего: {Messa.Instance.TotalAdepts()}");
        NewAdeptsLabel?.SetText($"(+{Messa.Instance.NewAdepts()} новых)");
        DailyIncomeLabel?.SetText($"Доход за день: ${Messa.Instance.DailyIncome:F2}");
        TotalIncomeLabel?.SetText($"Доход за всю игру: ${Messa.Instance.TotalIncome:F2}");
    }
    private void OnEnable()
    {
        try
        {
            Start();
        }
        catch { }
    }
}
