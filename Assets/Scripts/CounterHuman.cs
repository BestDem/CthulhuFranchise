using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterHuman : MonoBehaviour
{
    [Header("TMP UI")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text messaVisitorsText;

    [Header("Money")]
    [SerializeField] private int startMoney = 0;

    [Tooltip("Оставь false, если деньги начисляются только на мессе. Включай true только если хочешь старую логику денег на улице.")]
    [SerializeField] private bool addMoneyOnStreet = false;

    [SerializeField] private int workerIncome = 4;
    [SerializeField] private int studentIncome = 1;
    [SerializeField] private int retireeIncome = 2;
    [SerializeField] private int bloggerIncome = 2;
    [SerializeField] private int esotericIncome = 2;

    [Header("Text Settings")]
    [SerializeField] private string moneyPrefix = "Деньги: ";
    [SerializeField] private string visitorsPrefix = "В мессе: ";

    private int currentMoney;
    private int todayVisitors;
    private int totalRecruited;

    private readonly Dictionary<string, int> countRecruit = new Dictionary<string, int>();

    public int CurrentMoney => currentMoney;
    public int TodayVisitors => todayVisitors;
    public int TotalRecruited => totalRecruited;

    private void Awake()
    {
        currentMoney = startMoney;
        UpdateUI();
    }

    private void OnEnable()
    {
        DayController.dayEnd += OnDayEnd;
    }

    private void OnDisable()
    {
        DayController.dayEnd -= OnDayEnd;
    }

    private void OnDayEnd(bool end)
    {
        if (!end)
        {
            todayVisitors = 0;
            UpdateUI();
        }
    }

    public void AddHuman(int add, string humanName)
    {
        if (add <= 0)
            return;

        todayVisitors += add;
        totalRecruited += add;

        if (!countRecruit.ContainsKey(humanName))
            countRecruit.Add(humanName, 0);

        countRecruit[humanName] += add;

        int income = 0;
        if (addMoneyOnStreet)
        {
            income = GetIncomeByHumanName(humanName) * add;
            currentMoney += income;
        }

        Debug.Log("В мессе: +" + add + " " + humanName + " | Сегодня: " + todayVisitors + " | Деньги на улице: " + income);

        UpdateUI();
    }

    public void AddMoney(int value)
    {
        currentMoney += value;
        UpdateUI();
    }

    public bool TrySpendMoney(int value)
    {
        if (currentMoney < value)
            return false;

        currentMoney -= value;
        UpdateUI();
        return true;
    }

    public int GetTotalRecruited()
    {
        return totalRecruited;
    }

    public int GetTodayVisitors()
    {
        return todayVisitors;
    }

    public int GetHumanTypeCount(string humanName)
    {
        return countRecruit.TryGetValue(humanName, out int value) ? value : 0;
    }

    public void ClearTodayVisitors()
    {
        todayVisitors = 0;
        UpdateUI();
    }

    private int GetIncomeByHumanName(string humanName)
    {
        switch (humanName)
        {
            case "worker":
                return workerIncome;
            case "student":
                return studentIncome;
            case "retiree":
                return retireeIncome;
            case "blogger":
                return bloggerIncome;
            case "esoteric":
                return esotericIncome;
            default:
                return 0;
        }
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = moneyPrefix + currentMoney;

        if (messaVisitorsText != null)
            messaVisitorsText.text = visitorsPrefix + todayVisitors;
    }
}
