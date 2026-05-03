using UnityEngine;

public class Retiree : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 2.8f;

    protected override void Awake()
    {
        SetHumanType("retiree");
        name = "retiree";
        base.Awake();
    }

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedRetiree : new string[] { "retiree2" };
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
