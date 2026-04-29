using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject canvasPause;
    public bool isPause = false;

    private void Start()
    {

        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetInput();
        }
    }

    public void GetInput()
    {
        isPause = !isPause;

        if (isPause)
        {
            canvasPause.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            canvasPause.SetActive(false);
            Time.timeScale = 1;
        }
    }
}

