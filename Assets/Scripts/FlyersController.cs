using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyersController : MonoBehaviour
{
    public static FlyersController singltoneFlyers {get; private set;}
    [SerializeField] private Text textCurrentFlyer;
    private string currentFlyer;
    private void Start()
    {
        singltoneFlyers = this;
    }
    public string CurrentFlyer()
    {
        return currentFlyer;
    }
    public void DeletFlyer()
    {
        currentFlyer = "";
        textCurrentFlyer.text = "";
    }

    public void OnButtonSetFlyer(string nameFlyer)
    {
        currentFlyer = nameFlyer;
        textCurrentFlyer.text = nameFlyer;
    }
}
