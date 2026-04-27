using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour
{
    public static event Action<bool> dayEnd;
    private float timer = 300;

    public void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            Debug.Log("День закончен");
            dayEnd.Invoke(true);
        }
    }
}
