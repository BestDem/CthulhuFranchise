using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyersController : MonoBehaviour
{
    public static FlyersController singltoneFlyers {get; private set;}
    private string currentFlyer;
    private void Start()
    {
        singltoneFlyers = this;
    }
    public string CurrentFlyer()
    {
        return currentFlyer;
    }

    public void OnButtonSetFlyer(string nameFlyer)
    {
        currentFlyer = nameFlyer;
    }
}
