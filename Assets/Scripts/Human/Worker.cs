using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Human
{
    [SerializeField] private GameObject emotion;
    [SerializeField] private float speed;
    [SerializeField] private int reactionState;
    private Transform human;
    private void Start()
    {
        human = gameObject.transform;
    }
    private void Update()
    {
        float x = Time.deltaTime * speed;
        Vector2 movement = new Vector2(x, 0);
        transform.Translate(movement);
    }

    public override void SetReaction(GameObject reaction)
    {
        throw new System.NotImplementedException();
    }
}
