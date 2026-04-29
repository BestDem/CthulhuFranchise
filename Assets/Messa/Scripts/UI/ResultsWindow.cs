using UnityEngine;
using TMPro;

public class ResultsWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TotalAdeptsLabel;
    [SerializeField] private TextMeshProUGUI NewAdeptsLabel;
    [SerializeField] private TextMeshProUGUI DailyIncomeLabel;

    private void Start()
    {  
        TotalAdeptsLabel?.SetText($"Адептов всего: {Messa.Instance.TotalAdepts()}");
        NewAdeptsLabel?.SetText(Messa.Instance.GetAdeptsChange());
        DailyIncomeLabel?.SetText($"${(int)Messa.Instance.DailyIncome}");
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
