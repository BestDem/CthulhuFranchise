using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private Image ResultHeader;
    [SerializeField] private Sprite GoodResult;
    [SerializeField] private Sprite BadResult;
    [SerializeField] private TextMeshProUGUI ResultLabel;
    [SerializeField] private TextMeshProUGUI StatsLabel;

    void Start()
    {
        UpdateUI();
    }
    void OnEnable()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if(Messa.Instance.IsGoodGameResult())
        {
            ResultHeader.sprite = GoodResult;
            ResultLabel?.SetText("План выполнен");
        }
        else
        {
            ResultHeader.sprite = BadResult;
            ResultLabel?.SetText("План не выполнен");
        }
        StatsLabel?.SetText($"Привлечено адептов: {Messa.Instance.GetTotalAdeptsCount()} / {Messa.Instance.NeedAdepts}\nЗаработано денег: {(int)Messa.Instance.Money} / {Messa.Instance.NeedMoney}");
    }
}
