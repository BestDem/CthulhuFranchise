using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed;
    private void Awake()
    {
        needReaction = listR.NeedStudent;
    }
    private void Update()
    {
        float x = Time.deltaTime * speed;
        Vector2 movement = new Vector2(-x, 0);
        transform.Translate(movement);
    }
}
