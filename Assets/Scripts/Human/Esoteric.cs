using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Esoteric : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override string[] GetReactions()
    {
        return listR.NeedEsoteric;
    }
    private void Update()
    {
        float x = Time.deltaTime * speed;
        Vector2 movement = new Vector2(-x, 0);
        transform.Translate(movement);
    }
}
