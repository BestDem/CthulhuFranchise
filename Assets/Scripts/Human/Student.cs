using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Student : Human
{
    [SerializeField] private GameObject emotion;
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed;
    [SerializeField] private int reactionState = 0; //2 - придет, 1 - сомнение, 0 - не придет
    private Transform human;
    private void Start()
    {
        human = gameObject.transform;
    }
    private void Update()
    {
        float x = Time.deltaTime * speed;
        Vector2 movement = new Vector2(-x, 0);
        transform.Translate(movement);
    }

    public override void SetReaction(string reaction)
    {
        if(reaction == listR.NeedStudent[0])
        {
            reactionState = 2;
        }
        if(reaction == listR.NeedStudent[1])
        {
            reactionState = 1;
        }
        else
        {
            reactionState = 0;
        }
        Debug.Log("Студент состояние: " + reactionState + " дана листовка: " + reaction);
    }
}
