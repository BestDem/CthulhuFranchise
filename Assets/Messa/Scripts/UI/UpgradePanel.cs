using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(Image))]
public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI Header;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceLabel;
    private Image panel;

    private void Awake()
    {
        panel = GetComponent<Image>();
    }
    private void SwitchColor()
    {
        var panelColor = panel.color;
        var iconColor = icon.color;
        panelColor.a *= 0.9f;
        iconColor.a *= 0.6f;
        panel.color = panelColor;
        icon.color = iconColor;
    }
    public void BindUpgrade(int i)
    {
        var upgrade = Messa.Instance.UpgradeList[i];
        Header?.SetText(upgrade.Header);
        description?.SetText(upgrade.Description);
        priceLabel?.SetText($"Купить за ${upgrade.Price}");

        if(upgrade.Icon != null)icon.sprite = upgrade.Icon;      

        if(buyButton != null)
        {
            buyButton.interactable = true;
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => Messa.Instance.BuyUpgrade(i));
        }
    }
    public void DisableBuy()
    {
        SwitchColor();
        priceLabel?.SetText($"Куплено");
        buyButton.interactable = false;
        Debug.Log(this + " was disabled.");
    }
}
