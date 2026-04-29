using UnityEngine;

public class Worker : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 2f;

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedWorker : null;
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
