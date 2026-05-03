using UnityEngine;
using TMPro;

// Legacy wrapper. Новый основной скрипт — StreetDayFlowController.
// Этот компонент оставлен, чтобы старые кнопки не сломались.
public class StreetEndPanelController : MonoBehaviour
{
    [SerializeField] private StreetDayFlowController streetDayFlowController;

    [Header("Fallback if StreetDayFlowController is not assigned")]
    [SerializeField] private GameObject streetPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private DayController dayController;
    [SerializeField] private CounterHuman counterHuman;

    private void Start()
    {
        // Ничего не запускаем автоматически.
        if (streetDayFlowController == null)
        {
            if (streetPanel != null) streetPanel.SetActive(false);
            if (endPanel != null) endPanel.SetActive(false);
        }
    }

    public void StartStreetButton()
    {
        if (streetDayFlowController != null)
        {
            streetDayFlowController.StartStreetButton();
            return;
        }

        if (streetPanel != null) streetPanel.SetActive(true);
        if (endPanel != null) endPanel.SetActive(false);
        if (dayController != null) dayController.StartDay();
    }

    public void ForceEndDayButton()
    {
        if (streetDayFlowController != null)
        {
            streetDayFlowController.ForceEndDayButton();
            return;
        }

        if (dayController != null) dayController.CompleteDay("День завершён вручную");
        ShowEndPanel("День завершён вручную");
    }

    public void NextDayButton()
    {
        if (streetDayFlowController != null)
        {
            streetDayFlowController.NextDayPrepareButton();
            return;
        }

        if (GameSessionBridge.Instance != null)
            GameSessionBridge.Instance.NextDay();

        if (streetPanel != null) streetPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
    }

    private void ShowEndPanel(string reason)
    {
        if (streetPanel != null) streetPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(true);
        if (titleText != null) titleText.text = reason;
        if (resultText != null)
        {
            int visitors = GameSessionBridge.Instance != null ? GameSessionBridge.Instance.TotalVisitors : (counterHuman != null ? counterHuman.GetTodayVisitors() : 0);
            int money = GameSessionBridge.Instance != null ? GameSessionBridge.Instance.CurrentMoney : (counterHuman != null ? counterHuman.CurrentMoney : 0);
            resultText.text = "Людей в мессе: " + visitors + "\nДеньги: " + money;
        }
    }
}
