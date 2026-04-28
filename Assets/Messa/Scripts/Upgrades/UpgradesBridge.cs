using UnityEngine;

[System.Serializable]
public class UpgradesBridge
{
    public bool HasCookie;
    public bool HasAltar;
    public bool HasPremiumFlyer;
    public bool HasCandles;
    public bool HasChoir;
    public bool HasPaidFrontRow;
    public bool IsDayFive;

    public void ApplyToStatic()
    {
        Upgrades.HasCookie = HasCookie;
        Upgrades.HasAltar = HasAltar;
        Upgrades.HasPremiumFlyer = HasPremiumFlyer;
        Upgrades.HasCandles = HasCandles;
        Upgrades.HasChoir = HasChoir;
        Upgrades.HasPaidFrontRow = HasPaidFrontRow;
        Upgrades.IsDayFive = IsDayFive;
    }
}
