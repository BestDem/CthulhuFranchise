using UnityEngine;
using TMPro;

public class GameSessionBridge : MonoBehaviour
{
    public static GameSessionBridge Instance;

    [System.Serializable]
    public struct MessaInput
    {
        public int currentDay;
        public int currentMoney;
        public int totalAdepts;
        public int faith;

        public int officeVisitors;
        public int studentVisitors;
        public int retireeVisitors;
        public int bloggerVisitors;
        public int esotericVisitors;
        public int totalVisitors;

        public bool hasMegafon;
        public bool hasDevilAdvocate;
        public bool hasSelfImprovementClub;
        public bool hasPremiumFlyer;
        public bool hasCthulhuMerch;
        public bool hasWordOfMouth;
        public bool hasAbyssAccountant;
        public bool hasCookies;
        public bool hasAltar;
        public bool hasCandles;
        public bool hasPaidFrontRow;
        public bool hasChoir;

        public bool arrestHappenedToday;
        public bool devilAdvocateUsedToday;
        public float maxSuspicionToday;
    }

    [System.Serializable]
    public struct StreetPlan
    {
        public int day;
        public int duration;
        public int baseHumans;
        public int finalHumans;
        public int maxHumans;
        public int bonusHumansFromBloggers;
        public float bloggerFlowBonusPercent;
        public bool policeActive;
        public bool megafonActive;
        public bool devilAdvocateActive;
        public bool selfImprovementActive;
        public bool merchActive;
        public bool accountantActive;
        public int accountantMoney;
        public string description;
    }

    [Header("Core State")]
    [SerializeField, Min(1)] private int currentDay = 1;
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private int totalAdepts = 0;
    [SerializeField] private int faith = 0;

    [Header("Old adepts by type / future bonuses")]
    [SerializeField] private int officeAdeptCount = 0;
    [SerializeField] private int studentAdeptCount = 0;
    [SerializeField] private int retireeAdeptCount = 0;
    [SerializeField] private int bloggerAdeptCount = 0;
    [SerializeField] private int esotericAdeptCount = 0;

    [Header("Visitors Collected Today")]
    [SerializeField] private int officeVisitors = 0;
    [SerializeField] private int studentVisitors = 0;
    [SerializeField] private int retireeVisitors = 0;
    [SerializeField] private int bloggerVisitors = 0;
    [SerializeField] private int esotericVisitors = 0;

    [Header("Street / passive upgrades")]
    [SerializeField] private bool hasMegafon = false;
    [SerializeField] private bool hasDevilAdvocate = false;
    [SerializeField] private bool hasSelfImprovementClub = false;
    [SerializeField] private bool hasPremiumFlyer = false;
    [SerializeField] private bool hasCthulhuMerch = false;
    [SerializeField] private bool hasWordOfMouth = false;
    [SerializeField] private bool hasAbyssAccountant = false;

    [Header("Messa upgrades")]
    [SerializeField] private bool hasCookies = false;
    [SerializeField] private bool hasAltar = false;
    [SerializeField] private bool hasCandles = false;
    [SerializeField] private bool hasPaidFrontRow = false;
    [SerializeField] private bool hasChoir = false;

    [Header("Street day telemetry")]
    [SerializeField] private bool arrestHappenedToday = false;
    [SerializeField] private bool devilAdvocateUsedToday = false;
    [SerializeField] private float maxSuspicionToday = 0f;

    [Header("Balance")]
    [SerializeField] private ListReactions listReactions;
    [SerializeField] private int[] maxHumansWithBonusesByDay = new int[] { 10, 20, 25, 30, 30 };
    [SerializeField] private float bloggerFlowBonusPerAdept = 0.02f;
    [SerializeField] private float maxBloggerFlowBonus = 0.30f;
    [SerializeField] private int abyssAccountantStartMoney = 3;

    [Header("Panels")]
    [SerializeField] private GameObject streetPreparePanel;
    [SerializeField] private GameObject streetPanel;
    [SerializeField] private GameObject messaPanel;
    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private GameObject endGamePanel;

    [Header("TMP UI")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text messaVisitorsText;
    [SerializeField] private TMP_Text adeptsText;
    [SerializeField] private TMP_Text faithText;
    [SerializeField] private TMP_Text streetPlanText;

    [Header("Debug Launch From Inspector")]
    [SerializeField] private int debugDay = 1;
    [SerializeField] private int debugMoney = 30;
    [SerializeField] private int debugAdepts = 5;
    [SerializeField] private int debugFaith = 0;
    [SerializeField] private int debugOfficeVisitors = 3;
    [SerializeField] private int debugStudentVisitors = 3;
    [SerializeField] private int debugRetireeVisitors = 1;
    [SerializeField] private int debugBloggerVisitors = 1;
    [SerializeField] private int debugEsotericVisitors = 1;
    [SerializeField] private bool debugHasMegafon;
    [SerializeField] private bool debugHasDevilAdvocate;
    [SerializeField] private bool debugHasCookies;
    [SerializeField] private bool debugHasAltar;
    [SerializeField] private bool debugHasCandles;
    [SerializeField] private bool debugHasPaidFrontRow;
    [SerializeField] private bool debugHasChoir;

    public int CurrentDay => currentDay;
    public int CurrentMoney => currentMoney;
    public int TotalAdepts => totalAdepts;
    public int Faith => faith;

    public int OfficeVisitors => officeVisitors;
    public int StudentVisitors => studentVisitors;
    public int RetireeVisitors => retireeVisitors;
    public int BloggerVisitors => bloggerVisitors;
    public int EsotericVisitors => esotericVisitors;
    public int TotalVisitors => officeVisitors + studentVisitors + retireeVisitors + bloggerVisitors + esotericVisitors;

    public int BloggerAdeptCount => bloggerAdeptCount;

    public bool HasMegafon => hasMegafon;
    public bool HasDevilAdvocate => hasDevilAdvocate;
    public bool HasSelfImprovementClub => hasSelfImprovementClub;
    public bool HasPremiumFlyer => hasPremiumFlyer;
    public bool HasCthulhuMerch => hasCthulhuMerch;
    public bool HasWordOfMouth => hasWordOfMouth;
    public bool HasAbyssAccountant => hasAbyssAccountant;
    public bool HasCookies => hasCookies;
    public bool HasAltar => hasAltar;
    public bool HasCandles => hasCandles;
    public bool HasPaidFrontRow => hasPaidFrontRow;
    public bool HasChoir => hasChoir;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    private void Start()
    {
        OpenStreet();
        UpdateStreetPlanText();
    }


    public void OpenStreet()
    {
        streetPreparePanel?.SetActive(false);
        messaPanel?.SetActive(false);
        endLevelPanel?.SetActive(false);
        endGamePanel?.SetActive(false);

        streetPanel?.SetActive(true);
        StreetDayFlowController.Instance.StartStreetButton();
    }

    public void OpenMessa()
    {
        streetPreparePanel?.SetActive(false);
        streetPanel?.SetActive(false); 
        endLevelPanel?.SetActive(false);
        endGamePanel?.SetActive(false);
        messaPanel?.SetActive(true);
    }

    public void OpenResult()
    {
        streetPreparePanel?.SetActive(false);
        streetPanel?.SetActive(false);
        messaPanel?.SetActive(false);    

        if (CurrentDay < 5)
        {
            endLevelPanel?.SetActive(true);
        }
        else endGamePanel?.SetActive(true);  
    }

    // Кнопка 1: загрузить параметры улицы и показать игроку/программисту, что будет в этом дне.
    public void LoadStreetParametersButton()
    {
        OpenStreet();
        UpdateStreetPlanText();
    }

    public StreetPlan BuildStreetPlan()
    {
        StreetPlan plan = new StreetPlan();
        plan.day = Mathf.Max(1, currentDay);
        int dayIndex = plan.day - 1;

        plan.duration = GetDuration(dayIndex);
        plan.baseHumans = GetBaseHumans(dayIndex);
        plan.maxHumans = GetMaxHumans(dayIndex);

        float bloggerBonus = Mathf.Clamp(bloggerAdeptCount * bloggerFlowBonusPerAdept, 0f, maxBloggerFlowBonus);
        plan.bloggerFlowBonusPercent = bloggerBonus * 100f;
        int withBonus = Mathf.RoundToInt(plan.baseHumans * (1f + bloggerBonus));
        plan.finalHumans = Mathf.Clamp(withBonus, plan.baseHumans, plan.maxHumans);
        plan.bonusHumansFromBloggers = Mathf.Max(0, plan.finalHumans - plan.baseHumans);

        plan.policeActive = plan.day >= 2;
        plan.megafonActive = hasMegafon;
        plan.devilAdvocateActive = hasDevilAdvocate;
        plan.selfImprovementActive = hasSelfImprovementClub;
        plan.merchActive = hasCthulhuMerch;
        plan.accountantActive = hasAbyssAccountant;
        plan.accountantMoney = hasAbyssAccountant ? abyssAccountantStartMoney : 0;
        plan.description = BuildStreetPlanDescription(plan);
        return plan;
    }

    public string BuildStreetPlanDescription(StreetPlan plan)
    {
        string text = "УЛИЦА: ДЕНЬ " + plan.day + "\n";
        text += "Время улицы: " + plan.duration + " сек.\n";
        text += "Прохожих сегодня: " + plan.finalHumans + "\n";
        text += "База дня: " + plan.baseHumans + " / максимум: " + plan.maxHumans + "\n";

        if (plan.bonusHumansFromBloggers > 0)
            text += "Блогеры-адепты: +" + plan.bonusHumansFromBloggers + " прохожих (" + plan.bloggerFlowBonusPercent.ToString("0") + "%).\n";
        else
            text += "Бонус блогеров: нет.\n";

        text += plan.policeActive ? "Полиция: активна.\n" : "Полиция: нет в этот день.\n";
        text += plan.megafonActive ? "Мегафон: активен, цепляет рядом людей того же типа.\n" : "Мегафон: нет.\n";
        text += plan.devilAdvocateActive ? "Адвокат дьявола: спасает 1 раз от полиции.\n" : "Адвокат дьявола: нет.\n";
        text += plan.selfImprovementActive ? "Клуб саморазвития: подозрение растёт слабее на 30%.\n" : "Клуб саморазвития: нет.\n";
        text += plan.merchActive ? "Мерч Ктулху: блогеры появляются чаще.\n" : "Мерч Ктулху: нет.\n";
        text += plan.accountantActive ? "Бухгалтер бездны: +" + plan.accountantMoney + " денег в начале дня.\n" : "Бухгалтер бездны: нет.\n";
        return text;
    }

    public void UpdateStreetPlanText()
    {
        if (streetPlanText != null)
            streetPlanText.text = BuildStreetPlan().description;
    }

    public void PrepareNewStreetDay()
    {
        ClearVisitorsForNewStreetDay();
        arrestHappenedToday = false;
        devilAdvocateUsedToday = false;
        maxSuspicionToday = 0f;

        if (hasAbyssAccountant)
            AddMoney(abyssAccountantStartMoney);

        UpdateStreetPlanText();
        UpdateUI();
    }

    public int GetStreetHumanCountForCurrentDay()
    {
        return BuildStreetPlan().finalHumans;
    }

    public void AddVisitorToMessa(string humanType)
    {
        switch (NormalizeHumanType(humanType))
        {
            case "worker": officeVisitors++; break;
            case "student": studentVisitors++; break;
            case "retiree": retireeVisitors++; break;
            case "blogger": bloggerVisitors++; break;
            case "esoteric": esotericVisitors++; break;
            default:
                Debug.LogWarning("GameSessionBridge: неизвестный тип посетителя: " + humanType);
                break;
        }

        UpdateUI();
    }

    public void AddVisitorFromObject(GameObject obj)
    {
        if (obj == null)
            return;

        Human human = obj.GetComponent<Human>();
        if (human == null)
            human = obj.GetComponentInParent<Human>();

        if (human != null)
            AddVisitorToMessa(human.HumanType);
    }

    public void ClearVisitorsForNewStreetDay()
    {
        officeVisitors = 0;
        studentVisitors = 0;
        retireeVisitors = 0;
        bloggerVisitors = 0;
        esotericVisitors = 0;
        UpdateUI();
    }

    public MessaInput GetMessaInput()
    {
        MessaInput input = new MessaInput();
        input.currentDay = currentDay;
        input.currentMoney = currentMoney;
        input.totalAdepts = totalAdepts;
        input.faith = faith;

        input.officeVisitors = officeVisitors;
        input.studentVisitors = studentVisitors;
        input.retireeVisitors = retireeVisitors;
        input.bloggerVisitors = bloggerVisitors;
        input.esotericVisitors = esotericVisitors;
        input.totalVisitors = TotalVisitors;

        input.hasMegafon = hasMegafon;
        input.hasDevilAdvocate = hasDevilAdvocate;
        input.hasSelfImprovementClub = hasSelfImprovementClub;
        input.hasPremiumFlyer = hasPremiumFlyer;
        input.hasCthulhuMerch = hasCthulhuMerch;
        input.hasWordOfMouth = hasWordOfMouth;
        input.hasAbyssAccountant = hasAbyssAccountant;
        input.hasCookies = hasCookies;
        input.hasAltar = hasAltar;
        input.hasCandles = hasCandles;
        input.hasPaidFrontRow = hasPaidFrontRow;
        input.hasChoir = hasChoir;

        input.arrestHappenedToday = arrestHappenedToday;
        input.devilAdvocateUsedToday = devilAdvocateUsedToday;
        input.maxSuspicionToday = maxSuspicionToday;
        return input;
    }
    public void ApplyMessaResult(float moneyIncome, int faithIncome, int adeptsCount)
    {
        currentMoney += (int)moneyIncome;
        totalAdepts += adeptsCount;
        totalAdepts -= adeptsCount;
        faith += faithIncome;      
        if (faith < 0) faith = 0;
        currentDay++;
        UpdateUI();
    }

    public void SetArrestHappenedToday(bool value) { arrestHappenedToday = value; }
    public void SetDevilAdvocateUsedToday(bool value) { devilAdvocateUsedToday = value; }
    public void RegisterSuspicion(float value) { if (value > maxSuspicionToday) maxSuspicionToday = value; }

    public void AddMoney(int value)
    {
        currentMoney += value;
        UpdateUI();
    }

    public bool SpendMoney(int value)
    {
        if (currentMoney < value)
            return false;

        currentMoney -= value;
        UpdateUI();
        return true;
    }

    public void NextDay()
    {
        currentDay++;
        if (currentDay < 1) currentDay = 1;
        ClearVisitorsForNewStreetDay();
        UpdateStreetPlanText();
        UpdateUI();
    }

    public void RestartCurrentDay()
    {
        ClearVisitorsForNewStreetDay();
        arrestHappenedToday = false;
        devilAdvocateUsedToday = false;
        maxSuspicionToday = 0f;
        UpdateStreetPlanText();
        UpdateUI();
    }

    public void SetDay(int day)
    {
        currentDay = Mathf.Max(1, day);
        ClearVisitorsForNewStreetDay();
        UpdateStreetPlanText();
        UpdateUI();
    }

    public void SetDay1() { SetDay(1); }
    public void SetDay2() { SetDay(2); }
    public void SetDay3() { SetDay(3); }
    public void SetDay4() { SetDay(4); }
    public void SetDay5() { SetDay(5); }

    public void LaunchLevel(
        int day,
        int money,
        int adepts,
        int faithValue,
        int office,
        int students,
        int retirees,
        int bloggers,
        int esoterics,
        bool megafon,
        bool devilAdvocate,
        bool cookies,
        bool altar,
        bool candles,
        bool paidFrontRow,
        bool choir,
        bool openMessaPanel)
    {
        currentDay = Mathf.Max(1, day);
        currentMoney = Mathf.Max(0, money);
        totalAdepts = Mathf.Max(0, adepts);
        faith = Mathf.Max(0, faithValue);

        officeVisitors = Mathf.Max(0, office);
        studentVisitors = Mathf.Max(0, students);
        retireeVisitors = Mathf.Max(0, retirees);
        bloggerVisitors = Mathf.Max(0, bloggers);
        esotericVisitors = Mathf.Max(0, esoterics);

        hasMegafon = megafon;
        hasDevilAdvocate = devilAdvocate;
        hasCookies = cookies;
        hasAltar = altar;
        hasCandles = candles;
        hasPaidFrontRow = paidFrontRow;
        hasChoir = choir;

        UpdateStreetPlanText();
        UpdateUI();

        if (openMessaPanel)
        {
            OpenMessa();
        } 
        else OpenStreet();
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
            debugEsotericVisitors,
            debugHasMegafon,
            debugHasDevilAdvocate,
            debugHasCookies,
            debugHasAltar,
            debugHasCandles,
            debugHasPaidFrontRow,
            debugHasChoir,
            true
        );
    }

    public void LaunchMessaTest()
    {
        LaunchLevelFromInspector();
    }

    public void LaunchStreetTest()
    {
        LaunchLevel(
            debugDay,
            debugMoney,
            debugAdepts,
            debugFaith,
            0, 0, 0, 0, 0,
            debugHasMegafon,
            debugHasDevilAdvocate,
            debugHasCookies,
            debugHasAltar,
            debugHasCandles,
            debugHasPaidFrontRow,
            debugHasChoir,
            false
        );
    }

    public void SetMegafon(bool value) { hasMegafon = value; UpdateStreetPlanText(); }
    public void SetDevilAdvocate(bool value) { hasDevilAdvocate = value; UpdateStreetPlanText(); }
    public void SetSelfImprovementClub(bool value) { hasSelfImprovementClub = value; UpdateStreetPlanText(); }
    public void SetPremiumFlyer(bool value) { hasPremiumFlyer = value; UpdateStreetPlanText(); }
    public void SetCthulhuMerch(bool value) { hasCthulhuMerch = value; UpdateStreetPlanText(); }
    public void SetWordOfMouth(bool value) { hasWordOfMouth = value; UpdateStreetPlanText(); }
    public void SetAbyssAccountant(bool value) { hasAbyssAccountant = value; UpdateStreetPlanText(); }
    public void SetCookies(bool value) { hasCookies = value; }
    public void SetAltar(bool value) { hasAltar = value; }
    public void SetCandles(bool value) { hasCandles = value; }
    public void SetPaidFrontRow(bool value) { hasPaidFrontRow = value; }
    public void SetChoir(bool value) { hasChoir = value; }

    public int GetDuration(int dayIndex)
    {
        if (listReactions == null || listReactions.LenDaySec == null || listReactions.LenDaySec.Length == 0) return 60;


        dayIndex = Mathf.Clamp(dayIndex, 0, listReactions.LenDaySec.Length - 1);
        return listReactions.LenDaySec[dayIndex];
    }

    public int GetBaseHumans(int dayIndex)
    {
        if (listReactions == null || listReactions.CountHuman == null || listReactions.CountHuman.Length == 0) return 10;


        dayIndex = Mathf.Clamp(dayIndex, 0, listReactions.CountHuman.Length - 1);
        return listReactions.CountHuman[dayIndex];
    }

    private int GetMaxHumans(int dayIndex)
    {
        if (maxHumansWithBonusesByDay == null || maxHumansWithBonusesByDay.Length == 0) return GetBaseHumans(dayIndex);

        dayIndex = Mathf.Clamp(dayIndex, 0, maxHumansWithBonusesByDay.Length - 1);
        return Mathf.Max(GetBaseHumans(dayIndex), maxHumansWithBonusesByDay[dayIndex]);
    }

    private string NormalizeHumanType(string humanType)
    {
        if (string.IsNullOrEmpty(humanType))
            return string.Empty;

        string value = humanType.ToLower().Replace("(clone)", "").Trim();

        if (value.Contains("worker") || value.Contains("office")) return "worker";
        if (value.Contains("student")) return "student";
        if (value.Contains("retiree") || value.Contains("pension") || value.Contains("pens")) return "retiree";
        if (value.Contains("blogger")) return "blogger";
        if (value.Contains("esoteric")) return "esoteric";

        return value;
    }

    private void UpdateUI()
    {
        if (dayText != null) dayText.text = "День: " + currentDay;
        if (moneyText != null) moneyText.text = "Деньги: " + currentMoney;
        if (messaVisitorsText != null) messaVisitorsText.text = "В мессе: " + TotalVisitors;
        if (adeptsText != null) adeptsText.text = "Адепты: " + totalAdepts;
        if (faithText != null) faithText.text = "Вера: " + faith;
    }
}
