using UnityEngine;

public class Retiree : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 2f;

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedRetiree : null;
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
