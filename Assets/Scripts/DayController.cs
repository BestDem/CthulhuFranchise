using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour
{
    [SerializeField] private ListReactions listReactions;
    public static event Action<bool> dayEnd;
    private float timer;
    private int currentDay = 0;
    private bool isDay = true;
    private void Start()
    {
        timer = listReactions.LenDaySec[currentDay];
    }

    public void Update()
    {
        if(isDay)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                currentDay += 1;
                isDay = false;
                Debug.Log("День закончен");
                dayEnd.Invoke(true);
                timer = listReactions.LenDaySec[currentDay];
            }
        }
    }

    public void StartDay()
    {
        dayEnd.Invoke(false);
        isDay = true;
    }
}
