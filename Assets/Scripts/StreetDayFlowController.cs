using System.Collections;
using UnityEngine;
using TMPro;

public class StreetDayFlowController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject preparePanel;
    [SerializeField] private GameObject streetPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject messaPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text prepareText;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endResultText;

    [Header("Links")]
    [SerializeField] private GameSessionBridge bridge;
    [SerializeField] private DayController dayController;
    [SerializeField] private SpawnHuman spawnHuman;
    [SerializeField] private HandingFlyers handingFlyers;

    [Header("End text")]
    [SerializeField] private string timerEndTitle = "Время вышло";
    [SerializeField] private string policeEndTitle = "Полиция закончила улицу";
    [SerializeField] private string manualEndTitle = "День завершён";

    [Header("Behaviour")]
    [SerializeField] private bool disableGameplayOnAwake = true;
    [SerializeField] private bool openPrepareOnStart = true;

    [Header("Timer End")]
    [SerializeField] private float timerEndDelay = 3f;
    [SerializeField] private bool clearHumansAfterTimerEndDelay = true;

    [Header("Cleanup")]
    [Tooltip("Чистит всех оставшихся NPC при открытии итоговой панели. Важно для полиции: после поимки полиция и прохожие не остаются между днями.")]
    [SerializeField] private bool clearHumansWhenShowingEndPanel = true;

    private bool streetRunning;
    private bool waitingForClear;
    private Coroutine waitClearRoutine;
    public static StreetDayFlowController Instance;
    private void Awake()
    {
        Instance = this;
        if (bridge == null) bridge = GameSessionBridge.Instance;
        if (disableGameplayOnAwake) SetGameplayEnabled(false);
    }

    private void OnEnable()
    {
        DayController.timerExpired += OnTimerExpired;
        DayController.dayEnd += OnDayEnd;
    }

    private void OnDisable()
    {
        DayController.timerExpired -= OnTimerExpired;
        DayController.dayEnd -= OnDayEnd;
    }

    private void Start()
    {
        if (bridge == null) bridge = GameSessionBridge.Instance;

        if (openPrepareOnStart)
            LoadStreetParametersButton();
    }

    // КНОПКА 1: загрузить параметры дня и показать предпросмотр. Игра ещё НЕ стартует.
    public void LoadStreetParametersButton()
    {
        streetRunning = false;
        waitingForClear = false;
        StopWaitRoutine();
        SetGameplayEnabled(false);

        if (preparePanel != null) preparePanel.SetActive(true);
        if (streetPanel != null) streetPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
        if (messaPanel != null) messaPanel.SetActive(false);

        if (bridge != null)
        {
            bridge.OpenStreet();

            if (prepareText != null) prepareText.text = bridge.BuildStreetPlan().description + "\nНажми 'Начать улицу', чтобы запустить таймер и спавн.";

        }
    }

    // КНОПКА 2: начать улицу после предпросмотра.
    public void StartStreetButton()
    {
        if (streetRunning)
            return;

        StopWaitRoutine();
        waitingForClear = false;
        streetRunning = true;

        if (bridge == null) bridge = GameSessionBridge.Instance;

        if (bridge != null)
        {
            bridge.PrepareNewStreetDay();
            bridge.OpenStreet();
        }

        if (preparePanel != null) preparePanel.SetActive(false);
        if (streetPanel != null) streetPanel.SetActive(true);
        if (endPanel != null) endPanel.SetActive(false);
        if (messaPanel != null) messaPanel.SetActive(false);

        SetGameplayEnabled(true);

        int day = bridge != null ? bridge.CurrentDay : 1;
        if (dayController != null)
        {
            dayController.SetDay(day);
            dayController.StartDay(day);
        }
    }

    private void OnTimerExpired()
    {
        if (!streetRunning || waitingForClear)
            return;

        waitingForClear = true;
        streetRunning = false;

        if (handingFlyers != null)
            handingFlyers.enabled = false;

        if (spawnHuman != null)
            spawnHuman.StopNewSpawnsOnly();

        waitClearRoutine = StartCoroutine(WaitTimerEndDelayThenEnd(timerEndTitle));
    }

    private IEnumerator WaitTimerEndDelayThenEnd(string reason)
    {
        float delay = Mathf.Max(0f, timerEndDelay);

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // По таймеру больше не ждём, пока все NPC дойдут до конца.
        // Через timerEndDelay секунд чистим оставшихся людей и открываем итог улицы.
        if (clearHumansAfterTimerEndDelay && spawnHuman != null)
            spawnHuman.ClearAllSpawned();

        waitingForClear = false;

        if (dayController != null)
            dayController.CompleteDay(reason);
        else
            ShowEndPanel(reason);
    }

    private void OnDayEnd(bool ended)
    {
        if (!ended)
            return;

        string reason = dayController != null ? dayController.LastEndReason : manualEndTitle;
        ShowEndPanel(reason);
    }

    private void ShowEndPanel(string reason)
    {
        streetRunning = false;
        waitingForClear = false;
        StopWaitRoutine();
        SetGameplayEnabled(false);

        // ВАЖНО: чистим всех NPC при любом завершении улицы.
        // Это закрывает баг: после поимки полиции полицейский оставался на сцене между днями.
        if (clearHumansWhenShowingEndPanel && spawnHuman != null)
            spawnHuman.ClearAllSpawned();

        if (preparePanel != null) preparePanel.SetActive(false);
        if (streetPanel != null) streetPanel.SetActive(false);
        if (messaPanel != null) messaPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(true);

        if (bridge != null)
            bridge.OpenResult();

        if (endTitleText != null)
            endTitleText.text = string.IsNullOrEmpty(reason) ? manualEndTitle : reason;

        if (endResultText != null)
            endResultText.text = BuildEndResultText();
    }

    private string BuildEndResultText()
    {
        if (bridge == null)
            bridge = GameSessionBridge.Instance;

        if (bridge == null)
            return "Нет GameSessionBridge на сцене.";

        string text = "Итоги улицы:\n";
        text += "Всего в мессе: " + bridge.TotalVisitors + "\n";
        text += "Офисники: " + bridge.OfficeVisitors + "\n";
        text += "Студенты: " + bridge.StudentVisitors + "\n";
        text += "Пенсионеры: " + bridge.RetireeVisitors + "\n";
        text += "Блогеры: " + bridge.BloggerVisitors + "\n";
        text += "Эзотерики: " + bridge.EsotericVisitors + "\n";
        text += "Деньги: $" + bridge.CurrentMoney + "\n";
        return text;
    }

    public void ForceEndDayButton()
    {
        // Для теста: сразу закончить день, не ждать прохожих.
        if (spawnHuman != null)
            spawnHuman.StopNewSpawnsOnly();

        if (dayController != null)
            dayController.CompleteDay(manualEndTitle);
        else
            ShowEndPanel(manualEndTitle);
    }

    public void OpenMessaButton()
    {
        if (bridge != null)
            bridge.OpenMessa();

        if (preparePanel != null) preparePanel.SetActive(false);
        if (streetPanel != null) streetPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
        if (messaPanel != null) messaPanel.SetActive(true);
    }

    public void NextDayPrepareButton()
    {
        if (bridge != null)
            bridge.NextDay();

        if (dayController != null && bridge != null)
            dayController.SetDay(bridge.CurrentDay);

        LoadStreetParametersButton();
    }

    private void SetGameplayEnabled(bool enabled)
    {
        if (spawnHuman != null)
            spawnHuman.enabled = enabled;

        if (handingFlyers != null)
            handingFlyers.enabled = enabled;
    }

    private void StopWaitRoutine()
    {
        if (waitClearRoutine != null)
        {
            StopCoroutine(waitClearRoutine);
            waitClearRoutine = null;
        }
    }
}
