using UnityEngine;

public class Esoteric : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 2f;

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedEsoteric : null;
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
