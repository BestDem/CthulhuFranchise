using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeWindow : MonoBehaviour
{
    [SerializeField] private Button CookieButton;
    [SerializeField] private Button AltarButton;
    [SerializeField] private Button PremiumFlyerButton;
    [SerializeField] private Button CandleButton;
    [SerializeField] private Button ChoirButton;
    [SerializeField] private Button PaidFrontButton;

    [SerializeField] private TextMeshProUGUI CookieButtonLabel;
    [SerializeField] private TextMeshProUGUI AltarButtonLabel;
    [SerializeField] private TextMeshProUGUI PremiumFlyerButtonLabel;
    [SerializeField] private TextMeshProUGUI CandleButtonLabel;
    [SerializeField] private TextMeshProUGUI ChoirButtonLabel;
    [SerializeField] private TextMeshProUGUI PaidFrontButtonLabel;

    [SerializeField] private UpgradeInfo CookieUpgrade;
    [SerializeField] private UpgradeInfo AltarUpgrade;
    [SerializeField] private UpgradeInfo PremiumFlyerUpgrade;
    [SerializeField] private UpgradeInfo CandleUpgrade;
    [SerializeField] private UpgradeInfo ChoirUpgrade;
    [SerializeField] private UpgradeInfo PaidFrontUpgrade;

    private void Start()
    {
        CookieButtonLabel.SetText(CookieUpgrade.Header);
        AltarButtonLabel.SetText(AltarUpgrade.Header);
        PremiumFlyerButtonLabel.SetText(PremiumFlyerUpgrade.Header);
        CandleButtonLabel.SetText(CandleUpgrade.Header);
        ChoirButtonLabel.SetText(ChoirUpgrade.Header);
        PaidFrontButtonLabel.SetText(PaidFrontUpgrade.Header);

        CookieButton.onClick.AddListener(BuyCookieUpgrade);
        AltarButton.onClick.AddListener(BuyAltarUpgrade);
        PremiumFlyerButton.onClick.AddListener(BuyPremiumFlyerUpgrade);
        CandleButton.onClick.AddListener(BuyCandleUpgrade);
        ChoirButton.onClick.AddListener(BuyChoirUpgrade);
        PaidFrontButton.onClick.AddListener(BuyPaidFrontUpgrade);

        CookieButton.interactable = !Upgrades.HasCookie;
        AltarButton.interactable = !Upgrades.HasAltar;
        PremiumFlyerButton.interactable = !Upgrades.HasPremiumFlyer;
        CandleButton.interactable = !Upgrades.HasCandles;
        ChoirButton.interactable = !Upgrades.HasChoir;
        PaidFrontButton.interactable = !Upgrades.HasPaidFrontRow;     
    }
    public void BuyCookieUpgrade()
    {
        if(Messa.Instance.SpendMoney(CookieUpgrade.Cost))
        {
            Upgrades.HasCookie = true;
            CookieButton.interactable = false;
        }
        Messa.Instance.UpdateUI();
    }
    public void BuyAltarUpgrade()
    {
        if (Messa.Instance.SpendMoney(AltarUpgrade.Cost))
        {
            Upgrades.HasAltar = true;
            AltarButton.interactable = false;
        }
        Messa.Instance.UpdateUI();
    }
    public void BuyPremiumFlyerUpgrade()
    {
        if (Messa.Instance.SpendMoney(PremiumFlyerUpgrade.Cost))
        {
            Upgrades.HasPremiumFlyer = true;
            PremiumFlyerButton.interactable = false;
        }
        Messa.Instance.UpdateUI();
    }
    public void BuyCandleUpgrade()
    {
        if (Messa.Instance.SpendMoney(CandleUpgrade.Cost))
        {
            Upgrades.HasCandles = true;
            CandleButton.interactable = false;
        }
        Messa.Instance.UpdateUI();
    }
    public void BuyChoirUpgrade()
    {
        if (Messa.Instance.SpendMoney(ChoirUpgrade.Cost))
        {
            Upgrades.HasChoir = true;
            ChoirButton.interactable = false;
        }
        Messa.Instance.UpdateUI();
    }
    public void BuyPaidFrontUpgrade()
    {
        if (Messa.Instance.SpendMoney(PaidFrontUpgrade.Cost))
        {
            Upgrades.HasPaidFrontRow = true;
            PaidFrontButton.interactable = false;
        }
        Messa.Instance.UpdateUI();
    }
}
