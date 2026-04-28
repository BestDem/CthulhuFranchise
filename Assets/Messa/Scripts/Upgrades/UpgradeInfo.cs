using UnityEngine;

[System.Serializable]
public class UpgradeInfo
{
    public int Cost;
    public string Header;
    public string Description;
    public Sprite Icon;

    public string GetButtonLabel()
    {
        return $"{Header} ${Cost}";
    }
}
