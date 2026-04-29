using UnityEngine;
using TMPro;

public class StreetEndPanelController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject streetPanel;
    [SerializeField] private GameObject endPanel;

    [Header("TextMeshPro")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;

    [Header("Links")]
    [SerializeField] private DayController dayController;
    [SerializeField] private CounterHuman counterHuman;

    [Header("Text")]
    [SerializeField] private string defaultTitle = "День завершён";
    [SerializeField] private string visitorsLine = "Людей в мессе: ";
    [SerializeField] private string moneyLine = "Деньги: ";


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
        ShowStreet();
    }

    private void OnDayEnd(bool ended)
    {
        if (ended)
            ShowEndPanel();
        else
            ShowStreet();
    }

    private void ShowStreet()
    {
        if (streetPanel != null)
          //  streetPanel.SetActive(true);

        if (endPanel != null)
            endPanel.SetActive(false);
    }

    private void ShowEndPanel()
    {
        if (streetPanel != null)
            streetPanel.SetActive(false);

        if (endPanel != null)
            endPanel.SetActive(true);

        string reason = dayController != null ? dayController.LastEndReason : defaultTitle;

        if (titleText != null)
            titleText.text = reason;

        if (resultText != null)
        {
            int visitors = GameSessionBridge.Instance != null
                ? GameSessionBridge.Instance.TotalVisitors
                : (counterHuman != null ? counterHuman.GetTodayVisitors() : 0);

            int money = GameSessionBridge.Instance != null
                ? GameSessionBridge.Instance.CurrentMoney
                : (counterHuman != null ? counterHuman.CurrentMoney : 0);

            resultText.text = visitorsLine + visitors + "\n" + moneyLine + money;
        }
    }

    public void NextDayButton()
    {
        if (dayController != null)
            dayController.StartDay();
    }
}
