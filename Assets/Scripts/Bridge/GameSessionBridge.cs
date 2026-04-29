using System;
using UnityEngine;
using TMPro;

public class GameSessionBridge : MonoBehaviour
{
    public static GameSessionBridge Instance { get; private set; }

    public event Action OnDataChanged;

    [Header("Core State")]
    [SerializeField] private int currentDay = 1;

    [Header("Last Messa Result")]
    [SerializeField] private int lastMoneyEarned = 0;
    [SerializeField] private int lastNewAdepts = 0;
    [SerializeField] private int lastLostAdepts = 0;
    [SerializeField] private int lastFaithEarned = 0;

    [Header("Optional Runtime Links")]
    [SerializeField] private DayController dayController;

    [Header("Optional Panels")]
    [SerializeField] private GameObject streetPanel;
    [SerializeField] private GameObject messaPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Optional TMP UI")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text adeptsText;
    [SerializeField] private TMP_Text faithText;
    [SerializeField] private TMP_Text visitorsText;
    [SerializeField] private TMP_Text visitorsByTypeText;
    [SerializeField] private TMP_Text lastResultText;

    [Header("Debug Launch")]
    [SerializeField] private int debugDay = 1;
    [SerializeField] private int debugMoney = 0;
    [SerializeField] private int debugAdepts = 0;
    [SerializeField] private int debugFaith = 0;

    [SerializeField] private int debugOfficeVisitors = 0;
    [SerializeField] private int debugStudentVisitors = 0;
    [SerializeField] private int debugRetireeVisitors = 0;
    [SerializeField] private int debugBloggerVisitors = 0;
    [SerializeField] private int debugEsotericVisitors = 0;
    public bool HasMegafon => Messa.Instance != null && Messa.Instance.IsUnlocked(Upgrades.DemonicMegaphone);
    public bool HasDevilAdvocate => Messa.Instance != null && Messa.Instance.IsUnlocked(Upgrades.DevilsAdvocate);
    public int CurrentDay => currentDay;

    public int LastMoneyEarned => lastMoneyEarned;
    public int LastNewAdepts => lastNewAdepts;
    public int LastLostAdepts => lastLostAdepts;
    public int LastFaithEarned => lastFaithEarned;

    private Messa M => Messa.Instance;

    private void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void LaunchLevel(
        int day,
        int money,
        int adepts,
        int faith,
        int office,
        int students,
        int retirees,
        int bloggers,
        int esoterics,
        bool openMessaPanel = false
    )
    {
        currentDay = Mathf.Clamp(day, 1, 5);

        if (M != null)
        {
            M.CurrentDay = currentDay;
            M.Money = money;
            M.InitiateFromBridge(adepts);
            ApplyAuditory(office, students, retirees, bloggers, esoterics);
        }

        ClearLastMessaResultOnly();

        if (dayController != null)
            dayController.SetDay(currentDay);

        if (openMessaPanel) OpenMessa();
        else OpenStreet();

        UpdateUI();
    }

    public void LaunchLevelFromInspector()
    {
        LaunchLevel(
            debugDay,
            debugMoney,
            debugAdepts,
            debugFaith,
            debugOfficeVisitors,
            debugStudentVisitors,
            debugRetireeVisitors,
            debugBloggerVisitors,
            debugEsotericVisitors
        );
    }

    public void LaunchMessaTest()
    {
        LaunchLevel(3, 40, 10, 4, 4, 3, 2, 2, 2, true);
    }

    public void LaunchStreetTest()
    {
        LaunchLevel(1, 0, 0, 0, 0, 0, 0, 0, 0, false);
    }

    public MessaInputData GetMessaInput()
    {
        if (M == null) return default;

        return new MessaInputData
        {
            day = M.CurrentDay,
            money = (int)M.Money,
            adepts = M.GetTotalAdeptsCount(),
            faith = 0,

            officeVisitors = M.Auditory[0],
            studentVisitors = M.Auditory[1],
            retireeVisitors = M.Auditory[2],
            bloggerVisitors = M.Auditory[3],
            esotericVisitors = M.Auditory[4],
            totalVisitors = M.TotalCount(M.Auditory),

            hasCookies = M.IsUnlocked(Upgrades.CookiesAfterMessa),
            hasAltar = M.IsUnlocked(Upgrades.BeautifulAltar),
            hasCandles = M.IsUnlocked(Upgrades.PremiumCandles),
            hasPaidFrontRow = M.IsUnlocked(Upgrades.PaidFrontRow),
            hasChoir = M.IsUnlocked(Upgrades.Choir)
        };
    }
    public int TotalVisitors
    {
        get
        {
            if (Messa.Instance == null) return 0;
            return Messa.Instance.TotalCount(Messa.Instance.Auditory);
        }
    }

    public int CurrentMoney
    {
        get
        {
            if (Messa.Instance == null) return 0;
            return (int)Messa.Instance.Money;
        }
    }
    public void ApplyMessaResultFull(int moneyEarned, int newAdepts, int lostAdepts, int faithEarned)
    {
        lastMoneyEarned = Mathf.Max(0, moneyEarned);
        lastNewAdepts = Mathf.Max(0, newAdepts);
        lastLostAdepts = Mathf.Max(0, lostAdepts);
        lastFaithEarned = Mathf.Max(0, faithEarned);

        if (M != null)
        {
            M.Money += lastMoneyEarned;
            M.AddAdepts(lastNewAdepts - lastLostAdepts);
        }

        UpdateUI();
    }

    public void AddVisitorToMessa(string type)
    {
        if (M == null) return;

        switch (type)
        {
            case "office":
            case "worker":
                M.Auditory[0]++;
                break;

            case "student":
                M.Auditory[1]++;
                break;

            case "retiree":
                M.Auditory[2]++;
                break;

            case "blogger":
                M.Auditory[3]++;
                break;

            case "esoteric":
                M.Auditory[4]++;
                break;
        }

        UpdateUI();
    }

    public void AddVisitorFromObject(GameObject obj)
    {
        if (obj == null) return;

        string n = obj.name.ToLower();

        if (n.Contains("office") || n.Contains("worker")) AddVisitorToMessa("office");
        else if (n.Contains("student")) AddVisitorToMessa("student");
        else if (n.Contains("retiree")) AddVisitorToMessa("retiree");
        else if (n.Contains("blogger")) AddVisitorToMessa("blogger");
        else if (n.Contains("esoteric")) AddVisitorToMessa("esoteric");
    }

    public void ClearVisitorsForNewDay()
    {
        if (M == null) return;

        for (int i = 0; i < 5; i++)
            M.Auditory[i] = 0;

        UpdateUI();
    }

    public void ClearLastMessaResultOnly()
    {
        lastMoneyEarned = 0;
        lastNewAdepts = 0;
        lastLostAdepts = 0;
        lastFaithEarned = 0;
    }

    public void NextDay()
    {
        currentDay = Mathf.Clamp(currentDay + 1, 1, 5);

        ClearVisitorsForNewDay();
        ClearLastMessaResultOnly();

        if (dayController != null)
            dayController.SetDay(currentDay);

        OpenStreet();
        UpdateUI();
    }

    public void OpenStreet()
    {
        if (streetPanel) streetPanel.SetActive(true);
        if (messaPanel) messaPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);
    }

    public void OpenMessa()
    {
        if (streetPanel) streetPanel.SetActive(false);
        if (messaPanel) messaPanel.SetActive(true);
        if (resultPanel) resultPanel.SetActive(false);

        UpdateUI();
    }

    public void OpenResult()
    {
        if (streetPanel) streetPanel.SetActive(false);
        if (messaPanel) messaPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(true);

        UpdateUI();
    }

    private void ApplyAuditory(int o, int s, int r, int b, int e)
    {
        if (M == null) return;

        M.Auditory[0] = o;
        M.Auditory[1] = s;
        M.Auditory[2] = r;
        M.Auditory[3] = b;
        M.Auditory[4] = e;
    }

    private void UpdateUI()
    {
        if (M != null)
        {
            if (dayText) dayText.text = $"День: {M.CurrentDay}";
            if (moneyText) moneyText.text = $"Деньги: {(int)M.Money}";
            if (adeptsText) adeptsText.text = $"Адепты: {M.GetTotalAdeptsCount()}";

            if (visitorsText) visitorsText.text = $"В мессе: {M.TotalCount(M.Auditory)}";

            if (visitorsByTypeText)
            {
                visitorsByTypeText.text =
                    $"Офис: {M.Auditory[0]}\n" +
                    $"Студенты: {M.Auditory[1]}\n" +
                    $"Пенсионеры: {M.Auditory[2]}\n" +
                    $"Блогеры: {M.Auditory[3]}\n" +
                    $"Эзотерики: {M.Auditory[4]}";
            }
        }

        if (lastResultText)
        {
            lastResultText.text =
                $"Доход: {lastMoneyEarned}\n" +
                $"Адепты: +{lastNewAdepts} / -{lastLostAdepts}\n" +
                $"Вера: {lastFaithEarned}";
        }

        OnDataChanged?.Invoke();
    }
}

[Serializable]
public struct MessaInputData
{
    public int day;
    public int money;
    public int adepts;
    public int faith;

    public int officeVisitors;
    public int studentVisitors;
    public int retireeVisitors;
    public int bloggerVisitors;
    public int esotericVisitors;
    public int totalVisitors;

    public bool hasCookies;
    public bool hasAltar;
    public bool hasCandles;
    public bool hasPaidFrontRow;
    public bool hasChoir;
}