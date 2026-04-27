using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed;
    private void Awake()
    {
        needReaction = listR.NeedWorker;
    }
    private void Update()
    {
        float x = Time.deltaTime * speed;
        Vector2 movement = new Vector2(-x, 0);
        transform.Translate(movement);
    }
}
