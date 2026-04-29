using System;
using UnityEngine;
using TMPro;

public class DayController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ListReactions listReactions;
    [SerializeField, Min(1)] private int startDay = 1;

    [Header("TMP UI")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text timerText;

    [Header("Text Settings")]
    [SerializeField] private string dayPrefix = "День ";
    [SerializeField] private string finalText = "Финал";

    public static event Action<bool> dayEnd;

    private int currentDayIndex;
    private float timer;
    private bool isDay;
    private bool waitingForNextDay;

    public int CurrentDayNumber => currentDayIndex + 1;
    public float CurrentTimer => timer;
    public bool IsDay => isDay;
    public string LastEndReason { get; private set; } = "День завершён";

    private void Start()
    {
        currentDayIndex = Mathf.Clamp(startDay - 1, 0, GetLastDayIndex());
        StartCurrentDay(false);
    }

    private void Update()
    {
        if (!isDay)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ForceEndDay("Время вышло");
            return;
        }

        UpdateTimerText();
    }

    // Можно повесить на кнопку "Следующий день".
    public void StartDay()
    {
        if (waitingForNextDay)
        {
            if (currentDayIndex >= GetLastDayIndex())
            {
                Debug.Log("Все дни закончились. Нужно открыть финальный экран.");
                if (dayText != null)
                    dayText.text = finalText;
                return;
            }

            currentDayIndex++;
        }

        StartCurrentDay(true);
    }

    public void SetDay(int dayNumber)
    {
        currentDayIndex = Mathf.Clamp(dayNumber - 1, 0, GetLastDayIndex());
        StartCurrentDay(true);
    }

    public void RestartCurrentDay()
    {
        StartCurrentDay(true);
    }

    public void ForceEndDay(string reason)
    {
        if (!isDay && waitingForNextDay)
            return;

        timer = 0f;
        isDay = false;
        waitingForNextDay = true;
        LastEndReason = string.IsNullOrEmpty(reason) ? "День завершён" : reason;
        UpdateUI();

        Debug.Log("День закончен: " + CurrentDayNumber + " | Причина: " + LastEndReason);
        dayEnd?.Invoke(true);
    }

    private void StartCurrentDay(bool notifySystems)
    {
        timer = GetDayDuration(currentDayIndex);
        isDay = true;
        waitingForNextDay = false;
        LastEndReason = "День завершён";
        UpdateUI();

        if (notifySystems)
            dayEnd?.Invoke(false);
    }

    private int GetLastDayIndex()
    {
        if (listReactions == null || listReactions.LenDaySec == null || listReactions.LenDaySec.Length == 0)
            return 0;

        return listReactions.LenDaySec.Length - 1;
    }

    private int GetDayDuration(int index)
    {
        if (listReactions == null || listReactions.LenDaySec == null || listReactions.LenDaySec.Length == 0)
            return 30;

        index = Mathf.Clamp(index, 0, listReactions.LenDaySec.Length - 1);
        return listReactions.LenDaySec[index];
    }

    private void UpdateUI()
    {
        if (dayText != null)
            dayText.text = dayPrefix + CurrentDayNumber;

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int seconds = Mathf.CeilToInt(timer);
        timerText.text = seconds.ToString();
    }
}
