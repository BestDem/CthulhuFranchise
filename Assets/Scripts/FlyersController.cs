using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyersController : MonoBehaviour
{
    public static FlyersController singltoneFlyers {get; private set;}
    [SerializeField] private Image currentFlyerIm;
    [SerializeField] private Sprite nullImage;
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
        currentFlyerIm.sprite = nullImage;
    }

    public void OnButtonSetFlyer(string nameFlyer)
    {
        currentFlyer = nameFlyer;
    }
    public void OnButtonSprite(Sprite im)
    {
        currentFlyerIm.sprite = im;
    }
}
