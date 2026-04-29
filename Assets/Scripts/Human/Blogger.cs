using UnityEngine;

public class Blogger : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 2f;

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedBlogger : null;
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
